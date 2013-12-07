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
	public FSprite sprite1;
	public FSprite sprite2;
	public FSprite sprite3;

	SortType _sortType = null;
	float _spin = 0;
	bool _isFirstTime = true;
	
	public SortBox()
	{
		base.Init(Player.NullPlayer);

		contentContainer.AddChild(sprite1 = new FSprite("Icons/Sort_1"));
		contentContainer.AddChild(sprite2 = new FSprite("Icons/Sort_2"));
		contentContainer.AddChild(sprite3 = new FSprite("Icons/Sort_3"));

		contentSprites.Add(sprite1);
		contentSprites.Add(sprite2);
		contentSprites.Add(sprite3);

		sprite1.color = Color.black;
		sprite2.color = Color.black;
		sprite3.color = Color.black;

		_sortType = SKDataManager.sortType;

		UpdateSortType();
	}

	void UpdateSortType()
	{
		float destSpin = 0;

		if(_sortType == SortType.HighestAtTop)
		{
			destSpin = 180.0f;
		}

		if(_isFirstTime)
		{
			_isFirstTime = false;
			_spin = destSpin;
			UpdateSpin();
		}
		else
		{
			UpdateSpin();
			Go.to(this,0.5f,new TweenConfig().floatProp("spin",destSpin).expoInOut());
		}
	}

	void UpdateSpin()
	{
		float baseX = -1;
		float baseY = -1;

		float radius1 = 15.0f;
		float radius3 = 17.0f;
		float spinRads = _spin * RXMath.DTOR + RXMath.HALF_PI;

		sprite1.SetPosition(baseX+Mathf.Cos(spinRads)*radius1,baseY+Mathf.Sin(spinRads)*radius1);
		sprite2.SetPosition(baseX,baseY);
		sprite3.SetPosition(baseX-Mathf.Cos(spinRads)*radius3,baseY-Mathf.Sin(spinRads)*radius3);
	}

	public float spin
	{
		get {return _spin;}
		set {if(_spin != value) {_spin = value; UpdateSpin();}}
	}

	public SortType sortType
	{
		get {return _sortType;}
		set {if(_sortType != value) {_sortType = value; UpdateSortType();}}
	}
}

