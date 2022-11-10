#include "main.h"
#include "manager.h"
#include "renderer.h"
#include "polygon2D.h"
#include "field.h"
#include "camera.h"
#include "model.h"
#include "player.h"



std::unordered_map<std::string, GameObject*> Manager::m_GameObject;



void Manager::Init()
{
	Renderer::Init();

	m_GameObject["Camera"] = new Camera();
	m_GameObject["Camera"]->Init();

	m_GameObject["Field"] = new Field();
	m_GameObject["Field"]->Init();


}

void Manager::Uninit()
{
	for (auto object : m_GameObject)
	{
		object.second->Uninit();
		delete object.second;
	}

	Renderer::Uninit();
}

void Manager::Update()
{
	for (auto object : m_GameObject)
	{
		object.second->Update();
	}
}

void Manager::Draw(void * Resource, bool NewSurface)
{
	Renderer::Begin(Resource, NewSurface);
	
	for (auto object : m_GameObject)
	{
		object.second->Draw();
	}

	Renderer::End();
}


void Manager::AddGameObject(const char* ObjectName, const char* FileName)
{
	Player* player = new Player();
	player->Init();
	player->Load(FileName);

	m_GameObject[ObjectName] = player;
}
