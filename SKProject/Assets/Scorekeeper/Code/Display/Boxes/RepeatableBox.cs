using UnityEngine;
using System;
using System.Collections.Generic;

public class RepeatableBox : Box
{
	public Action SignalTick;

	public string normalSoundName;
	public string fastSoundName;
	public string fastestSoundName;

	private bool _shouldRepeat = true;
	private bool _isTouchDown = false;

	public bool hasHyperRepeatZones = false;

	public RepeatableBox() : base()
	{

	}

	override public void Init(Player player)
	{
		base.Init(player);

		SignalPress += HandlePress;
		SignalRelease += HandleRelease;
		SignalReleaseOutside += HandleRelease;

		UpdateRepeat();

		ListenForUpdate(HandleUpdate);
	}

	private float _touchTime = 0.0f;
	private int _liveTicks = 0;

	private void HandleUpdate()
	{
		if(_shouldRepeat && _isTouchDown)
		{
			float totalTime = Time.time - _touchTime;
			int totalTicks = Mathf.FloorToInt(totalTime / 0.01f);


			while(_liveTicks < totalTicks)
			{
				_liveTicks++;

				if(hasHyperRepeatZones && _liveTicks >= 400) //4 seconds hyper
				{
					if(_liveTicks % 15 == 0) //every 150 ms
					{
						DoTickEffectWithSound(fastestSoundName);
					}

					//every 10 ms
					if(SignalTick != null) SignalTick();
				}
				else if(hasHyperRepeatZones && _liveTicks >= 200) //2 seconds faster
				{
					if(_liveTicks % 15 == 0) //every 150 ms
					{
						DoTickEffectWithSound(fastSoundName);
					}

					if(_liveTicks % 5 == 0) //every 50 ms
					{
						if(SignalTick != null) SignalTick();
					}
				}
				else if(_liveTicks >= 40) //0.4 seconds fast
				{
					if(_liveTicks % 15 == 0) //every 150 ms
					{
						DoTickEffectWithSound(normalSoundName);
						if(SignalTick != null) SignalTick();
					}
				}
				else 
				{
					//do nothing for the first 0.4 seconds
				}
			}
		}
	}

	private void DoTickEffectWithSound(string soundName)
	{
		FSoundManager.PlaySound(soundName);
		DoTapEffect();
	}
	
	private void HandlePress()
	{
		DoTickEffectWithSound(normalSoundName);
		if(SignalTick != null) SignalTick();

		if(_shouldRepeat)
		{
			_touchTime = Time.time;
			_liveTicks = 0;
			_isTouchDown = true;
		}
	}

	private void HandleRelease()
	{
		_isTouchDown = false;
	}

	private void UpdateRepeat()
	{
	}

	public bool shouldRepeat
	{
		get {return _shouldRepeat;}
		set {if(_shouldRepeat != value) {_shouldRepeat = value; UpdateRepeat();}}
	}

}

