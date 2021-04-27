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

// This class contains all code related to the menu for Headless Builder.
[InitializeOnLoad]
public class HeadlessMenu : MonoBehaviour
{

	private static bool initializedMenu = false;


	static HeadlessMenu ()
	{
		EditorApplication.update += Update;
	}

	static void Update() {
		InitializeMenu ();
	}

	// This function shows a message to the user on special occasions.
	static void InitializeMenu() {
		if (!initializedMenu) {
			initializedMenu = true;
			bool firstStart = EditorPrefs.GetBool ("HEADLESSBUILDER_FIRSTSTART", true);
			if (firstStart) {
				EditorPrefs.SetBool ("HEADLESSBUILDER_FIRSTSTART", false);
				EditorUtility.DisplayDialog ("Headless Builder", "You have just installed Headless Builder. Awesome!\n\nYou can set-up your headless build in the Unity Editor menu, under \"Tools\".", "Got it!");
			}
			int finishedBuilds = EditorPrefs.GetInt ("HEADLESSBUILDER_FINISHEDBUILDS", 0);
			bool remindThanks = EditorPrefs.GetBool ("HEADLESSBUILDER_REMINDTHANKS", true);
			if (finishedBuilds > 25 && remindThanks) {
				EditorPrefs.SetBool ("HEADLESSBUILDER_REMINDTHANKS", false);
				bool wantThanks = EditorUtility.DisplayDialog ("Headless Builder", "You've done over 25 builds with Headless Builder. Awesome!\n\nWould you help us out and leave a nice review on the Unity Asset Store?", "Sure!", "Nah :(");
				if (wantThanks) {
					Application.OpenURL("https://www.assetstore.unity3d.com/#!/content/108317");
				}
			}

			EditorApplication.update -= Update;
		}
	}

	// This function returns whether menu items should be enabled.
	private static bool isMenuEnabled() {
		return !Headless.IsBuildingHeadless() && !BuildPipeline.isBuildingPlayer && !HeadlessEditor.dirtySettings && !HeadlessEditor.savingSettings;
	}


	// MENU ITEMS:

	[MenuItem("Tools/Headless Builder/Build (Current Profile)", false, 100)]
	static void SelectBuildHeadless()
	{
		HeadlessBuilder.ManualBuild ();
	}

	[MenuItem("Tools/Headless Builder/Build (Current Profile)", true)]
	static bool ValidateBuildHeadless()
	{
		return isMenuEnabled();
    }

    [MenuItem("Tools/Headless Builder/Build (All Profiles)", false, 101)]
    static void SelectBuildHeadlessAll()
    {
        HeadlessBuilder.ManualBuildQueue();
    }

    [MenuItem("Tools/Headless Builder/Build (All Profiles)", true)]
    static bool ValidateBuildHeadlessAll()
    {
        return isMenuEnabled() && HeadlessProfiles.GetProfileList().Count > 1;
    }

    /*[MenuItem("Tools/Headless Builder/Debug Scene (Current Profile)", false, 200)]
    static void DebugHeadlessScene()
    {
        if (!EditorUtility.DisplayDialog("Headless Builder",
            "This is a highly experimental feature that allows you " +
            "to debug your headless build inside the Unity Editor.\n\n" +
            "You should only continue if you have made a backup of your project.\n\n" +
            "Are you sure you want to continue?", "Yes", "Cancel"))
        {

            return;
        }
        HeadlessDebug.DebugScene();
    }

    [MenuItem("Tools/Headless Builder/Debug Scene (Current Profile)", true)]
    static bool ValidateDebugHeadlessScene()
    {
        return isMenuEnabled() && !EditorApplication.isPlaying;
    }*/

    [MenuItem("Tools/Headless Builder/Settings...", false, 300)]
	static void SelectSettings()
	{
		HeadlessEditor editor = EditorWindow.GetWindow <HeadlessEditor> ();
		editor.OnSettings ();
	}

	[MenuItem("Tools/Headless Builder/Documentation...", false, 301)]
	static void SelectDocumentation()
	{
		HeadlessEditor editor = EditorWindow.GetWindow <HeadlessEditor> ();
		editor.OnDocumentation ();
	}

	[MenuItem("Tools/Headless Builder/About...", false, 302)]
	static void SelectAbout()
	{
		HeadlessEditor editor = EditorWindow.GetWindow <HeadlessEditor> ();
		editor.OnAbout ();
	}

}