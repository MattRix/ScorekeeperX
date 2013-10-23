using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SKMain : MonoBehaviour
{	
	static public SKMain instance;
	
	private void Start()
	{
		instance = this; 
		
		Go.defaultEaseType = EaseType.Linear;
		Go.duplicatePropertyRule = DuplicatePropertyRuleType.RemoveRunningProperty;
		
		// Uncomment if you need to delete bad save data on startup
		// PlayerPrefs.DeleteAll();
		
		//Time.timeScale = 0.1f; //use for checking timings of things in slow motion

		//only support portrait
		FutileParams fparams = new FutileParams(true, true, false, true);
		
		fparams.backgroundColor = RXUtils.GetColorFromHex(0x050122); 
		fparams.shouldLerpToNearestResolutionLevel = false;
		
		bool shouldHaveScale1 = true;
		bool shouldHaveScale2 = true;
		bool shouldHaveScale4 = true;
		
		#if UNITY_WEBPLAYER
		//webplayer has everything but scale2 stripped out
		shouldHaveScale1 = false;
		shouldHaveScale4 = false;
		#endif
		
		if(shouldHaveScale1) fparams.AddResolutionLevel(480.0f,		1.0f,	1.0f,	"_Scale1"); //iPhone
		if(shouldHaveScale2) fparams.AddResolutionLevel(960.0f,		2.0f,	2.0f,	"_Scale2"); //iPhone retina
		if(shouldHaveScale2) fparams.AddResolutionLevel(1024.0f,	2.0f,	2.0f,	"_Scale2"); //iPad
		if(shouldHaveScale2) fparams.AddResolutionLevel(1280.0f,	2.0f,	2.0f,	"_Scale2"); //Nexus 7
		if(shouldHaveScale4) fparams.AddResolutionLevel(2048.0f,	4.0f,	4.0f,	"_Scale4"); //iPad Retina
		
		fparams.origin = new Vector2(0.5f,0.5f);
		
		Futile.instance.Init (fparams);
		
		Futile.atlasManager.LoadAtlas("Atlases/MainAtlas");
		Futile.atlasManager.LoadImage("Atlases/Fonts/Raleway");
		
		FTextParams textParams;
		
		textParams = new FTextParams();
		textParams.kerningOffset = -0.5f;
		textParams.lineHeightOffset = -8.0f;
		Futile.atlasManager.LoadFont("Raleway","Atlases/Fonts/Raleway", "Atlases/Fonts/Raleway"+Futile.resourceSuffix, -2.0f,-5.0f,textParams);

		FSprite ph = new FSprite("Icons/Box");
		Futile.stage.AddChild(ph);
		ph.width = Futile.screen.width - 10.0f;
		ph.height = Futile.screen.height - 10.0f;

		//FLabel label = new FLabel("Raleway","HELLO WORLD");
		//Futile.stage.AddChild(label);
	}
}









