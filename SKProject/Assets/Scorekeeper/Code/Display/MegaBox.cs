using UnityEngine;
using System;
using System.Collections.Generic;

public class MegaBox : Box
{
	public Func<Vector2> posFunc = null;

	public MegaBox() : base()
	{
		Init(Config.MEGA_WIDTH,Config.HALF_SCREEN_HEIGHT,Player.NullPlayer);

		ListenForUpdate(HandleUpdate);
	}

	void HandleUpdate ()
	{
		if(posFunc != null) this.SetPosition(posFunc());
	}
}

