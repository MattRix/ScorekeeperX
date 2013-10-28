using UnityEngine;
using System;
using System.Collections.Generic;

public class RepeatableBox : Box
{
	public Action<int> SignalTick;

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

			int ticksToSend = 0;

			string soundName = null;
			bool shouldPlaySound = false;

			while(_liveTicks < totalTicks)
			{
				_liveTicks++;

				if(_liveTicks % 15 == 0) //every 150 ms play a sound
				{
					shouldPlaySound = true;
				}

				if(hasHyperRepeatZones && _liveTicks >= 1000) //10 seconds hyper
				{
					//every 10 ms
					ticksToSend += 5;
				}
				if(hasHyperRepeatZones && _liveTicks >= 400) //4 seconds hyper
				{
					soundName = fastestSoundName;

					//every 10 ms
					ticksToSend += 1;
				}
				else if(hasHyperRepeatZones && _liveTicks >= 200) //2 seconds faster
				{
					soundName = fastSoundName;

					if(_liveTicks % 5 == 0) //every 50 ms
					{
						ticksToSend += 1;
					}
				}
				else if(_liveTicks >= 40) //0.4 seconds fast
				{
					if(_liveTicks % 15 == 0) //every 150 ms
					{
						soundName = normalSoundName;
						ticksToSend += 1;
					}
				}
				else 
				{
					//do nothing for the first 0.4 seconds
				}
			}

			if(shouldPlaySound && soundName != null)
			{
				DoTickEffectWithSound(soundName);
			}

			if(ticksToSend > 0)
			{
				if(ticksToSend >= 5)
				{
					if(Time.frameCount % 30 == 0)
					{
						DoTickEffectWithSound(soundName);
					}
				}

				if(SignalTick != null) SignalTick(ticksToSend);
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
		if(SignalTick != null) SignalTick(1);

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

