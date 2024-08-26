using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;

public class OutlineStencilPass : ScriptableRenderPass
{
    private Material _material;
    
    private RenderTextureDescriptor outlineTextureDescriptor;
    private RTHandle outlineTextureHandle;

    private FilteringSettings m_FilteringSettings;
    public OutlineStencilPass(Material outlineStencilMaterial)
    {
        // Create the material for the stencil pass using your shader
        _material = outlineStencilMaterial;

        outlineTextureDescriptor = new RenderTextureDescriptor(Screen.width,
            Screen.height, RenderTextureFormat.Default, 0);
        
        RenderQueueRange renderQueueRange = RenderQueueRange.opaque;
        m_FilteringSettings = new FilteringSettings(renderQueueRange, LayerMask.GetMask("Outline"));
    }
    
    public override void Configure(CommandBuffer cmd,
        RenderTextureDescriptor cameraTextureDescriptor)
    {
        // Set the blur texture size to be the same as the camera target size.
        outlineTextureDescriptor.width = cameraTextureDescriptor.width;
        outlineTextureDescriptor.height = cameraTextureDescriptor.height;

        // Check if the descriptor has changed, and reallocate the RTHandle if necessary
        RenderingUtils.ReAllocateIfNeeded(ref outlineTextureHandle, outlineTextureDescriptor);
    }

  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();

        RTHandle cameraTargetHandle =
                    renderingData.cameraData.renderer.cameraColorTargetHandle;
        
        using (new ProfilingScope(cmd, new ProfilingSampler("Outline Stencil Pass")))
        {
            SortingCriteria sortingCriteria = SortingCriteria.CommonOpaque;
            DrawingSettings drawingSettings = RenderingUtils.CreateDrawingSettings(new ShaderTagId("OutlineStencilPass"), ref renderingData, sortingCriteria);
            drawingSettings.overrideMaterial = _material;
            drawingSettings.overrideMaterialPassIndex = 0;
            
            // Create & schedule the RendererList
            RendererListDesc rendererListDesc = new RendererListDesc(new ShaderTagId("OutlineStencilPass"), renderingData.cullResults, renderingData.cameraData.camera)
            {
                sortingCriteria = SortingCriteria.CommonOpaque,
                renderQueueRange = RenderQueueRange.opaque,
                layerMask = LayerMask.GetMask("Outline")
            };
            
            // Blit(cmd, cameraTargetHandle, outlineTextureHandle, _material, 0);
            // context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings);
            RendererList rendererList = context.CreateRendererList(rendererListDesc);
            context.Submit();
            // cmd.DrawRendererList(rendererList);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
    
    public void Dispose()
    {
        outlineTextureHandle?.Release();
    }
}
