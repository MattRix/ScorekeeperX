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

	public Keeper ()
	{
		instance = this;	

		CellManager.Recalculate();

		AddChild(mainContainer = new FContainer());

		SetupMegaBoxes();

		Futile.screen.SignalResize += HandleSignalResize;
		Futile.instance.SignalLateUpdate += HandleLateUpdate; 
	}

	void SetupMegaBoxes ()
	{
		AddChild(newPlayerBox = new PlaceholderBox());
		newPlayerBox.GoToCellInstantly(CellManager.megaNewPlayer);
		megaBoxes.Add(newPlayerBox);

		AddChild(timerBox = new PlaceholderBox());
		timerBox.GoToCellInstantly(CellManager.megaTimer);
		megaBoxes.Add(timerBox);

		AddChild(sortBox = new PlaceholderBox());
		sortBox.GoToCellInstantly(CellManager.megaSort);
		megaBoxes.Add(sortBox);

		AddChild(resetBox = new PlaceholderBox());
		resetBox.GoToCellInstantly(CellManager.megaReset);
		megaBoxes.Add(resetBox);

		AddChild(settingsBox = new PlaceholderBox());
		settingsBox.GoToCellInstantly(CellManager.megaSettings);
		megaBoxes.Add(settingsBox);
	}

	void HandleLateUpdate ()
	{
		CellManager.Refresh();
	}

	void HandleSignalResize (bool wasResizedDueToOrientationChange)
	{
		CellManager.Recalculate();
	}

	public class MegaBoxes
	{
		public List<Box> all = new List<Box>();
		
		public Box newPlayer;
		public Box timer;
		public Box sort;
		public Box reset;
		public Box settings;

		public MegaBoxes()
		{
		}
	}

}

