using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SKMain : MonoBehaviour
{	
	private void Start()
	{
		Go.defaultEaseType = EaseType.Linear;
		Go.duplicatePropertyRule = DuplicatePropertyRuleType.RemoveRunningProperty;
		
		// Uncomment if you need to delete bad save data on startup
		// PlayerPrefs.DeleteAll();
		
		//Time.timeScale = 0.1f; //use for checking timings of things in slow motion

		//only support portrait
		FutileParams fparams = new FutileParams(true, true, false, true);
		
		fparams.backgroundColor = RXUtils.GetColorFromHex(0x050122); 
		fparams.shouldLerpToNearestResolutionLevel = false;
		fparams.resolutionLevelPickMode = FResolutionLevelPickMode.Closest;
		
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
		if(shouldHaveScale2) fparams.AddResolutionLevel(1136.0f,	2.0f,	2.0f,	"_Scale2"); //iPhone 5 retina
		if(shouldHaveScale2) fparams.AddResolutionLevel(1280.0f,	2.0f,	2.0f,	"_Scale2"); //Nexus 7
		if(shouldHaveScale4) fparams.AddResolutionLevel(2048.0f,	4.0f,	4.0f,	"_Scale4"); //iPad Retina
		
		fparams.origin = new Vector2(0.5f,0.5f);
		
		Futile.instance.Init (fparams);
		
		Futile.atlasManager.LoadAtlas("Atlases/MainAtlas");
		//Futile.atlasManager.LoadImage("Atlases/Fonts/Raleway");
		//Futile.atlasManager.LoadImage("Atlases/Fonts/Ostrich");

		FTextParams textParams;
		
		textParams = new FTextParams();
		textParams.kerningOffset = -0.0f;
		textParams.lineHeightOffset = -15.0f;
		Futile.atlasManager.LoadFont("Raleway","Fonts/Raleway"+Futile.resourceSuffix, "Atlases/Fonts/Raleway"+Futile.resourceSuffix, -1.0f,-1.0f,textParams);

//		
//		textParams = new FTextParams();
//		textParams.kerningOffset = -0.0f;
//		textParams.lineHeightOffset = -15.0f;
//		Futile.atlasManager.LoadFont("Ostrich","Atlases/Fonts/Ostrich", "Atlases/Fonts/Ostrich"+Futile.resourceSuffix, 0.0f,-2.0f,textParams);
//		
		for(int s = 0; s<10; s++)
		{
			FSoundManager.PreloadSound("Musical/Note"+s+"_bass");
			FSoundManager.PreloadSound("Musical/Note"+s+"_normal");
		}

		Futile.stage.AddChild(new Keeper()); //keeper statically retains itself and never gets removed
	}

	void OnApplicationQuit()
	{
		if(Keeper.instance != null)
		{
			SKDataManager.SaveData();
		}
	}
	
	void OnApplicationPause(bool isPaused)
	{
		if(Keeper.instance != null)
		{
			SKDataManager.SaveData();
		}
	}
}









