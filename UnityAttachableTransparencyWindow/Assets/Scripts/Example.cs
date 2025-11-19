
using UnityEngine;

public class Example : MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSplashScreen)]
	static void Initialize()
	{
	#if !UNITY_EDITOR
		m_LogCallback = msg => UnityEngine.Debug.LogError( "[Native] " + msg);
		LibWindows.SetLogCallback( m_LogCallback);
		LibWindows.Initialize( System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle, 2);
	#endif
	}
	void OnDestroy()
	{
		LibWindows.Terminate();
	}
	void Update()
	{
		if( Input.GetKeyUp( KeyCode.Alpha1) != false)
		{
			m_UI1.Enable();
		}
		else if( Input.GetKeyUp( KeyCode.Keypad1) != false)
		{
			m_UI1.Disable();
		}
		else if( Input.GetKeyUp( KeyCode.Alpha2) != false)
		{
			m_UI2.Enable();
		}
		else if( Input.GetKeyUp( KeyCode.Keypad2) != false)
		{
			m_UI2.Disable();
		}
	}
	[SerializeField]
	SubWindow m_UI1;
	[SerializeField]
	SubWindow m_UI2;
	
	static LibWindows.LogDelegate m_LogCallback;
}

