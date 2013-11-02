using UnityEngine;
using System;
using System.Collections.Generic;


public class NameBox : Box
{
	public Slot slot;
	public FLabel nameLabel;
	private bool _isEditMode = false;
	
	public NameBox(Slot slot)
	{
		this.slot = slot;
		
		base.Init(slot.player);
		
		contentContainer.AddChild(nameLabel = new FLabel("Raleway",slot.player.name));
		nameLabel.color = Color.black;
		nameLabel.anchorX = 0.0f;
		
		slot.player.SignalNameChange += HandleNameChange;
		
		HandleNameChange();
	}
	
	private void HandleNameChange()
	{
		nameLabel.text = slot.player.name;
		
		DoLayout();
	}

	void UpdateEditMode()
	{
		_isEditMode = true;
	}
	
	override public void DoLayout()
	{
		base.DoLayout();
		if(nameLabel != null)
		{
			float availWidth = this.width - Config.PADDING_L*2;
			float availHeight = this.height - Config.PADDING_M*2;
			
			float labelScale = Mathf.Min(0.75f, availHeight/nameLabel.textRect.height,availWidth/nameLabel.textRect.width);
			
			nameLabel.scale = Mathf.Clamp01(labelScale); 
			nameLabel.x = -availWidth/2.0f;
		}
	}

	public bool isEditMode
	{
		get {return _isEditMode;}
		set {if(_isEditMode != value) {_isEditMode = value; UpdateEditMode();}}
	}
}