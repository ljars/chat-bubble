using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class ChatBubbleEntry : MonoBehaviour
    {
        public TextWrapper messageText;

        public float TransitionDuration { get; set; } // Assigned by the ChatBubble class
        [SerializeField] private Graphic[] tintedGraphics;

        private Vector2 defaultScale;
        private RectTransform rectTransform;
        public Coroutine bubbleRoutine;

        private void Setup()
        {
            rectTransform = (RectTransform) transform;
            
            defaultScale = rectTransform.localScale;
            rectTransform.localScale = Vector3.zero;
        }
        
        /// <summary>
        /// Transitions the chat bubble in.
        /// </summary>
        public IEnumerator TransitionIn()
        {
            Setup();
            rectTransform.pivot = new Vector2(0.5f, 0f);
            yield return ChatBubble.TransitionToScale(rectTransform, defaultScale, TransitionDuration);
        }

        /// <summary>
        /// Transitions the chat bubble out.
        /// </summary>
        public IEnumerator TransitionOut()
        {
            yield return ChatBubble.TransitionToScale(transform, Vector3.zero, TransitionDuration);
        }

        /// <summary>
        /// Sets the color of the `tintedGraphics`.
        /// </summary>
        /// <param name="tint">The color to set</param>
        public void SetTint(Color tint)
        {
            foreach (var graphic in tintedGraphics)
            {
                graphic.color = tint;
            }
        }
    }
}
