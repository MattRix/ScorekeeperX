using UnityEngine;
using System;
using System.Collections.Generic;

public class Box : FContainer
{
	public FSprite boxSprite;

	public Content content;

	protected Player _player;

	protected float _width = 100;
	protected float _height = 100;

	public Box(Content content, Player player)
	{
		AddChild(boxSprite = new FSprite("Box"));
		boxSprite.color = player.color.color;

		this.content = content;
		content.Init(this);
		AddChild(content);
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

