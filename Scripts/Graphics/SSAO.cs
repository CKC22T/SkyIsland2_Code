using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngineInternal;

[ReloadGroup]
public class SSAO : ScriptableRendererFeature
{
    [System.Serializable]
    public class SSAOSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

        [Range(0, 2)] public float totalStrength = 1.1f;
        [Range(0, 1)] public float brightnessCorrection = 0.0f;
        [Range(0.01f, 5)] public float area = 0.55f;
        public float falloff = 0.0001f;
        [Range(0.01f, 2.5f)] public float radius = 0.04f;
        public bool debug = false;
        public LayerMask maskObjects;
        public BuiltinRenderTextureType textureType;
    }

    public SSAOSettings settings = new SSAOSettings();

    [ReloadGroup]
    class CustomRenderPass : ScriptableRenderPass
    {
        [Reload("Materials/ssao.mat")]
        public Material ssaoMaterial = null;
        // public Material upsampleMaterial;
        public float totalStrength;
        public float brightnessCorrection;
        public float area;
        public float falloff;
        public float radius;
        public bool debug;

        string profilerTag;
        public RenderTargetIdentifier tmpRT1;
        public RenderTargetIdentifier tmpDS1;

        public RenderTexture tmpRTBuffer;
        public RenderTexture tmpDSBuffer;

        public TextureHandle depthHandle;

        private RenderTargetIdentifier source { get; set; }

        public void Setup(RenderTargetIdentifier source)
        {
            ssaoMaterial = Resources.Load("Materials/ssao") as Material;

            this.source = source;
        }

        public CustomRenderPass(string profilerTag)
        {
            this.profilerTag = profilerTag;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var width = cameraTextureDescriptor.width;
            var height = cameraTextureDescriptor.height;

            int tmpId = Shader.PropertyToID("ssao_RT");
            int tmpDepthId = Shader.PropertyToID("ssao_DS");
            cmd.GetTemporaryRT(tmpId, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            cmd.GetTemporaryRT(tmpDepthId, width, height, 32, FilterMode.Bilinear, RenderTextureFormat.Depth);

            tmpRT1 = new RenderTargetIdentifier(tmpId);
            tmpDS1 = new RenderTargetIdentifier(tmpDepthId);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (ssaoMaterial == null)
            {
                ssaoMaterial = Resources.Load("Materials/ssao") as Material;
                return;
            }
            
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;

            ssaoMaterial.SetFloat("_TotalStrength", totalStrength);
            ssaoMaterial.SetFloat("_Base", brightnessCorrection);
            ssaoMaterial.SetFloat("_Area", area);
            ssaoMaterial.SetFloat("_Falloff", falloff);
            ssaoMaterial.SetFloat("_Radius", radius);
            ssaoMaterial.SetFloat("_Debug", debug ? 0.0f : 1.0f);

            cmd.Blit(source, tmpRT1);

            cmd.SetRenderTarget(tmpRT1, renderingData.cameraData.renderer.cameraDepthTarget);

            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, ssaoMaterial, 0);
            
            Blit(cmd, tmpRT1, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }

    CustomRenderPass scriptablePass;

    public override void Create()
    {
        scriptablePass = new CustomRenderPass("SSAO");
        
        scriptablePass.totalStrength = settings.totalStrength;
        scriptablePass.brightnessCorrection = settings.brightnessCorrection;
        scriptablePass.area = settings.area;
        scriptablePass.falloff = settings.falloff;
        scriptablePass.radius = settings.radius;
        scriptablePass.debug = settings.debug;

        scriptablePass.renderPassEvent = settings.renderPassEvent;

      //  BlitThatCanPassTheFuckingDSV.Init();

    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.isPreviewCamera)
        {
            return;
        }

        var src = renderer.cameraColorTarget;
        scriptablePass.Setup(src);
        scriptablePass.ConfigureInput(ScriptableRenderPassInput.Depth);
        scriptablePass.ConfigureTarget(src, renderer.cameraDepthTarget);
        renderer.EnqueuePass(scriptablePass);
    }
}


