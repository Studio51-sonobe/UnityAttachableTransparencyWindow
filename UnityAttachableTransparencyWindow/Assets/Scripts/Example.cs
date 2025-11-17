
using System;
using UnityEngine;

public class Example : MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSplashScreen)]
	static void Initialize()
	{
		/* ウィンドウの透過設定を行っているが、PAINT の方が早くて一瞬不透明色が出る
		 * 初動から完全に透明にするなら別のプロセスから呼び出して設定した方が確実で楽
		 */
		IntPtr hWnd = Win32.GetSelfViewWindow();
		
		if( hWnd != IntPtr.Zero)
		{
		#if UNITY_EDITOR
			Win32.SetWindowPos( hWnd, Win32.HWND_TOPMOST, 0, 0, 0, 0, Win32.SWP_NOSIZE | Win32.SWP_NOMOVE);
		#else
			Win32.SetWindowLong( hWnd, Win32.GWL_STYLE, Win32.WS_VISIBLE | Win32.WS_POPUP);
			Win32.SetWindowLong( hWnd, Win32.GWL_EXSTYLE, Win32.WS_EX_LAYERED);
			Win32.SetWindowPos( hWnd, Win32.HWND_TOPMOST, 0, 0, Screen.width, Screen.height, Win32.SWP_NOMOVE);
			/* 少なくとも黒で抜かないことで LUT や PP による変化があることを想定できる環境にしておく */
			Win32.SetLayeredWindowAttributes( hWnd, 0x00ffffff, 0, Win32.LWA_COLORKEY);
		#endif
		}
	}
	void OnDestroy()
	{
		DetachWindow();
		
	#if UNITY_EDITOR
		IntPtr hWnd = Win32.GetSelfViewWindow();
		
		if( hWnd != IntPtr.Zero)
		{
			Win32.SetWindowPos( hWnd, Win32.HWND_NOTOPMOST, 0, 0, 0, 0, Win32.SWP_NOSIZE | Win32.SWP_NOMOVE);
		}
	#endif
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
		if( Input.GetKeyDown( KeyCode.Escape) != false)
		{
			Application.Quit();
		}
		else if( m_DragPoint.HasValue == false)
		{
			if( Input.GetMouseButtonDown( 0) != false)
			{
				DetachWindow();
				
				if( Win32.GetCursorPos( out Win32.POINT point) != 0)
				{
					m_DragPoint = point;
				}
			}
		}
		else if( Win32.GetCursorPos( out Win32.POINT point) != 0)
		{
			int deltaX = point.x - m_DragPoint.Value.x;
			int deltaY = point.y - m_DragPoint.Value.y;
			
			if( deltaX != 0 || deltaY != 0)
			{
				IntPtr hWnd = Win32.GetSelfViewWindow();
				if( hWnd != IntPtr.Zero)
				{
					if( Win32.GetWindowRect( hWnd, out Win32.RECT rect) != 0)
					{
						int x = rect.left + deltaX;
						int y = rect.top + deltaY;
						int width = rect.right - rect.left;
						int height = rect.bottom - rect.top;
						Win32.MoveWindow( hWnd, x, y, width, height, 0);
					}
				}
			}
			if( Input.GetMouseButton( 0) != false)
			{
				m_DragPoint = point;
			}
			else
			{
				m_DragPoint = null;
				
				if( m_AttachedWindowHandle == IntPtr.Zero)
				{
					Win32.EnumWindows( FindWindow, IntPtr.Zero);
				}
			}
		}
	}
	[AOT.MonoPInvokeCallback( typeof( Win32.WNDENUMPROC))]
	static bool FindWindow( IntPtr hTargetWnd, IntPtr lParam)
	{
		IntPtr hSelfWindow = Win32.GetSelfViewWindow();
		
		if( hSelfWindow != IntPtr.Zero)
		{
			if( Win32.GetWindowRect( hSelfWindow, out Win32.RECT currentRect) != 0)
			{
				if( hTargetWnd != hSelfWindow && Win32.IsWindowVisible( hTargetWnd) != false)
				{
					int style = Win32.GetWindowLong( hTargetWnd, Win32.GWL_STYLE);
					
					/* スタイルによるフィルタリングは適当 */
					if( (style & WS_ATTACH_STYLE) == WS_ATTACH_STYLE)
					{
						if( Win32.GetWindowRect( hTargetWnd, out Win32.RECT targetRect) != 0)
						{
							if( currentRect.Contains( ref targetRect) != false)
							{
								AttachWindow( hTargetWnd, ref targetRect);
								return false;
							}
						}
					}
				}
			}
		}
		return true;
	}
	static void DetachWindow()
	{
		if( m_AttachedHookHandle != IntPtr.Zero)
		{
			Win32.UnhookWinEvent( m_AttachedHookHandle);
			m_AttachedHookHandle = IntPtr.Zero;
		}
		if( m_AttachedWindowHandle != IntPtr.Zero)
		{
			m_AttachedWindowHandle = IntPtr.Zero;
		}
	}
	static void AttachWindow( IntPtr hWnd, ref Win32.RECT rect)
	{
		DetachWindow();
		
		if( hWnd != IntPtr.Zero)
		{
			if( Win32.GetWindowThreadProcessId( hWnd, out uint processId) != 0)
			{
				m_AttachedHookHandle = Win32.SetWinEventHook( 
					Win32.EVENT_OBJECT_DESTROY,
					Win32.EVENT_OBJECT_LOCATIONCHANGE,
					IntPtr.Zero,
					EventCallback, 
					processId, 0, 
					Win32.WINEVENT_OUTOFCONTEXT);
			}
		}
		if( m_AttachedHookHandle != IntPtr.Zero)
		{
			m_AttachedWindowRect = rect;
			m_AttachedWindowHandle = hWnd;
		}
	}
	[AOT.MonoPInvokeCallback( typeof( Win32.WINEVENTPROC))]
	static void EventCallback( IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
	{
		if( idObject != Win32.OBJID_WINDOW || hWnd == IntPtr.Zero)
		{
			return;
		}
		IntPtr rWnd = Win32.GetAncestor( hWnd, Win32.GA_ROOT);
		
		if( rWnd == IntPtr.Zero)
		{
			rWnd = Win32.GetAncestor( hWnd, Win32.GA_ROOTOWNER);
		}
		if( rWnd != m_AttachedWindowHandle)
		{
			return;
		}
		switch( eventType)
		{
			case Win32.EVENT_OBJECT_DESTROY:
			case Win32.EVENT_OBJECT_HIDE:
			{
				DetachWindow();
				break;
			}
			case Win32.EVENT_OBJECT_LOCATIONCHANGE:
			{
				if( Win32.GetWindowRect( hWnd, out Win32.RECT targetRect) != 0)
				{
					IntPtr hSelfWindow = Win32.GetSelfViewWindow();
					
					if( hSelfWindow != IntPtr.Zero && Win32.GetWindowRect( hSelfWindow, out Win32.RECT selfRect) != 0)
					{
						int deltaLeft = targetRect.left - m_AttachedWindowRect.left;
						int deltaTop = targetRect.top - m_AttachedWindowRect.top;
						int deltaRight = targetRect.right - m_AttachedWindowRect.right;
						int deltaBottom = targetRect.bottom - m_AttachedWindowRect.bottom;
						
						/* 移動 */
						if( deltaLeft == deltaRight && deltaTop == deltaBottom)
						{
							int x = selfRect.left + deltaLeft;
							int y = selfRect.top + deltaTop;
							int width = selfRect.right - selfRect.left;
							int height = selfRect.bottom - selfRect.top;
							Win32.MoveWindow( hSelfWindow, x, y, width, height, 0);
						}
						/* サイズ変更 */
						else
						{
							/* 各々実装 */
							if( selfRect.Contains( ref targetRect) == false)
							{
								DetachWindow();
							}
						}
					}
					m_AttachedWindowRect = targetRect;
				}
				break;
			}
		}
	}
	const int WS_ATTACH_STYLE = 
		Win32.WS_OVERLAPPED
	|	Win32.WS_CAPTION
	|	Win32.WS_THICKFRAME
	|	Win32.WS_MINIMIZEBOX
	|	Win32.WS_MAXIMIZEBOX;
	
	[SerializeField]
	Transform m_Cube;
	
	static Win32.POINT? m_DragPoint;
	static Win32.RECT m_AttachedWindowRect;
	static IntPtr m_AttachedHookHandle = IntPtr.Zero;
	static IntPtr m_AttachedWindowHandle = IntPtr.Zero;
}

