using UnityEngine;
using System;
using System.Collections.Generic;

public class ResetGroup : FContainer
{
	public MathBox plusBox;
	public MathBox minusBox;
	public ZeroBox zeroBox;
	public Box cancelBox;
	public Box okBox;

	public ResetGroup()
	{
		AddChild(plusBox = new MathBox(MathType.Plus));
		AddChild(minusBox = new MathBox(MathType.Minus));

		AddChild(zeroBox = new ZeroBox());

		AddChild(cancelBox = new SpriteBox(Player.NullPlayer,"Icons/Cancel",100,100));
		AddChild(okBox = new SpriteBox(Player.NullPlayer,"Icons/Checkmark",100,100));

		float slotSize = Config.SLOT_HEIGHT;
		float padding = Config.PADDING_S;


	}

	public void Show()
	{
		this.alpha = 0.0f;
		Go.to(this,0.5f, new TweenConfig().alpha(1.0f));
	}

	public void Close()
	{
		Go.to(this,0.5f, new TweenConfig().alpha(0.0f).removeWhenComplete());
	}
}

public class ZeroBox : Box
{

}