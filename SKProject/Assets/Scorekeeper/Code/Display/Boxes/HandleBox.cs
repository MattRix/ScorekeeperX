using UnityEngine;
using System;
using System.Collections.Generic;


public class HandleBox : Box
{
	public Slot slot;
	
	public HandleBox(Slot slot)
	{
		this.slot = slot;
		
		base.Init(slot.player);
		
		contentContainer.AddChild(new FLabel("Raleway","H"));
	}
	
	public float GetNeededWidth()
	{
		return slot.height;
	}
}