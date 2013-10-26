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
		AddChild(minusBox = new MathBox(this));
		AddChild(plusBox = new MathBox(this));

		_width = 300.0f;
		_height = Config.SLOT_HEIGHT;

		DoLayout();
	}

	public void DoLayout()
	{
		float padding = Config.PADDING_XS+1;
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

		float nameWidth = freeWidth * 3.0f/5.0f;
		float scoreWidth = freeWidth * 2.0f/5.0f;

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
	
	public NameBox(Slot slot)
	{
		this.slot = slot;

		base.Init(slot.player);

		contentContainer.AddChild(new FLabel("Raleway","N"));
	}
}

public class ScoreBox : Box
{
	public Slot slot;
	
	public ScoreBox(Slot slot)
	{
		this.slot = slot;

		base.Init(slot.player);

		contentContainer.AddChild(new FLabel("Raleway","S"));
	}
}

public class MathBox : Box
{
	public Slot slot;
	
	public MathBox(Slot slot)
	{
		this.slot = slot;

		base.Init(slot.player);

		contentContainer.AddChild(new FLabel("Raleway","M"));
	}

	public float GetNeededWidth()
	{
		return slot.height;
	}
}

