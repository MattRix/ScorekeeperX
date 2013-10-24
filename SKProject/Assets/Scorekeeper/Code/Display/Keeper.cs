using UnityEngine;
using System;
using System.Collections.Generic;

public class Keeper : FContainer
{
	public static Keeper instance;

	public FContainer mainContainer;

	public Keeper ()
	{
		instance = this;	

		Config.Setup();

		AddChild(mainContainer = new FContainer());

		MegaBox myBox = new MegaBox();

		AddChild(myBox);

		myBox.posFunc = () => 
		{
			return GetLocalMousePosition();
		};
	
	}
}

