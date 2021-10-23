using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    [DefaultExecutionOrder(200)]
    public class ChatBubble : MonoBehaviour
    {
        [SerializeField] private ChatBubbleEntry bubbleEntryPrefab;
        [Tooltip("These will have the same color assigned to them as the most recently created bubble.")]
        [SerializeField] private Graphic[] tintedGraphics; 
        [Tooltip("These objects will be hidden when there are no active bubbles.")]
        [SerializeField] private GameObject[] transitionObjects;
        [SerializeField] private Transform messagesContainer;
        [SerializeField] private Canvas canvas;
        
        [Tooltip("If set false, the chat bubble will not rotate to face the camera.")]
        [SerializeField] private bool billboard = true;

        [Header("Word-Wrap")] [SerializeField] private float maxBubbleWidth = 650f;
        
        [Header("Durations")]
        [Tooltip("The base time in seconds to display the chat bubble")]
        [SerializeField] private float minDuration = 3f;
        [Tooltip("The maximum time in seconds to display the chat bubble")]
        [SerializeField] private float maxDuration = 10f;
        [Tooltip("Extra time to display the message (in seconds per character)")]
        [SerializeField] private float extraDurationPerCharacter = 0.1f;
        [Tooltip("The time to transition the bubble and transitionObjects")]
        [SerializeField] private float transitionDuration = 0.2f;
        [Tooltip("If true, a bubble can only close after all previous messages close (regardless of message lengths).")]
        [SerializeField] private bool bubblesMustExpireInOrder = true;
        [Tooltip("The max number of bubbles displayed at once")]
        [SerializeField] private int maxBubbles = 3;

        [Header("Channel Settings")]
        [SerializeField] private Color messageTintLocal = Color.white;
        [SerializeField] private Color messageTintGlobal = Color.white;
        [SerializeField] private Color messageTintWhisper = Color.green;
        [SerializeField] private Color messageTintParty = Color.cyan;
        [SerializeField] private Color messageTintGuild = Color.blue;
        [SerializeField] private bool showLocalBubble = true;
        [SerializeField] private bool showGlobalBubble = true;
        [SerializeField] private bool showWhisperBubble = true;
        [SerializeField] private bool showPartyBubble = true;
        [SerializeField] private bool showGuildBubble = true;

        private LinkedList<ChatBubbleEntry> chatBubbles; // contains the active chat bubbles
        private LinkedList<Coroutine> transitionRoutines; // stores the coroutines of the `transitionObjects`
        private Vector3[] defaultScales; // contains the default localScale of all elements in `transitionObjects`
        private Color bubbleTint; // color of the newest chat bubble
        private Transform camTransform;
        private Transform canvasTransform;
        private Camera cam;
        
        public void Start()
        {
            defaultScales = new Vector3[transitionObjects.Length];
            for (int i = 0; i < transitionObjects.Length; i++)
            {
                GameObject o = transitionObjects[i];
                if (o == null)
                    continue;
                defaultScales[i] = o.transform.localScale;
                o.transform.localScale = Vector3.zero;
                o.SetActive(true);
            }
            bubbleTint = messageTintLocal;
            chatBubbles = new LinkedList<ChatBubbleEntry>();
            transitionRoutines = new LinkedList<Coroutine>();
            foreach(Transform child in messagesContainer)
            {
                child.gameObject.SetActive(false);
            }
            cam = Camera.main;
            if (cam && canvas)
            {
                canvas.worldCamera = cam;
                camTransform = cam.transform;
                canvasTransform = canvas.transform;
            }

            RectTransform rt = messagesContainer.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(maxBubbleWidth, rt.sizeDelta.y);

        }

        private void LateUpdate()
        {
            // Rotate chat bubble to face camera
            if (billboard &&canvasTransform && camTransform)
                canvasTransform.rotation = camTransform.rotation;
        }

        /// <summary>
        /// This function is called to initiate the process of displaying a given chat message.
        /// </summary>
        public void Show(string message)
        {
            if (bubbleEntryPrefab != null && messagesContainer != null && bubbleEntryPrefab.messageText != null)
            {
                // If too many bubbles, remove the oldest (regardless of remaining duration)
                if (chatBubbles.Count >= maxBubbles)
                {
                    var oldestBubble = chatBubbles.Last.Value;
                    StopCoroutine(oldestBubble.bubbleRoutine);
                    StartCoroutine(RemoveBubble(oldestBubble));
                }
                // Show the new message
                Coroutine bubbleRoutine = StartCoroutine(ShowBubbleRoutine(message));
                // Store reference to the coroutine in case it needs to be terminated early
                chatBubbles.First.Value.bubbleRoutine = bubbleRoutine;
            }
            else
            {
                Debug.LogError("Please configure the Chat Bubble!");
            }
        }

        /// <summary>
        /// This coroutine creates a new chat bubble, transitions it in, waits for the message's duration, then
        /// transitions it out.
        /// </summary>
        private IEnumerator ShowBubbleRoutine(string message)
        {
            ChatBubbleEntry bubble = AddBubble(message);
            
            // Wait for transition before starting timer
            UpdateTransitionObjects();
            yield return bubble.TransitionIn();

            // Show message for given duration
            float duration = Mathf.Min(maxDuration, minDuration + (extraDurationPerCharacter * bubble.messageText.text.Length));
            yield return new WaitForSeconds(duration);

            yield return RemoveBubble(bubble);
        }

        /// <summary>
        /// Instantiates and configures the chat bubble.
        /// </summary>
        private ChatBubbleEntry AddBubble(string message)
        { 
            ChatBubbleEntry bubble = Instantiate(bubbleEntryPrefab, messagesContainer);
            bubble.messageText.text = message;
            bubble.TransitionDuration = transitionDuration;
            bubble.SetTint(bubbleTint);
            chatBubbles.AddFirst(bubble);
            bubble.gameObject.SetActive(true);
            return bubble;
        }

        /// <summary>
        /// Transitions the chat bubble out, then destroys it.
        /// </summary>
        private IEnumerator RemoveBubble(ChatBubbleEntry bubble)
        {
            if (bubblesMustExpireInOrder)
            {
                // Wait until all previous messages have been removed before removing this one.
                while (chatBubbles.Last.Value != bubble)
                {
                    yield return null;
                }
            }
            chatBubbles.Remove(bubble);
            UpdateTransitionObjects();
            yield return bubble.TransitionOut();
            Destroy(bubble.gameObject);
        }

        /// <summary>
        /// This displays the `transitionObjects` if there are at least 1 active chat bubbles, else it hides them
        /// </summary>
        private void UpdateTransitionObjects()
        {
            foreach (var c in transitionRoutines)
            {
                if (c != null)
                    StopCoroutine(c);
            }

            transitionRoutines.Clear();

            for (var i = 0; i < transitionObjects.Length; i++)
            {
                Vector3 targetScale;
                if (chatBubbles.Count > 0) 
                    targetScale = defaultScales[i]; // Transition in
                else
                    targetScale = Vector3.zero; // Transition out
                
                GameObject o = transitionObjects[i];
                if (o == null)
                    continue;
                Coroutine c = StartCoroutine(TransitionToScale(o.transform, targetScale, transitionDuration));
                transitionRoutines.AddFirst(c);
            }  
        }
        
        /// <summary>
        /// This message sets the color based on channel, and returns true only if the message was on a channel that
        /// the bubble is configured to display for.
        /// </summary>
        /// <param name="channel">The channel of the message</param>
        /// <returns>True if the bubble should be shown (based on the channel), else false</returns>
        public bool CheckChannel(ChatChannel channel)
        {
            Color tint;

            switch (channel)
            {
                case ChatChannel.Local:
                    if (!showLocalBubble) return false;
                    tint = messageTintLocal;
                    break;
                case ChatChannel.Global:
                    if (!showGlobalBubble) return false;
                    tint = messageTintGlobal;
                    break;
                case ChatChannel.Whisper:
                    if (!showWhisperBubble) return false;
                    tint = messageTintWhisper;
                    break;
                case ChatChannel.Party:
                    if (!showPartyBubble) return false;
                    tint = messageTintParty;
                    break;
                case ChatChannel.Guild:
                    if (!showGuildBubble) return false;
                    tint = messageTintGuild;
                    break;
                default:
                    return false;
            }

            foreach (var graphic in tintedGraphics)
            {
                graphic.color = tint;
            }
            bubbleTint = tint;
            return true;
        }
        
        /// <summary>
        /// Scales transform `t` to the `targetScale` in `transitionDuration` seconds. Used to transition the bubbles
        /// and other elements in and out. Used by this class and by `ChatBubbleEntry`.
        /// </summary>
        /// <param name="t">The transform to apply the scaling to</param>
        /// <param name="targetScale">The scale to transition to</param>
        /// <param name="transitionDuration">The time in seconds to transition</param>
        public static IEnumerator TransitionToScale(Transform t, Vector3 targetScale, float transitionDuration)
        {
            if (t == null)
                yield break;
            float smoothing = 1f;
            Vector3 difference = t.localScale - targetScale;
            while (smoothing >= 0f)
            {
                t.localScale = targetScale + (difference * smoothing);
                smoothing -= (Time.deltaTime / transitionDuration);
                yield return null;
            }
            t.localScale = targetScale;
        }
    }
}

