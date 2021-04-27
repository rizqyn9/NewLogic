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
using System.IO;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;

// This class contains everything related to the Headless Builder window and loading/saving settings.
public class HeadlessEditor : EditorWindow
{

	// The below constants are used throughout Headless Builder to refer to platforms and architectures.
	public static readonly int WINDOWS = 0;
	public static readonly int OSX = 1;
	public static readonly int LINUX = 2;

	public static readonly int X64 = 0;
	public static readonly int X86 = 1;
	public static readonly int UNIVERSAL = 2;

	// GUI-related variables
	private static int VIEW_SETTINGS = 0;
	private static int VIEW_DOCUMENTATION = 1;
	private static int VIEW_ABOUT = 2;

	private int currentView = VIEW_SETTINGS;
	private Vector2 settingsPosition = new Vector2 (0, 0);
	private Vector2 documentationPosition = new Vector2 (0, 0);

	private GUIStyle titleStyle = new GUIStyle ();
	private GUIStyle subtitleStyle = new GUIStyle ();
	private GUIStyle logoStyle = new GUIStyle ();
	private GUIStyle sectionStyle = new GUIStyle ();
	private GUIStyle aboutSectionStyle = new GUIStyle ();
	private GUIStyle centerStyle = new GUIStyle ();
	private GUIStyle infoStyle = new GUIStyle ();
	private GUIStyle contentStyle = new GUIStyle ();
	private GUIStyle linkCenterStyle = new GUIStyle ();
	private GUIStyle linkLeftStyle = new GUIStyle ();
	private GUIStyle saveStyle = new GUIStyle ();
	private GUIStyle contentPaddingStyle = new GUIStyle ();
	private GUIStyle headerPaddingStyle = new GUIStyle ();
	private GUIStyle documentationPaddingStyle = new GUIStyle ();
	private GUIStyle pagePaddingStyle = new GUIStyle ();
	private GUIStyle foldoutStyle = new GUIStyle ();
	private GUIStyle commentStyle = new GUIStyle ();
	private GUIStyle codeStyle = new GUIStyle ();
	private GUIStyle codeLabelStyle = new GUIStyle ();
	private GUIStyle inputStyle = new GUIStyle ();
	private GUIStyle selectionStyle = new GUIStyle ();
	private GUIStyle buttonStyle = new GUIStyle ();

	private Texture2D logoAsset = null;
	private Texture iconAsset = null;
	private Texture addIconAsset = null;
	private Texture removeIconAsset = null;
	private Texture selectIconAsset = null;
	private Texture donateAsset = null;
	private static Dictionary<string,string> documentationContent = new Dictionary<string, string> ();

	public Font consolaAsset = null;
	public Font consolaiAsset = null;
	public Font consolabAsset = null;
	public Font consolazAsset = null;
	public Font leelawadeeAsset = null;

	// Settings-related variables
	private static HeadlessSettings headlessSettings = null;
	public static bool dirtySettings = false;
	public static bool dirtyProfiles = false;
	public static bool savingSettings = false;
	private static float stampSettings = 0f;

	public HeadlessEditor ()
	{
		HeadlessRoutine.start (AsyncPreloadDocumentationContent ());
	}

	public void OnEnable ()
	{
		HeadlessProfiles.FindProfiles ();

		bool found = false;
		string lastProfile = EditorPrefs.GetString ("HEADLESSBUILDER_LASTPROFILE", HeadlessProfiles.defaultProfile);

		foreach (KeyValuePair<string,string> profile in HeadlessProfiles.GetProfileList()) {
			if (profile.Key.Equals (lastProfile)) {
				found = true;
			}
		}

		if (found) {
			SelectProfile (lastProfile);
		} else {
			SelectProfile (HeadlessProfiles.defaultProfile);
		}

	}

	public void OnSettings ()
	{
		currentView = VIEW_SETTINGS;
		SetStyle (this);
	}

	public void OnDocumentation ()
	{
		currentView = VIEW_DOCUMENTATION;
		SetStyle (this);
	}

	public void OnAbout ()
	{
		currentView = VIEW_ABOUT;
		SetStyle (this);
	}

	private void SetStyle (HeadlessEditor editor)
	{
		if (iconAsset == null) {
			iconAsset = AssetDatabase.LoadAssetAtPath<Texture> (HeadlessExtensions.GetHeadlessBuilderPath (false) + "/Editor/Assets/Texture/icon.png");
			GUIContent newTitleContent = new GUIContent ("Headless", iconAsset);
			editor.titleContent = newTitleContent;
		}

		if (logoAsset == null) {
			logoAsset = AssetDatabase.LoadAssetAtPath<Texture2D> (HeadlessExtensions.GetHeadlessBuilderPath (false) + "/Editor/Assets/Texture/logo.png");
		}

		if (addIconAsset == null) {
			addIconAsset = AssetDatabase.LoadAssetAtPath<Texture> (HeadlessExtensions.GetHeadlessBuilderPath (false) + "/Editor/Assets/Texture/add.png");
		}

		if (removeIconAsset == null) {
			removeIconAsset = AssetDatabase.LoadAssetAtPath<Texture> (HeadlessExtensions.GetHeadlessBuilderPath (false) + "/Editor/Assets/Texture/remove.png");
		}

		if (selectIconAsset == null) {
			selectIconAsset = AssetDatabase.LoadAssetAtPath<Texture> (HeadlessExtensions.GetHeadlessBuilderPath (false) + "/Editor/Assets/Texture/select.png");
		}

		if (donateAsset == null) {
			donateAsset = AssetDatabase.LoadAssetAtPath<Texture> (HeadlessExtensions.GetHeadlessBuilderPath (false) + "/Editor/Assets/Texture/donate.gif");
		}

		if (consolaAsset == null) {
			consolaAsset = AssetDatabase.LoadAssetAtPath<Font> (HeadlessExtensions.GetHeadlessBuilderPath (false) + "/Editor/Assets/Fonts/consola.ttf");
		}

		if (consolaiAsset == null) {
			consolaiAsset = AssetDatabase.LoadAssetAtPath<Font> (HeadlessExtensions.GetHeadlessBuilderPath (false) + "/Editor/Assets/Fonts/consolai.ttf");
		}

		if (consolabAsset == null) {
			consolabAsset = AssetDatabase.LoadAssetAtPath<Font> (HeadlessExtensions.GetHeadlessBuilderPath (false) + "/Editor/Assets/Fonts/consolai.ttf");
		}

		if (consolazAsset == null) {
			consolazAsset = AssetDatabase.LoadAssetAtPath<Font> (HeadlessExtensions.GetHeadlessBuilderPath (false) + "/Editor/Assets/Fonts/consolaz.ttf");
		}

		if (leelawadeeAsset == null) {
			leelawadeeAsset = AssetDatabase.LoadAssetAtPath<Font> (HeadlessExtensions.GetHeadlessBuilderPath (false) + "/Editor/Assets/Fonts/leelawadee.ttf");
		}

		editor.minSize = new Vector2 (570, 660);
	}

	public static Color generateColor(float alpha) {
		Color defaultColor = GUI.skin.label.normal.textColor;
		return new Color (defaultColor.r, defaultColor.g, defaultColor.b, alpha);
	}

	void OnGUI ()
	{
		bool dropFrame = false;

		titleStyle.font = consolaAsset;
		titleStyle.fontSize = EditorStyles.standardFont.fontSize + 9;
		titleStyle.fontStyle = FontStyle.Bold;
		titleStyle.alignment = TextAnchor.MiddleCenter;
		titleStyle.normal.textColor = GUI.skin.button.normal.textColor;

		subtitleStyle.font = consolaAsset;
		subtitleStyle.fontSize = EditorStyles.standardFont.fontSize + 4;
		subtitleStyle.fontStyle = FontStyle.Bold;
		subtitleStyle.alignment = TextAnchor.MiddleCenter;
		subtitleStyle.normal.textColor = GUI.skin.button.normal.textColor;

		logoStyle.alignment = TextAnchor.MiddleCenter;

		aboutSectionStyle.font = consolaAsset;
		aboutSectionStyle.alignment = TextAnchor.MiddleCenter;
		aboutSectionStyle.fontStyle = FontStyle.Bold;
		aboutSectionStyle.normal.textColor = GUI.skin.button.normal.textColor;
        aboutSectionStyle.fontSize = EditorStyles.label.fontSize;

        sectionStyle.fontStyle = FontStyle.Bold;

        centerStyle = new GUIStyle(EditorStyles.label);
        centerStyle.font = leelawadeeAsset;
		centerStyle.alignment = TextAnchor.MiddleCenter;
		centerStyle.normal.textColor = GUI.skin.button.normal.textColor;
        centerStyle.fontSize = EditorStyles.label.fontSize;
        
        infoStyle.fontStyle = FontStyle.Italic;
        
        contentStyle.font = leelawadeeAsset;
		contentStyle.wordWrap = true;
		contentStyle.richText = true;
		contentStyle.normal.textColor = GUI.skin.button.normal.textColor;
        contentStyle.fontSize = EditorStyles.foldout.fontSize;

        linkCenterStyle = new GUIStyle (GUI.skin.label);
		linkCenterStyle.font = leelawadeeAsset;
		linkCenterStyle.normal.textColor = Color.blue;
		linkCenterStyle.alignment = TextAnchor.MiddleCenter;
		if (EditorGUIUtility.isProSkin) {
			linkCenterStyle.normal.textColor = Color.white;
		}

		linkLeftStyle = new GUIStyle (GUI.skin.label);
		linkLeftStyle.font = leelawadeeAsset;
		linkLeftStyle.normal.textColor = Color.blue;
		linkLeftStyle.alignment = TextAnchor.MiddleLeft;
		if (EditorGUIUtility.isProSkin) {
			linkLeftStyle.normal.textColor = Color.white;
		}
        linkLeftStyle.fontSize = EditorStyles.foldout.fontSize;

        saveStyle = new GUIStyle (GUI.skin.label);
		saveStyle.alignment = TextAnchor.UpperRight;
		saveStyle.font = consolaiAsset;
		saveStyle.normal.textColor = generateColor(0.46f);

		contentPaddingStyle = new GUIStyle (EditorStyles.helpBox);
		contentPaddingStyle.margin = new RectOffset (6, 10, 5, 10);

		headerPaddingStyle = new GUIStyle (EditorStyles.helpBox);
		headerPaddingStyle.margin = new RectOffset (11, 10, 5, 10);

		documentationPaddingStyle = new GUIStyle (EditorStyles.helpBox);
		documentationPaddingStyle.margin = new RectOffset (6, 0, 5, 10);

		pagePaddingStyle = new GUIStyle (EditorStyles.inspectorFullWidthMargins);
		pagePaddingStyle.margin = new RectOffset (5, 5, 0, 0);

		foldoutStyle = new GUIStyle (EditorStyles.foldout);
		foldoutStyle.font = consolaAsset;

		commentStyle = new GUIStyle (EditorStyles.label);
		commentStyle.font = consolaiAsset;
		commentStyle.normal.textColor = generateColor(0.46f);

		codeStyle = new GUIStyle (EditorStyles.label);
		codeStyle.font = consolaAsset;

		codeLabelStyle = new GUIStyle (codeStyle);
        codeLabelStyle.padding.top += (int)EditorGUIUtility.singleLineHeight - 16;
        codeLabelStyle.padding.left = 4;

        inputStyle = new GUIStyle (EditorStyles.numberField);
		inputStyle.padding.top += 1;
		inputStyle.padding.left += 1;
		inputStyle.font = consolaAsset;

		selectionStyle = new GUIStyle (EditorStyles.radioButton);
		selectionStyle.font = consolaAsset;
		selectionStyle.padding.top += (int)EditorGUIUtility.singleLineHeight - 14;
        selectionStyle.padding.left += 2;
        selectionStyle.fontSize = codeStyle.fontSize;

		buttonStyle = new GUIStyle(GUI.skin.button);
		buttonStyle.font = consolaAsset;
		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.padding.top += 4;
		buttonStyle.padding.bottom += 3;
		if (EditorGUIUtility.isProSkin) {
			buttonStyle.normal.textColor = Color.white;
		}


		if (headlessSettings == null) {
			headlessSettings = LoadSettings (HeadlessProfiles.currentProfile, false);
		}

		if (!Headless.IsBuildingHeadless() && dirtySettings && !savingSettings && stampSettings + 1f < Time.time) {
			savingSettings = true;
			HeadlessRoutine.start (SaveSettingsAsync (headlessSettings));
			dirtySettings = false;
		}
		/*if (dirtySettings) {
			SaveSettings (headlessSettings, HeadlessProfiles.currentProfile);
			dirtySettings = false;
		}*/

		HeadlessSettings newSettings = ScriptableObject.CreateInstance<HeadlessSettings> ();
		JsonUtility.FromJsonOverwrite (JsonUtility.ToJson (headlessSettings), newSettings);
		newSettings = verifySettings (newSettings);

		GUI.SetNextControlName ("focusOut");
		GUI.Button (new Rect (-100, -100, 1, 1), "");

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Toggle (currentView == VIEW_SETTINGS, "Settings", buttonStyle)) {
			OnSettings ();
		}
		if (GUILayout.Toggle (currentView == VIEW_DOCUMENTATION, "Documentation", buttonStyle)) {
			OnDocumentation ();
		}
		if (GUILayout.Toggle (currentView == VIEW_ABOUT, "About", buttonStyle)) {
			OnAbout ();
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		if (currentView == VIEW_SETTINGS) {
			
			EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);

			GUILayout.Space (15);

			EditorGUILayout.BeginVertical (headerPaddingStyle);
			EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);
			GUILayout.Space (2);

			EditorGUILayout.BeginHorizontal ();
			//EditorGUI.BeginDisabledGroup (true);
			EditorGUILayout.BeginVertical ();
			GUILayout.Space (7);

			EditorGUILayout.BeginHorizontal ();
			GUI.SetNextControlName ("profileName");

			EditorGUILayout.LabelField ("Current Profile: ", codeStyle, GUILayout.MaxWidth (150.0f));
			string newProfileName = CoerceValidFileName (EditorGUILayout.TextField (CoerceValidFileName (headlessSettings.profileName), inputStyle));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();

			if (!newProfileName.Equals (headlessSettings.profileName)) {
				bool found = false;
				foreach (KeyValuePair<string,string> profile in HeadlessProfiles.GetProfileList()) {
					if (profile.Value.Equals (newProfileName)) {
						found = true;
					}
				}

				if (found) {
					/*EditorUtility.DisplayDialog ("Headless Builder", 
							"There is already a profile with the name \"" + newProfileName + "\".", "OK");

						GUI.FocusControl ("focusOut");*/
				} else {
					newSettings.profileName = newProfileName;
					dirtyProfiles = true;
				}
			}


			//EditorGUI.EndDisabledGroup();
			GUILayout.Space (6);

			if (dirtySettings || savingSettings || HeadlessProfiles.GetProfileList ().Count < 1) {
				EditorGUI.BeginDisabledGroup (true);
			}

			if (GUILayout.Button (addIconAsset, GUILayout.Width (24), GUILayout.Height (24))) {

				if (EditorUtility.DisplayDialog ("Headless Builder",
					     "Would you like to create a new profile?", "Yes", "Cancel")) {

					GUI.FocusControl ("focusOut");

					HeadlessProfiles.CreateProfile ();
					string copyProfileName = headlessSettings.profileName + "_copy";
					int i = 0;
					while (HeadlessProfiles.GetProfileList ().Values.Contains (copyProfileName)) {
						i++;
						copyProfileName = headlessSettings.profileName + "_copy" + i;
					}

					newSettings.profileName = copyProfileName;
					newSettings.buildPath = null;
					newSettings.buildID = 0;

					SaveSettings (newSettings, HeadlessProfiles.currentProfile);
					headlessSettings = newSettings;

					HeadlessProfiles.FindProfiles ();

					dropFrame = true;

				}

			}

			if (HeadlessProfiles.currentProfile.Equals (HeadlessProfiles.defaultProfile)) {
				EditorGUI.BeginDisabledGroup (true);
			}
			if (GUILayout.Button (removeIconAsset, GUILayout.Width (24), GUILayout.Height (24))) {

				GUI.FocusControl ("focusOut");

				if (EditorUtility.DisplayDialog ("Headless Builder",
					     "Would you like to remove the profile \"" + newSettings.profileName + "\" ?\n\n" +
					     "This action cannot be undone!", "Yes", "Cancel")) {

					RemoveProfile ();

					dropFrame = true;
				}

			}
			if (HeadlessProfiles.currentProfile.Equals (HeadlessProfiles.defaultProfile)) {
				EditorGUI.EndDisabledGroup ();
			}

			Rect dropdownRectangle = GUILayoutUtility.GetLastRect ();
			if (GUILayout.Button (selectIconAsset, GUILayout.Width (24), GUILayout.Height (24))) {

				GUI.FocusControl ("focusOut");

				HeadlessProfiles.FindProfiles ();

				GenericMenu toolsMenu = new GenericMenu ();

				foreach (KeyValuePair<string,string> profile in HeadlessProfiles.GetProfileList()) {
					string profileName = profile.Value;
					bool selected = false;
					if (profile.Key.Equals (HeadlessProfiles.defaultProfile)) {
						profileName += " (default)";
					}
					if (profile.Key.Equals (HeadlessProfiles.currentProfile)) {
						selected = true;
					}

					toolsMenu.AddItem (new GUIContent (profileName), selected, SelectProfile, profile.Key);
				}

				toolsMenu.DropDown (dropdownRectangle);

				dropFrame = true;

			}


			if (dirtySettings || savingSettings || HeadlessProfiles.GetProfileList ().Count < 1) {
				EditorGUI.EndDisabledGroup ();
			}

			EditorGUILayout.EndHorizontal ();


			EditorGUILayout.Space ();
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndVertical ();


			string saveText = "";
			if (savingSettings) {
				saveText = "//Auto-saving...";
			}
			EditorGUILayout.LabelField (saveText, saveStyle);

			EditorGUILayout.Space ();

			settingsPosition = GUILayout.BeginScrollView (settingsPosition, false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
			GUILayout.BeginHorizontal ();
			GUILayout.BeginVertical ();


			newSettings.scenesSection = EditorGUILayout.Foldout (headlessSettings.scenesSection, "Scenes In Build", foldoutStyle);
			if (headlessSettings.scenesSection) {
				EditorGUILayout.BeginVertical (contentPaddingStyle);
				EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);
				GUILayout.Space (6);

				EditorGUILayout.BeginHorizontal ();
				newSettings.valueOverrideScenes = EditorGUILayout.ToggleLeft ("", headlessSettings.valueOverrideScenes, GUILayout.MaxWidth(13f));
				EditorGUILayout.LabelField ("Override Unity's Build Settings window", codeLabelStyle);
				EditorGUILayout.EndHorizontal ();

				if (headlessSettings.valueOverrideScenes) {
					List<SceneAsset> listSceneAssets = headlessSettings.valueSceneAssets.ToList ();
					if (listSceneAssets.Count == 0 || listSceneAssets [listSceneAssets.Count - 1] != null) {
						listSceneAssets.Add (null);
					}

					for (int i = 0; i < listSceneAssets.Count; ++i) {
						if (listSceneAssets [i] != null || i == listSceneAssets.Count - 1) {
							GUILayout.Space (3);
							listSceneAssets [i] = (SceneAsset)EditorGUILayout.ObjectField (listSceneAssets [i], typeof(SceneAsset), false);
						}
					}

					List<int> deleteScenes = new List<int> ();
					for (int i = 0; i < listSceneAssets.Count; i++) {
						string scenePath = AssetDatabase.GetAssetPath (listSceneAssets [i]);
						if (string.IsNullOrEmpty (scenePath)) {
							deleteScenes.Add (i);
						}
					}
					for (int i = deleteScenes.Count - 1; i >= 0; i--) {
						listSceneAssets.RemoveAt (deleteScenes [i]);
					}

					newSettings.valueSceneAssets = listSceneAssets.Distinct ().ToArray ();

					EditorGUILayout.Space ();
					GUILayout.Label ("//To add a scene, click the circle button on the right.", commentStyle);
					GUILayout.Space (3);
					GUILayout.Label ("//To remove a scene, select it and press backspace.", commentStyle);
				} else {
					GUILayout.Space (3);
					GUILayout.Label ("//Check this to manually select scenes for this build.", commentStyle);
				}


				EditorGUILayout.Space ();
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();

			newSettings.targetSection = EditorGUILayout.Foldout (headlessSettings.targetSection, "Build Target", foldoutStyle);
			if (headlessSettings.targetSection) {
				EditorGUILayout.BeginVertical (contentPaddingStyle);
				EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);
				GUILayout.Space (6);

				EditorGUILayout.BeginHorizontal ();
				newSettings.valueOverrideTarget = EditorGUILayout.ToggleLeft ("", headlessSettings.valueOverrideTarget, GUILayout.MaxWidth(13f));
				EditorGUILayout.LabelField ("Override Unity's Build Settings window", codeLabelStyle);
				EditorGUILayout.EndHorizontal ();

				if (headlessSettings.valueOverrideTarget) {
					GUILayout.Space (4);
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.BeginVertical ();

					newSettings.valuePlatform = GUILayout.SelectionGrid (headlessSettings.valuePlatform, new string[] {
						"Windows",
						"Mac OS X",
						"Linux"
					}, 1, selectionStyle, GUILayout.Height(60));
					EditorGUILayout.EndVertical ();
					EditorGUILayout.BeginVertical ();

					if (headlessSettings.valuePlatform == WINDOWS) {
						newSettings.valueArchitectureWindows = GUILayout.SelectionGrid (headlessSettings.valueArchitectureWindows, new string[] {
							"64-bit (x86_64)",
							"32-bit (x86)"
						}, 1, selectionStyle, GUILayout.Height(40));
					} else if (headlessSettings.valuePlatform == OSX) {
						newSettings.valueArchitectureOSX = GUILayout.SelectionGrid (headlessSettings.valueArchitectureOSX, new string[]{ "64-bit (x86_64)" }, 1, selectionStyle, GUILayout.Height(40));
					} else if (headlessSettings.valuePlatform == LINUX) {
						newSettings.valueArchitectureLinux = GUILayout.SelectionGrid (headlessSettings.valueArchitectureLinux, new string[] {
							"64-bit (x86_64)",
							"32-bit (x86)"
						}, 1, selectionStyle, GUILayout.Height(40));
					}
					
					EditorGUILayout.EndVertical ();
					EditorGUILayout.EndHorizontal ();
				} else {
					GUILayout.Space (3);
					GUILayout.Label ("//Check this to manually select the target platform for this build.", commentStyle);
				}

				EditorGUILayout.Space ();
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();

			newSettings.applicationSection = EditorGUILayout.Foldout (headlessSettings.applicationSection, "Application", foldoutStyle);
			if (headlessSettings.applicationSection) {
				EditorGUILayout.BeginVertical (contentPaddingStyle);
				EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);
				GUILayout.Space (6);

				if (headlessSettings.valueLimitFramerate) {
					GUILayout.Space (2);
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("Target Framerate", codeStyle, GUILayout.MaxWidth (150.0f));
					newSettings.valueFramerate = EditorGUILayout.IntField (headlessSettings.valueFramerate, inputStyle);
					EditorGUILayout.LabelField ("FPS", codeStyle, GUILayout.MaxWidth (30.0f));
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.Space ();
				}

				EditorGUILayout.BeginHorizontal ();
				newSettings.valueLimitFramerate = EditorGUILayout.ToggleLeft ("", headlessSettings.valueLimitFramerate, GUILayout.MaxWidth(13f));
				EditorGUILayout.LabelField ("Set application target framerate", codeLabelStyle);
				EditorGUILayout.EndHorizontal ();

				GUILayout.Space (3);
				GUILayout.Label ("//Uncheck this only if you already set the target framerate elsewhere.", commentStyle);
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				EditorGUILayout.BeginHorizontal ();
				newSettings.valueCamera = EditorGUILayout.ToggleLeft ("", headlessSettings.valueCamera, GUILayout.MaxWidth(13f));
				EditorGUILayout.LabelField ("Dynamically keep all cameras disabled", codeLabelStyle);
				EditorGUILayout.EndHorizontal ();

				GUILayout.Space (3);
				GUILayout.Label ("//Enhances performance by disabling all runtime graphics rendering.", commentStyle);
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				EditorGUILayout.BeginHorizontal ();
				newSettings.valueConsole = EditorGUILayout.ToggleLeft ("", headlessSettings.valueConsole, GUILayout.MaxWidth(13f));
				EditorGUILayout.LabelField ("Output log messages to console instead of file", codeLabelStyle);
				EditorGUILayout.EndHorizontal ();

				GUILayout.Space (3);
				GUILayout.Label ("//This will override the default Unity logging.", commentStyle);
				EditorGUILayout.Space ();
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndVertical ();

				EditorGUIUtility.fieldWidth = 0;
			}

			EditorGUILayout.Space ();

			newSettings.assetsSection = EditorGUILayout.Foldout (headlessSettings.assetsSection, "Assets", foldoutStyle);
			if (headlessSettings.assetsSection) {
				EditorGUILayout.BeginVertical (contentPaddingStyle);
				EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);
				GUILayout.Space (6);

				EditorGUILayout.BeginHorizontal ();
				newSettings.valueDummy = EditorGUILayout.ToggleLeft ("", headlessSettings.valueDummy, GUILayout.MaxWidth(13f));
				EditorGUILayout.LabelField ("Replace visual and audio assets with dummies", codeLabelStyle);
				EditorGUILayout.EndHorizontal ();

				GUILayout.Space (3);
				GUILayout.Label ("//Increases build time, but greatly enhances performance.", commentStyle);
				EditorGUILayout.Space ();
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();

			newSettings.preProcessingSection = EditorGUILayout.Foldout (headlessSettings.preProcessingSection, "Pre-processing", foldoutStyle);
			if (headlessSettings.preProcessingSection) {
				EditorGUILayout.BeginVertical (contentPaddingStyle);
				EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);
				GUILayout.Space (6);


				EditorGUILayout.BeginHorizontal ();
				newSettings.valueGI = EditorGUILayout.ToggleLeft ("", headlessSettings.valueGI, GUILayout.MaxWidth(13f));
				EditorGUILayout.LabelField ("Disable Global Illumination", codeLabelStyle);
				EditorGUILayout.EndHorizontal ();

				GUILayout.Space (3);
				GUILayout.Label ("//Prevents errors, decreases build time and enhances performance.", commentStyle);
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();


				EditorGUILayout.BeginHorizontal ();
				newSettings.valueAudio = EditorGUILayout.ToggleLeft ("", headlessSettings.valueAudio, GUILayout.MaxWidth(13f));
				EditorGUILayout.LabelField ("Disable Audio I/O", codeLabelStyle);
				EditorGUILayout.EndHorizontal ();

				GUILayout.Space (3);
				GUILayout.Label ("//Prevents the headless build from playing audio.", commentStyle);
				EditorGUILayout.Space ();
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();

			newSettings.optionsSection = EditorGUILayout.Foldout (headlessSettings.optionsSection, "Build Options", foldoutStyle);
			if (headlessSettings.assetsSection) {
				EditorGUILayout.BeginVertical (contentPaddingStyle);
				EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);
				GUILayout.Space (6);

				EditorGUILayout.BeginHorizontal ();
				newSettings.valueRememberPath = EditorGUILayout.ToggleLeft ("", headlessSettings.valueRememberPath, GUILayout.MaxWidth(13f));
				EditorGUILayout.LabelField ("Remember output folder for this profile", codeLabelStyle);
				EditorGUILayout.EndHorizontal ();

				GUILayout.Space (3);
				GUILayout.Label ("//Check this to always use the same output folder.", commentStyle);
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				EditorGUILayout.BeginHorizontal ();
				newSettings.valueSkipConfirmation = EditorGUILayout.ToggleLeft ("", headlessSettings.valueSkipConfirmation, GUILayout.MaxWidth(13f));
				EditorGUILayout.LabelField ("Skip confirmation messages for this profile", codeLabelStyle);
				EditorGUILayout.EndHorizontal ();

				GUILayout.Space (3);
				GUILayout.Label ("//Check this to hide informative confirmation messages when building.", commentStyle);
				EditorGUILayout.Space ();
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();

			newSettings.postProcessingSection = EditorGUILayout.Foldout (headlessSettings.postProcessingSection, "Advanced", foldoutStyle);
			if (headlessSettings.postProcessingSection) {
				EditorGUILayout.BeginVertical (contentPaddingStyle);
				EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);
				GUILayout.Space (6);

				EditorGUILayout.BeginHorizontal ();
				newSettings.valueBackup = EditorGUILayout.ToggleLeft ("", headlessSettings.valueBackup, GUILayout.MaxWidth(13f));
				EditorGUILayout.LabelField ("Remove backup after building (not recommended)", codeLabelStyle);
				EditorGUILayout.EndHorizontal ();

				GUILayout.Space (3);
				GUILayout.Label ("//This frees up disk space on your computer,", commentStyle);
				GUILayout.Label ("//but you won't be able to manually revert changes to your project.", commentStyle);
				EditorGUILayout.Space ();
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.EndVertical ();
		
			GUILayout.EndScrollView ();

			GUILayout.EndVertical ();
			//GUILayout.Label ("", GUILayout.MaxWidth (20f));
			GUILayout.EndHorizontal ();
		}


		if (currentView == VIEW_DOCUMENTATION) {

			documentationPosition = GUILayout.BeginScrollView (documentationPosition, false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
			EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);
			EditorGUILayout.BeginVertical (pagePaddingStyle);

			GUILayout.Space (15);

			newSettings.startingDocumentation = EditorGUILayout.Foldout (headlessSettings.startingDocumentation, "Getting Started", foldoutStyle);
			if (headlessSettings.startingDocumentation) {
				EditorGUILayout.BeginVertical (documentationPaddingStyle);
				EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);

				GUILayout.Space (4);
				GUILayout.Label (GetDocumentationContent ("gettingStarted"), contentStyle);

				EditorGUILayout.Space ();
				if (GUILayout.Button ("Open this documentation in browser", linkLeftStyle)) {
					Application.OpenURL (HeadlessExtensions.LocalizePath (HeadlessExtensions.GetHeadlessBuilderPath (true) + "/Editor/Documentation/Index.html"));
				}
				EditorGUIUtility.AddCursorRect (GUILayoutUtility.GetLastRect (), MouseCursor.Link);

				GUILayout.Space (2);
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();


			newSettings.profilesDocumentation = EditorGUILayout.Foldout (headlessSettings.profilesDocumentation, "Profiles", foldoutStyle);
			if (headlessSettings.applicationDocumentation) {
				EditorGUILayout.BeginVertical (documentationPaddingStyle);
				EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);

				GUILayout.Space (4);
				GUILayout.Label (GetDocumentationContent ("profiles"), contentStyle);

				EditorGUILayout.Space ();
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();


			newSettings.applicationDocumentation = EditorGUILayout.Foldout (headlessSettings.applicationDocumentation, "Application", foldoutStyle);
			if (headlessSettings.applicationDocumentation) {
				EditorGUILayout.BeginVertical (documentationPaddingStyle);
				EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);

				GUILayout.Space (4);
				GUILayout.Label (GetDocumentationContent ("application"), contentStyle);

				EditorGUILayout.Space ();
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();


			newSettings.assetsDocumentation = EditorGUILayout.Foldout (headlessSettings.assetsDocumentation, "Assets", foldoutStyle);
			if (headlessSettings.assetsDocumentation) {
				EditorGUILayout.BeginVertical (documentationPaddingStyle);
				EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);

				GUILayout.Space (4);
				GUILayout.Label (GetDocumentationContent ("assets"), contentStyle);

				EditorGUILayout.Space ();
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();


			newSettings.codeDocumentation = EditorGUILayout.Foldout (headlessSettings.cloudDocumentation, "Scripting", foldoutStyle);
			if (headlessSettings.codeDocumentation) {
				EditorGUILayout.BeginVertical (documentationPaddingStyle);
				EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);

				GUILayout.Space (4);
				GUILayout.Label (GetDocumentationContent ("scripting"), contentStyle);

				EditorGUILayout.Space ();
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();

			newSettings.cloudDocumentation = EditorGUILayout.Foldout (headlessSettings.cloudDocumentation, "Unity Cloud Build", foldoutStyle);
			if (headlessSettings.cloudDocumentation) {
				EditorGUILayout.BeginVertical (documentationPaddingStyle);
				EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);

				GUILayout.Space (4);
				GUILayout.Label (GetDocumentationContent ("cloud"), contentStyle);

				EditorGUILayout.Space ();
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();

			newSettings.batchDocumentation = EditorGUILayout.Foldout (headlessSettings.batchDocumentation, "Batch Build", foldoutStyle);
			if (headlessSettings.batchDocumentation) {
				EditorGUILayout.BeginVertical (documentationPaddingStyle);
				EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);

				GUILayout.Space (4);
				GUILayout.Label (GetDocumentationContent ("batch"), contentStyle);

				EditorGUILayout.Space ();
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();

			newSettings.batchDocumentation = EditorGUILayout.Foldout (headlessSettings.batchDocumentation, "Permissions", foldoutStyle);
			if (headlessSettings.batchDocumentation) {
				EditorGUILayout.BeginVertical (documentationPaddingStyle);
				EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);

				GUILayout.Space (4);
				GUILayout.Label (GetDocumentationContent ("permissions"), contentStyle);

				EditorGUILayout.Space ();
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();

			newSettings.troubleshootDocumentation = EditorGUILayout.Foldout (headlessSettings.troubleshootDocumentation, "Troubleshooting", foldoutStyle);
			if (headlessSettings.troubleshootDocumentation) {
				EditorGUILayout.BeginVertical (documentationPaddingStyle);
				EditorGUILayout.BeginVertical (EditorStyles.inspectorFullWidthMargins);

				GUILayout.Space (4);
				GUILayout.Label (GetDocumentationContent ("troubleshooting"), contentStyle);

				EditorGUILayout.Space ();
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndVertical ();
			GUILayout.EndScrollView ();
		}

		if (currentView == VIEW_ABOUT) {

			GUILayout.Space (30);

			GUILayout.Label ("Headless Builder", titleStyle);
			GUILayout.Label ("by Salty Devs", subtitleStyle);
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			GUILayout.Label (logoAsset, logoStyle);
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			GUILayout.Label ("v" + Headless.version, subtitleStyle);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.Label ("Questions or requests?", aboutSectionStyle);
			EditorGUILayout.Space ();
            GUILayout.Label("Please email us at team@saltydevs.com,\nand we'll happily try and help out!", centerStyle);
			GUILayout.Space (3);
			if (GUILayout.Button ("Send an email", linkCenterStyle)) {
				Application.OpenURL ("mailto:team@saltydevs.com");
			}
			EditorGUIUtility.AddCursorRect (GUILayoutUtility.GetLastRect (), MouseCursor.Link);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.Label ("Wanna help?", aboutSectionStyle);
			EditorGUILayout.Space ();
			GUILayout.Label ("We'd love to hear your review at the Unity Asset store!", centerStyle);
			GUILayout.Space (3);
			if (GUILayout.Button ("Leave a review", linkCenterStyle)) {
				Application.OpenURL ("https://www.assetstore.unity3d.com/#!/content/108317");
			}
			EditorGUIUtility.AddCursorRect (GUILayoutUtility.GetLastRect (), MouseCursor.Link);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.Label ("Released a game?", aboutSectionStyle);
			EditorGUILayout.Space ();
			GUILayout.Label ("Headless Builder is priced to be affordable for anyone.", centerStyle);
			EditorGUILayout.Space ();
			GUILayout.Label ("If you feel Headless Builder gave you much more than it cost,\nwe would truly appreciate it if you considered giving a donation.", centerStyle);
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			if (GUILayout.Button (donateAsset, linkCenterStyle)) {
				Application.OpenURL ("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=Y2F99K65T4D58");
			}
			EditorGUIUtility.AddCursorRect (GUILayoutUtility.GetLastRect (), MouseCursor.Link);
		}

		if (!dropFrame) {
			if (!JsonUtility.ToJson (newSettings).Equals (JsonUtility.ToJson (headlessSettings))) {
				headlessSettings = verifySettings (newSettings);
				stampSettings = Time.time;
				dirtySettings = true;
			}
		}

	}

    public bool IsDocked()
    {
#pragma warning disable 0168
        try
        {
            BindingFlags fullBinding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            MethodInfo isDockedMethod = typeof(EditorWindow).GetProperty("docked", fullBinding).GetGetMethod(true);

            if ((bool)isDockedMethod.Invoke(this, null) == false) // if not docked
                return false;

            return true;
        } catch (System.Exception e)
        {
            return false;
        }
#pragma warning restore 0168
    }


    public static void RemoveProfile ()
	{
		HeadlessProfiles.RemoveProfile ();
		headlessSettings = LoadSettings (HeadlessProfiles.currentProfile, false);
	}

	public static void SelectProfile (object data)
	{
		string profile = (string)data;
		HeadlessProfiles.SetProfile (profile);
		headlessSettings = LoadSettings (HeadlessProfiles.currentProfile, false);
		EditorPrefs.SetString ("HEADLESSBUILDER_LASTPROFILE", HeadlessProfiles.currentProfile);
	}

	public static HeadlessSettings GetSettings ()
	{
		if (headlessSettings == null) {
			headlessSettings = LoadSettings (HeadlessProfiles.currentProfile, true);
		}

		return headlessSettings;
	}

	public static IEnumerator SaveSettingsAsync (HeadlessSettings newSettings)
	{
		yield return new WaitForSeconds (0.0f);
		SaveSettings (newSettings, HeadlessProfiles.currentProfile);
	}

	public static void SaveSettings (HeadlessSettings newSettings, string profilePath)
	{

		if (newSettings == null) {
			savingSettings = false;
			return;
		}

		//headlessSettings = newSettings;
		CreateOrReplaceAsset<HeadlessSettings> (newSettings, profilePath);

		HeadlessRuntime newRuntime = ScriptableObject.CreateInstance<HeadlessRuntime> ();
		newRuntime.profileName = newSettings.profileName;
		newRuntime.valueFramerate = newSettings.valueFramerate;
		newRuntime.valueLimitFramerate = newSettings.valueLimitFramerate;
		newRuntime.valueCamera = newSettings.valueCamera;
		newRuntime.valueConsole = newSettings.valueConsole;
		CreateOrReplaceAsset<HeadlessRuntime> (newRuntime, HeadlessExtensions.GetHeadlessBuilderPath (false) + "/Resources/HeadlessRuntime.asset");

		AssetDatabase.SaveAssets ();
		//AssetDatabase.Refresh ();

		if (dirtyProfiles) {
			HeadlessProfiles.FindProfiles ();
			dirtyProfiles = false;
		}

		savingSettings = false;

	}

	public void OnInspectorUpdate ()
	{
		Repaint ();
	}

	public static HeadlessSettings LoadSettings (string profilePath, bool preserve)
	{
		if (preserve && (dirtySettings || savingSettings)) {
			return headlessSettings;
		}

		HeadlessSettings newSettings = ScriptableObject.CreateInstance<HeadlessSettings> ();
		HeadlessSettings oldSettings = AssetDatabase.LoadAssetAtPath<HeadlessSettings> (profilePath);

		if (oldSettings != null) {
			JsonUtility.FromJsonOverwrite (JsonUtility.ToJson (oldSettings), newSettings);
		}

		return newSettings;
	}

	private static T CreateOrReplaceAsset<T> (T asset, string path) where T:Object
	{
		T existingAsset = AssetDatabase.LoadAssetAtPath<T> (path);

		if (existingAsset == null) {
			AssetDatabase.CreateAsset (asset, path);
			existingAsset = asset;
		} else {
			EditorUtility.CopySerialized (asset, existingAsset);
		}

		return existingAsset;
	}


	public static IEnumerator AsyncPreloadDocumentationContent ()
	{
		yield return new WaitForSeconds (0.0f);
		GetDocumentationContent ("application");
		GetDocumentationContent ("assets");
		GetDocumentationContent ("batch");
		GetDocumentationContent ("cloud");
		GetDocumentationContent ("gettingStarted");
		GetDocumentationContent ("permissions");
		GetDocumentationContent ("profiles");
		GetDocumentationContent ("scripting");
		GetDocumentationContent ("troubleshooting");
	}

	private static string GetDocumentationContent (string identifier)
	{
		if (!documentationContent.ContainsKey (identifier)) {
			string content = "";
			StreamReader reader = new StreamReader (HeadlessExtensions.GetHeadlessBuilderPath (false) + "/Editor/Documentation/" + UppercaseFirst (identifier) + ".html");
			content = reader.ReadToEnd ();
			reader.Close ();

			content = content.Replace ("\n", "");
			content = content.Replace ("\r", "");
			content = content.Replace ("<br>", "\n");

			documentationContent.Add (identifier, content);
		}

		return documentationContent [identifier];
	}


	static string UppercaseFirst (string s)
	{
		if (string.IsNullOrEmpty (s)) {
			return string.Empty;
		}
		return char.ToUpper (s [0]) + s.Substring (1);
	}


	public static string CoerceValidFileName (string filename)
	{
		var invalidChars = Regex.Escape (new string (Path.GetInvalidFileNameChars ()));
		var invalidReStr = string.Format (@"[{0}]+", invalidChars);

		var reservedWords = new [] {
			"CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4",
			"COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
			"LPT5", "LPT6", "LPT7", "LPT8", "LPT9", "_Core"
		};

		var sanitisedNamePart = Regex.Replace (filename, invalidReStr, "_");
		foreach (var reservedWord in reservedWords) {
			var reservedWordPattern = string.Format ("^{0}\\.", reservedWord);
			sanitisedNamePart = Regex.Replace (sanitisedNamePart, reservedWordPattern, "_reservedWord_.", RegexOptions.IgnoreCase);
		}

		return sanitisedNamePart.Replace (" ", string.Empty);
	}

	private static HeadlessSettings verifySettings (HeadlessSettings newSettings)
	{
		if (newSettings.valuePlatform == WINDOWS) {
			newSettings.valueArchitecture = newSettings.valueArchitectureWindows;
		} else if (newSettings.valuePlatform == OSX) {
			newSettings.valueArchitecture = newSettings.valueArchitectureOSX;
		} else if (newSettings.valuePlatform == LINUX) {
			newSettings.valueArchitecture = newSettings.valueArchitectureLinux;
		}

		List<int> deleteScenes = new List<int> ();
		List<SceneAsset> listSceneAssets = newSettings.valueSceneAssets.ToList ();
		for (int i = 0; i < listSceneAssets.Count; i++) {
			string scenePath = AssetDatabase.GetAssetPath (listSceneAssets [i]);
			if (string.IsNullOrEmpty (scenePath)) {
				deleteScenes.Add (i);
			}
		}
		for (int i = deleteScenes.Count - 1; i >= 0; i--) {
			listSceneAssets.RemoveAt (deleteScenes [i]);
		}
		newSettings.valueSceneAssets = listSceneAssets.ToArray ();

		return newSettings;
	}

}
