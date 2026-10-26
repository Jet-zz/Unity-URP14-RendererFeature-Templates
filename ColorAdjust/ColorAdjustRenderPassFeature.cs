using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorAdjustRenderPassFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Setting
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public Shader shader;
    }
    public Setting setting = new Setting();
    ColorAdjustRenderPass m_ScriptablePass;

    class ColorAdjustRenderPass : ScriptableRenderPass
    {
        const string profilerTag = "ColorAdjust"; 
        Material m_material;
        ColorAdjustVolume colorAdjustVolume;
        static readonly int tempTexID = Shader.PropertyToID("_TempTexture"); 
        public ColorAdjustRenderPass(RenderPassEvent passEvent, Shader m_shader)
        {
            renderPassEvent = passEvent;

            if(m_shader == null) return;
            m_material = CoreUtils.CreateEngineMaterial(m_shader);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_material == null)
            {
                Debug.LogError("材质初始化失败！");
                return;
            }
            var strack = VolumeManager.instance.stack;
            colorAdjustVolume = strack.GetComponent<ColorAdjustVolume>();

            if (colorAdjustVolume == null || !colorAdjustVolume.IsActive())
            {
                return;
            }

            RenderTargetIdentifier cameraColorTexture = renderingData.cameraData.renderer.cameraColorTargetHandle;
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

            int rtWidth = descriptor.width;
            int rtHeight = descriptor.height;    

            CommandBuffer cmd = CommandBufferPool.Get(name:"ColorAdjustRenderPass");
            cmd.GetTemporaryRT(tempTexID , rtWidth , rtHeight , depthBuffer:0 , FilterMode.Trilinear , format:RenderTextureFormat.Default);

            m_material.SetFloat("_Intensity", colorAdjustVolume.Intensity.value);
            m_material.SetFloat("_Brightness", colorAdjustVolume.Brightness.value);
            m_material.SetFloat("_Contrast", colorAdjustVolume.Contrast.value);
            m_material.SetFloat("_Gamma",1.0f / Mathf.Max(colorAdjustVolume.Gamma.value, 0.01f));
            m_material.SetFloat("_Hue", colorAdjustVolume.Hue.value);
            m_material.SetFloat("_Saturation", colorAdjustVolume.Saturation.value);

            cmd.Blit(cameraColorTexture, tempTexID, m_material);
            cmd.Blit(tempTexID , cameraColorTexture);
            cmd.ReleaseTemporaryRT(tempTexID);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    public override void Create()
    {
        if (setting.shader == null) return;
        m_ScriptablePass = new ColorAdjustRenderPass(setting.renderPassEvent, setting.shader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!renderingData.cameraData.postProcessEnabled) return;

        if (m_ScriptablePass == null) return;
        
        renderer.EnqueuePass(m_ScriptablePass);
    }
}

