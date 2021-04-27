/* 
 * Headless Builder
 * (c) Salty Devs, 2019
 * 
 * Please do not publish or pirate this code.
 * We worked really hard to make it.
 * 
 */

using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.IO;

// This class allows the application to output to the Windows console.
namespace Windows
{
	
	public class HeadlessConsole
	{
		TextWriter oldOutput;

		public void Initialize()
		{
			
			if ( !AttachConsole( 0x0ffffffff ) )
			{
				AllocConsole();
			}

			oldOutput = Console.Out;

			try
			{
				IntPtr stdHandle = GetStdHandle( STD_OUTPUT_HANDLE );
				#pragma warning disable 618
				FileStream fileStream = new FileStream( stdHandle, FileAccess.Write );
				#pragma warning restore 618
				System.Text.Encoding encoding = System.Text.Encoding.ASCII;
				StreamWriter standardOutput = new StreamWriter( fileStream, encoding );
				standardOutput.AutoFlush = true;
				Console.SetOut( standardOutput );
			}
			catch ( Exception e )
			{
				Debug.Log( "Couldn't redirect output: " + e.Message );
			}
		}

		public void Shutdown()
		{
			Console.SetOut( oldOutput );
			FreeConsole();
		}

		public void SetTitle( string strName )
		{
			SetConsoleTitle( strName );
		}

		private const int STD_OUTPUT_HANDLE = -11;

		[DllImport( "kernel32.dll", SetLastError = true )]
		static extern bool AttachConsole( uint dwProcessId );

		[DllImport( "kernel32.dll", SetLastError = true )]
		static extern bool AllocConsole();

		[DllImport( "kernel32.dll", SetLastError = true )]
		static extern bool FreeConsole();

		[DllImport( "kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall )]
		private static extern IntPtr GetStdHandle( int nStdHandle );

		[DllImport( "kernel32.dll" )]
		static extern bool SetConsoleTitle( string lpConsoleTitle );

	}
}