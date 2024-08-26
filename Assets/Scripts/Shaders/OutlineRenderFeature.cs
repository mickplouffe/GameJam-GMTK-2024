using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = System.Object;

public class OutlineRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private Shader outlineStencilShader;

    private Material outlineStencilMaterial;
    
    private OutlineStencilPass _outlineStencilPass;
    private OutlineRenderPass _outlineRenderPass;

    public override void Create()
    {
        outlineStencilMaterial = CoreUtils.CreateEngineMaterial(outlineStencilShader);
        // Create the stencil pass
        _outlineStencilPass = new OutlineStencilPass(outlineStencilMaterial)
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques,
        };

        // Create the outline render pass
        // _outlineRenderPass = new OutlineRenderPass
        // {
        //     renderPassEvent = RenderPassEvent.AfterRenderingOpaques
        // };
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_outlineStencilPass);
        // renderer.EnqueuePass(_outlineRenderPass);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        CoreUtils.Destroy(outlineStencilMaterial);
        _outlineStencilPass.Dispose();
    }
}


