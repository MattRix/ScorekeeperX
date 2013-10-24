using UnityEngine;
using System;
using System.Collections.Generic;

public class Keeper : FContainer
{
	public static Keeper instance;

	public FContainer mainContainer;

	public MegaEntities megaEntities;

	public Keeper ()
	{
		instance = this;	

		CellManager.Recalculate();

		AddChild(mainContainer = new FContainer());
		megaEntities = new MegaEntities();

		megaEntities.newPlayer.MoveToCellInstantly(CellManager.megaNewPlayer);
		megaEntities.timer.MoveToCellInstantly(CellManager.megaTimer);
		megaEntities.sort.MoveToCellInstantly(CellManager.megaSort);
		megaEntities.reset.MoveToCellInstantly(CellManager.megaReset);
		megaEntities.settings.MoveToCellInstantly(CellManager.megaSettings);

		megaEntities.all.ForEach(entity => {AddChild(entity);});


		megaEntities.reset.MoveToCellTweened(CellManager.middleCell,10.0f);

		Futile.screen.SignalResize += HandleSignalResize;
		Futile.instance.SignalLateUpdate += HandleLateUpdate;
	}

	void HandleLateUpdate ()
	{
		CellManager.Refresh();
	}

	void HandleSignalResize (bool wasResizedDueToOrientationChange)
	{
		CellManager.Recalculate();
	}

	public class MegaEntities
	{
		public List<Entity> all = new List<Entity>();
		
		public Entity newPlayer;
		public Entity timer;
		public Entity sort;
		public Entity reset;
		public Entity settings;

		public MegaEntities()
		{
			all.Add(newPlayer = 	new PlaceholderEntity());
			all.Add(timer = 		new PlaceholderEntity());
			all.Add(sort = 			new PlaceholderEntity());
			all.Add(reset = 		new PlaceholderEntity());
			all.Add(settings = 		new PlaceholderEntity());
		}
	}

}

