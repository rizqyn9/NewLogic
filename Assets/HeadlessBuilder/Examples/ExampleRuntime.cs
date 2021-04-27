/* 
 * Headless Builder
 * (c) Salty Devs, 2019
 * 
 * Please do not publish or pirate this code.
 * We worked really hard to make it.
 * 
 */

using UnityEngine;

// This class provides example callbacks for Headless Builder during runtime
[HeadlessCallbacks]
public static class ExampleRuntime {

	public static void HeadlessBeforeFirstSceneLoad() {
		// This code will be executed right before the first scene is loaded in a headless build
		//Debug.Log ("The first scene is about to be loaded.");
	}

	public static void HeadlessBeforeSceneLoad() {
		// This code will be executed right before any scene is loaded in a headless build
		//Debug.Log ("A scene is about to be loaded.");
	}

	public static void HeadlessAfterSceneLoad() {
		// This code will be executed right after any scene is loaded in a headless build
		//Debug.Log ("A scene was just loaded.");
	}

}
