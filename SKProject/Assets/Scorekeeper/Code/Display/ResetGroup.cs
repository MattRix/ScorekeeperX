using UnityEngine;
using System;
using System.Collections.Generic;

public class ResetGroup : FContainer
{
	public static ResetGroup instance = null;

	public const string PREFS_KEY = "SKResetAmount";

	public float width;

	public MathBox plusBox;
	public MathBox minusBox;
	public ZeroBox zeroBox;
	public Box cancelBox;
	public Box okBox;

	public ResetGroup(float width)
	{
		this.width = width;

		AddChild(plusBox = new MathBox(MathType.Plus));
		AddChild(minusBox = new MathBox(MathType.Minus));

		AddChild(zeroBox = new ZeroBox());

		AddChild(cancelBox = new SpriteBox(Player.NullPlayer,"Icons/Cancel",100,100));
		AddChild(okBox = new SpriteBox(Player.NullPlayer,"Icons/Checkmark",100,100));

		plusBox.hasFastRepeatZones = true;
		minusBox.hasFastRepeatZones = true;

		if(PlayerPrefs.HasKey(PREFS_KEY))
		{
			zeroBox.resetAmount = PlayerPrefs.GetInt(PREFS_KEY);
		}

		float boxSize = Config.SLOT_HEIGHT;
		float padding = Config.PADDING_S;

		plusBox.SetSize(boxSize,boxSize);
		minusBox.SetSize(boxSize,boxSize);
		zeroBox.SetSize(width-boxSize*2-padding*2,boxSize);

		plusBox.SetPosition(zeroBox.width/2 + padding + plusBox.width/2, padding/2 + plusBox.height/2);
		minusBox.SetPosition(-zeroBox.width/2 - padding - minusBox.width/2, padding/2 + minusBox.height/2);
		zeroBox.SetPosition(0,padding/2 + zeroBox.height/2);

		cancelBox.SetSize((width-padding)/2,boxSize);
		okBox.SetSize((width-padding)/2,boxSize);

		cancelBox.SetPosition(-padding/2 - cancelBox.width/2, -padding/2 - cancelBox.height/2);
		okBox.SetPosition(padding/2 + okBox.width/2, -padding/2 - okBox.height/2);

		minusBox.SignalTick += (b,ticks) =>
		{
			zeroBox.resetAmount -= ticks;
			UpdateResetAmount();
		};

		plusBox.SignalTick += (b,ticks) =>
		{
			zeroBox.resetAmount += ticks;
			UpdateResetAmount();
		};

		zeroBox.SignalRelease += (b) =>
		{
			zeroBox.DoTapEffect();
			FSoundManager.PlaySound("UI/ResetToZero");
			zeroBox.resetAmount = 0;
			UpdateResetAmount();
		};

		cancelBox.SignalRelease += (b) =>
		{
			Keeper.instance.slotList.ApplyResetScores(false);
			cancelBox.isTouchable = false;
			cancelBox.DoTapEffect();
			Keeper.instance.EndResetMode();
			FSoundManager.PlaySound("UI/Cancel");
		};

		okBox.SignalRelease += (b) =>
		{
			Keeper.instance.slotList.ApplyResetScores(true);
			okBox.isTouchable = false;
			okBox.DoTapEffect();
			Keeper.instance.EndResetMode();
			FSoundManager.PlaySound("UI/ResetOk");

			SKDataManager.MarkDirty();
		};

		UpdateResetAmount();

		instance = this;
	}

	void UpdateResetAmount()
	{
		PlayerPrefs.SetInt(PREFS_KEY,zeroBox.resetAmount);
	}

	public void Show()
	{
		float farY = Config.HEIGHT/2 + 25;

		plusBox.y += farY;
		minusBox.y += farY;
		zeroBox.y += farY;

		cancelBox.y -= farY;
		okBox.y -= farY;

		float baseDelay = 0.35f;

		Go.to(plusBox,0.4f, new TweenConfig().setDelay(baseDelay+0.2f).y(plusBox.y-farY).expoOut());
		Go.to(zeroBox,0.5f, new TweenConfig().setDelay(baseDelay+0.1f).y(zeroBox.y-farY).expoOut());
		Go.to(minusBox,0.6f, new TweenConfig().setDelay(baseDelay+0.0f).y(minusBox.y-farY).expoOut());

		Go.to(okBox,0.4f, new TweenConfig().setDelay(baseDelay+0.2f).y(okBox.y+farY).expoOut());
		Go.to(cancelBox,0.6f, new TweenConfig().setDelay(baseDelay+0.0f).y(cancelBox.y+farY).expoOut());

		float changeX = 40;
		this.x -= changeX;
		Go.to(this,0.6f, new TweenConfig().x(this.x+changeX).setDelay(baseDelay).expoOut());
	}

	public void Close()
	{
		instance = null;
		Go.to(this,0.6f, new TweenConfig().x(this.x-width-25).expoInOut().removeWhenComplete());
	}
}

public class ZeroBox : Box
{
	private FLabel _scoreLabel;

	private int _resetAmount = 0;

	public ZeroBox()
	{
		Init(Player.NullPlayer);

		contentContainer.AddChild(_scoreLabel = new FLabel("Raleway",""));
		_scoreLabel.color = Color.black;

		UpdateResetAmount();
	}

	private void UpdateResetAmount()
	{
		_scoreLabel.text = resetAmount.ToString();
		
		DoLayout();
	}

	override public void DoLayout()
	{
		if(_scoreLabel == null) return; //has to finish initing first
		base.DoLayout();

		float availWidth = this.width - Config.PADDING_M*3;
		float availHeight = this.height - Config.PADDING_M;
		
		float fullScale = Mathf.Min(1.0f, availWidth/_scoreLabel.textRect.width);
		_scoreLabel.scale = Mathf.Clamp01(fullScale);
	}

	public int resetAmount
	{
		get {return _resetAmount;}
		set {if(_resetAmount != value) {_resetAmount = value; UpdateResetAmount();}}
	}
}









