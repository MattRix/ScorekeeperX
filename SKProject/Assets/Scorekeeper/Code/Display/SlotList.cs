using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

public class SlotList : FContainer
{
	private List<Slot> _slots = new List<Slot>();

	public Action SignalPlayerChange;

	public FContainer slotContainer;

	private float _minScrollY = 0;
	private float _maxScrollY = 0;

	private bool _canScroll = false;

	private float _width;
	private float _height;

	private RXScroller _scroller;

	private FTouchSlot _touchSlot;

	public SlotList(float width, float height)
	{
		_width = width;
		_height = height;

		_scroller = new RXScroller(0,_minScrollY,_maxScrollY);

		_touchSlot = Futile.touchManager.GetTouchSlot(0);

		AddChild(slotContainer = new FContainer());

		List<Player> players = SKDataManager.GetPlayers();

		for(int p = 0; p<players.Count; p++)
		{
			AddSlotForPlayer(players[p]);
		}

		ListenForUpdate(HandleUpdate);
	}

	public void RemoveSlotForPlayer(Player player, bool shouldDoInstantly, bool shouldReorder)
	{
		Slot slotToRemove = GetSlotForPlayer(player);
		if(slotToRemove == null) return;

		_slots.Remove(slotToRemove);

		Go.killAllTweensWithTarget(slotToRemove.buildIn);
		float duration = shouldDoInstantly ? 0.0f : 0.3f;
		slotToRemove.buildIn.Tween(duration,0.0f).setEaseType(EaseType.Linear).onComplete(slotToRemove.RemoveFromContainer);

		if(shouldReorder) Reorder(false,false,false);

		if(SignalPlayerChange != null) SignalPlayerChange();

		SKDataManager.MarkDirty();
	}

	public Slot GetSlotForPlayer(Player player)
	{
		for(int s = 0; s<_slots.Count; s++)
		{
			if(_slots[s].player == player) return _slots[s];
		}
		return null;
	}

	private void HandleUpdate()
	{
		_scroller.SetBounds(_minScrollY,_maxScrollY);

		if(_canScroll)
		{
			if(_touchSlot.didJustBegin)
			{
				_scroller.BeginDrag(GetLocalTouchPosition(_touchSlot.touch).y);
			}
			else if (_touchSlot.didJustEnd || _touchSlot.didJustCancel)
			{
				_scroller.EndDrag(GetLocalTouchPosition(_touchSlot.touch).y);
			}
			else if(_touchSlot.doesHaveTouch)
			{
				_scroller.UpdateDrag(GetLocalTouchPosition(_touchSlot.touch).y);

				if(!_touchSlot.wasArtificiallyCanceled)
				{
					if(_scroller.GetDragDistance() > 10.0f)
					{
						_touchSlot.Cancel();
					}
				}
			}
		}

		bool isMoving = _scroller.Update();

		if(isMoving)
		{
			Go.killAllTweensWithTarget(this);
			slotContainer.y = -_scroller.GetPos();
		}
		else 
		{
			_scroller.SetPos(-slotContainer.y);
		}
	}

	public void AddSlotForPlayer(Player player)
	{
		Slot slot = new Slot(player, _width, Config.SLOT_HEIGHT);

		slotContainer.AddChild(slot);

		_slots.Add(slot);

		Reorder(false,false,false);

		if(SignalPlayerChange != null) SignalPlayerChange();
	}

	public void Reorder(bool shouldWaitUntilMathModeFinishes, bool isFlipping, bool shouldScrollToTop)
	{
		if(shouldWaitUntilMathModeFinishes)
		{
			for(int s = 0; s<_slots.Count; s++)
			{
				if(_slots[s].isMathMode) return; //don't sort if one of them is in math mode
			}
		}

		List<Slot> originalSlots = new List<Slot>(_slots); //create a copy

		Slot oldWinningSlot = _slots[0];

		float totalHeight = _slots.Count * (Config.SLOT_HEIGHT+Config.PADDING_M) - Config.PADDING_M;

		_minScrollY = Mathf.Min(0,-(totalHeight - _height)/2 - Config.PADDING_M);
		_maxScrollY = Mathf.Max(0,(totalHeight - _height)/2 + Config.PADDING_M);

		_canScroll = (totalHeight > _height);

		if(isFlipping)
		{
			_slots.Reverse();
		}
		else 
		{
			_slots.Sort(SlotSorter);
		}

		for(int s = 0; s<_slots.Count; s++)
		{
			Slot slot = _slots[s];

			slotContainer.AddChildAtIndex(slot,0); //add at the bottom

			float newY = totalHeight*0.5f - Config.SLOT_HEIGHT*0.5f - (Config.SLOT_HEIGHT+Config.PADDING_M)*s;

			if(slot.index == -1) //it's new!
			{
				slot.y = newY;

				float delay = _slots.Count == 1 ? 0 : 0.3f; //only delay if there are other players

				slot.buildIn.Tween(0.5f,1.0f).setEaseType(EaseType.Linear).setDelay(delay);

				//slot.buildIn.amount = 1.0f;

				//NOTE: EaseType.ExpoOut is broken!
			}
			else if(slot.index < s) //moving down
			{
				slot.buildIn.Tween(0.5f,1.0f).setEaseType(EaseType.Linear);

				slot.Tween(0.5f).floatProp("y",newY).setEaseType(EaseType.ExpoInOut);

				//do shrink tween
				slot.scaleX = 1.0f;
				slot.scaleY = 1.0f;
			}
			else if(slot.index > s) //moving up
			{
				slot.buildIn.Tween(0.5f,1.0f).setEaseType(EaseType.Linear);
				slot.Tween(0.5f).floatProp("y",newY).setEaseType(EaseType.ExpoInOut);
				//do grow tween
				slot.scaleX = 1.0f;
				slot.scaleY = 1.0f;
			}
			else 
			{
				if(slot.y != newY)
				{
					slot.buildIn.Tween(0.5f,1.0f).setEaseType(EaseType.Linear);
					slot.Tween(0.5f).floatProp("y",newY).setEaseType(EaseType.ExpoInOut);
				}
			}

			slot.index = s;
		}

		bool isThereANewWinner = (oldWinningSlot != _slots[0]);

		if(isThereANewWinner || shouldScrollToTop)
		{
			ScrollToTop(1.5f);
		}

		if(isThereANewWinner) 
		{
			//TODO: play winner sound
			_slots[0].player.color.PlaySound();
		}

		if(!RXUtils.AreListsEqual(_slots,originalSlots))
		{
			FSoundManager.PlaySound("UI/Sort");
		}

		SKDataManager.MarkDirty();
	}

	private void ScrollToTop(float time)
	{
		Go.killAllTweensWithTarget(this);
		Go.to(slotContainer, time, new TweenConfig().floatProp("y", _minScrollY).setEaseType(EaseType.ExpoInOut));
	}

	private int SlotSorter(Slot slotA, Slot slotB)
	{
		if(SKDataManager.sortType != SortType.Manual)
		{
			bool isHighestAtTop = (SKDataManager.sortType == SortType.HighestAtTop);

			int indexMultiplier = isHighestAtTop ? -1 : 1;

			int scoreA = indexMultiplier*slotA.index + slotA.player.score * 100;
			int scoreB = indexMultiplier*slotB.index + slotB.player.score * 100;

			if(isHighestAtTop)
			{
				if(scoreA < scoreB) return 1;
				if(scoreA > scoreB) return -1;
			}
			else
			{
				if(scoreA < scoreB) return -1;
				if(scoreA > scoreB) return 1;
			}
		}

		return 0;
	}

	public List<Player> GetPlayers()
	{
		List<Player> players = new List<Player>();
		
		for(int s = 0; s<_slots.Count; s++)
		{
			players.Add(_slots[s].player);
		}

		return players;
	}

//	#region FMultiTouchableInterface implementation
//
//	void FMultiTouchableInterface.HandleMultiTouch(FTouch[] touches)
//	{
//		if(touches[0].slot.index == 0)
//		{
//		}
//	}
//
//	#endregion

	public List<Slot> slots
	{
		get {return _slots;}
	}
}










