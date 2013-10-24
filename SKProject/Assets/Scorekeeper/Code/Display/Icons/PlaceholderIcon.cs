using UnityEngine;
using System;
using System.Collections.Generic;

public class PlaceholderIcon : Content
{
	public FSprite sprite;

	public PlaceholderIcon()
	{
		
	}
	
	override public void Init(Box box)
	{
		this.box = box;

		AddChild(sprite = new FSprite("Icons/Placeholder"));
		sprites.Add(sprite);
	}
}

