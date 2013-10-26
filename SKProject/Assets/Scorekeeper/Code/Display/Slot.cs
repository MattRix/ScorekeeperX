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

	private float _width;
	private float _height;

	private bool _hasHandle = false;

	public Slot(Player player)
	{
		this.player = player;

		AddChild(handleBox = new HandleBox(this));
		AddChild(nameBox = new NameBox(this));
		AddChild(scoreBox = new ScoreBox(this));
		AddChild(minusBox = new MathBox(this, MathType.Minus));
		AddChild(plusBox = new MathBox(this, MathType.Plus));

		_width = Config.LIST_WIDTH;
		_height = Config.SLOT_HEIGHT;

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

	private void HandleMinusTick() 
	{
		player.score--;
	}

	private void HandlePlusTick()
	{
		player.score++;
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

public class HandleBox : Box
{
	public Slot slot;
	
	public HandleBox(Slot slot)
	{
		this.slot = slot;
		
		base.Init(slot.player);

		contentContainer.AddChild(new FLabel("Raleway","H"));
	}

	public float GetNeededWidth()
	{
		return slot.height;
	}
}

public class NameBox : Box
{
	public Slot slot;
	public FLabel nameLabel;
	
	public NameBox(Slot slot)
	{
		this.slot = slot;

		base.Init(slot.player);

		AddChild(nameLabel = new FLabel("Raleway",slot.player.name));
		nameLabel.color = Color.black;

		slot.player.SignalNameChange += HandleNameChange;
		
		HandleNameChange();
	}

	private void HandleNameChange()
	{
		nameLabel.text = slot.player.name;
		
		float availWidth = this.width - Config.PADDING_M*3;
		float availHeight = this.height - Config.PADDING_M*3;
		
		float labelScale = Mathf.Min(0.75f, availHeight/nameLabel.textRect.height,availWidth/nameLabel.textRect.width);
		
		nameLabel.scale = Mathf.Clamp01(labelScale);
	}
}

public class ScoreBox : Box
{
	public Slot slot;
	public FLabel scoreLabel;
	
	public ScoreBox(Slot slot)
	{
		this.slot = slot;

		base.Init(slot.player);

		AddChild(scoreLabel = new FLabel("Raleway","0"));
		scoreLabel.color = Color.black;

		isTouchable = false;

		slot.player.SignalScoreChange += HandleScoreChange;

		HandleScoreChange();
	}

	private void HandleScoreChange()
	{
		scoreLabel.text = slot.player.score.ToString();

		float availWidth = this.width - Config.PADDING_M*3;
		float availHeight = this.height - Config.PADDING_M*3;

		float labelScale = Mathf.Min(0.75f, availHeight/scoreLabel.textRect.height,availWidth/scoreLabel.textRect.width);

		scoreLabel.scale = Mathf.Clamp01(labelScale);
	}
}

public class MathBox : RepeatableBox
{
	public Slot slot;
	public MathType mathType;
	public MathSprite mathSprite;
	
	public MathBox(Slot slot, MathType mathType)
	{
		this.slot = slot;
		this.mathType = mathType;

		base.Init(slot.player);

		if(mathType == MathType.Plus)
		{
			normalSoundName = "UI/Add";
			fastSoundName = "UI/AddFast";
			fastestSoundName = "UI/AddFastest";
			contentContainer.AddChild(mathSprite = new MathSprite("Icons/Plus"));
		}
		else 
		{
			normalSoundName = "UI/Subtract";
			fastSoundName = "UI/SubtractFast";
			fastestSoundName = "UI/SubtractFastest";
			contentContainer.AddChild(mathSprite = new MathSprite("Icons/Minus"));
		}

		mathSprite.color = Color.black;
	}

	public float GetNeededWidth()
	{
		return slot.height;
	}
}

public enum MathType
{
	Plus,
	Minus
}

