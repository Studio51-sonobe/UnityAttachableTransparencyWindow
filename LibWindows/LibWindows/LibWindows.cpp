
#include "pch.h"
#include "LibWindows.h"
#include <stdio.h>
#pragma comment(lib, "d3d11.lib")

#define WS_DEFAULT_STYLE	(WS_VISIBLE | WS_POPUP)
#define WS_DEFAULT_EXSTYLE	(WS_EX_LAYERED/* | WS_EX_TRANSPARENT*/)
#define SUB_WND_CLASS_NAME	L"UnitySubWnd"

typedef struct _TSubWindow
{
	HWND hWnd;
	ID3D11Texture2D *pTexture;
	ID3D11Device *pDevice;
	ID3D11DeviceContext *pContext;
	IDXGISwapChain *pSwapChain;
	HANDLE pThreadHandle;
	BOOL threadKeep;
}TSubWindow;

static LogCallback s_LogCallback = NULL;
static HWND s_CurrentWindowHandle = NULL;
static WNDPROC s_DefaultWindowProc = NULL;
static TSubWindow *s_pSubWindows = NULL;
static int s_SubWindowMaxCount = 1;

void Log( const char *p)
{
	if( s_LogCallback != NULL)
	{
		s_LogCallback(p);
	}
	printf(p);
	OutputDebugStringA( p);
}
void DLL_API NOP()
{
}
void DLL_API SetLogCallback( LogCallback logCallback)
{
	s_LogCallback = logCallback;
}
LRESULT CALLBACK WndProc( HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
	switch( msg)
	{
		case WM_MOVE:
		{
			Log( "WM_MOVE");
			break;
		}
		case WM_SIZE:
		{
			Log( "WM_SIZE");
			break;
		}
	}
	return CallWindowProc( s_DefaultWindowProc, hWnd, msg, wParam, lParam);
}
void DLL_API Initialize( HWND hWnd)
{
	if( s_pSubWindows == NULL)
	{
		s_pSubWindows = (TSubWindow *)malloc( sizeof( TSubWindow) * s_SubWindowMaxCount);

		if(s_pSubWindows != NULL)
		{
			memset( s_pSubWindows, 0, sizeof( TSubWindow) * s_SubWindowMaxCount);
		}
	}
	if( s_CurrentWindowHandle == NULL && hWnd != NULL)
	{
		s_CurrentWindowHandle = hWnd;
		s_DefaultWindowProc = (WNDPROC)SetWindowLongPtrA(
			s_CurrentWindowHandle, GWLP_WNDPROC, (LONG_PTR)WndProc);

		//COLORREF cref = { 0 };
		//SetLayeredWindowAttributes( s_CurrentWindowHandle, cref, 0xff, LWA_ALPHA);

		//MARGINS margins0 = { 0, 0, 0, 0 };
		//DwmExtendFrameIntoClientArea( s_CurrentWindowHandle, &margins0);

		MARGINS margins1 = { -1 };
		DwmExtendFrameIntoClientArea( s_CurrentWindowHandle, &margins1);

		SetWindowLongA( s_CurrentWindowHandle, GWL_STYLE, WS_DEFAULT_STYLE);

		LONG exstyle = GetWindowLongA( s_CurrentWindowHandle, GWL_EXSTYLE);
		exstyle |= WS_DEFAULT_EXSTYLE;
		SetWindowLongA( s_CurrentWindowHandle, GWL_EXSTYLE, exstyle);

		SetWindowPos( s_CurrentWindowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE);

		WNDCLASSEXW wcex;
		memset(&wcex, 0, sizeof(wcex));
		wcex.cbSize = sizeof(WNDCLASSEXW);
		wcex.style = CS_HREDRAW | CS_VREDRAW;
		wcex.lpfnWndProc = WndProc;
		wcex.hInstance = GetModuleHandle( NULL);
		wcex.lpszClassName = SUB_WND_CLASS_NAME;
		wcex.hbrBackground = reinterpret_cast<HBRUSH>( COLOR_WINDOW + 1);
		RegisterClassExW( &wcex);
	}
}
void DLL_API Terminate()
{
	if( s_pSubWindows != NULL)
	{
		for( int i0 = 0; i0 < s_SubWindowMaxCount; ++i0)
		{
			TSubWindow *pWindow = &s_pSubWindows[ i0];

			if( pWindow != NULL && pWindow->pThreadHandle != NULL)
			{
				pWindow->threadKeep = FALSE;
				WaitForSingleObject( pWindow->pThreadHandle, INFINITE);
			}
		}
		free( s_pSubWindows);
		s_pSubWindows = NULL;
	}
	if( s_DefaultWindowProc != NULL)
	{
		SetWindowLongPtrA( s_CurrentWindowHandle, 
			GWLP_WNDPROC, (LONG_PTR)s_DefaultWindowProc);
		s_DefaultWindowProc = NULL;
	}
}
TSubWindow *GetEmptyWindow()
{
	TSubWindow *pWindow = NULL;

	for( int i0 = 0; i0 < s_SubWindowMaxCount; ++i0)
	{
		pWindow = &s_pSubWindows[ i0];

		if( pWindow->hWnd == NULL)
		{
			return pWindow;
		}
	}
	return NULL;
}

DWORD WINAPI RenderThread( void *pArgs)
{
	TSubWindow *pWindow = (TSubWindow *)pArgs;
	ID3D11Texture2D *pBackBuffer;

	while( pWindow->threadKeep != FALSE)
	{
		pWindow->pSwapChain->GetBuffer( 0, __uuidof( ID3D11Texture2D), (void **)&pBackBuffer);
		pWindow->pContext->CopyResource( pBackBuffer, pWindow->pTexture);
		pWindow->pSwapChain->Present( 1, 0);
		pBackBuffer->Release();
		Sleep( 16);
	}
	return 0;
}
DWORD CreateSubWindow( ID3D11Texture2D *pTexture, int width, int height)
{
	if( s_CurrentWindowHandle == NULL)
	{
		return 1407;
	}
	TSubWindow *pWindow = GetEmptyWindow();

	if( pWindow == NULL)
	{
		return 0;
	}
	pWindow->pTexture = pTexture;
	pWindow->hWnd = CreateWindowExW(
		0,
		SUB_WND_CLASS_NAME,
		L"Un",
		WS_DEFAULT_STYLE,
		CW_USEDEFAULT, CW_USEDEFAULT,
		512, 512,
		NULL, NULL,
		GetModuleHandle(NULL),
		NULL);

	if( pWindow->hWnd != NULL)
	{
		MARGINS margins1 = { -1 };
		DwmExtendFrameIntoClientArea( pWindow->hWnd, &margins1);

		DXGI_SWAP_CHAIN_DESC sd = {};
		sd.BufferCount = 1;
		sd.BufferDesc.Width = 512;
		sd.BufferDesc.Height = 512;
		sd.BufferDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
		sd.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
		sd.OutputWindow = pWindow->hWnd;
		sd.SampleDesc.Count = 1;
		sd.Windowed = TRUE;

		HRESULT hr = D3D11CreateDeviceAndSwapChain(
			NULL,
			D3D_DRIVER_TYPE_HARDWARE,
			NULL,
			0,
			NULL, 0,
			D3D11_SDK_VERSION,
			&sd,
			&pWindow->pSwapChain,
			&pWindow->pDevice,
			NULL,
			&pWindow->pContext);

		if( SUCCEEDED( hr))
		{
			ShowWindow( pWindow->hWnd, SW_SHOW);
			pWindow->threadKeep = TRUE;
			pWindow->pThreadHandle = CreateThread( NULL, 0, RenderThread, pWindow, 0, NULL);
		}
	}
	else
	{
		return GetLastError();
	}
	return 0;
}

