# chat-bubble
Chat bubble addon for Suriyun's MMORPG Kit
Tested on MMORPG Kit v1.66b

## Installation
1. Import the folder
2. Done

## Uninstall
1. Delete UnityMultiplayerARPG_ChatBubble folder

## Temporarily disable
1. Open each of your PlayerCharacterEntity prefabs
2. Un-tick 'Show Chat Bubbles' on each PlayerCharacterEntity component

## Edit chat bubble appearance
1. Open the ChatBubblePrefab in ChatBubble/Resources
2. Make your changes to the prefab
3. If the chat bubble is too high or too low for your characters, you can move the BubbleRoot object up or down
4. Do not change prefab name: it must be named ChatBubblePrefab and it must be in the Resources folder

## Edit chat bubble duration
1. Open the ChatBubblePrefab in ChatBubble/Resources
2. Change the value of 'Min Duration' and 'Extra Duration Per Character' on the ChatBubble component.
   The extra duration allows longer messages to be displayed for a longer amount of time.
   totalDuration = minDuration + (extraDurationPerCharacter * lengthOfMessage)

## Change display based on channel (e.g. disable chat bubble for Guild messages, or change color of bubble for Whisper)
1. Open the ChatBubblePrefab in ChatBubble/Resources
2. Change the Message Tint Colors to whatever you want
3. Tick the boxes for the message channels for which you want the bubble to display 
	(e.g. unticking 'Show Guild Bubble' will hide the bubble for Guild chat)
