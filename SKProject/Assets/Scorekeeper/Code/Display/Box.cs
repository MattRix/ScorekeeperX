using UnityEngine;
using System;
using System.Collections.Generic;

public class Box : FSprite
{
	public Entity entity;
	
	public Box(Entity entity) : base("Box")
	{
		this.entity = entity;
	}
	
	public void SetSize(float width, float height)
	{
		this.width = width;
		this.height = height; 
	}

}

