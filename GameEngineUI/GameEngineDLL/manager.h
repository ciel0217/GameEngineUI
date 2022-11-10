#pragma once

#include <string>
#include <unordered_map>

#include "gameObject.h"



class Manager
{
private:
	static std::unordered_map<std::string, GameObject*> m_GameObject;


public:
	static void Init();
	static void Uninit();
	static void Update();
	static void Draw(void * Resource, bool NewSurface);


	static void AddGameObject(const char* ObjectName, const char* FileName);

	static GameObject* GetGameObject(const char* Name)
	{
		return m_GameObject[Name];
	}
};