using UnityEngine;
using System;
using System.Collections.Generic;


public class KeyBox : Box
{
	public Player player;
	
	public KeyBox(Player player)
	{
		base.Init(player);
	}

	override public void DoLayout()
	{
		base.DoLayout();
	}
}