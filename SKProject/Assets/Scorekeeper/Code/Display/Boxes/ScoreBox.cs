using UnityEngine;
using System;
using System.Collections.Generic;


public class ScoreBox : Box
{
	public Slot slot;

	public RXTweenable mathMode;
	private bool _isMathMode = false;

	private FLabel _scoreLabel;
	private FLabel _baseLabel;
	private FLabel _deltaLabel;
	private FSprite _signIcon;
	private FSprite _equalsIcon;
	private float _deltaTargetX;
	private float _signTargetX;
	private float _equalsTargetX;
	private float _baseTargetX;

	private int _baseScore = 0;
	
	public ScoreBox(Slot slot)
	{
		this.slot = slot;

		mathMode = new RXTweenable(0.0f,HandleMathModeChange);
		
		base.Init(slot.player);
		
		contentContainer.AddChild(_scoreLabel = new FLabel("Raleway",""));
		_scoreLabel.color = Color.black;

		_baseLabel = new FLabel("Raleway","123");
		_baseLabel.color = Color.black;

		_deltaLabel = new FLabel("Raleway","22");
		_deltaLabel.color = Color.black;

		_signIcon = new FSprite("Icons/Plus");
		_signIcon.color = Color.black;

		_equalsIcon = new FSprite("Icons/Equals");
		_equalsIcon.color = Color.black;

		
		isTouchable = false;
		
		slot.player.SignalScoreChange += HandleScoreChange;

		ListenForUpdate(HandleUpdate);
		HandleMathModeChange();
		HandleScoreChange();
	}

	void HandleMathModeChange()
	{

	}
	
	private void HandleScoreChange()
	{
		_scoreLabel.text = slot.player.score.ToString();
		
		DoLayout();
	}

	public void ResetBaseScore()
	{
		_baseScore = slot.player.score;
	}

	void StartMathMode()
	{
		AddChild(_baseLabel);
		AddChild(_deltaLabel);
		AddChild(_signIcon);
		AddChild(_equalsIcon);
	}

	void EndMathMode()
	{
		_baseLabel.RemoveFromContainer();
		_deltaLabel.RemoveFromContainer();
		_signIcon.RemoveFromContainer();
		_equalsIcon.RemoveFromContainer();
	}

	private void HandleUpdate()
	{
		if(mathMode.amount > 0 && !_isMathMode)
		{
			_isMathMode = true;
			StartMathMode();
		} 
		else if(mathMode.amount == 0 && _isMathMode)
		{
			_isMathMode = false;
			EndMathMode();
		}

		if(_isMathMode)
		{
			_baseLabel.alpha = RXMath.GetSubPercent(mathMode.amount,0.4f,1.0f);
			_signIcon.alpha = RXMath.GetSubPercent(mathMode.amount,0.5f,1.0f);
			_deltaLabel.alpha = RXMath.GetSubPercent(mathMode.amount,0.6f,1.0f);
			_equalsIcon.alpha = RXMath.GetSubPercent(mathMode.amount,0.7f,1.0f);

			float scorePercent = RXEase.Linear(RXMath.GetSubPercent(mathMode.amount,0.0f,0.8f));
			float scoreTargetX = _scoreLabelFullX + (_scoreLabelCompactX - _scoreLabelFullX) * scorePercent;
			_scoreLabel.scale = _scoreLabelFullScale + (_scoreLabelCompactScale - _scoreLabelFullScale) * scorePercent;

			_scoreLabel.x += (scoreTargetX - _scoreLabel.x) * 0.5f;
			_deltaLabel.x += (_deltaTargetX - _deltaLabel.x) * 0.5f;
			_baseLabel.x += (_baseTargetX - _baseLabel.x) * 0.5f;
			_signIcon.x += (_signTargetX - _signIcon.x) * 0.5f;
			_equalsIcon.x += (_equalsTargetX - _equalsIcon.x) * 0.5f;
		}
		else 
		{
			_scoreLabel.x = _scoreLabelFullX;
			_scoreLabel.scale = _scoreLabelFullScale;
		}
	}

	private float _scoreLabelFullScale;
	private float _scoreLabelFullX;
	private float _scoreLabelCompactScale;
	private float _scoreLabelCompactX;
	
	override public void DoLayout()
	{
		if(_scoreLabel == null) return; //has to finish initing first
		base.DoLayout();

		float availWidth = this.width - Config.PADDING_M*3;
		float availHeight = this.height - Config.PADDING_M*2;

		float fullScale = Mathf.Min(0.75f, availWidth/_scoreLabel.textRect.width);
		_scoreLabelFullScale = Mathf.Clamp01(fullScale);
		_scoreLabelFullX = 0;

		// compactScale = Mathf.Min(0.5f, availWidth/_scoreLabel.textRect.width);
		//_scoreLabelCompactScale = Mathf.Clamp01(compactScale);
		//_scoreLabelCompactX = availWidth*0.5f - _scoreLabel.textRect.width*0.5f*compactScale;

		if(_isMathMode) //position the math mode elements here
		{
			int deltaScore = (slot.player.score - _baseScore);
			//TODO: update text here
			_baseLabel.text = _baseScore.ToString();
			_deltaLabel.text = Mathf.Abs(deltaScore).ToString(); //abs because we have our own minus symbol

//			if(deltaScore == 0)
//			{
//				//do nothing at 0, don't change the sign unless we have to
//			}

			if(deltaScore < 0)
			{
				_signIcon.element = Futile.atlasManager.GetElementWithName("Icons/Minus");
			}
			else 
			{
				_signIcon.element = Futile.atlasManager.GetElementWithName("Icons/Plus");
			}

			//update sign too
			float baseWidth = _baseLabel.textRect.width;
			float deltaWidth = _deltaLabel.textRect.width; 
			float scoreWidth = _scoreLabel.textRect.width;

			float masterScale = Mathf.Clamp01(availWidth/150.0f);

			float padding = 0.0f;
			float signSize = 28.0f*0.5f*masterScale;
			float deltaScale = 0.75f*masterScale;
			float availLabelWidth = availWidth-signSize*2.0f-padding*4.0f-deltaScale*deltaWidth;

			float scoreScale = Mathf.Min(deltaScale/1.2f, availLabelWidth/(baseWidth+scoreWidth));

			_signIcon.scale = 0.5f*masterScale;
			_equalsIcon.scale = 0.5f*masterScale;

			_baseLabel.scale = scoreScale;
			_deltaLabel.scale = deltaScale;
			_scoreLabelCompactScale = scoreScale;

			baseWidth *= _baseLabel.scale;
			deltaWidth *= _deltaLabel.scale;
			scoreWidth *= _scoreLabelCompactScale;

			float leftOverWidth = availLabelWidth - baseWidth - scoreWidth;

			float shrinker = Math.Max(0, leftOverWidth-16.0f);//we want at least 16 padding

			//left align baseWidth
			_baseTargetX = -availWidth/2.0f + baseWidth/2.0f + shrinker/2;

			//right align score
			_scoreLabelCompactX = availWidth/2.0f - scoreWidth/2.0f -shrinker/2;

			//put deltaLabel at the center between base and score
			_deltaTargetX = ((_baseTargetX + baseWidth/2.0f) + (_scoreLabelCompactX - scoreWidth/2.0f)) / 2.0f;

			//put the sign icon at the center beween base and delta
			_signTargetX = ((_baseTargetX + baseWidth/2.0f) + (_deltaTargetX - deltaWidth/2.0f)) / 2.0f;

			//put the equals icon at the center between delta and score
			_equalsTargetX = ((_deltaTargetX + deltaWidth/2.0f) + (_scoreLabelCompactX - scoreWidth/2.0f)) / 2.0f;
		}
		else 
		{

		}
	}
}





