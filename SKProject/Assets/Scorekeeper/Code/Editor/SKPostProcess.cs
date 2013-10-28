using System.IO;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;



//use this to do different things with different build targets
public class SKPostProcessBuild
{
	[PostProcessBuild (0)]
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
		Debug.Log("POST PROCESSED BUILD! " + target);
	}
}

//use this to run something in the editor every time the game is run
public static class SKPostProcessScene
{
	[PostProcessScene (0)]
	public static void OnPostProcessScene()
	{
		Debug.Log("POST PROCESSED SCENE!");
	} 
}

class SKAssetProcessor : AssetPostprocessor 
{
	public static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) 
	{ 
		//return; //comment this out if you want verbose logs when importing
		
		//Debug.Log("OnPostprocessAllAssets:");
		if(importedAssets.Length != 0) importedAssets.Log("importedAssets");
		if(deletedAssets.Length != 0) deletedAssets.Log("deletedAssets");
		if(movedAssets.Length != 0) movedAssets.Log("movedAssets");
		if(movedFromAssetPaths.Length != 0) movedFromAssetPaths.Log("movedFromAssetPaths");
		
		bool doesNeedRefresh = false;
		
		for(int s = 0;s<importedAssets.Length;s++)
		{
			
			//			// LOADING XML and then REMOVING COMMENT NODES and then SAVING IT IN /CLEAN
			//
			//			if(importedAssets[s] == "Assets/Resources/Data/Episodes.xml")
			//			{
			//				TextAsset asset = AssetDatabase.LoadAssetAtPath(importedAssets[s],typeof(TextAsset)) as TextAsset;
			//
			//				string result = asset.text;
			//
			//				int startCommentIndex = result.IndexOf("<!--");
			//
			//				while(startCommentIndex != -1)
			//				{
			//					int endCommentIndex = result.IndexOf("-->", startCommentIndex);
			//
			//					result = result.Remove(startCommentIndex,(endCommentIndex-startCommentIndex + 3));
			//
			//					startCommentIndex = result.IndexOf("<!--");
			//				}
			//
			//				File.WriteAllText(Application.dataPath + "/Resources/Data/Clean/EpisodesClean.xml", result);
			//				doesNeedRefresh = true; 
			//			}
		}
		
		if(doesNeedRefresh)
		{
			AssetDatabase.Refresh();
		}
		
		
	}
}