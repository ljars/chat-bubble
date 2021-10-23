# Chat Bubble Addon
Chat bubble addon for Suriyun's MMORPG Kit
Tested on MMORPG Kit v1.70f

## Installation
1. Import the folder
2. Done

## Uninstall
1. Delete the ChatBubble folder

## Temporarily disable
1. Open each of your PlayerCharacterEntity prefabs
2. Un-tick `Show Chat Bubbles` on each PlayerCharacterEntity component

## Edit chat bubble appearance
1. Open the ChatBubblePrefab in ChatBubble/Resources
2. Make your changes to the prefab
3. Do not change prefab name: it must be named `ChatBubblePrefab` and it must be in the Resources folder
4. If the chat bubble is too high or too low for your characters, you can move the `BubbleRoot` object up or down
5. The `MessageBubble-Template` object is the template that will be instantiated at runtime to create multiple chat bubbles.
* You can try setting a new sprite on the Image components of the `MessageBubble-Template` and `Arrow` objects (sliced sprites look best).
6. The channel colors will be applied to all the items in the `Tinted Graphics` arrays in the `ChatBubble` and `ChatBubbleElement` components
7. The items in the `Transition Objects` array will be transitioned out when there are no bubbles visible
* By default, the `Arrow` object should be in the `Tinted Graphics` and `Transition Objects` arrays.

## Edit word-wrap width
1. Open the `ChatBubblePrefab` in ChatBubble/Resources
2. Edit the `Max Bubble Width` field on the ChatBubble component

## Edit chat bubble duration
1. Open the ChatBubblePrefab in ChatBubble/Resources
2. Change the value of `Min Duration`, `Max Duration` and `Extra Duration Per Character` on the `ChatBubble` component.
* The `Extra Duration Per Character` allows longer messages to be displayed for a longer amount of time, measured in seconds per character.
* `Min Duration` is the base duration. If `Extra Duration Per Character` is 0, the total duration will be `Min Duration`.
* `Max Duration` is the upper bound on the total duration (so that `Extra Duration Per Character` does not make the duration too long for really long messages).

## Change display based on channel (e.g. disable chat bubble for Guild messages, or change color of bubble for Whisper)
1. Open the `ChatBubblePrefab` in ChatBubble/Resources
2. Change the channel message tint colors to whatever you want
3. Tick the boxes for the message channels for which you want the bubble to display 
	(e.g. unticking `Show Guild Bubble` will hide the bubble for Guild chat)
4. Note that the channel colors will be applied to all the items in the `Tinted Graphics` arrays in the `ChatBubble` and `ChatBubbleElement` components
