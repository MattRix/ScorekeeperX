using UnityEngine;
using System;
using System.Collections.Generic;

public class Box : FContainer
{
	public FSprite boxSprite;
	public List<FSprite> foregroundSprites = new List<FSprite>();
	public FContainer foregroundContainer;

	protected Player _player;

	protected float _width;
	protected float _height;

	protected bool _hasInited = false;

	public Box()
	{
	}

	virtual public void Init(float width, float height, Player player)
	{
		if(_hasInited) return; 
		_hasInited = true;

		AddChild(boxSprite = new FSprite("Box"));
		AddChild(foregroundContainer = new FContainer());
		boxSprite.color = player.color.color;

		SetSize(width,height);
	}

	public Player player 
	{
		get {return _player;}

		set 
		{
			if(_player != value)
			{
				_player = value;
			}
		}
	}

	public void SetSize(float width, float height)
	{
		boxSprite.width = _width = width;
		boxSprite.height = _height = height;
	}

	public float width 
	{
		get {return _width;}
	}

	public float height 
	{
		get {return _height;}
	}
}

