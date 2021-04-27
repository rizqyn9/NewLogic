/* 
 * Headless Builder
 * (c) Salty Devs, 2019
 * 
 * Please do not publish or pirate this code.
 * We worked really hard to make it.
 * 
 */

using UnityEditor;

// This class makes Headless Builder compatible with a broader range of Unity versions,
// by functioning as a layer between Headless Builder and Unity for script functions
// that have changed between versions.
public static class HeadlessAPI {

	public static BuildTarget VersionedBuildTargetOSX() {
		#if UNITY_2017_1_OR_NEWER
			return BuildTarget.StandaloneOSX;
		#else
			return BuildTarget.StandaloneOSXUniversal;
		#endif
	}

	public static void VersionedSwitchActiveBuildTarget(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget) {
		#if UNITY_5_6_OR_NEWER
			EditorUserBuildSettings.SwitchActiveBuildTarget (buildTargetGroup, buildTarget);
		#else
			EditorUserBuildSettings.SwitchActiveBuildTarget (buildTarget);
		#endif
	}

}
