using UnityEngine;
using System;
using System.Collections.Generic;

public class BoxSprite : FSprite
{
	public BoxSprite() : base("Box")
	{

	}
	
	public void SetSize(float width, float height)
	{
		this.width = width;
		this.height = height; 
	}

}

