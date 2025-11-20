
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SubWindow : MonoBehaviour
{
#if UNITY_EDITOR
	[UnityEditor.MenuItem( "GameObject/Test")]
	static void Test()
	{
		var eventSystem = UnityEditor.Selection.activeGameObject?.GetComponent<EventSystem>();
		if( eventSystem != null)
		{
			var subWindows = eventSystem.GetComponentsInChildren<SubWindow>( true);
			if( subWindows.Length > 0)
			{
				foreach( var subWindow in subWindows)
				{
					subWindow.m_InputModule = eventSystem.AddComponent<WindowsInputModule>();
				}
			}
		}
	}
#endif
	public bool Enable()
	{
		if( m_Camera != null && m_InputModule != null && m_InputModule.SubWindowIndex < 0)
		{
			RenderTexture renderTexture = m_Camera.targetTexture;
			// if( m_RenderTexture == null && m_Camera.targetTexture == null)
			// {
			// 	m_RenderTexture = new RenderTexture( 512, 512, 
			// 		UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm, 
			// 		UnityEngine.Experimental.Rendering.GraphicsFormat.D24_UNorm_S8_UInt, 1);
				
				if( renderTexture != null)
				{
					// m_Camera.targetTexture = m_RenderTexture;
					m_InputModule.SubWindowIndex = LibWindows.CreateSubWindow( renderTexture.GetNativeTexturePtr(),
						renderTexture.width, renderTexture.height, WindowsInputModule.OnSubWindowEventCallback);
					if( m_InputModule.SubWindowIndex >= 0)
					{
						return true;
					}
				}
				// m_Camera.targetTexture = null;
				// m_RenderTexture.Release();
				// m_RenderTexture = null;
			// }
		}
		return false;
	}
	public void Disable()
	{
		if( m_Camera != null && m_InputModule != null)
		{
			if( m_InputModule.SubWindowIndex >= 0)
			{
				LibWindows.DisposeSubWindow( m_InputModule.SubWindowIndex);
				m_InputModule.SubWindowIndex = -1;
			}
		}
		// if( m_Camera.targetTexture != null)
		// {
		// 	m_Camera.targetTexture = null;
		// }
		// if( m_RenderTexture != null)
		// {
		// 	m_RenderTexture.Release();
		// 	m_RenderTexture = null;
		// }
	}
	void Update()
	{
		if( m_Canvas != null)
		{
			var canvasGraphics = GraphicRegistry.GetRaycastableGraphicsForCanvas( m_Canvas);
			Debug.LogError( m_InputModule.SubWindowIndex + ", " + canvasGraphics.Count);
		}
		if( m_InputModule != null && m_Raycaster != null)
		{
			m_InputModule.Process( m_Raycaster);
		}
	}
	[SerializeField]
	Camera m_Camera;
	[SerializeField]
	Canvas m_Canvas;
	[SerializeField]
	GraphicRaycaster m_Raycaster;
	[SerializeField]
	WindowsInputModule m_InputModule;
	[System.NonSerialized]
	RenderTexture m_RenderTexture;
}
