/* 
 * Headless Builder
 * (c) Salty Devs, 2019
 * 
 * Please do not publish or pirate this code.
 * We worked really hard to make it.
 * 
 */

using UnityEngine;
using System;
using System.IO;

// This class handles the initialization of headless builds.
public static class Headless {

	public static readonly string version = "1.6.4";

	private static bool isHeadless = false;
	private static bool checkedHeadless = false;
	private static bool initializedHeadless = false;

	private static bool buildingHeadless = false;
    private static bool debuggingHeadless = false;

	private static HeadlessRuntime headlessRuntime;
	private static string currentProfile = "";

	// This function returns the name of the current profile.
	public static string GetProfileName() {
		if (!IsHeadless ()) {
			return null;
		}
		InitializeHeadless ();
		return currentProfile;
	}

	// This function checks whether the build that the user is currently running is headless.
	public static bool IsHeadless() {
		if (checkedHeadless) {
			return isHeadless;
		}

        if (File.Exists(Application.dataPath + "/~HeadlessDebug.txt"))
        {
            debuggingHeadless = true;
            isHeadless = true;
        }
        else
        {
            string[] args = Environment.GetCommandLineArgs();
            if (Array.IndexOf(args, "-batchmode") >= 0)
            {
                isHeadless = true;
            }
            else if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
            {
                isHeadless = true;
            }
        }

		checkedHeadless = true;
		return isHeadless;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void OnBeforeSceneLoadRuntimeMethod()
	{
		if (IsHeadless ()) {
			InitializeHeadless ();
			HeadlessCallbacks.InvokeCallbacks ("HeadlessBeforeSceneLoad");
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	static void OnAfterSceneLoadRuntimeMethod()
	{
		if (IsHeadless ()) {
			if (headlessRuntime.valueCamera) {
				GameObject headlessObject = GameObject.Find ("HeadlessBehaviour");
				if (headlessObject == null) {
					headlessObject = (GameObject) GameObject.Instantiate (Resources.Load ("HeadlessBehaviour"));
				}
				HeadlessBehaviour headlessBehaviour = headlessObject.GetComponent<HeadlessBehaviour> ();
				if (headlessBehaviour == null) {
					headlessBehaviour = headlessObject.AddComponent<HeadlessBehaviour> ();
				}
				Camera.onPreCull += headlessBehaviour.GetComponent<HeadlessBehaviour> ().NullifyCamera;
			}

			HeadlessCallbacks.InvokeCallbacks ("HeadlessAfterSceneLoad");
		}
	}

	private static void InitializeHeadless() {
		if (initializedHeadless) {
			return;
		}

		headlessRuntime = Resources.Load ("HeadlessRuntime") as HeadlessRuntime;
		if (headlessRuntime != null) {

			currentProfile = headlessRuntime.profileName;
			
			#if UNITY_STANDALONE_WIN
			if (headlessRuntime.valueConsole && !Application.isEditor) {
				Windows.HeadlessConsole console = new Windows.HeadlessConsole();
				console.Initialize();
				console.SetTitle(Application.productName);

				Application.logMessageReceived += HandleLog;
			}
			#endif

			if (headlessRuntime.valueLimitFramerate) {
				Application.targetFrameRate = headlessRuntime.valueFramerate;
                QualitySettings.vSyncCount = 0;
                Debug.Log ("Application target framerate set to " + headlessRuntime.valueFramerate);
			}
		}

		initializedHeadless = true;

		HeadlessCallbacks.InvokeCallbacks ("HeadlessBeforeFirstSceneLoad");
	}

	private static void HandleLog(string logString, string stackTrace, LogType type) {
		Console.WriteLine (logString);
		if (stackTrace.Length > 1) {
			Console.WriteLine ("in: " + stackTrace);
		}

	}


	// This function checks whether the user is currently making a headless build
	public static bool IsBuildingHeadless() {
		if (buildingHeadless) {
			return true;
		}
		return false;
    }


    // This function checks whether the user is currently debugging a headless build
    public static bool IsDebuggingHeadless()
    {
        if (debuggingHeadless)
        {
            return true;
        }
        return false;
    }

    public static void SetBuildingHeadless(bool value, string profileName) {
		buildingHeadless = value;
		currentProfile = profileName;
	}

}
