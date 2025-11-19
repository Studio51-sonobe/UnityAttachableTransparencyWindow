
using UnityEngine;

public class Example : MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSplashScreen)]
	static void Initialize()
	{
	#if !UNITY_EDITOR
		m_LogCallback = msg => UnityEngine.Debug.LogError( "[Native] " + msg);
		LibWindows.SetLogCallback( m_LogCallback);
		LibWindows.Initialize( System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle);
	#endif
	}
	void OnDestroy()
	{
		LibWindows.Terminate();
	}
	void Update()
	{
		if( m_Cube != null)
		{
			/* なんか動きが欲しかっただけ */
			m_Cube.localEulerAngles = new Vector3( 
				m_Cube.localEulerAngles.x, 
				(m_Cube.localEulerAngles.y + 1) % 360, 
				m_Cube.localEulerAngles.z);
		}
		if( Input.GetMouseButtonDown( 0) != false)
		{
			if( m_Camera.targetTexture != null)
			{
				Debug.LogError( LibWindows.CreateSubWindow(
					m_Camera.targetTexture.GetNativeTexturePtr(),
					m_Camera.targetTexture.width, m_Camera.targetTexture.height));
			}
		}
		else if( Input.GetMouseButtonDown( 1) != false)
		{
			LibWindows.DisposeSubWindow( 0);
		}
	}
	[SerializeField]
	Camera m_Camera;
	[SerializeField]
	Transform m_Cube;
	
	static LibWindows.LogDelegate m_LogCallback;
}

