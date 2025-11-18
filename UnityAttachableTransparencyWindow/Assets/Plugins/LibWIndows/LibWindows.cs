using System;
using System.Runtime.InteropServices;

public static class LibWindows
{
	[UnmanagedFunctionPointer( CallingConvention.Winapi)]
    public delegate void LogDelegate( string message);
	
	[DllImport( "LibWindows", CallingConvention = CallingConvention.Winapi)]
	public static extern void NOP();
	[DllImport( "LibWindows", CallingConvention = CallingConvention.Winapi)]
	public static extern void SetLogCallback( LogDelegate logCallback);
	[DllImport( "LibWindows", CallingConvention = CallingConvention.Winapi)]
	public static extern void Initialize( IntPtr hWnd);
	[DllImport( "LibWindows", CallingConvention = CallingConvention.Winapi)]
	public static extern void Terminate();
	[DllImport( "LibWindows", CallingConvention = CallingConvention.Winapi)]
	public static extern uint CreateSubWindow( IntPtr texturePtr, int width, int height);
	
	// public static extern void DisposeSubWindow( uint handle);
	// public static extern bool TryGetSubWindowInfo( uint handle, out Info info);
}
