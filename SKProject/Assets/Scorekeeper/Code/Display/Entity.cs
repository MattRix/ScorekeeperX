using UnityEngine;
using System;
using System.Collections.Generic;

public class Entity : FContainer
{
	public List<Box> boxes = new List<Box>();
	public List<FSprite> contentSprites = new List<FSprite>();
	public FContainer contentContainer;

	private Player _player;

	protected float _width = 100;
	protected float _height = 100;

	public Entity()
	{
		
	}
	
	virtual public void Init(Player player)
	{

		Box box = new Box(this);
		AddChild(box);
		boxes.Add(box);

		AddChild(contentContainer = new FContainer());

		_player = player;
		UpdatePlayer();
	}

	public void SetSize(float width, float height)
	{
		_width = width;
		_height = height;

		boxes.ForEach(box => {box.SetSize(_width,_height);});
	}

	void UpdatePlayer ()
	{
		boxes.ForEach(box => {box.color = _player.color.color;});
	}
	
	public float width 
	{
		get {return _width;}
	}
	
	public float height 
	{
		get {return _height;}
	}

	public Player player 
	{
		get {return _player;}
		set 
		{
			if(_player != value)
			{
				_player = value;
				UpdatePlayer();
			}
		}
	}
}

