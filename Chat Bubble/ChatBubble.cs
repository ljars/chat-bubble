using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG {
    public class ChatBubble : MonoBehaviour
    {
        [SerializeField] private GameObject bubbleRoot;
        [SerializeField] private Graphic[] tintedGraphics;
        [SerializeField] private Text messageText;
        [SerializeField] private float minDuration = 3f;
        [SerializeField] private float extraDurationPerCharacter = 0.1f;
        [SerializeField] private float transitionDuration = 1f;

        public Color messageTintLocal = Color.white;
        public Color messageTintGlobal = Color.white;
        public Color messageTintWhisper = Color.green;
        public Color messageTintParty = Color.cyan;
        public Color messageTintGuild = Color.blue;
        public bool showLocalBubble = true;
        public bool showGlobalBubble = true;
        public bool showWhisperBubble = true;
        public bool showPartyBubble = true;
        public bool showGuildBubble = true;

        private Vector2 defaultScale;
        private float animationScale;

        private void Awake()
        {
            defaultScale = bubbleRoot.transform.localScale;
            bubbleRoot.SetActive(false);
        }

        public void Show(string message)
        {
            StopAllCoroutines();
            messageText.text = message;
            bubbleRoot.SetActive(true);
            StartCoroutine(BubbleRoutine());
        }

        private IEnumerator BubbleRoutine()
        {
            // Transition in
            animationScale = 0f;
            while (animationScale <= 1f)
            {
                bubbleRoot.transform.localScale = defaultScale * animationScale;
                animationScale += (Time.deltaTime / transitionDuration);
                yield return null;
            }

            // Show message for given duration
            float duration = minDuration + (extraDurationPerCharacter * messageText.text.Length);
            yield return new WaitForSeconds(duration);

            // Transition out
            animationScale = 1f;
            while (animationScale >= 0f)
            {
                bubbleRoot.transform.localScale = defaultScale * animationScale;
                animationScale -= (Time.deltaTime / transitionDuration);
                yield return null;
            }
            bubbleRoot.SetActive(false);
        }

        public bool CheckChannel(ChatChannel channel)
        {
            Color tint;

            switch (channel) {
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

            foreach (Graphic g in tintedGraphics)
            {
                g.color = tint;
            }

            return true;
        }
    }
}
