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
		iconSprite.rotation = 0;
		Go.to(iconSprite, 0.5f, new TweenConfig().rotation(720.0f).setEaseType(EaseType.ExpoIn));
	}
}

public class VolumeBox : Box
{
	public FSprite mainSprite;
	public FSliceSprite barASprite;
	public FSliceSprite barBSprite;

	RXTweenable muteTweenable;

	bool _isMuted;
	
	public VolumeBox()
	{
		base.Init(Player.NullPlayer);

		contentContainer.AddChild(mainSprite = new FSprite("Icons/Volume_Main"));
		contentSprites.Add(mainSprite);
		mainSprite.color = Color.black;

		contentContainer.AddChild(barASprite = new FSliceSprite("Icons/Volume_Bar",6,20,8,0,8,0));
		contentSprites.Add(barASprite);
		barASprite.color = Color.black;

		contentContainer.AddChild(barBSprite = new FSliceSprite("Icons/Volume_Bar",6,20,8,0,8,0));
		contentSprites.Add(barBSprite);
		barBSprite.color = Color.black;

		FSoundManager.Init();
		_isMuted = FSoundManager.isMuted;

		//1.0f = muted (X), 0.0f = unmuted (||)
		muteTweenable = new RXTweenable(_isMuted ? 1.0f : 0.0f);
		muteTweenable.SignalChange += HandleMuteChange;
		HandleMuteChange(); 
	}

	void HandleMuteChange ()
	{
		float percent = muteTweenable.amount;

		float barAStartRotation = 0.0f;
		float barAEndRotation = 45.0f;

		float barBStartRotation = 0.0f;
		float barBEndRotation = -45.0f;

		float exX = 5;

		float barAStartX = 2;
		float barAEndX = exX;

		float barBStartX = 9;
		float barBEndX = exX;

		float exHeight = 15;

		float barAStartHeight = 17;
		float barAEndHeight = exHeight;

		float barBStartHeight = 27;
		float barBEndHeight = exHeight;

		barASprite.rotation = barAStartRotation + (barAEndRotation - barAStartRotation) * percent;
		barBSprite.rotation = barBStartRotation + (barBEndRotation - barBStartRotation) * percent;

		barASprite.x = barAStartX + (barAEndX - barAStartX) * percent;
		barBSprite.x = barBStartX + (barBEndX - barBStartX) * percent;

		barASprite.height = barAStartHeight + (barAEndHeight - barAStartHeight) * percent;
		barBSprite.height = barBStartHeight + (barBEndHeight - barBStartHeight) * percent;

		float contentStartX = 7;
		float contentEndX = 7;
		contentContainer.x = contentStartX + (contentEndX - contentStartX) * percent;
	}

	void UpdateMuted ()
	{
		muteTweenable.To(_isMuted ? 1.0f : 0.0f, 0.4f, new TweenConfig().expoInOut());
	}

	public bool isMuted
	{
		get {return _isMuted;}
		set {if(_isMuted != value) {_isMuted = value; UpdateMuted();}}
	}
}

public class NewPlayerBox : Box
{
	public FSprite bodySprite;
	public FSprite plusSprite;
	
	public NewPlayerBox()
	{
		base.Init(Player.NullPlayer);

		contentContainer.AddChild(bodySprite = new FSprite("Icons/NewPlayer_Body"));
		contentSprites.Add(bodySprite);
		bodySprite.color = Color.black;

		contentContainer.AddChild(plusSprite = new FSprite("Icons/NewPlayer_Plus"));
		contentSprites.Add(plusSprite);
		plusSprite.color = Color.black;
		plusSprite.SetPosition(0.0f,7.0f);
	}

	public void DoTapAnimation()
	{
		plusSprite.rotation = 0;
		Go.to(plusSprite, 0.33f, new TweenConfig().rotation(360.0f).setEaseType(EaseType.ExpoIn));
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
			destSpin = 1.0f;
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
			Go.to(this,0.4f,new TweenConfig().floatProp("spin",destSpin).expoInOut());
		}
	}

	void UpdateSpin()
	{
		float baseX = -1;
		float baseY = -1;

		float radius1 = 15.0f;
		float radius3 = 17.0f;
		float spinRads = _spin * RXMath.PI + RXMath.HALF_PI;

		sprite1.SetPosition(baseX+Mathf.Cos(spinRads)*radius1,baseY+Mathf.Sin(spinRads)*radius1);
		sprite2.SetPosition(baseX,baseY);
		sprite3.SetPosition(baseX-Mathf.Cos(spinRads)*radius3,baseY-Mathf.Sin(spinRads)*radius3 + (1.0f-_spin));
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

