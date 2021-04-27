/* 
 * Headless Builder
 * (c) Salty Devs, 2019
 * 
 * Please do not publish or pirate this code.
 * We worked really hard to make it.
 * 
 */

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

// This class contains code for displaying a progress bar window during builds.
public class HeadlessProgress : EditorWindow
{

    private static HeadlessProgress progressWindow = null;
	
	private static string currentStep = null;
	private static Dictionary<string, string> stepList = null;

	public static HeadlessProgress Init()
	{
        if (progressWindow == null)
        {
            progressWindow = EditorWindow.GetWindow<HeadlessProgress>(true);
        }

		stepList = new Dictionary<string, string>();

		stepList.Add ("INIT", "Initializing...");
		stepList.Add ("PREPROCESS", "Preparing build...");
		stepList.Add ("BUILD", "Building...");
		stepList.Add ("BACKUP", "Processing assets...");
		stepList.Add ("POSTPROCESS", "Finalizing build...");
		stepList.Add ("REVERT", "Applying settings...");

		Texture iconAsset = AssetDatabase.LoadAssetAtPath<Texture> (HeadlessExtensions.GetHeadlessBuilderPath(false) + "/Editor/Assets/Texture/icon.png");
        progressWindow.titleContent = new GUIContent ("Headless Builder", iconAsset);

        progressWindow.minSize = new Vector2 (300, 70);
        progressWindow.maxSize = new Vector2 (300, 70);

        progressWindow.ShowUtility();
		HeadlessExtensions.CenterOnMainWin (progressWindow, 0);

        return progressWindow;
    }

	public void SetProgress(string newStep)
	{
		if (stepList == null) {
			Init ();
		}

		if (newStep == null) {
			currentStep = null;
		} else if (stepList.ContainsKey (newStep)) {
			currentStep = newStep;
		}
	}

    public static void Hide()
    {
        if (progressWindow != null)
        {
            progressWindow.Close();
        }
    }

	void OnGUI()
	{

		string stepText = null;
		float stepProgress = 0f;
		if (currentStep == null) {
			if (!HeadlessBuilder.buildError) {
				stepText = "Build success!";
				GUI.color = Color.green;
			} else {
				stepText = "Build failed!";
                GUI.color = Color.red;
			}
			stepProgress = 1f;

		} else {
			
			GUI.color = Color.white;
			stepText = stepList [currentStep];

			float stepID = 0;
			foreach (string loopStep in stepList.Keys) {
				if (loopStep.Equals (currentStep)) {
					break;
				}
				stepID += 1;
			}
			stepProgress = stepID / stepList.Count;

		}


		GUIStyle abortStyle = new GUIStyle ();
		abortStyle.alignment = TextAnchor.MiddleCenter;
		abortStyle.fontSize = 10;
		abortStyle.fontStyle = FontStyle.Bold;

		GUIStyle noteStyle = new GUIStyle ();
		noteStyle.alignment = TextAnchor.MiddleCenter;
		noteStyle.fontSize = 10;

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);
		EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);
		Rect r = EditorGUILayout.BeginVertical();
		EditorGUI.ProgressBar(r, stepProgress, stepText);
		GUILayout.Space(18);
		EditorGUILayout.EndVertical();
		EditorGUILayout.Space ();
		if (stepProgress < 1f) {
			GUILayout.Label ("Please do not abort", abortStyle);
		} else {
			GUILayout.Label ("You can now close this window", noteStyle);
		}
		EditorGUILayout.EndVertical ();
		EditorGUILayout.EndVertical ();

	}

}