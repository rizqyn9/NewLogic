/* 
 * Headless Builder
 * (c) Salty Devs, 2019
 * 
 * Please do not publish or pirate this code.
 * We worked really hard to make it.
 * 
 */

using System.Collections;
using UnityEditor;
using Object = UnityEngine.Object;

// This class contains code for executing asynchronous code inside the Unity Editor.
public class HeadlessRoutine
{
	public static HeadlessRoutine start( IEnumerator _routine )
	{
		HeadlessRoutine coroutine = new HeadlessRoutine(_routine);
		coroutine.start();
		return coroutine;
	}

	readonly IEnumerator routine;
	HeadlessRoutine( IEnumerator _routine )
	{
		routine = _routine;
	}

	void start()
	{
		EditorApplication.update += update;
	}

	public void stop()
	{
		EditorApplication.update -= update;
	}

	void update()
	{
			if (!routine.MoveNext())
			{
				stop();
			}
	}
}