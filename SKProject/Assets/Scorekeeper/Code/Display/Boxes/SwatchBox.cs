using UnityEngine;
using System;
using System.Collections.Generic;


public class SwatchBox : Box
{
	protected bool _isSelected = false;

	public SwatchBox(PlayerColor color)
	{
		base.Init(new Player("",color,0));
	}

	override public void DoLayout()
	{
		if(_isSelected)//shrink when selected
		{
			boxSprites[0].SetSize(_width-Config.PADDING_M*2,_height-Config.PADDING_M*2);
		}
		else 
		{
			boxSprites[0].SetSize(_width,_height);
		}
	}

	public void UpdateSelected()
	{
		DoLayout();
	}

	public bool isSelected
	{
		get {return _isSelected;}
		set {if(_isSelected != value) {_isSelected = value; UpdateSelected();}}
	}
}