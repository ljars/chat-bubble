using UnityEngine;

namespace MultiplayerARPG
{
    public partial class PlayerCharacterEntity
    {
        [Header("Chat Bubbles")] public bool showChatBubbles = true;

        private ChatBubble bubble;

        [DevExtMethods("Awake")]
        protected void Awake_ChatBubble()
        {
            var bubbleObject = Instantiate(Resources.Load<GameObject>("ChatBubblePrefab"), transform);
            bubbleObject.transform.localPosition = Vector3.zero;
            bubble = bubbleObject.GetComponent<ChatBubble>();

            // Add addon hook
            ClientGenericActions.onClientReceiveChatMessage += ShowChatBubble;
        }

        [DevExtMethods("OnDestroy")]
        protected void OnDestroy_ChatBubble()
        {
            // Remove addon hook
            ClientGenericActions.onClientReceiveChatMessage -= ShowChatBubble;
        }

        private void ShowChatBubble(ChatMessage chatMessage)
        {
            if (!showChatBubbles || chatMessage.sender != CharacterName)
            {
                return;
            }

            if (bubble.CheckChannel(chatMessage.channel))
            {
                bubble.Show(chatMessage.message);
            }
        }
    }
}