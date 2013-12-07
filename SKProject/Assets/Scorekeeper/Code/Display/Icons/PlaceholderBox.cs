using UnityEngine;
using System;
using System.Collections.Generic;

public class PlaceholderBox : Box
{
	public FSprite iconSprite;

	public PlaceholderBox()
	{
		base.Init(Player.NullPlayer);
		contentContainer.AddChild(iconSprite = new FSprite("Icons/Placeholder"));
		contentSprites.Add(iconSprite);
		iconSprite.color = Color.black;
	}
}

public class ResetBox : Box
{
	public FSprite iconSprite;
	
	public ResetBox()
	{
		base.Init(Player.NullPlayer);
		contentContainer.AddChild(iconSprite = new FSprite("Icons/Reset"));
		contentSprites.Add(iconSprite);
		iconSprite.color = Color.black;
	}

	public void DoTapAnimation()
	{
		Go.to(iconSprite, 0.5f, new TweenConfig().rotation(720.0f).setEaseType(EaseType.ExpoIn));
	}
}

public class VolumeBox : Box
{
	public FSprite iconSprite;
	
	public VolumeBox()
	{
		base.Init(Player.NullPlayer);
		contentContainer.AddChild(iconSprite = new FSprite("Icons/Volume_Main"));
		contentSprites.Add(iconSprite);
		iconSprite.color = Color.black;
	}
}

public class NewPlayerBox : Box
{
	public FSprite iconSprite;
	
	public NewPlayerBox()
	{
		base.Init(Player.NullPlayer);
		contentContainer.AddChild(iconSprite = new FSprite("Icons/NewPlayer_Body"));
		contentSprites.Add(iconSprite);
		iconSprite.color = Color.black;
	}
}

public class SortBox : Box
{
	public FSprite iconSprite;
	
	public SortBox()
	{
		base.Init(Player.NullPlayer);
		contentContainer.AddChild(iconSprite = new FSprite("Icons/Sort_1"));
		contentSprites.Add(iconSprite);
		iconSprite.color = Color.black;
	}
}

