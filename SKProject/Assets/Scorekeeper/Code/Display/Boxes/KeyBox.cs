using UnityEngine;
using System;
using System.Collections.Generic;


public class KeyBox : RepeatableBox
{
	public int index;
	public KeyBox(Player player, int index)
	{
		base.Init(player);
		this.index = index;
	}

	override public void DoLayout()
	{
		base.DoLayout();
	}
}