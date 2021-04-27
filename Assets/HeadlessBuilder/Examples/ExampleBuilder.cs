/* 
 * Headless Builder
 * (c) Salty Devs, 2019
 * 
 * Please do not publish or pirate this code.
 * We worked really hard to make it.
 * 
 */

using UnityEngine;

// This class provides example callbacks for Headless Builder during building
[HeadlessCallbacks]
public static class ExampleBuilder {

	public static void HeadlessBuildBefore() {
		// This code will be executed before making an headless build
		//Debug.Log ("The build is about to start.");
	}

	public static void HeadlessBuildSuccess() {
		// This code will be executed after succesfully making an headless build
		//Debug.Log ("The build just finished.");
	}

	public static void HeadlessBuildFailed() {
		// This code will be executed after failing to make an headless build
		//Debug.Log ("The build just failed.");
	}

}
