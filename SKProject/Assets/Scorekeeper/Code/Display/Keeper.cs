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

	public PlayerEditor playerEditor;

	public FContainer effectContainer;


	public Keeper ()
	{
		instance = this;	

		SKDataManager.LoadData();

		CellManager.Recalculate();

		AddChild(mainContainer = new FContainer());

		SetupMegaBoxes();

		mainContainer.AddChild(slotList = new SlotList(Config.LIST_WIDTH, Config.HEIGHT));

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

		newPlayerBox.SignalPress += HandleNewPlayerTap;
		sortBox.SignalPress += HandleSortTap;
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

	void HandleNewPlayerTap (Box box)
	{
		newPlayerBox.DoTapEffect();
		FSoundManager.PlaySound("UI/Button1");

		Player player = new Player();
		player.name = Config.DEFAULT_NAME;
		player.color = PlayerColor.GetNextUnusedColor();
		player.score = 0;

		slotList.AddSlotForPlayer(player, true);

		EditPlayer(player);
	}

	void HandleSortTap (Box box)
	{
		sortBox.DoTapEffect();
		FSoundManager.PlaySound("UI/Button1");

		if(SKDataManager.sortType == SortType.HighestAtTop)
		{
			SKDataManager.sortType = SortType.LowestAtTop;
		}
		else 
		{
			SKDataManager.sortType = SortType.HighestAtTop;
		}
		
		slotList.Reorder(false,false,true);
	}

	public void EditPlayer(Player player)
	{
		Slot slot = slotList.GetSlotForPlayer(player);
		if(slot == null) return;

		Go.to(mainContainer, 0.4f, new TweenConfig().floatProp("scale",0.75f).setEaseType(EaseType.Linear).removeWhenComplete());
		Go.to(mainContainer, 0.4f, new TweenConfig().floatProp("alpha",0.0f).setDelay(0.0f).setEaseType(EaseType.Linear));

		//slot.PauseMathMode();

		//mainholdertween
		//disable touches on mainholder

		playerEditor = new PlayerEditor();
		AddChild(playerEditor);

		playerEditor.Setup(slot);

		AddChild(effectContainer); //effects on top

		FSoundManager.PlaySound("UI/Woosh");
	}

	//pass null unless you want to remove the player
	public void StopEditing(Player playerToRemove)
	{
		FSoundManager.PlaySound("UI/Woosh");

		if(playerToRemove != null)
		{
			slotList.RemoveSlotForPlayer(playerToRemove,true,true);
		}

		AddChildAtIndex(mainContainer,0);

		Go.to(mainContainer, 0.4f, new TweenConfig().floatProp("alpha",1.0f).setDelay(0.1f).setEaseType(EaseType.Linear));
		Go.to(mainContainer, 0.4f, new TweenConfig().floatProp("scale",1.0f).setDelay(0.1f).setEaseType(EaseType.ExpoOut));
	}

	//called by player editor
	public void RemovePlayerEditor()
	{
		//TODO: resume math mode on the player view (if needed)
		//if(playerEditor.slot != null) playerEditor.slot.ResumeMathMode();

		//TODO: enable mainHolder touches

		RemoveChild(playerEditor);
		playerEditor = null;

		SKDataManager.MarkDirty();
	}

	void HandleLateUpdate ()
	{
		CellManager.Refresh();
		SKDataManager.Update();
	}

	void HandleSignalResize (bool wasResizedDueToOrientationChange)
	{
		CellManager.Recalculate();
	}

	public void CreateTapEffect(Box box, float borderThickness)
	{
		TrackerBorderBox borderBox = new TrackerBorderBox(box, box.width,box.height,-borderThickness);
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

