using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class NormalOutlines : ScriptableRendererFeature {

    public static Color globalOutlineColor = Color.white;
    public static bool enableOutlines = true;

    [System.Serializable]
    private class NormalOutlineSettings
    {
        [Range(0.0f, 1.0f)]
        public float outlineWidth = 1.0f;
        public StencilStateData stencilSettings = new StencilStateData();
    }

    private class NormalOutlinePass : ScriptableRenderPass {

        private readonly Material outlineMaterial;

        private FilteringSettings outlineFilteringSettings;

        private readonly List<ShaderTagId> shaderTagIdList;

        RenderTargetIdentifier cameraColorTarget;
        RenderTargetIdentifier temporaryBuffer;
        int temporaryBufferID = Shader.PropertyToID("_TemporaryBuffer");

        RenderStateBlock renderStateBlock;

        public NormalOutlinePass(RenderPassEvent renderPassEvent, LayerMask layerMask, NormalOutlineSettings settings)
        {
            this.renderPassEvent = renderPassEvent;
            outlineFilteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);

            shaderTagIdList = new List<ShaderTagId> {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("LightweightForward"),
                new ShaderTagId("SRPDefaultUnlit")
            };

            outlineMaterial = new Material(Shader.Find("Hidden/SmoothNormalOutline"));
            outlineMaterial.SetFloat("_OutlineWidth", settings.outlineWidth);

            renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor temporaryTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            temporaryTargetDescriptor.depthBufferBits = 0;
            cmd.GetTemporaryRT(temporaryBufferID, temporaryTargetDescriptor, FilterMode.Bilinear);
            temporaryBuffer = new RenderTargetIdentifier(temporaryBufferID);

            cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!outlineMaterial || !enableOutlines)
                return;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler("SceneViewSpaceNormalsTextureCreation")))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                DrawingSettings drawSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
                drawSettings.enableDynamicBatching = true;
                outlineMaterial.SetColor("_OutlineColor", globalOutlineColor);
                drawSettings.overrideMaterial = outlineMaterial;

                DrawingSettings occluderSettings = drawSettings;
                occluderSettings.overrideMaterial = null;

                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref outlineFilteringSettings, ref renderStateBlock);
               // context.DrawRenderers(renderingData.cullResults, ref occluderSettings, ref outlineFilteringSettings);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(temporaryBufferID);
        }

        public void SetStencilState(int reference, CompareFunction compareFunction, StencilOp passOp, StencilOp failOp, StencilOp zFailOp)
        {
            StencilState stencilState = StencilState.defaultValue;
            stencilState.enabled = true;
            stencilState.SetCompareFunction(compareFunction);
            stencilState.SetPassOperation(passOp);
            stencilState.SetFailOperation(failOp);
            stencilState.SetZFailOperation(zFailOp);

            renderStateBlock.mask |= RenderStateMask.Stencil;
            renderStateBlock.stencilReference = reference;
            renderStateBlock.stencilState = stencilState;
        }
    }

    [SerializeField] private RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    [SerializeField] private LayerMask outlinesLayerMask;
    
    [SerializeField] private NormalOutlineSettings outlineSettings = new NormalOutlineSettings();

    private NormalOutlinePass normalOutlinePass;
    
    public override void Create() {
        if (renderPassEvent < RenderPassEvent.BeforeRenderingPrePasses)
            renderPassEvent = RenderPassEvent.BeforeRenderingPrePasses;

        normalOutlinePass = new NormalOutlinePass(renderPassEvent,outlinesLayerMask, outlineSettings);

        if (outlineSettings.stencilSettings.overrideStencilState)
            normalOutlinePass.SetStencilState(outlineSettings.stencilSettings.stencilReference,
                outlineSettings.stencilSettings.stencilCompareFunction, outlineSettings.stencilSettings.passOperation,
                outlineSettings.stencilSettings.failOperation, outlineSettings.stencilSettings.zFailOperation);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        renderer.EnqueuePass(normalOutlinePass);
    }

}
