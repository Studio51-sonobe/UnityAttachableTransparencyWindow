
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
		
			if( renderTexture != null)
			{
				m_InputModule.SubWindowIndex = LibWindows.CreateSubWindow( renderTexture.GetNativeTexturePtr(),
					renderTexture.width, renderTexture.height, WindowsInputModule.OnSubWindowEventCallback);
				return m_InputModule.SubWindowIndex >= 0;
			}
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
	}
	void Update()
	{
		if( m_InputModule != null && m_Raycaster != null)
		{
			m_InputModule.Process( m_Raycaster);
		}
	}
	[SerializeField]
	Camera m_Camera;
	[SerializeField]
	GraphicRaycaster m_Raycaster;
	[SerializeField]
	WindowsInputModule m_InputModule;
}
