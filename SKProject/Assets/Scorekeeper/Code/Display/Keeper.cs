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

	public SlotList slotList;

	public FContainer effectContainer;


	public Keeper ()
	{
		instance = this;	

		SKDataManager.LoadData();

		CellManager.Recalculate();

		AddChild(mainContainer = new FContainer());

		SetupMegaBoxes();

		AddChild(slotList = new SlotList(Config.LIST_WIDTH, Config.HEIGHT));

		AddChild(effectContainer = new FContainer());

		slotList.SignalPlayerChange += HandlePlayerChange;
		
		HandlePlayerChange();

		Futile.screen.SignalResize += HandleSignalResize;
		Futile.instance.SignalLateUpdate += HandleLateUpdate; 

		//FSoundManager.PlaySound("UI/Start");
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
	}

	void HandlePlayerChange()
	{
		if(slotList.slots.Count < Config.MAX_PLAYERS)
		{
			newPlayerBox.isEnabled = true;
		}
		else 
		{
			newPlayerBox.isEnabled = false;
		}
	}

	void HandleNewPlayerPress ()
	{
		newPlayerBox.DoTapEffect();
		FSoundManager.PlaySound("UI/Button1");

		Player player = new Player();
		player.name = (string)RXRandom.GetRandomItem("BELLA", "JOHNNY", "darko", "wallice fourteen", "everyone", "johnny b", "wick","j");
		player.color = PlayerColor.GetNextUnusedColor();
		player.score = RXRandom.Range(-1000,1000);

		slotList.AddSlotForPlayer(player);
	}

	void HandleLateUpdate ()
	{
		CellManager.Refresh();
	}

	void HandleSignalResize (bool wasResizedDueToOrientationChange)
	{
		CellManager.Recalculate();
	}

	public void CreateTapEffect(Box box, float borderThickness)
	{
		BorderBox borderBox = new BorderBox(box.width,box.height,-borderThickness);
		Vector2 boxPos = effectContainer.OtherToLocal(box,new Vector2());
		borderBox.x = boxPos.x;
		borderBox.y = boxPos.y;
		borderBox.rotation = box.rotation;
		borderBox.alpha = 0.35f;
		borderBox.scale = 1.0f;
		borderBox.shader = FShader.Additive;
		borderBox.color = box.player.color.color + new Color(0.3f,0.3f,0.3f); //add grey to make it brighter
		effectContainer.AddChild(borderBox);

		float growSize = 10.0f;
		float growScaleX = (borderBox.width+growSize)/borderBox.width;
		float growScaleY = (borderBox.height+growSize)/borderBox.height;

		Go.to(borderBox,0.2f,new TweenConfig()
		      .setEaseType(EaseType.Linear)
		      .floatProp("scaleX",growScaleX)
		      .floatProp("scaleY",growScaleY)
		      .floatProp("alpha",0.0f)
		      .removeWhenComplete());
	}

}

