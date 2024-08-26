using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class OutlineRenderPass : ScriptableRenderPass
{
    public OutlineRenderPass(Material material)
    {

    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
     
    }
    
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
       
    }
}
