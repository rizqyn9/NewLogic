/* 
 * Headless Builder
 * (c) Salty Devs, 2018
 * 
 * Please do not publish or pirate this code.
 * We worked really hard to make it.
 * 
 */

using UnityEngine;

// This class contains all settings for Headless Builder that need to be available during runtime.
public class HeadlessRuntime : ScriptableObject {

	public string profileName = "";

	public int valueFramerate = 60;
	public bool valueLimitFramerate = true;
	public bool valueCamera = true;
	public bool valueConsole = true;

}