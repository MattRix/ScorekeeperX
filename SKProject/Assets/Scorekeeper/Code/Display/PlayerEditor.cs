using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerEditor : FContainer
{
	public Slot slot;
	public NameBox nameBox;
	public FContainer keyboardAndSwatchContainer;
	
	public PlayerEditor()
	{
		AddChild(keyboardAndSwatchContainer = new FContainer());
	}

	public void Setup(Slot slot)
	{
		this.slot = slot;
		this.nameBox = slot.nameBox;

		Vector2 pos = OtherToLocal(nameBox,new Vector2(0,0));

		AddChild(nameBox);
		nameBox.SetPosition(pos);

		Cell nameCell = CellManager.GetCellFromGrid(3,6,2,2);

		Go.to(nameBox, 1.0f, new TweenConfig()
		      .floatProp("x",nameCell.x)
		      .floatProp("y",nameCell.y)
		      .floatProp("width",nameCell.width)
		      .floatProp("height",nameCell.height)
		      .setEaseType(EaseType.ExpoInOut)
		      .onComplete(HandleSlotTweenInComplete)
		);

	}

	void HandleSlotTweenInComplete()
	{
	}
}