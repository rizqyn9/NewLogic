/* 
 * Headless Builder
 * (c) Salty Devs, 2019
 * 
 * Please do not publish or pirate this code.
 * We worked really hard to make it.
 * 
 */

using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

// This class contains code for post-processing builds.
public class Postprocessor : AssetPostprocessor
{   
	
	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
	{
		if (Headless.IsBuildingHeadless()) {
			HeadlessBuilder.Postprocess(Directory.GetParent (path).ToString ());
		}
	}

}