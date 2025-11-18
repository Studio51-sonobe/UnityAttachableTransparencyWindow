#pragma once
#include <d3d11.h>

#ifdef LIBUNIWINC_EXPORTS
#define DLL_API __stdcall
#define DLL_EXPORT extern "C" __declspec(dllexport)
#else
#define DLL_API __stdcall
#define DLL_EXPORT extern "C" __declspec(dllimport)
#endif

typedef void(* LogCallback)( const char*);

DLL_EXPORT void DLL_API NOP();
DLL_EXPORT void DLL_API SetLogCallback( LogCallback logCallback);

DLL_EXPORT void DLL_API Initialize( HWND hWnd);
DLL_EXPORT void DLL_API Terminate();
DLL_EXPORT DWORD DLL_API CreateSubWindow( ID3D11Texture2D *pTexture, int width, int height);
