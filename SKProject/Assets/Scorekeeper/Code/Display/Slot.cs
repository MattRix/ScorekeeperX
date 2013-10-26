using UnityEngine;
using System;
using System.Collections.Generic;

public class Slot : FContainer
{
	public Player player;
	public NameBox nameBox;

	public Slot(Player player)
	{
		this.player = player;
		this.nameBox = new NameBox(this);
		AddChild(nameBox);

		nameBox.AddChild(new FSprite("Icons/Placeholder"));
	}

	public void DoLayout()
	{
		float padding = Config.PADDING_XS+1;

	}
}

public class NameBox : Box
{
	public Slot slot;
	public FSprite tempSprite;
	
	public NameBox(Slot slot)
	{
		this.slot = slot;

		base.Init(Player.NullPlayer);

		contentContainer.AddChild(tempSprite = new FSprite("Icons/Placeholder"));
		contentSprites.Add(tempSprite);
	}
}