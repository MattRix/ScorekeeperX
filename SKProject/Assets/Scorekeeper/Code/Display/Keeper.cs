using UnityEngine;
using System;
using System.Collections.Generic;

public class Keeper : FContainer
{
	public static Keeper instance;

	public FContainer mainContainer;

	public List<Box> megaBoxes = new List<Box>();
	public Box newPlayerBox;
	public Box timerBox;
	public Box sortBox;
	public Box resetBox;
	public Box settingsBox;

	public FContainer effectContainer;

	public Keeper ()
	{
		instance = this;	

		CellManager.Recalculate();

		AddChild(mainContainer = new FContainer());
		AddChild(effectContainer = new FContainer());

		SetupMegaBoxes();

		Futile.screen.SignalResize += HandleSignalResize;
		Futile.instance.SignalLateUpdate += HandleLateUpdate; 
	}

	void SetupMegaBoxes ()
	{
		mainContainer.AddChild(newPlayerBox = new PlaceholderBox());
		newPlayerBox.SetToCell(CellManager.megaNewPlayer);
		megaBoxes.Add(newPlayerBox);

		mainContainer.AddChild(timerBox = new PlaceholderBox());
		timerBox.SetToCell(CellManager.megaTimer);
		megaBoxes.Add(timerBox);

		mainContainer.AddChild(sortBox = new PlaceholderBox());
		sortBox.SetToCell(CellManager.megaSort);
		megaBoxes.Add(sortBox);

		mainContainer.AddChild(resetBox = new PlaceholderBox());
		resetBox.SetToCell(CellManager.megaReset);
		megaBoxes.Add(resetBox);

		mainContainer.AddChild(settingsBox = new PlaceholderBox());
		settingsBox.SetToCell(CellManager.megaSettings);
		megaBoxes.Add(settingsBox);

		newPlayerBox.SignalPress += HandleNewPlayerPress;

		resetBox.isEnabled = false;

		testSlot = new Slot(new Player("MATT", PlayerColor.Red, 17));
		mainContainer.AddChild(testSlot);
	}

	public Slot testSlot;

	void HandleNewPlayerPress ()
	{
		FSoundManager.PlaySound("UI/Button1");
		Debug.Log ("Go team");
	}

	void HandleLateUpdate ()
	{
		CellManager.Refresh();

//		for(int w = 0; w<10; w++)
//		{
//			BorderBox borderBox = new BorderBox(RXRandom.Range(0,300.0f),RXRandom.Range(0,300.0f),RXRandom.Range(1,30.0f));
//			borderBox.alpha = 0.5f;
//			borderBox.scale = 1.00f;
//			borderBox.shader = FShader.Additive;
//			effectContainer.AddChild(borderBox);
//			Go.to(borderBox,0.2f,new TweenConfig().floatProp("scale",1.1f).floatProp("alpha",0.0f).removeWhenComplete());
//		}
	}

	void HandleSignalResize (bool wasResizedDueToOrientationChange)
	{
		CellManager.Recalculate();
	}

	public void CreateEffect(Box box, float borderThickness)
	{
		float scaleRatio = box.width / box.height;
		BorderBox borderBox = new BorderBox(box.width,box.height,-borderThickness);
		borderBox.x = box.x;
		borderBox.y = box.y;
		borderBox.rotation = box.rotation;
		borderBox.alpha = 0.5f;
		borderBox.scale = 1.0f;
		borderBox.shader = FShader.Additive;
		effectContainer.AddChild(borderBox);
		Go.to(borderBox,0.2f,new TweenConfig().setEaseType(EaseType.Linear).floatProp("scaleX",1.0f + 0.1f).floatProp("scaleY",1.0f + scaleRatio*0.1f).floatProp("alpha",0.0f).removeWhenComplete());
	}

}

