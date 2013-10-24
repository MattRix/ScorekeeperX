using UnityEngine;
using System;
using System.Collections.Generic;

public class Box : FSprite
{
	public Box() : base("Box")
	{

	}
	
	public void SetSize(float width, float height)
	{
		this.width = width;
		this.height = height; 
	}

}

