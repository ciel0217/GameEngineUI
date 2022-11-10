

#include "main.h"
#include "manager.h"
#include "camera.h"


const char* CLASS_NAME = "AppClass";
const char* WINDOW_NAME = "DX11ƒQ[ƒ€";


void Init()
{

	Manager::Init();

}

void Cleanup()
{

	Manager::Uninit();

}


void Render(void * Resource, bool NewSurface)
{

	Manager::Update();
	Manager::Draw(Resource, NewSurface);

}



void SetObjectPosition(const char* ObjectName, D3DXVECTOR3 Position)
{
	Manager::GetGameObject(ObjectName)->SetPosition(Position);
}

void SetObjectRotation(const char* ObjectName, D3DXVECTOR3 Rotation)
{
	Manager::GetGameObject(ObjectName)->SetRotation(Rotation);
}

void SetObjectScale(const char* ObjectName, D3DXVECTOR3 Scale)
{
	Manager::GetGameObject(ObjectName)->SetScale(Scale);
}



D3DXVECTOR3 GetObjectPosition(const char* ObjectName)
{
	return Manager::GetGameObject(ObjectName)->GetPosition();
}

D3DXVECTOR3 GetObjectRotation(const char* ObjectName)
{
	return Manager::GetGameObject(ObjectName)->GetRotation();
}

D3DXVECTOR3 GetObjectScale(const char* ObjectName)
{
	return Manager::GetGameObject(ObjectName)->GetScale();
}



void AddObject(const char* ObjectName, const char* FileName)
{
	Manager::AddGameObject(ObjectName, FileName);
}
