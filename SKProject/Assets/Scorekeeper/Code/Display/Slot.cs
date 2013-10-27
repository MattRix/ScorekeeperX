using UnityEngine;
using System;
using System.Collections.Generic;

public class Slot : FContainer
{
	public Player player;

	public HandleBox handleBox;
	public NameBox nameBox;
	public ScoreBox scoreBox;
	public MathBox minusBox;
	public MathBox plusBox;

	public bool isMathMode;

	private float _width;
	private float _height;

	private bool _hasHandle = false;

	public float buildInAmount = 0.0f;

	public int index = -1;

	public Slot(Player player, float width, float height)
	{
		this.player = player;

		_width = width;
		_height = height;

		AddChild(handleBox = new HandleBox(this));
		AddChild(nameBox = new NameBox(this));
		AddChild(scoreBox = new ScoreBox(this));
		AddChild(minusBox = new MathBox(this, MathType.Minus));
		AddChild(plusBox = new MathBox(this, MathType.Plus));

//		Box box = new Box();
//		AddChild(box);
//		box.Init(Player.NullPlayer);
//		box.SetSize(_width,_height);
//		box.y -= _height;

		DoLayout();
	}

	public void DoLayout()
	{
		float padding = Config.PADDING_S;
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

		float maxScoreWidth = 100.0f;
		float scoreWidth = Mathf.Min(maxScoreWidth,freeWidth * 2.0f/5.0f);
		float nameWidth = freeWidth - scoreWidth;

		nameBox.SetSize(nameWidth,_height);
		nameBox.SetTopLeft(cursor.x,cursor.y);
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
		
		//score
		//name
		//plus
		//minus

		minusBox.SignalTick += HandleMinusTick;
		plusBox.SignalTick += HandlePlusTick;

		nameBox.SignalRelease += HandleNameTap;
	}

	private void HandleNameTap()
	{
		FSoundManager.PlaySound("UI/Button1");
		nameBox.DoTapEffect();

		player.name = RXRandom.GetRandomItem("BELLA", "JOHNNY", "darko", "wallice fourteen", "everyone", "johnny b", "wick","j","","             ", "do ya", "hollaber four") as string;
	}

	private void HandleMinusTick(int ticks) 
	{
		player.score -= ticks;
	}

	private void HandlePlusTick(int ticks)
	{
		player.score += ticks;
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
}




