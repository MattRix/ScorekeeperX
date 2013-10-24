using UnityEngine;
using System;
using System.Collections.Generic;
using System.Timers;

public class RXWeak
{
	private static List<RXWeakListener> _listeners;
	private static Timer _cleanUpTimer;

	static RXWeak()
	{
		_listeners = new List<RXWeakListener>();

		_cleanUpTimer = new Timer(5000); //run cleanup every 5 seconds
		_cleanUpTimer.Elapsed += HandleCleanUpTimerTick;
		_cleanUpTimer.Enabled = true; 
	}

	static void HandleCleanUpTimerTick (object sender, ElapsedEventArgs e)
	{
		CleanUp();
	}

	//removes unused listeners periodically (and can be called manually if needed)
	//an unused listener is one where the target has already been garbage collected
	public static void CleanUp()
	{
		//reverse order so removals are easy
		for(int n = _listeners.Count-1; n>=0; n--)
		{
			if(!_listeners[n].weakRef.IsAlive)
			{
				_listeners.RemoveAt(n);
			}
		}
	}

	public static Action Add(Action callback)
	{
		int timesAdded = 0;
		//if we already have a listener, remove it!
		for(int n = 0; n<_listeners.Count; n++)
		{
			Action dele = (_listeners[n].weakRef.Target as Action);

			if(dele == callback)
			{
				timesAdded = _listeners[n].timesAdded;
				_listeners.RemoveAt(n);
				break;
			}
		}

		Debug.Log ("add " + timesAdded);

		RXWeakListener listener = new RXWeakListener();
		listener.weakRef = new WeakReference(callback);
		listener.timesAdded += timesAdded;
		_listeners.Add(listener);
		return listener.InnerCallback;
	}

	public static Action Remove(Action callback)
	{
		for(int n = 0; n<_listeners.Count; n++)
		{
			Action dele = (_listeners[n].weakRef.Target as Action);

			if(dele == callback)
			{
				RXWeakListener listener = _listeners[n];
				listener.timesAdded--;
				if(listener.timesAdded <= 0)
				{
					_listeners.RemoveAt(n);
				}
				return listener.InnerCallback;
			}
		}
		return null; //this shouldn't ever really happen unless someone adds more listeners than they remove
	}

	private class RXWeakListener
	{
		public WeakReference weakRef;
		public int timesAdded = 1;

		public void InnerCallback()
		{
			Action dele = (weakRef.Target as Action);

			if(dele != null)
			{
				dele();
			}
			else 
			{
				_listeners.Remove(this);
			}
		}
	}
}

