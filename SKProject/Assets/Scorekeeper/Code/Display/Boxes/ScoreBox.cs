using UnityEngine;
using System;
using System.Collections.Generic;


public class ScoreBox : Box
{
	public Slot slot;
	public FLabel scoreLabel;

	public float _mathModeTweenAmount = 0.0f;

	public float mathModeTweenAmount
	{
		get {return _mathModeTweenAmount;}
		set {_mathModeTweenAmount = value;}
	}
	
	public ScoreBox(Slot slot)
	{
		this.slot = slot;
		
		base.Init(slot.player);
		
		contentContainer.AddChild(scoreLabel = new FLabel("Raleway","0"));
		scoreLabel.color = Color.black;
		
		isTouchable = false;
		
		slot.player.SignalScoreChange += HandleScoreChange;
		
		HandleScoreChange();
	}
	
	private void HandleScoreChange()
	{
		scoreLabel.text = slot.player.score.ToString();
		
		DoLayout();
	}
	
	override public void DoLayout()
	{
		base.DoLayout();
		if(scoreLabel != null)
		{
			float availWidth = this.width - Config.PADDING_M*3;
			float availHeight = this.height - Config.PADDING_M*2;
			
			float labelScale = Mathf.Min(1.0f, availHeight/scoreLabel.textRect.height,availWidth/scoreLabel.textRect.width);
			
			scoreLabel.scale = Mathf.Clamp01(labelScale);
		}
	}
}