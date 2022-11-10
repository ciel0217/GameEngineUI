#include "main.h"
#include "manager.h"
#include "renderer.h"
#include "camera.h"

void Camera::Init()
{
	m_Position = D3DXVECTOR3(0.0f, 2.0f, -5.0f);
	m_Rotation = D3DXVECTOR3(0.0f, 0.0f, 0.0f);
}


void Camera::Uninit()
{

}

void Camera::Update()
{

}

void Camera::Draw()
{
	//ビューマトリクス設定
	D3DXMATRIX viewMatrix;
	D3DXMATRIX world, rot, trans;
	D3DXMatrixRotationYawPitchRoll(&rot, m_Rotation.y, m_Rotation.x, m_Rotation.z);
	D3DXMatrixTranslation(&trans, m_Position.x, m_Position.y, m_Position.z);
	world = rot * trans;
	D3DXMatrixInverse(&viewMatrix, NULL, &world);

	Renderer::SetViewMatrix(&viewMatrix);


	//プロジェクションマトリクス設定
	D3DXMATRIX projectionMatrix;
	D3DXMatrixPerspectiveFovLH(&projectionMatrix, 1.0f,
		(float)Renderer::GetScreenWidth() / Renderer::GetScreenHeight(), 0.1f, 100.0f);

	Renderer::SetProjectionMatrix(&projectionMatrix);
}

