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

		Config.Setup();

		AddChild(mainContainer = new FContainer());
		megaEntities = new MegaEntities();

		megaEntities.all.ForEach(entity => {AddChild(entity);});

		megaEntities.UpdatePositions();

		Futile.screen.SignalResize += HandleSignalResize;
	}

	void HandleSignalResize (bool wasResizedDueToOrientationChange)
	{
		Config.Setup();
		megaEntities.UpdatePositions();
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

		public void UpdatePositions()
		{
			float padding = Config.PADDING_M;
			float fullHeight = Mathf.Round(Config.HEIGHT - padding*2);
			float fullWidth = Mathf.Round(Config.MEGA_WIDTH - padding*2);
			float halfHeight = Mathf.Round((fullHeight - padding)/2.0f);
			float thirdHeight = Mathf.Round((fullHeight - padding*2)/3.0f);

			newPlayer.SetSize(fullWidth,halfHeight);
			newPlayer.SetPosition(-Config.HALF_WIDTH + newPlayer.width/2 + padding,Config.HALF_HEIGHT-newPlayer.height/2-padding);

			timer.SetSize(fullWidth,halfHeight);
			timer.SetPosition(-Config.HALF_WIDTH + timer.width/2 + padding,-Config.HALF_HEIGHT+timer.height/2+padding);

			sort.SetSize(fullWidth,thirdHeight);
			sort.SetPosition(Config.HALF_WIDTH - sort.width/2-padding,Config.HALF_HEIGHT-sort.height/2-padding);

			reset.SetSize(fullWidth,thirdHeight);
			reset.SetPosition(Config.HALF_WIDTH - reset.width/2-padding,0);

			settings.SetSize(fullWidth,thirdHeight);
			settings.SetPosition(Config.HALF_WIDTH - settings.width/2-padding,-Config.HALF_HEIGHT+settings.height/2+padding);
		}
	}

}

