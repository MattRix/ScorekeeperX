using UnityEngine;
using System;
using System.Collections.Generic;

public class Player
{
	public static Player NullPlayer = new Player("NullPlayer",PlayerColor.NullGray,0);

	public const int NAME_CHANGE = 0;
	public const int COLOR_CHANGE = 1;
	public const int SCORE_CHANGE = 2;

	private string _name;
	private PlayerColor _color;
	private int _score;

	public Action SignalNameChange;
	public Action SignalColorChange;
	public Action SignalScoreChange;

	public Player ()
	{

	}

	public Player (string name, PlayerColor color, int score)
	{
		_name = name.ToUpper();
		_color = color;
		_score = score;
	}

	public string name 
	{
		get {return _name;}
		
		set 
		{
			value = value.ToUpper();
			if(_name != value)
			{
				_name = value;
				if(SignalNameChange != null) SignalNameChange();
				SKDataManager.MarkDirty();
			}
		}
	}

	public PlayerColor color 
	{
		get {return _color;}

		set 
		{
			if(_color != value)
			{
				_color = value;
				if(SignalColorChange != null) SignalColorChange();
				SKDataManager.MarkDirty();
			}
		}
	}

	public int score 
	{
		get {return _score;}
		
		set 
		{
			if(_score != value)
			{
				_score = value;
				if(SignalScoreChange != null) SignalScoreChange();
				SKDataManager.MarkDirty();
			}
		}
	}
}

