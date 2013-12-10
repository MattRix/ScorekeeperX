using UnityEngine;
using System;
using System.Collections.Generic;

public class Slot : FContainer, SKDestroyable
{
	public Player player;

	public HandleBox handleBox;
	public NameBox nameBox;
	public ScoreBox scoreBox;
	public MathBox minusBox;
	public MathBox plusBox;

	private bool _isMathMode;
	private bool _isResetMode = false;

	private float _width;
	private float _height;

	private bool _hasHandle = false;

	public RXTweenable buildInTweenable;

	public int index = -1;

	public Cell nameCell;

	private bool _isMathModePaused = false;

	private RXTweenable _resetTweenable;

	public float resetWidth;

	public Slot(Player player, float width, float height)
	{
		this.player = player;

		_width = width;
		_height = height;

		nameCell = new Cell();

		AddChild(handleBox = new HandleBox(this));
		AddChild(nameBox = new NameBox(this));
		AddChild(scoreBox = new ScoreBox(this));
		AddChild(minusBox = new MathBox(this, MathType.Minus));
		AddChild(plusBox = new MathBox(this, MathType.Plus));
		
		minusBox.SignalTick += HandleMinusTick;
		plusBox.SignalTick += HandlePlusTick;
		scoreBox.SignalRelease += HandleScoreTap;
		nameBox.SignalRelease += HandleNameTap;

		ListenForUpdate(HandleUpdate);
		DoLayout();

		_resetTweenable = new RXTweenable(0.0f, HandleResetTweenableChange);
		buildInTweenable = new RXTweenable(0.0f, HandleBuildInChange);

		HandleBuildInChange();
	}

	public void Destroy()
	{
		this.RemoveFromContainer();
		handleBox.Destroy();
		nameBox.Destroy();
		scoreBox.Destroy();
		minusBox.Destroy();
		plusBox.Destroy();
		//TODO: clean up signals etc.
	}

	public void DoLayout()
	{
		float padding = Config.PADDING_XS;
		Vector2 cursor = new Vector2(-_width*0.5f, _height*0.5f); //the top left

		float freeWidth = _width;

		freeWidth -= minusBox.GetNeededWidth()+padding; //minus width
		freeWidth -= plusBox.GetNeededWidth()+padding; //plus width
		freeWidth -= padding; //padding between name and score

		if(_hasHandle)
		{
			AddChild(handleBox);
			handleBox.SetSize(handleBox.GetNeededWidth(),_height);
			handleBox.SetTopLeft(cursor.x,cursor.y);
			cursor.x += handleBox.width + padding;
			freeWidth -= handleBox.width+padding; //handle width
		}
		else 
		{
			handleBox.RemoveFromContainer();
		}

		bool shouldRound = (scoreBox.mathMode.amount == 0 || scoreBox.mathMode.amount == 1);

		freeWidth = Mathf.Round(freeWidth);

		resetWidth = freeWidth;

		float mathModeMultiplier = (1.0f + 1.1f*scoreBox.mathMode.amount);
		float maxScoreWidth = 100.0f;
		float scoreWidth = Mathf.Min(maxScoreWidth,freeWidth * 1.5f/5.0f) * mathModeMultiplier;
		float nameWidth = freeWidth - scoreWidth;

		scoreWidth = Mathf.Round(scoreWidth);
		nameWidth = Mathf.Round(nameWidth);


		nameBox.SetSize(nameWidth,_height);
		nameBox.SetTopLeft(cursor.x,cursor.y);
		nameCell.Set(cursor.x + nameWidth*0.5f,cursor.y - _height*0.5f, nameWidth,_height);
		cursor.x += nameBox.width + padding;

		scoreBox.SetSize(scoreWidth,_height);
		scoreBox.SetTopLeft(cursor.x,cursor.y);
		cursor.x += scoreBox.width + padding;

		minusBox.SetSize(minusBox.GetNeededWidth(),_height);
		minusBox.SetTopLeft(cursor.x,cursor.y);
		cursor.x += minusBox.width + padding;

		plusBox.SetSize(plusBox.GetNeededWidth(),_height);
		plusBox.SetTopLeft(cursor.x,cursor.y);
		cursor.x += plusBox.width + padding;
	}

	private void HandleBuildInChange()
	{
		float amount = buildInTweenable.amount;

		handleBox.scale = RXEase.ExpoOut(RXMath.GetSubPercent(amount, 0.0f,0.4f));
		nameBox.scale = RXEase.ExpoOut(RXMath.GetSubPercent(amount, 0.15f,0.55f));
		scoreBox.scale = RXEase.ExpoOut(RXMath.GetSubPercent(amount, 0.3f,0.7f));
		minusBox.scale = RXEase.ExpoOut(RXMath.GetSubPercent(amount, 0.45f,0.85f)) * (1.0f - _resetTweenable.amount);
		plusBox.scale = RXEase.ExpoOut(RXMath.GetSubPercent(amount, 0.6f,1.0f)) * (1.0f - _resetTweenable.amount);
	}

	private void HandleScoreTap(Box box)
	{
		if(!_isResetMode) return;
		if(Keeper.instance.isEditorOpen) return; 
		
		FSoundManager.PlaySound("UI/Button1");
		scoreBox.DoTapEffect();
		scoreBox.shouldRemove = !scoreBox.shouldRemove;
	}

	private void HandleNameTap(Box box)
	{
		if(_isResetMode) return;
		if(Keeper.instance.isEditorOpen) return; 

		FSoundManager.PlaySound("UI/Button1");
		nameBox.DoTapEffect();

		Keeper.instance.EditPlayer(player);
	}

	private void HandleMinusTick(Box box, int ticks) 
	{
		if(_isResetMode) return;
		if(Keeper.instance.isEditorOpen) return; 
		StartMathMode();
		player.score -= ticks;
	}

	private void HandlePlusTick(Box box, int ticks)
	{
		if(_isResetMode) return;
		if(Keeper.instance.isEditorOpen) return; 
		StartMathMode();
		player.score += ticks;
	}

	private void HandleUpdate()
	{
		if(_isMathModePaused) return;

		if(_mathModeAmount > 0)
		{
			_mathModeAmount -= Time.deltaTime/Config.MATH_MODE_TIME/Time.timeScale;
			if(_mathModeAmount <= 0)
			{
				CloseMathMode();
			}
		}

		if(_isMathMode || scoreBox.mathMode.amount > 0)
		{
			DoLayout();
		}
	}

	private float _mathModeAmount; 

	public void StartMathMode()
	{
		_mathModeAmount = 1.0f;

		if(!_isMathMode) //just increment the timer
		{
			if(scoreBox.mathMode.amount == 0)
			{
				scoreBox.ResetBaseScore();
			}
			_isMathMode = true;
			Go.killAllTweensWithTarget(scoreBox);
			Go.to(scoreBox.mathMode, 0.4f, new TweenConfig().floatProp("amount",1.0f).setEaseType(EaseType.ExpoOut).onComplete(HandleMathModeOpenComplete));
			FSoundManager.PlaySound("UI/MathOpen");
		}
	}

	void HandleMathModeOpenComplete(AbstractTween obj)
	{

	}

	private void CloseMathMode()
	{
		_mathModeAmount = 0.0f;
		_isMathMode = false;
		Go.killAllTweensWithTarget(scoreBox);
		Go.to(scoreBox.mathMode, 0.4f, new TweenConfig().floatProp("amount",0.0f).setEaseType(EaseType.ExpoOut).onComplete(HandleMathModeCloseComplete));
		FSoundManager.PlaySound("UI/MathClose");
	}
	
	void HandleMathModeCloseComplete(AbstractTween obj)
	{
		_isMathMode = false;
		Keeper.instance.slotList.Reorder(true,false);
	}

	public void PauseMathMode()
	{
		_isMathModePaused = true;
	}

	public void ResumeMathMode()
	{
		_isMathModePaused = false;
	}

	void HandleResetTweenableChange()
	{
		HandleBuildInChange();
	}

	void UpdateResetMode()
	{
		if(_isResetMode)
		{
			if(_isMathMode)
			{
				CloseMathMode();
			}
			_resetTweenable.To(1.0f,0.3f,new TweenConfig().expoIn());
		}
		else 
		{
			_resetTweenable.To(0,0.3f,new TweenConfig().expoOut());
		}
	}

	public float width
	{
		get {return _width;}
		set {if(_width != value) {_width = value; DoLayout();}}
	}
	
	public float height
	{
		get {return _height;}
		set {if(_height != value) {_height = value; DoLayout();}}
	}

	public bool isMathMode
	{
		get {return _isMathMode;}
	}

	public bool isResetMode
	{
		get {return _isResetMode;}
		set {if(_isResetMode != value) {_isResetMode = value; UpdateResetMode();}}
	}
}




