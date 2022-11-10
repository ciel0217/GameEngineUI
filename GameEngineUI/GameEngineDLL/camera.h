#pragma once


#include "gameObject.h"

class Camera : public GameObject
{

private:


public:
	void Init();
	void Uninit();
	void Update();
	void Draw();


};