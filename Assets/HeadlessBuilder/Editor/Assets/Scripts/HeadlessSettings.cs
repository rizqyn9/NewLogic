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
using System;

// This class functions as a model for the Headless Builder settings object and file.
public class HeadlessSettings : ScriptableObject {

	public string buildPath = null;
	public int buildID = 0;

	public bool profileSection = true;
	public String profileName = "Untitled";

	public bool scenesSection = true;
	public bool valueOverrideScenes = false;
	public SceneAsset[] valueSceneAssets = new SceneAsset[] {};

	public bool targetSection = true;
	public bool valueOverrideTarget = false;
	public int valuePlatform = 0;
	public int valueArchitecture = 0;
	public int valueArchitectureWindows = 0;
	public int valueArchitectureOSX = 0;
	public int valueArchitectureLinux = 0;

	public bool optionsSection = true;
	public bool valueRememberPath = false;
	public bool valueSkipConfirmation = false;

	public bool applicationSection = true;
	public int valueFramerate = 60;
	public bool valueLimitFramerate = true;
	public bool valueCamera = true;
	public bool valueConsole = true;

	public bool assetsSection = true;
	public bool valueDummy = false;

	public bool preProcessingSection = true;
	public bool valueGI = true;
	public bool valueAudio = true;

	public bool postProcessingSection = true;
	public bool valueBackup = false;

	public bool startingDocumentation = true;
	public bool profilesDocumentation = true;
	public bool codeDocumentation = true;
	public bool applicationDocumentation = true;
	public bool assetsDocumentation = true;
	public bool cloudDocumentation = true;
	public bool batchDocumentation = true;
	public bool troubleshootDocumentation = true;

}