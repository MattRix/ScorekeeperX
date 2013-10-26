using UnityEngine;
using System;
using System.Collections.Generic;

public class Box : FContainer, FSmartTouchableInterface
{
	public Action SignalPress;
	public Action SignalRelease;
	public Action SignalReleaseOutside;

	public List<BoxSprite> boxSprites = new List<BoxSprite>();
	public List<FSprite> contentSprites = new List<FSprite>();
	public FContainer contentContainer;

	protected Player _player;

	protected float _tweenTimeElapsed;
	protected float _tweenTimeTotal;

	protected float _percent = 0.0f;

	protected bool _isTouchable = true;
	protected bool _isEnabled = true;

	protected float _width = 100.0f;
	protected float _height = 100.0f;

	protected FTouch _theTouch;

	public Box()
	{

	}

	virtual public void Init(Player player)
	{
		Init(player,100,100);
	}

	virtual public void Init(Player player, float width, float height)
	{
		_width = width;
		_height = height;

		BoxSprite boxSprite = new BoxSprite();
		boxSprites.Add(boxSprite);
		AddChild(boxSprite);

		AddChild(contentContainer = new FContainer());

		_player = player;
		UpdatePlayer();

		EnableSmartTouch();
		DoLayout();
	}

	virtual public void DoLayout ()
	{
		boxSprites[0].SetSize(_width,_height);
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


	protected void UpdatePlayer ()	
	{
		boxSprites.ForEach(boxSprite => {boxSprite.color = _player.color.color;});
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

		if(GetLocalRect().Contains(GetLocalTouchPosition(touch)))
		{
			_theTouch = touch;
			if(SignalPress != null) SignalPress();
			return true;
		}
		else 
		{
			return false;
		}
	}

	void FSmartTouchableInterface.HandleSmartTouchMoved (int touchIndex, FTouch touch)
	{
	}

	void FSmartTouchableInterface.HandleSmartTouchEnded (int touchIndex, FTouch touch)
	{
		if(GetLocalRect().Contains(GetLocalTouchPosition(touch)))
		{
			if(SignalRelease != null) SignalRelease();
		}
		else 
		{
			if(SignalReleaseOutside != null) SignalReleaseOutside();
		}
	}

	void FSmartTouchableInterface.HandleSmartTouchCanceled (int touchIndex, FTouch touch)
	{
		if(SignalReleaseOutside != null) SignalReleaseOutside();
	}

	#endregion

	public Rect GetLocalRect()
	{
		return new Rect(-_width*0.5f,-_height*0.5f,_width,_height);
	}

	private void UpdateEnabled()
	{
		if(_isEnabled)
		{
			this.alpha = 1.0f;
		}
		else 
		{
			this.alpha = 0.2f;
		}
	}

	private void UpdateTouchable()
	{
	}

	public Player player 
	{
		get {return _player;}
		set {if(_player != value) {_player = value; UpdatePlayer();}}
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

