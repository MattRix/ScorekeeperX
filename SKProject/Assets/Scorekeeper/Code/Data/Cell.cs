using UnityEngine;
using System;
using System.Collections.Generic;

public class Cell
{
	private float _x = 0;
	private float _y = 0;
	private float _width = 100;
	private float _height = 100;
	private float _rotation = 0;
	private float _alpha = 1.0f;

	public bool didHaveMajorChange = false;

	public Cell()
	{
		
	}

	public Cell(float x, float y, float width, float height, float rotation, float alpha)
	{
		_x = x; 
		_y = y;
		_width = width;
		_height = height;
		_rotation = rotation;
		_alpha = alpha;
	}

	public Rect GetGlobalRect()
	{
		return new Rect(_x - _width*0.5f, _y - _height*0.5f, _width, _height);
	}

	public Rect GetLocalRect()
	{
		return new Rect(-_width*0.5f, -_height*0.5f, _width, _height);
	}

	public void SetInterpolated (Cell baseCell, Cell targetCell, float percent)
	{
		_x = baseCell.x + (targetCell.x - baseCell.x) * percent;
		_y = baseCell.y + (targetCell.y - baseCell.y) * percent;
		_width = baseCell.width + (targetCell.width - baseCell.width) * percent;
		_height = baseCell.height + (targetCell.height - baseCell.height) * percent;
		_rotation = baseCell.rotation + (targetCell.rotation - baseCell.rotation) * percent;
		_alpha = baseCell.alpha + (targetCell.alpha - baseCell.alpha) * percent;
	}

	public void SetSize(float width, float height)
	{
		_width = width;
		_height = height;
	}

	public void SetPosition(float x, float y)
	{
		_x = x;
		_y = y;
	}

	public float width 
	{
		get {return _width;}
		set 
		{ 
			if(_width != value)
			{
				_width = value; 
			}
		}
	}
	
	public float height 
	{
		get {return _height;}
		set 
		{ 
			if(_height != value)
			{
				_height = value; 
			}
		}
	}

	public float x 
	{
		get {return _x;}
		set 
		{ 
			if(_x != value)
			{
				_x = value; 
			}
		}
	}
	
	public float y 
	{
		get {return _y;}
		set 
		{ 
			if(_y != value)
			{
				_y = value; 
			}
		}
	}

	public float rotation 
	{
		get {return _rotation;}
		set 
		{ 
			if(_rotation != value)
			{
				_rotation = value; 
			}
		}
	}

	public float alpha 
	{
		get {return _alpha;}
		set 
		{ 
			if(_alpha != value)
			{
				_alpha = value; 
			}
		}
	}

	public Cell Clone()
	{
		return new Cell(_x,_y,_width,_height,_rotation,_alpha);
	}
}

