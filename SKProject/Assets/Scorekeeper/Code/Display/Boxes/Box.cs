using UnityEngine;
using System;
using System.Collections.Generic;

public class Box : FContainer, FSmartTouchableInterface, SKDestroyable
{
	public Action<Box> SignalPress;
	public Action<Box> SignalRelease;
	public Action<Box> SignalReleaseOutside;

	public BoxSprite boxSprite;
	public List<FSprite> contentSprites = new List<FSprite>();
	public FContainer contentContainer;

	protected Player _player = null;

	protected float _tweenTimeElapsed;
	protected float _tweenTimeTotal;

	protected float _percent = 0.0f;

	protected bool _isTouchable = true;
	protected bool _isEnabled = true;

	protected float _width = 100.0f;
	protected float _height = 100.0f;

	protected FTouch _theTouch;

	protected RXTweenable _alphaTweenable;
	private RXTweenable _colorTweenable;
	private RXTweenable _colorDelayTweenable;

	protected bool _isTouchInBounds = false;

	protected bool _isFirstTimeEnabledSet = true;

	public Cell anchorCell;

	public float colorTweenDelay = 0.0f;


	public Box()
	{
		_alphaTweenable = new RXTweenable(1.0f);
		_alphaTweenable.SignalChange += () =>
		{
			this.alpha = _alphaTweenable.amount;
		};

		_colorTweenable = new RXTweenable(1.0f);

		_colorDelayTweenable = new RXTweenable(0.0f);
	}

	virtual public void Init(Player player)
	{
		Init(player,100,100);
	}

	virtual public void Init(Player player, float width, float height)
	{
		_width = width;
		_height = height;

		boxSprite = new BoxSprite();
		AddChild(boxSprite);

		AddChild(contentContainer = new FContainer());

		_player = player;
		InitPlayer();

		EnableSmartTouch();
		DoLayout();
	}

	virtual public void DoLayout ()
	{
		boxSprite.SetSize(_width,_height);
	}

	public void SetToCell(Cell cell)
	{
		_width = cell.width;
		_height = cell.height;
		this.x = cell.x;
		this.y = cell.y;
		DoLayout();
	}

	public void SetSize(float width, float height)
	{
		_width = width;
		_height = height; 
		DoLayout();
	}

	public void SetTopLeft(float leftX, float topY)
	{
		this.x = leftX + _width*0.5f;
		this.y = topY - _height*0.5f;
	}

	private void InitPlayer()
	{
		_player.SignalColorChange += HandlePlayerColorChange;
		boxSprite.color = _player.color.color;
	}

	private void HandlePlayerColorChange()
	{
		Color newColor = _player.color.color;

		RXTweenable delayTweenable = new RXTweenable(0.0f);

		delayTweenable.To(1.0f,colorTweenDelay+0.001f,new TweenConfig().onComplete(
		()=>
		{
			Color oldColor = boxSprite.color;
			
			//note: intentionally not using +=
			_colorTweenable.SignalChange = () =>
			{
				boxSprite.color = oldColor + (newColor - oldColor) * _colorTweenable.amount;
			};
			
			_colorTweenable.amount = 0.0f;
			
			_colorTweenable.To(1.0f,0.3f);
		}
		));
	}

	void HandleColorTweenDelayComplete()
	{
	}

	virtual public void Destroy()
	{
		this.RemoveFromContainer();
		_player.SignalColorChange -= HandlePlayerColorChange;
	}

	public void DoTapEffect()
	{
		Keeper.instance.CreateTapEffect(this,Config.PADDING_S);
	}
	
	#region FSmartTouchableInterface implementation

	bool FSmartTouchableInterface.HandleSmartTouchBegan (int touchIndex, FTouch touch)
	{
		if(!_isEnabled) return false;
		if(!_isTouchable) return false;
		if(touchIndex > 0) return false; //we only want the first touch for now

		_isTouchInBounds = GetLocalRect().Contains(GetLocalTouchPosition(touch));

		if(_isTouchInBounds)
		{
			_theTouch = touch;
			if(SignalPress != null) SignalPress(this);
			return true;
		}
		else 
		{
			return false;
		}
	}

	void FSmartTouchableInterface.HandleSmartTouchMoved (int touchIndex, FTouch touch)
	{
		_isTouchInBounds = GetLocalRect().Contains(GetLocalTouchPosition(touch));
	}

	void FSmartTouchableInterface.HandleSmartTouchEnded (int touchIndex, FTouch touch)
	{
		_isTouchInBounds = GetLocalRect().Contains(GetLocalTouchPosition(touch));

		if(_isTouchInBounds)
		{
			if(SignalRelease != null) SignalRelease(this);
		}
		else 
		{
			if(SignalReleaseOutside != null) SignalReleaseOutside(this);
		}
	}

	void FSmartTouchableInterface.HandleSmartTouchCanceled (int touchIndex, FTouch touch)
	{
		if(SignalReleaseOutside != null) SignalReleaseOutside(this);
	}

	#endregion

	public Rect GetLocalRect()
	{
		return new Rect(-_width*0.5f,-_height*0.5f,_width,_height);
	}

	private void UpdateEnabled()
	{
		float duration = 0.15f;

		if(_isFirstTimeEnabledSet)
		{
			duration = 0.01f;//super fast
			_isFirstTimeEnabledSet = false;
		}

		if(_isEnabled) 
		{
			_alphaTweenable.To(1.0f,duration);
		}
		else 
		{
			_alphaTweenable.To(0.5f,duration);
		}
	}

	private void UpdateTouchable()
	{
	}

	public Player player 
	{
		get {return _player;}
	}

	public bool isEnabled
	{
		get {return _isEnabled;}
		set {if(_isEnabled != value) {_isEnabled = value; UpdateEnabled();}}
	}

	public bool isTouchable
	{
		get {return _isTouchable;}
		set {if(_isTouchable != value) {_isTouchable = value; UpdateTouchable();}}
	}

	public float width
	{
		get {return _width;}
		set {if(_width != value) {_width = value; DoLayout();}}
	}
	
	public float height
	{
		get {return _height;}
		set {if(_height != value) {_height = value; DoLayout();}}
	}
}

public class SpriteBox : Box
{
	public FSprite sprite;
	public SpriteBox(Player player, string elementName, float width, float height)
	{
		Init(player,width,height);
		contentContainer.AddChild(sprite = new FSprite(elementName));
		contentSprites.Add(sprite);
		sprite.color = Color.black;
	}
}
