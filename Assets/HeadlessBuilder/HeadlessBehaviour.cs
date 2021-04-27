/* 
 * Headless Builder
 * (c) Salty Devs, 2019
 * 
 * Please do not publish or pirate this code.
 * We worked really hard to make it.
 * 
 */

using UnityEngine;

// This class affects the game's behaviour during runtime.
public class HeadlessBehaviour : MonoBehaviour {
	
	// Callback to be called before any culling
	public void NullifyCamera(Camera camera)
	{
		camera.enabled = false;
	}

}
