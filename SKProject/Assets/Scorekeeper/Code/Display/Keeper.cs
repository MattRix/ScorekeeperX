using UnityEngine;
using System;
using System.Collections.Generic;

public class Keeper : FContainer
{
	public static Keeper instance;

	public Keeper ()
	{
		instance = this;	

		Config.Setup();
	}
}

