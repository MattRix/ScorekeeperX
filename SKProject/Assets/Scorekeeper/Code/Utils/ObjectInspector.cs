using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor (typeof(UnityEngine.Object),true)]
[CanEditMultipleObjects]
public class ObjectInspector : Editor
{
	public MethodInfo inspectMeth;
	public MethodInfo sceneMeth;

	public List<MethodInfo>buttonMethods = new List<MethodInfo>();

	public void OnEnable()
	{
		var type = target.GetType();
		inspectMeth = target.GetType().GetMethod("OnInspectorGUI",BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);

		sceneMeth = target.GetType().GetMethod("OnSceneGUI",BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);

		var meths = type.GetMethods(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);

		foreach(var meth in meths)
		{
			if(meth.GetCustomAttributes(typeof(MakeButton),true).Length > 0)
			{
				buttonMethods.Add(meth);
			}
		}
	}

	override public void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if(inspectMeth != null)
		{
			foreach(var eachTarget in targets)
			{
				inspectMeth.Invoke(eachTarget, new object[0]);
			}
		}

		foreach(var meth in buttonMethods)
		{
			if(GUILayout.Button(meth.Name))
			{
				foreach(var eachTarget in targets)
				{
					meth.Invoke(eachTarget, new object[0]);
				}
			}
		}
	}

	public void OnSceneGUI()
	{
		if(sceneMeth != null)
		{
			sceneMeth.Invoke(target, new object[0]);
		}
	}
}
#endif

[AttributeUsage (AttributeTargets.Method)]
public class MakeButton : Attribute
{
	public MakeButton(){}
}








