using UnityEngine;
using System;
using System.Collections.Generic;


public class NameBox : Box
{
	public Slot slot;
	public FLabel nameLabel;
	private bool _isEditMode = false;
	private NameBoxCursor _cursor;
	
	public NameBox(Slot slot)
	{
		this.slot = slot;
		
		base.Init(slot.player);
		
		contentContainer.AddChild(nameLabel = new FLabel("Raleway",slot.player.name));
		nameLabel.color = Color.black;
		nameLabel.anchorX = 0.0f;

		contentContainer.AddChild(_cursor = new NameBoxCursor());
		_cursor.alpha = 0;
		
		slot.player.SignalNameChange += HandleNameChange;
		
		HandleNameChange();
		ListenForUpdate(HandleUpdate);
	}
	
	private void HandleNameChange()
	{
		nameLabel.text = slot.player.name;

		DoLayout();
	}

	void UpdateEditMode()
	{
		if(_isEditMode)
		{
			Go.to(_cursor, 0.3f, new TweenConfig().floatProp("alpha",1.0f));
		}
		else 
		{
			Go.to(_cursor, 0.3f, new TweenConfig().floatProp("alpha",0.0f));
		}
	}

	void HandleUpdate()
	{
		float nameLabelScale;

		if(nameLabel.text == "") //empty
		{
			nameLabelScale = 1.0f;
		}
		else
		{
			nameLabelScale = nameLabel.scale;
		}

		float targetX = nameLabel.x + nameLabel.textRect.width*nameLabelScale + Config.PADDING_M*nameLabelScale;

		if(targetX < _cursor.x)
		{
			_cursor.x += (targetX - _cursor.x) * 0.2f;
		}
		else 
		{
			_cursor.x = targetX;
		}

	}

	override public void DoLayout()
	{
		base.DoLayout();

		if(nameLabel != null)
		{
			float availWidth = this.width - Config.PADDING_L*2;
			float availHeight = this.height - Config.PADDING_M*2;
			
			float labelScale = Mathf.Min(1, availHeight/nameLabel.textRect.height,availWidth/nameLabel.textRect.width);

			nameLabel.scale = Mathf.Clamp01(labelScale); 
			nameLabel.x = -availWidth/2.0f;

			if(nameLabel.text == "") //empty
			{
				_cursor.scale = 1.0f;
			}
			else
			{
				_cursor.scale = nameLabel.scale;
			}

			if(!_isEditMode)
			{

			}

		}
	}

	public bool isEditMode
	{
		get {return _isEditMode;}
		set {if(_isEditMode != value) {_isEditMode = value; UpdateEditMode();}}
	}
}

public class NameBoxCursor : FContainer
{
	private FSprite _sprite;

	public NameBoxCursor()
	{
		AddChild(_sprite = new FSprite("Box"));
		_sprite.width = 5.0f;
		_sprite.height = 28.0f;
		_sprite.color = Color.black;

		ListenForUpdate(() => 
		{
			_sprite.alpha = 0.2f + RXMath.Saw(Time.time/0.4f);
		});
	}
}













