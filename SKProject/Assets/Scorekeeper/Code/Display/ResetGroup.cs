using UnityEngine;
using System;
using System.Collections.Generic;

public class ResetGroup : FContainer
{
	public float width;

	public MathBox plusBox;
	public MathBox minusBox;
	public ZeroBox zeroBox;
	public Box cancelBox;
	public Box okBox;

	public ResetGroup(float width)
	{
		this.width = width;

		AddChild(plusBox = new MathBox(MathType.Plus));
		AddChild(minusBox = new MathBox(MathType.Minus));

		AddChild(zeroBox = new ZeroBox());

		AddChild(cancelBox = new SpriteBox(Player.NullPlayer,"Icons/Cancel",100,100));
		AddChild(okBox = new SpriteBox(Player.NullPlayer,"Icons/Checkmark",100,100));

		float boxSize = 48.0f;
		float padding = Config.PADDING_S;

		plusBox.SetSize(boxSize,boxSize);
		minusBox.SetSize(boxSize,boxSize);
		zeroBox.SetSize(width-boxSize*2-padding*2,boxSize);

		plusBox.SetPosition(zeroBox.width/2 + padding + plusBox.width/2, padding/2 + plusBox.height/2);
		minusBox.SetPosition(-zeroBox.width/2 - padding - minusBox.width/2, padding/2 + minusBox.height/2);
		zeroBox.SetPosition(0,padding/2 + zeroBox.height/2);

		cancelBox.SetSize((width-padding)/2,boxSize);
		okBox.SetSize((width-padding)/2,boxSize);

		cancelBox.SetPosition(-padding/2 - cancelBox.width/2, -padding/2 - cancelBox.height/2);
		okBox.SetPosition(padding/2 + okBox.width/2, -padding/2 - okBox.height/2);

		zeroBox.SignalRelease += (b) =>
		{
			zeroBox.DoTapEffect();
			FSoundManager.PlaySound("UI/ResetToZero");
		};

		cancelBox.SignalRelease += (b) =>
		{
			cancelBox.isTouchable = false;
			cancelBox.DoTapEffect();
			Keeper.instance.EndResetMode();
			FSoundManager.PlaySound("UI/Cancel");
		};

		okBox.SignalRelease += (b) =>
		{
			okBox.isTouchable = false;
			okBox.DoTapEffect();
			Keeper.instance.EndResetMode();
			FSoundManager.PlaySound("UI/ResetOk");
		};

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
	public ZeroBox()
	{
		Init(Player.NullPlayer);
	}
}









