# Operator Voice Listener

一个基于 WinUI 3 的干员语音播放器

目前能够播放中文(包括方言)、日语、英语、韩语及意大利语的干员语音，并且能够显示中文字幕。

## 目前存在的BUG
由于WinUI 3的一个[bug](https://github.com/microsoft/WindowsAppSDK/issues/3305)，OGG音频播放30秒后会出现解码错误，这时请重新开始播放并将播放进度调节到自己需要的位置。

## 使用的开源项目
* [ArknightsResources/OperatorsVoiceResources](https://github.com/ArknightsResources/OperatorsVoiceResources) (提供语音资源的库)
* [ArknightsResources/Utility](https://github.com/ArknightsResources/Utility) (读取语音资源的库)


## 许可
MIT 许可证
