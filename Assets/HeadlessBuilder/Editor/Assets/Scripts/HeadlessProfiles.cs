/* 
 * Headless Builder
 * (c) Salty Devs, 2019
 * 
 * Please do not publish or pirate this code.
 * We worked really hard to make it.
 * 
 */

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// This class handles profiles
public static class HeadlessProfiles {

	private static Dictionary<string,string> profileList = null;
	public static string defaultProfile = "Assets/HeadlessSettings.asset";
	public static string currentProfile = defaultProfile;

	public static Dictionary<string,string> GetProfileList() {
		if (profileList == null) {
			FindProfiles ();
		}
		return profileList;
	}

	public static void FindProfiles() {
		Dictionary<string,string> newList = new Dictionary<string, string>();

		string[] profiles = AssetDatabase.FindAssets ("HeadlessSettings");
		foreach (var guid in profiles) {
			string profilePath = AssetDatabase.GUIDToAssetPath (guid);
			if (profilePath.EndsWith (".asset")) {
				HeadlessSettings profileData = HeadlessEditor.LoadSettings (profilePath, false);
				newList.Add (profilePath, profileData.profileName);
			}
		}

		profileList = newList;
	}

	public static void RemoveProfile() {
		AssetDatabase.DeleteAsset (currentProfile);
		SetProfile (defaultProfile);
		FindProfiles ();
	}

	public static void CreateProfile() {
		System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
		int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;

		string newProfile = "Assets/HeadlessSettings_" + cur_time + Random.Range(100,1000) + ".asset";

		AssetDatabase.CopyAsset (currentProfile, newProfile);
		AssetDatabase.Refresh ();
		FindProfiles ();
		currentProfile = newProfile;
	}

	public static void SetProfile(string path) {
		currentProfile = path;
	}

}
