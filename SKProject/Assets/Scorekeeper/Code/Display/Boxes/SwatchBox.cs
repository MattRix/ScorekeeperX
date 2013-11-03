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
		Go.killAllTweensWithTarget(boxSprite);
		if(_isSelected)//shrink when selected
		{
			Go.to(boxSprite,0.075f,new TweenConfig()
			      .floatProp("width",_width-Config.PADDING_S*2)
			      .floatProp("height",_height-Config.PADDING_S*2)
			      .setEaseType(EaseType.Linear)
			      );

			//boxSprites[0].SetSize(_width-Config.PADDING_S*2,_height-Config.PADDING_S*2);
		}
		else 
		{
			Go.to(boxSprite,0.3f,new TweenConfig()
			      .floatProp("width",_width)
			      .floatProp("height",_height)
			      .setEaseType(EaseType.Linear)
			      );
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