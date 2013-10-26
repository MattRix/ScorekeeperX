using UnityEngine;
using System;
using System.Collections.Generic;

public class PlaceholderBox : Box
{
	public FSprite tempSprite;

	public PlaceholderBox()
	{
		base.Init(Player.NullPlayer);
		contentContainer.AddChild(tempSprite = new FSprite("Icons/Placeholder"));
		contentSprites.Add(tempSprite);
	}
}

