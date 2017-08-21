using UnityEngine;
using System;
using System.Collections;
using System.IO;

public class Photographer : MonoBehaviour
{
	public static Photographer instance = null;
	public static int counter = 0;

	private void Start()
	{
		instance = this;
	}

	#if UNITY_EDITOR
	public void OnInspectorGUI()
	{
		if (GUILayout.Button("Take iPhone 4 screenshot"))
		{
			DoImmediateScreenshot("sw_iPhone4", 640, 960);
		}
		if (GUILayout.Button("Take iPhone 5 screenshot"))
		{
			DoImmediateScreenshot("sw_iPhone5", 640, 1136);
		}
		if (GUILayout.Button("Take iPhone 7 screenshot"))
		{
			DoImmediateScreenshot("sw_iPhone7", 750, 1334);
		}
		if (GUILayout.Button("Take iPhone 7 Plus screenshot"))
		{
			DoImmediateScreenshot("sw_iPhone7Plus", 1242, 2208);
		}
		if (GUILayout.Button("Take iPad screenshot"))
		{
			DoImmediateScreenshot("sw_iPad", 1536, 2048);
		}
		if (GUILayout.Button("Take iPad Pro screenshot"))
		{
			DoImmediateScreenshot("sw_iPadPro", 2048, 2732);
		}
		if (GUILayout.Button("Take 1920x2560 iPad shot"))
		{
			DoImmediateScreenshot("sw_iPad1920x2560", 1920, 2560);
		}
		if (GUILayout.Button("Take 1920x1080 PC shot"))
		{
			DoImmediateScreenshot("sw_PC_1920x1080", 1920, 1080);
		}
		if (GUILayout.Button("Take 800x1280 Android Tablet shot"))
		{
			DoImmediateScreenshot("sw_Android_800x1280", 800, 1280);
		}
		if (GUILayout.Button("Take 1200x1920 Android Tablet shot"))
		{
			DoImmediateScreenshot("sw_Android_1200x1920", 1200, 1920);
		}

		if (GUILayout.Button("---Take Landscape iPhone Screenshot"))
		{
			DoImmediateScreenshot("sk_iPhoneScreen", 2208, 1242);
		}
		if (GUILayout.Button("---Take Landscape iPad Screenshot"))
		{
			DoImmediateScreenshot("sk_iPadScreen", 2732, 2048);
		}
	}
#endif


	public void TakeScreenshot(string fileName, Action<string, string, float, float> doShare, Action onComplete)
	{
		StartCoroutine(DoTakeScreenshot(fileName, "", 0, 0, doShare, onComplete));
	}
	
	public void TakeScreenshotWithMessage(string fileName, string message, float x, float y, Action<string, string, float, float> doShare, Action onComplete)
	{
		Debug.Log("TakeScreenshotWithMessage: " + message);
		StartCoroutine(DoTakeScreenshot(fileName, message, x, y, doShare, onComplete));
	}

	private IEnumerator DoTakeScreenshot(string fileName, string message, float x, float y, Action<string, string, float, float> doShare, Action onComplete)
	{
		yield return new WaitForEndOfFrame();

		// This is the easiest way to grab a full-res screenshot
		Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		screenshot.Apply();

		// Grab a screenshot at an arbitrary resolution
//		int resWidth = Mathf.FloorToInt((float)Screen.width/Futile.displayScale * 2);
//		int resHeight = Mathf.FloorToInt((float)Screen.height/Futile.displayScale * 2);
//		RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
//		Futile.instance.camera.targetTexture = rt;
//		Texture2D screenshot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
//		Futile.instance.camera.Render();
//		RenderTexture.active = rt;
//		screenshot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
//		Futile.instance.camera.targetTexture = null;
//		RenderTexture.active = null;
//		Destroy(rt);

		// Encode texture into PNG
		byte[] bytes = screenshot.EncodeToPNG();
		UnityEngine.Object.Destroy(screenshot);
		// For testing purposes, also write to a file in the project folder
		string filePath = Application.persistentDataPath + "/" + fileName + ".png";
		File.WriteAllBytes(filePath, bytes);
		Debug.Log("Saved image to: " + filePath);

		if (doShare != null)
		{
			doShare(filePath, message, x, y);
		}
		if (onComplete != null)
		{
			onComplete();
		}
	}

	private void DoImmediateScreenshot(string fileName, int picWidth, int picHeight)
	{
		// This is the easiest way to grab a full-res screenshot
		//		Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		//		screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		//		screenshot.Apply();

		// Grab a screenshot at an arbitrary resolution
		//Camera mainCam = gameObject.GetComponent<Camera>();
		Camera mainCam = Camera.main;
		RenderTexture rt = new RenderTexture(picWidth, picHeight, 24);
		mainCam.targetTexture = rt;
		Texture2D screenshot = new Texture2D(picWidth, picHeight, TextureFormat.RGB24, false);
		mainCam.Render();
		RenderTexture.active = rt;
		screenshot.ReadPixels(new Rect(0, 0, picWidth, picHeight), 0, 0);
		mainCam.targetTexture = null;
		RenderTexture.active = null;
		Destroy(rt);

		// Encode texture into PNG
		byte[] bytes = screenshot.EncodeToPNG();
		UnityEngine.Object.Destroy(screenshot);
		// For testing purposes, also write to a file in the project folder
		string filePath = Application.persistentDataPath + "/" + fileName + "_" + counter + ".png";
		File.WriteAllBytes(filePath, bytes);
		Debug.Log("Saved image to: " + filePath);
		counter++;
	}
}

