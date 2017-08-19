using System.IO;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class SKWindow : EditorWindow
{
	[MenuItem ("Scorekeeper/Open Window")]
	static void Init () 
	{
		// Get existing open window or if none, make a new one:
		SKWindow window = (SKWindow)EditorWindow.GetWindow (typeof (SKWindow));
		window.position = new Rect(100,100,300,500);
		window.title = "SKWindow";
		window.Show(); 


	} 

	public void OnGUI()
	{
		Time.timeScale = EditorGUILayout.Slider("TimeScale: ",Time.timeScale,0.025f,2.0f);

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("0.1")) Time.timeScale = 0.1f;
		if(GUILayout.Button("0.5")) Time.timeScale = 0.5f;
		if(GUILayout.Button("1.0")) Time.timeScale = 1.0f;
		EditorGUILayout.EndHorizontal();

		GUILayout.Space(10);
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Clear PlayerPrefs")) PlayerPrefs.DeleteAll();
		EditorGUILayout.EndHorizontal();
	} 

	[MenuItem ("Scorekeeper/Build for iOS")]
	static void BuildForIOS() 
	{
		BuildOptions options = BuildOptions.None;
		options |= BuildOptions.AcceptExternalModificationsToPlayer;
		options |= BuildOptions.SymlinkLibraries;
		UnityEditor.BuildPipeline.BuildPlayer(new string[] {"Assets/Scorekeeper.unity"},"Export/iOS",BuildTarget.iOS,options);
	}

	[MenuItem ("Scorekeeper/Screenshot A 1x")]
	static void CaptureScreenshotA1() 
	{
		CaptureScreenshot("A",1);
	}

	[MenuItem ("Scorekeeper/Screenshot A 2x")]
	static void CaptureScreenshotA2() 
	{
		CaptureScreenshot("A",2);
	}

	[MenuItem ("Scorekeeper/Screenshot B 1x")]
	static void CaptureScreenshotB1() 
	{
		CaptureScreenshot("B",1);
	}
	
	[MenuItem ("Scorekeeper/Screenshot B 2x")]
	static void CaptureScreenshotB2() 
	{
		CaptureScreenshot("B",2);
	}

	static void CaptureScreenshot(string name, int superSize) 
	{
		int width = superSize*Mathf.RoundToInt(Futile.screen.pixelWidth);
		int height = superSize*Mathf.RoundToInt(Futile.screen.pixelHeight);
		Application.CaptureScreenshot("Screenshots/Screenshot_"+name+"_"+width+"x"+height+".png",superSize);
	}
} 



