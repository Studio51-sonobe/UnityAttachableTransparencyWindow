
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public static class Win32
{
/* ウィンドウスタイル */
	/* ウィンドウは重なり合ったウィンドウです。 
	 * オーバーラップ ウィンドウには、タイトル バーと境界線があります。 
	 * WS_TILED スタイルと同じです。 */
	public const int WS_OVERLAPPED = 0x00000000;
	/* ウィンドウには最大化ボタンがあります。 
	 * WS_EX_CONTEXTHELP スタイルと組み合わせることはできません。 
	 WS_SYSMENU スタイルも指定する必要があります。 */
	public const int WS_MAXIMIZEBOX = 0x00010000;
	/* ウィンドウには最小化ボタンがあります。 
	 * WS_EX_CONTEXTHELP スタイルと組み合わせることはできません。 
	 * WS_SYSMENU スタイルも指定する必要があります。 */
	public const int WS_MINIMIZEBOX = 0x00020000;
	/* ウィンドウにはサイズ設定の境界線があります。 
	 * WS_SIZEBOX スタイルと同じです。 */
	public const int WS_THICKFRAME = 0x00040000;
	/* ウィンドウには、タイトル バーにウィンドウ メニューがあります。 
	 * WS_CAPTION スタイルも指定する必要があります。 */
	public const int WS_SYSMENU = 0x00080000;
	/* ウィンドウにはタイトル バーがあります (WS_BORDER スタイルが含まれます)。 */
	public const int WS_CAPTION = 0x00C00000;
	/* ウィンドウは最初に表示されます。
	 * このスタイルのオンとオフを切り替えるには、ShowWindow 関数または SetWindowPos 関数を使用します。 */
	public const int WS_VISIBLE = 0x10000000;
	/* ウィンドウは重なり合ったウィンドウです。 
	 * WS_TILEDWINDOW スタイルと同じです。 */
	public const int WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;
	
/* 拡張ウィンドウスタイル */
	/* ウィンドウはポップアップ ウィンドウです。 
	 * このスタイルは、WS_CHILD スタイルでは使用できません。 */
	public const uint WS_POPUP = 0x80000000;
	/* ウィンドウは、レイヤーウィンドウです。 
	 * ウィンドウに CS_OWNDC または CS_CLASSDCのいずれかの クラス スタイル がある場合、このスタイルは使用できません。
	 * Windows 8: 最上位のウィンドウと子ウィンドウでは、WS_EX_LAYERED スタイルがサポートされています。 
	 * 以前のバージョンの Windows では、最上位のウィンドウに対してのみ WS_EX_LAYERED がサポートされています。*/
	public const uint WS_EX_LAYERED = 0x00080000;
	
/* SetWindowLong nIndex */
	/* 新しい 拡張ウィンドウ スタイルのを設定します。 */
	public const int GWL_EXSTYLE = -20;
	/* 新しい ウィンドウ スタイルのを設定します。 */
	public const int GWL_STYLE = -16;
	
/* SetLayeredWindowAttributes dwFlags */
	/* 透明度の色として crKey を使用します。 */
	public const int LWA_COLORKEY = 0x00000001;
	/* bAlpha を使用して、レイヤーウィンドウの不透明度を決定します。 */
	public const int LWA_ALPHA = 0x00000002;
	
/* SetWindowPos hWndInsertAfter */
	/* 最上位以外のすべてのウィンドウ (つまり、すべての最上位ウィンドウの背後) の上にウィンドウをPlacesします。 
	 * ウィンドウが既に最上位以外のウィンドウである場合、このフラグは無効です。 */
	public static readonly IntPtr HWND_NOTOPMOST = new( -2);
	/* 最上位以外のすべてのウィンドウの上にウィンドウをPlacesします。 
	 * 非アクティブ化された場合でも、ウィンドウは最上位の位置を維持します。*/
	public static readonly IntPtr HWND_TOPMOST = new( -1);
	/* Z オーダーの上部にあるウィンドウをPlacesします。 */
	public static readonly IntPtr HWND_TOP = new( 0);
	/* Z オーダーの下部にあるウィンドウをPlacesします。 
	 * hWnd パラメーターが一番上のウィンドウを識別する場合、ウィンドウは最上位の状態を失い、他のすべてのウィンドウの下部に配置されます。 */
	public static readonly IntPtr HWND_BOTTOM = new( 1);
	
/* SetWindowPos uFlags */
	/* 現在のサイズを保持します ( cx パラメーターと cy パラメーターは無視されます)。  */
	public const uint SWP_NOSIZE = 0x0001;
	/* 現在位置を保持します ( X パラメーターと Y パラメーターは無視されます)。 */
	public const uint SWP_NOMOVE = 0x0002;
	
/* SetWinEventHook dwFlags */
	public const uint WINEVENT_OUTOFCONTEXT = 0x0000;
	public const uint WINEVENT_SKIPOWNPROCESS = 0x0002;
	
/* GetAncestor gaFlags */
	/* 親ウィンドウのチェーンを歩いてルート ウィンドウを取得します。  */
	public const uint GA_ROOT = 2;
	/* GetParent によって返される親ウィンドウと所有者ウィンドウのチェーンを移動して、所有ルート ウィンドウを取得します。 */
	public const uint GA_ROOTOWNER = 3;
	
/* イベント定数 */
	public const uint EVENT_OBJECT_DESTROY = 0x8001;
	public const uint EVENT_OBJECT_SHOW = 0x8002;
	public const uint EVENT_OBJECT_HIDE = 0x8003;
	public const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;
	
/* オブジェクト識別子 */
	public const int OBJID_WINDOW = 0;
	
	public struct POINT
	{
		public int x;
		public int y;
	}
	public struct RECT
	{
		public int left;
		public int top;
		public int right;
		public int bottom;
		
		public readonly bool Contains( ref RECT rect)
		{
			return left < rect.right
				&& right > rect.left
				&& top < rect.bottom
				&& bottom > rect.top;
		}
		public override readonly string ToString()
		{
			return $"({left}, {top}, {right}, {bottom})";
		}
	}
	[UnmanagedFunctionPointer( CallingConvention.StdCall)]
	public delegate bool WNDENUMPROC( IntPtr hWnd, IntPtr lParam);
	[UnmanagedFunctionPointer( CallingConvention.StdCall)]
	public delegate void WINEVENTPROC( IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
	
	public static bool EnumWindowsEx( WNDENUMPROC lpEnumFunc, IntPtr lParam)
	{
		var handle = GCHandle.Alloc( lpEnumFunc);
		bool ret = EnumWindows( lpEnumFunc, lParam);
		handle.Free();
		return ret;
	}
	public static IntPtr GetSelfViewWindow()
	{
	#if UNITY_EDITOR
		uint unityPid = (uint)Process.GetCurrentProcess().Id;
		IntPtr found = IntPtr.Zero;
		
		EnumWindowsEx( (hWnd, lParam) =>
		{
			if( GetWindowThreadProcessId( hWnd, out uint pid) == 0)
			{
				return true;
			}
			if( pid != unityPid)
			{
				return true;
			}
			if( GetWindowText( hWnd) != "Game")
			{
				return true;
			}
			found = hWnd;
			return false;
		}, IntPtr.Zero);
		
		return found;
	#else
		return Process.GetCurrentProcess().MainWindowHandle;
	#endif
	}
	public static string GetWindowText( IntPtr hWnd)
	{
		var builder = new System.Text.StringBuilder( 256);
		GetWindowText( hWnd, builder, builder.Capacity);
		return builder.ToString();
	}
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
	[DllImport("user32.dll")]
	public static extern uint GetWindowThreadProcessId( IntPtr hWnd, out uint lpdwProcessId);
	[DllImport("user32.dll")]
	public static extern bool EnumWindows( WNDENUMPROC lpEnumFunc, IntPtr lParam);
	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr FindWindow( string lpClassName, string lpWindowName);
	[DllImport("user32.dll")]
	public static extern IntPtr GetActiveWindow();
	[DllImport("user32.dll")]
	public static extern bool IsWindowVisible( IntPtr hWnd);
	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern int GetWindowText( IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);
	[DllImport("user32.dll")]
	public static extern int GetWindowLong( IntPtr hWnd, int nIndex);
	[DllImport("user32.dll")]
	public static extern int SetWindowLong( IntPtr hWnd, int nIndex, uint dwNewLong);
	[DllImport("user32.dll")]
	public static extern bool SetForegroundWindow( IntPtr hWnd);
	[DllImport("user32.dll")]
	public static extern int SetLayeredWindowAttributes( IntPtr hWnd, int crKey, byte bAlpha, uint dwFlags);
	[DllImport("user32.dll", SetLastError = true)]
	public static extern int SetWindowPos( IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
	[DllImport("user32.dll")]
	public static extern int GetWindowRect( IntPtr hWnd, out RECT lpRect);
	[DllImport("user32.dll")]
	public static extern int MoveWindow( IntPtr hWnd, int x, int y, int nWidth, int nHeight, int bRepaint);
	[DllImport("user32.dll")]
	public static extern int GetCursorPos( out POINT lpPoint);
	[DllImport("user32.dll")]
	public static extern IntPtr SetWinEventHook( uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WINEVENTPROC pfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
	[DllImport("user32.dll")]
	public static extern bool UnhookWinEvent( IntPtr hWinEventHook);
	[DllImport("user32.dll")]
	public static extern IntPtr GetAncestor( IntPtr hWnd, uint gaFlags);
	
#else
	public static uint GetWindowThreadProcessId( IntPtr hWnd, out uint lpdwProcessId){ return 0; }
	public static bool EnumWindows( WNDENUMPROC lpEnumFunc, IntPtr lParam){ return false; }
	public static IntPtr FindWindow( string lpClassName, string lpWindowName){ return IntPtr.Zero; }
	public static IntPtr GetActiveWindow(){ return IntPtr.Zero; }
	public static bool IsWindowVisible( IntPtr hWnd){ return false;}
	public static int GetWindowText( IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount){ return 0; }
	public static int GetWindowLong( IntPtr hWnd, int nIndex){ return 0; }
	public static int SetWindowLong( IntPtr hWnd, int nIndex, uint dwNewLong){ return 0; }
	public static bool SetForegroundWindow( IntPtr hWnd){ return false; }
	public static int SetLayeredWindowAttributes( IntPtr hWnd, int crKey, byte bAlpha, uint dwFlags){ return 0; }
	public static int SetWindowPos( IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags){ return 0; }
	public static int GetWindowRect( IntPtr hWnd, out RECT lpRect){ lpRect = new RECT{ 0, 0, 0, 0 } return 0; }
	public static int MoveWindow( IntPtr hWnd, int x, int y, int nWidth, int nHeight, int bRepaint){ return 0; }
	public static int GetCursorPos( out POINT lpPoint){ lpPoint = new POINT{ 0, 0 } return 0; }
	public static IntPtr SetWinEventHook( uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WINEVENTPROC pfnWinEventProc, uint idProcess, uint idThread, uint dwFlags){ return IntPtr.Zero; }
	public static bool UnhookWinEvent( IntPtr hWinEventHook){ return true; }
	public static IntPtr GetAncestor( IntPtr hWnd, uint gaFlags){ return IntPtr.Zero; }
#endif
}
