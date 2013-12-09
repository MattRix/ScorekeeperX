using UnityEngine;
using System;
using System.Collections.Generic;

public class Keeper : FContainer
{
	public static Keeper instance;

	public FContainer mainContainer;

	public List<Box> megaBoxes = new List<Box>();

	public NewPlayerBox newPlayerBox;
	public ResetBox resetBox;
	public SortBox sortBox;
	public VolumeBox volumeBox;

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
		mainContainer.AddChild(newPlayerBox = new NewPlayerBox());
		newPlayerBox.SetToCell(CellManager.megaNewPlayer);
		megaBoxes.Add(newPlayerBox);

		mainContainer.AddChild(resetBox = new ResetBox());
		resetBox.SetToCell(CellManager.megaReset);
		megaBoxes.Add(resetBox);

		mainContainer.AddChild(sortBox = new SortBox());
		sortBox.SetToCell(CellManager.megaSort);
		megaBoxes.Add(sortBox);

		mainContainer.AddChild(volumeBox = new VolumeBox());
		volumeBox.SetToCell(CellManager.megaVolume);
		megaBoxes.Add(volumeBox);

		newPlayerBox.SignalPress += HandleNewPlayerTap;
		resetBox.SignalPress += HandleResetTap;
		sortBox.SignalPress += HandleSortTap;
		volumeBox.SignalPress += HandleVolumeTap;
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

		if(slotList.slots.Count == 0)
		{
			sortBox.isEnabled = false;
			resetBox.isEnabled = false;
			volumeBox.isEnabled = false;
		}
		else 
		{
			sortBox.isEnabled = true;
			resetBox.isEnabled = true;
			volumeBox.isEnabled = true;
		}
	}

	void HandleResetTap (Box box)
	{
		DisableMegaBoxes();

		resetBox.DoTapEffect();
		resetBox.DoTapAnimation();
		FSoundManager.PlaySound("UI/Button1");

		ShrinkMainboxes();
	}

	void HandleVolumeTap (Box box)
	{
		volumeBox.DoTapEffect();
		//volumeBox.DoTapAnimation();

		FSoundManager.isMuted = !FSoundManager.isMuted;

		volumeBox.isMuted = FSoundManager.isMuted;

		if(!FSoundManager.isMuted)
		{
			FSoundManager.PlaySound("UI/SoundOn");
		}
	}

	void HandleNewPlayerTap (Box box)
	{
		newPlayerBox.DoTapEffect();
		newPlayerBox.DoTapAnimation();
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

		sortBox.sortType = SKDataManager.sortType;
	}

	void DisableMegaBoxes()
	{
		newPlayerBox.isTouchable = false;
		resetBox.isTouchable = false;
		sortBox.isTouchable = false;
		volumeBox.isTouchable = false;
	}

	void EnableMegaBoxes()
	{
		newPlayerBox.isTouchable = true;
		resetBox.isTouchable = true;
		sortBox.isTouchable = true;
		volumeBox.isTouchable = true;
	}

	
	void ShrinkMainboxes()
	{
		TweenConfig config = new TweenConfig().scaleXY(0).setEaseType(EaseType.ExpoIn).hideWhenComplete();
		Go.to(newPlayerBox, 0.5f,config);
		Go.to(resetBox, 0.5f,config);
		Go.to(sortBox, 0.5f,config);
		Go.to(volumeBox, 0.5f,config);
	}

	void UnshrinkMainboxes()
	{
		newPlayerBox.isVisible = true;
		resetBox.isVisible = true;
		sortBox.isVisible = true;
		volumeBox.isVisible = true;

		TweenConfig config = new TweenConfig().scaleXY(1.0f).setEaseType(EaseType.ExpoOut).removeWhenComplete();
		Go.to(newPlayerBox, 0.5f,config);
		Go.to(resetBox, 0.5f,config);
		Go.to(sortBox, 0.5f,config);
		Go.to(volumeBox, 0.5f,config);
	}

	void DisableList()
	{
		slotList.isTouchable = false;
	}

	void EnableList()
	{
		slotList.isTouchable = true;
	}


	public void EditPlayer(Player player)
	{
		DisableMegaBoxes();
		DisableList();

		Slot slot = slotList.GetSlotForPlayer(player);
		if(slot == null) return;

		Go.to(mainContainer, 0.4f, new TweenConfig().floatProp("scale",0.75f).setEaseType(EaseType.Linear).removeWhenComplete());
		Go.to(mainContainer, 0.4f, new TweenConfig().floatProp("alpha",0.0f).setDelay(0.0f).setEaseType(EaseType.Linear));

		slot.PauseMathMode();

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
		EnableMegaBoxes();
		EnableList();

		//resume math mode on the player view (if needed)
		if(playerEditor.slot != null) playerEditor.slot.ResumeMathMode();

		playerEditor.Destroy();
		playerEditor = null;

		SKDataManager.MarkDirty();
	}

	void HandleLateUpdate ()
	{
		FSoundManager.soundSource.pitch = Time.timeScale; //slow down sounds when time is slow
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

