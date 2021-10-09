# tgc-mg-better-skinned-sample
Testing with skinned animation

[#BuiltWithMonoGame](http://www.monogame.net), [.NET Core](https://dotnet.microsoft.com) and [Mixamo](https://www.mixamo.com) models.

For now the animation of xbot and ybot is working (colors are wrong), because [SkinnedEffect.fx](https://github.com/MonoGame/MonoGame/blob/master/MonoGame.Framework/Platform/Graphics/Effect/Resources/SkinnedEffect.fx) does not use diffuse color.
Textured models works fine, you need to fix the texture path after downloading (download the ascii version) and put the texture file next to the fbx (you can get the texture by downloading the dae format).
