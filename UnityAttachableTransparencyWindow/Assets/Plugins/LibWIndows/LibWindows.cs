using System;
using System.Runtime.InteropServices;

public static class LibWindows
{
	[StructLayout( LayoutKind.Sequential)]
	public struct SubWindowEvent
	{
		public int index;
		public uint msg;
		public int x;
		public int y;
	}
	[UnmanagedFunctionPointer( CallingConvention.Winapi)]
    public delegate void LogDelegate( string message);
	[UnmanagedFunctionPointer(CallingConvention.Winapi)]
	public delegate void SubWindowEventCallback( SubWindowEvent msg);
	
	[DllImport( "LibWindows", CallingConvention = CallingConvention.Winapi)]
	public static extern void SetLogCallback( LogDelegate logCallback);
	[DllImport( "LibWindows", CallingConvention = CallingConvention.Winapi)]
	public static extern void Initialize( IntPtr hWnd, int subWindowMaxCount);
	[DllImport( "LibWindows", CallingConvention = CallingConvention.Winapi)]
	public static extern void Terminate();
	[DllImport( "LibWindows", CallingConvention = CallingConvention.Winapi)]
	public static extern int CreateSubWindow( IntPtr texturePtr, int width, int height, SubWindowEventCallback callback);
	[DllImport( "LibWindows", CallingConvention = CallingConvention.Winapi)]
	public static extern void DisposeSubWindow( int windowIndex);
}
