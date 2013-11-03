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

	private bool _isInitializing = true;

	private bool _isFlipping = false;

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
			AddSlotForPlayer(players[p], false);
		}

		Reorder(false,false);

		ListenForUpdate(HandleUpdate);

		_isInitializing = false;
	}

	public void RemoveSlotForPlayer(Player player, bool shouldDoInstantly, bool shouldReorder)
	{
		Slot slotToRemove = GetSlotForPlayer(player);
		if(slotToRemove == null) return;

		_slots.Remove(slotToRemove);

		if(shouldDoInstantly)
		{
			slotToRemove.Destroy();
		}
		else 
		{
			float duration = 0.3f;
			slotToRemove.buildIn.To(0,duration,new TweenConfig().onComplete(slotToRemove.Destroy));
			Go.to(slotToRemove, duration, new TweenConfig().floatProp("scale",0.8f));
		}

		//move all the indexes below it down (so they don't think they have to animate)
		for(int s = 0; s<_slots.Count; s++)
		{
			if(_slots[s].index > slotToRemove.index)
			{
				_slots[s].index --;
			}
		}

		if(shouldReorder) Reorder(false,false);

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
		_scroller.SetPos(-slotContainer.y);
		_scroller.SetBounds(_minScrollY,_maxScrollY);

		if(_canScroll)
		{
			if(_touchSlot.didJustBegin)
			{
				_scroller.BeginDrag(GetLocalTouchPosition(_touchSlot.touch).y);
			}
		}

		if (_touchSlot.didJustEnd || _touchSlot.didJustCancel)
		{
			_scroller.EndDrag(GetLocalTouchPosition(_touchSlot.touch).y);
		}
		else if(_touchSlot.doesHaveTouch)
		{
			if(_canScroll)
			{
				_scroller.UpdateDrag(GetLocalTouchPosition(_touchSlot.touch).y);

				if(!_touchSlot.wasArtificiallyCanceled)
				{
					if(_scroller.GetDragDistance() > Config.MIN_DRAG_DISTANCE)
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

		}
	}

	public void AddSlotForPlayer(Player player, bool shouldReorder)
	{
		Slot slot = new Slot(player, _width, Config.SLOT_HEIGHT);

		slotContainer.AddChild(slot);

		_slots.Add(slot);

		if(shouldReorder)
		{
			Reorder(false,false);
		}

		if(SignalPlayerChange != null) SignalPlayerChange();
	}

	public void Reorder(bool shouldWaitUntilMathModeFinishes, bool shouldScrollToTop)
	{
		Reorder(shouldWaitUntilMathModeFinishes,shouldScrollToTop,false);
	}

	public void Reorder(bool shouldWaitUntilMathModeFinishes, bool shouldScrollToTop, bool isFlipping)
	{
		if(_slots.Count == 0) return; //no need sorting things that don't exist :) 

		if(shouldWaitUntilMathModeFinishes)
		{
			for(int s = 0; s<_slots.Count; s++)
			{
				if(_slots[s].scoreBox.mathMode.amount > 0) return; //don't sort if one of them is in math mode
			}
		}

		_isFlipping = isFlipping;

		List<Slot> originalSlots = new List<Slot>(_slots); //create a copy

		Slot oldWinningSlot = _slots[0];

		float totalHeight = _slots.Count * (Config.SLOT_HEIGHT+Config.PADDING_M) - Config.PADDING_M;

		_minScrollY = Mathf.Min(0,-(totalHeight - _height)/2 - Config.PADDING_M);
		_maxScrollY = Mathf.Max(0,(totalHeight - _height)/2 + Config.PADDING_M);

		_canScroll = (totalHeight > _height);

		_slots.Sort(SlotSorter);

		for(int s = 0; s<_slots.Count; s++)
		{
			Slot slot = _slots[s];

			slotContainer.AddChildAtIndex(slot,0); //add at the bottom

			float newY = totalHeight*0.5f - Config.SLOT_HEIGHT*0.5f - (Config.SLOT_HEIGHT+Config.PADDING_M)*s;

			if(slot.index == -1) //it's new!
			{
				slot.y = newY;

				float delay;

				if(_isInitializing)
				{
					delay = 0.2f + (float)s * 0.1f;
				}
				else 
				{
					delay = _slots.Count == 1 ? 0 : 0.3f; //only delay if there are other players
				}

				//note how we make the tween longer AND delay it by the delay. weird effect :)
				Go.killAllTweensWithTarget(slot.buildIn);
				Go.to(slot.buildIn, 0.5f + delay, new TweenConfig().floatProp("amount",1.0f).setDelay(delay)); 
			}
			else if(slot.index < s) //moving down
			{
				Go.killAllTweensWithTarget(slot.buildIn);
				Go.to(slot.buildIn, 0.5f, new TweenConfig().floatProp("amount",1.0f));

				Go.killAllTweensWithTarget(slot);
				Go.to(slot, 0.5f, new TweenConfig().floatProp("y",newY).setEaseType(EaseType.ExpoInOut));

				float delta = s - slot.index ;
				float scaleAmount = 0.04f + delta * 0.0075f;

				//slot shrink more the farther it travels downward

				RXTweenable tw = new RXTweenable(0.0f);
				tw.SignalChange += () => 
				{
					slot.scale = 1.0f - scaleAmount * RXEase.UpDown(tw.amount,RXEase.SineIn);
				};

				Go.to(tw, 0.5f, new TweenConfig().floatProp("amount",1.0f).setEaseType(EaseType.Linear));
			}
			else if(slot.index > s) //moving up
			{
				Go.killAllTweensWithTarget(slot.buildIn);
				Go.to(slot.buildIn, 0.5f, new TweenConfig().floatProp("amount",1.0f));

				Go.killAllTweensWithTarget(slot);
				Go.to(slot, 0.5f, new TweenConfig().floatProp("y",newY).setEaseType(EaseType.ExpoInOut));

				//slot grows more the farther it travels upward
				float delta = slot.index - s;
				float scaleAmount = 0.04f + delta * 0.0075f;

				RXTweenable tw = new RXTweenable(0.0f);
				tw.SignalChange += () => 
				{
					slot.scale = 1.0f + scaleAmount * RXEase.UpDown(tw.amount,RXEase.SineIn);
				};
				
				Go.to(tw, 0.5f, new TweenConfig().floatProp("amount",1.0f).setEaseType(EaseType.Linear));
			}
			else 
			{
				if(slot.y != newY)
				{
					Go.killAllTweensWithTarget(slot.buildIn);
					Go.to(slot.buildIn, 0.5f, new TweenConfig().floatProp("amount",1.0f));

					Go.killAllTweensWithTarget(slot);
					Go.to(slot, 0.5f, new TweenConfig().floatProp("y",newY).setEaseType(EaseType.ExpoInOut));
				}
			}

			slot.index = s;
		}

		bool isThereANewWinner = (oldWinningSlot != _slots[0]);

		if(isThereANewWinner || shouldScrollToTop)
		{
			ScrollToTop(1.0f);
		}
		else if (!_canScroll)
		{
			ScrollToTop(0.5f); //make sure it's always centered
		}

		if(isThereANewWinner) 
		{
			//TODO: play winner sound
			_slots[0].player.color.PlayNormalSound();
		}

		if(!RXUtils.AreListsEqual(_slots,originalSlots))
		{
			FSoundManager.PlaySound("UI/Sort");
		}

		SKDataManager.MarkDirty();

		_isFlipping = false;
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


			//this indexMultiplier stuff has to do with how to handle ties

			int indexMultiplier;

			if(_isFlipping) //when flipping we want EVERYTHING to flip, even tied numbers
			{
				indexMultiplier = isHighestAtTop ? 1 : -1;
			}
			else //when a tie happens normally, we want to keep the incumbent "on top"
			{
				indexMultiplier = isHighestAtTop ? -1 : 1;
			}

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










