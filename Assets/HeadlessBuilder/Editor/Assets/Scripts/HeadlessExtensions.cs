/* 
 * Headless Builder
 * (c) Salty Devs, 2019
 * 
 * Please do not publish or pirate this code.
 * We worked really hard to make it.
 * 
 */

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

// This class contains various functions and utilities that are used throughout Headless Builder
public static class HeadlessExtensions
{
	
	// This function returns all types that are derrived from a given type 
	public static System.Type[] GetAllDerivedTypesHB(this System.AppDomain aAppDomain, System.Type aType)
	{
		var result = new List<System.Type>();
		var assemblies = aAppDomain.GetAssemblies();
		foreach (var assembly in assemblies)
		{
            try
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(aType))
                        result.Add(type);
                }
            } catch (Exception)
            {

            }
		}
		return result.ToArray();
	}

	// This function centers a given window inside the Unity Editor main window
	public static void CenterOnMainWin(this UnityEditor.EditorWindow aWin, float offsetY)
	{
		var main = GetEditorMainWindowPos();
		var pos = aWin.position;
		float w = (main.width - pos.width)*0.5f;
		float h = (main.height - pos.height)*0.5f;
		pos.x = main.x + w;
		pos.y = main.y + h + offsetY;
		aWin.position = pos;
	}

	// This function gets the position and size of the Unity Editor main window
	public static Rect GetEditorMainWindowPos()
	{
		var containerWinType = System.AppDomain.CurrentDomain.GetAllDerivedTypesHB(typeof(ScriptableObject)).Where(t => t.Name == "ContainerWindow").FirstOrDefault();
		if (containerWinType == null)
			throw new System.MissingMemberException("Can't find internal type ContainerWindow. Maybe something has changed inside Unity");
		var showModeField = containerWinType.GetField("m_ShowMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var positionProperty = containerWinType.GetProperty("position", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
		if (showModeField == null || positionProperty == null)
			throw new System.MissingFieldException("Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");
		var windows = Resources.FindObjectsOfTypeAll(containerWinType);
		foreach (var win in windows)
		{
			var showmode = (int)showModeField.GetValue(win);
			if (showmode == 4) // main window
			{
				var pos = (Rect)positionProperty.GetValue(win, null);
				return pos;
			}
		}
		throw new System.NotSupportedException("Can't find internal main window. Maybe something has changed inside Unity");
	}

	// This function gets the absolute path to the Headless Builder folder
	public static string GetHeadlessBuilderPath(bool absolute) {
		string[] res = Directory.GetFiles(Application.dataPath, "HeadlessRuntime.cs", SearchOption.AllDirectories);
		if (res.Length == 0)
		{
			return null;
		}
		if (!absolute) {
			res[0] = res[0].Replace ('\\', '/').Replace (Directory.GetParent(Application.dataPath).ToString().Replace ('\\', '/') + "/", "");
		}
		return res[0].Replace ('\\', '/').Replace("/HeadlessRuntime.cs", "");
	}

	// This function converts a normalized path to an OS-specific path (Unity Editor only)
	public static string LocalizePath(string path) {
		#if UNITY_EDITOR_WIN
		return path.Replace ('/', '\\');
		#else
		return path.Replace ('\\', '/');
		#endif
	}

}