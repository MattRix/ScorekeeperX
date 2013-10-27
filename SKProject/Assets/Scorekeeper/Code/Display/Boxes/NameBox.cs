using UnityEngine;
using System;
using System.Collections.Generic;


public class NameBox : Box
{
	public Slot slot;
	public FLabel nameLabel;
	
	public NameBox(Slot slot)
	{
		this.slot = slot;
		
		base.Init(slot.player);
		
		contentContainer.AddChild(nameLabel = new FLabel("Raleway",slot.player.name));
		nameLabel.color = Color.black;
		
		slot.player.SignalNameChange += HandleNameChange;
		
		HandleNameChange();
	}
	
	private void HandleNameChange()
	{
		nameLabel.text = slot.player.name;
		
		DoLayout();
	}
	
	override public void DoLayout()
	{
		base.DoLayout();
		if(nameLabel != null)
		{
			float availWidth = this.width - Config.PADDING_L*2;
			float availHeight = this.height - Config.PADDING_M*2;
			
			float labelScale = Mathf.Min(1.0f, availHeight/nameLabel.textRect.height,availWidth/nameLabel.textRect.width);
			
			nameLabel.scale = Mathf.Clamp01(labelScale); 
		}
	}
}