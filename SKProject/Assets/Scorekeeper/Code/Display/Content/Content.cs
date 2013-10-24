using UnityEngine;
using System;
using System.Collections.Generic;

public class Content : FContainer
{
	public Box box;

	public List<FSprite> sprites = new List<FSprite>();

	public Content()
	{

	}

	virtual public void Init(Box box)
	{
		this.box = box;


	}
}

