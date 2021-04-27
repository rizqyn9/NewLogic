/* 
 * Headless Builder
 * (c) Salty Devs, 2019
 * 
 * Please do not publish or pirate this code.
 * We worked really hard to make it.
 * 
 */

using UnityEngine;

// This class contains code to show a folder to the user.
public class HeadlessExplore : MonoBehaviour {


	public static bool IsInMacOS
	{
		get
		{
			return SystemInfo.operatingSystem.IndexOf("Mac OS") != -1;
		}
	}

	public static bool IsInWinOS
	{
		get
		{
			return SystemInfo.operatingSystem.IndexOf("Windows") != -1;
		}
	}

	public static void OpenInMac(string path)
	{
		bool openInsidesOfFolder = false;

		string macPath = path.Replace("\\", "/");

		if ( System.IO.Directory.Exists(macPath) )
		{
			openInsidesOfFolder = true;
		}

		if ( !macPath.StartsWith("\"") )
		{
			macPath = "\"" + macPath;
		}

		if ( !macPath.EndsWith("\"") )
		{
			macPath = macPath + "\"";
		}

		string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;

		try
		{
			System.Diagnostics.Process.Start("open", arguments);
		}
		catch ( System.ComponentModel.Win32Exception e )
		{
			e.HelpLink = "";
		}
	}

	public static void OpenInWin(string path)
	{
		bool openInsidesOfFolder = false;

		string winPath = path.Replace("/", "\\");

		if ( System.IO.Directory.Exists(winPath) )
		{
			openInsidesOfFolder = true;
		}

		try
		{
			System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
		}
		catch ( System.ComponentModel.Win32Exception e )
		{
			e.HelpLink = "";
		}
	}

	public static void Open(string path)
	{
		if ( IsInWinOS )
		{
			OpenInWin(path);
		}
		else if ( IsInMacOS )
		{
			OpenInMac(path);
		}
		else // couldn't determine OS
		{
			OpenInWin(path);
			OpenInMac(path);
		}
	}

}
