# Unity-URP14-RendererFeature-Templates
- Unity 2022.3.27f1
- Universal Render Pipeline 14.0.11
1. 将三个文件复制进 Unity 工程。
2. 在 URP Renderer Data 中添加 ColorAdjustRenderPassFeature。
3. 给 Feature 指定 ColorAdjust.shader。
4. 开启 Camera 的 Post Processing。
5. 在 Global Volume 中添加 CustomPostProcessing/ColorAdjust。
6. 勾选参数左侧的 Override，并将 Intensity 调到大于 0。
