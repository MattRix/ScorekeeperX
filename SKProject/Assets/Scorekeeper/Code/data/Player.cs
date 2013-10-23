using UnityEngine;
using System;
using System.Collections.Generic;

public class Player
{
	public const int NAME_CHANGE = 0;
	public const int COLOR_CHANGE = 1;
	public const int SCORE_CHANGE = 2;

	private string _name;
	private PlayerColor _color;
	private int _score;

	public RXDispatcher dispatcher = new RXDispatcher();

	public delegate void NoArgumentDelegate();

	public event NoArgumentDelegate SignalNameChange;

	public Player (string name, PlayerColor color, int score)
	{
		_name = name;
		_color = color;
		_score = score;
	}

	public void SetName()
	{
		dispatcher.Dispatch(NAME_CHANGE);

		if(SignalNameChange != null)
		{
			SignalNameChange();
		}

	}
}

