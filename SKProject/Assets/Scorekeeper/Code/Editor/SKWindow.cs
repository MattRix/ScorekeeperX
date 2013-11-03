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
	} 
} 

