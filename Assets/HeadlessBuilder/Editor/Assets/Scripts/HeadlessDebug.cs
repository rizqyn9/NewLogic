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
using UnityEditor.SceneManagement;
using System.IO;

[InitializeOnLoadAttribute]
public class HeadlessDebug
{

    static HeadlessDebug() {
#if UNITY_2017_2_OR_NEWER
        EditorApplication.playModeStateChanged += PlayModeStateChanged;
#endif
    }

#if UNITY_2017_2_OR_NEWER
    private static void PlayModeStateChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode)
        {
            if (File.Exists(Application.dataPath + "/~HeadlessDebug.txt"))
            {
                File.Delete(Application.dataPath + "/~HeadlessDebug.txt");
                if (File.Exists(Application.dataPath + "/~HeadlessDebug.txt.meta"))
                {
                    File.Delete(Application.dataPath + "/~HeadlessDebug.txt.meta");
                }
                HeadlessBuilder.RestoreBuild();
                EditorSceneManager.LoadScene(EditorSceneManager.GetActiveScene().path);
            }
        }
    }
#endif

    public static void DebugScene()
    {
        HeadlessBuilder.DebugBuild();
    }

    public static void StartDebug()
    {
        System.IO.File.WriteAllText(Application.dataPath + "/~HeadlessDebug.txt", "The existence of this file means that a backup was created and not (yet) reverted.");
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }
}
