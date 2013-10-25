using UnityEngine;
using System;
using System.Collections.Generic;

public class Keeper : FContainer
{
	public static Keeper instance;

	public FContainer mainContainer;

	public MegaEntities megaEntities;

	private BorderBox _borderBox;

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

		_borderBox = new BorderBox(100,100,10);
		AddChild (_borderBox);
		_borderBox.anchorX = 0.0f;
		_borderBox.anchorY = 0.0f;
	}

	void HandleLateUpdate ()
	{
		CellManager.Refresh();

		_borderBox.borderThickness = Mathf.Abs(GetLocalMousePosition().x);
		_borderBox.scale = Mathf.Abs(GetLocalMousePosition().y/Futile.screen.halfHeight);
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

