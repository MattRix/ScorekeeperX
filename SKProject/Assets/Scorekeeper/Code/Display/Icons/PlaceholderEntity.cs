using UnityEngine;
using System;
using System.Collections.Generic;

public class PlaceholderEntity : Entity
{
	public FSprite tempSprite;

	public PlaceholderEntity()
	{
		base.Init(Player.NullPlayer);
		contentContainer.AddChild(tempSprite = new FSprite("Icons/Placeholder"));
		contentSprites.Add(tempSprite);
	}
}

