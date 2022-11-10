#pragma once

#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>
#include <windows.h>
#include <assert.h>



#pragma warning(push)
#pragma warning(disable:4005)

#include <d3d11.h>
#include <d3dx9.h>
#include <d3dx11.h>

#pragma warning(pop)



#pragma comment (lib, "winmm.lib")
#pragma comment (lib, "d3d11.lib")
#pragma comment (lib, "d3dx9.lib")
#pragma comment (lib, "d3dx11.lib")


#define SCREEN_WIDTH	(960)
#define SCREEN_HEIGHT	(540)


HWND GetWindow();




extern "C" {
	__declspec(dllexport) void __cdecl Init();
	__declspec(dllexport) void __cdecl Cleanup();
	__declspec(dllexport) void __cdecl Render(void * Resource, bool NewSurface);


	__declspec(dllexport) void __cdecl SetObjectPosition(const char* ObjectName, D3DXVECTOR3 Position);
	__declspec(dllexport) void __cdecl SetObjectRotation(const char* ObjectName, D3DXVECTOR3 Rotation);
	__declspec(dllexport) void __cdecl SetObjectScale(const char* ObjectName, D3DXVECTOR3 Scale);

	__declspec(dllexport) D3DXVECTOR3 __cdecl GetObjectPosition(const char* ObjectName);
	__declspec(dllexport) D3DXVECTOR3 __cdecl GetObjectRotation(const char* ObjectName);
	__declspec(dllexport) D3DXVECTOR3 __cdecl GetObjectScale(const char* ObjectName);


	__declspec(dllexport) void __cdecl AddObject(const char* ObjectName, const char* FileName);
}
