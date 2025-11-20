
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if UNITY_6000_0_OR_NEWER || UNITY_2023_3_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
#endif

internal sealed class FlipVerticalSyncPass : ScriptableRenderPass
{
	public FlipVerticalSyncPass()
	{
	}
// #if !WITH_LEGACY
	public override void RecordRenderGraph( RenderGraph renderGraph, ContextContainer frameData)
	{
		var cameraData = frameData.Get<UniversalCameraData>();
		
		if( (cameraData.camera.cameraType & kInvalidCameraType) != 0)
		{
			return;
		}
		UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
		TextureHandle cameraTexture = resourceData.activeColorTexture;
		
		// if( cameraTexture.IsValid() != false)
		{
			var tempDesc = renderGraph.GetTextureDesc( cameraTexture);
			tempDesc.name = kTempColorTarget;
			TextureHandle tempTexture = renderGraph.CreateTexture( tempDesc);
			
			// if( tempTexture.IsValid() != false)
			{
				renderGraph.AddBlitPass( cameraTexture, tempTexture, new Vector2( 1.0f, -1.0f), new Vector2( 0.0f, 1.0f));
				
				// if( renderPassEvent <= RenderPassEvent.BeforeRenderingPostProcessing)
				// {
				// 	resourceData.cameraColor = tempTexture;
				// }
				// else
				{
					renderGraph.AddCopyPass( tempTexture, cameraTexture);
				}
			}
		}
	}
// #else
// 	public override void Configure( CommandBuffer commandBuffer, RenderTextureDescriptor cameraTextureDescriptor)
// 	{
// 		RenderTextureDescriptor descriptor = cameraTextureDescriptor;
// 		descriptor.msaaSamples = 1;
// 		descriptor.depthBufferBits = (int)DepthBits.None;
// 		RenderingUtils.ReAllocateIfNeeded( ref m_CopiedColorTarget, descriptor, name: kTempColorTarget);
// 	}
// 	public override void Execute( ScriptableRenderContext context, ref RenderingData renderingData)
// 	{
// 		ref var cameraData = ref renderingData.cameraData;
		
// 		if( (cameraData.camera.cameraType & kInvalidCameraType) != 0)
// 		{
// 			return;
// 		}
// 		CommandBuffer commandBuffer = CommandBufferPool.Get( kProfilerTag);
// 		RTHandle cameraColorTarget = cameraData.renderer.cameraColorTargetHandle;
		
// 		commandBuffer.SetRenderTarget( m_CopiedColorTarget, 
// 			RenderBufferLoadAction.DontCare,  RenderBufferStoreAction.Store, 
// 			RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);
// 		Blitter.BlitTexture( commandBuffer, cameraColorTarget, GetScaleBias( cameraColorTarget), 0, false);
		
// 		commandBuffer.SetRenderTarget( cameraColorTarget, 
// 			RenderBufferLoadAction.DontCare,  RenderBufferStoreAction.Store, 
// 			RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);
// 		Blitter.BlitTexture( commandBuffer, m_CopiedColorTarget, GetScaleBias( m_CopiedColorTarget), 0, false);
		
// 		context.ExecuteCommandBuffer( commandBuffer);
// 		CommandBufferPool.Release( commandBuffer);
// 	}
// 	static Vector4 GetScaleBias( RTHandle source)
// 	{
// 		if( source.useScaling == false)
// 		{
// 			return Vector2.one;
// 		}
// 		RTHandleProperties rtHandleProperties = source.rtHandleProperties;
// 		ref Vector4 rtHandleScale = ref rtHandleProperties.rtHandleScale;
// 		return new Vector2( rtHandleScale.x, rtHandleScale.y);
// 	}
// 	const string kProfilerTag = nameof( FlipVerticalSyncPass);
// 	RTHandle m_CopiedColorTarget;
// #endif
	const CameraType kInvalidCameraType = CameraType.SceneView | CameraType.Preview;
	const string kTempColorTarget = "_FlipVerticalTarget";
}
