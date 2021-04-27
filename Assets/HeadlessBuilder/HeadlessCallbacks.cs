/* 
 * Headless Builder
 * (c) Salty Devs, 2019
 * 
 * Please do not publish or pirate this code.
 * We worked really hard to make it.
 * 
 */

using System;
using System.Reflection;
using System.Collections;
using System.Linq;
using UnityEngine;

// This class finds all callbacks and allows other scripts to call them
public class HeadlessCallbacks : Attribute {

	private static IEnumerable callbackRegistry = null;

	public static void FindCallbacks() {
		if (callbackRegistry != null) {
			return;
		}

        try
        {
            var myAssembly = System.Reflection.Assembly.GetExecutingAssembly();

            callbackRegistry =
                from t in myAssembly.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(HeadlessCallbacks), true)
                where attributes != null && attributes.Length > 0
                select t;
        }
        catch (ReflectionTypeLoadException e)
        {
            try
            {
                callbackRegistry = e.Types.Where(t => t != null);
            } catch (Exception ex)
            {
                UnityEngine.Debug.Log("Headless Builder could not find callbacks (" + ex.GetType().Name + "), but will still continue as planned");
                callbackRegistry = Enumerable.Empty<Type>();
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("Headless Builder could not find callbacks (" + e.GetType().Name + "), but will still continue as planned");
            callbackRegistry = Enumerable.Empty<Type>();
        }
	}

	public static void InvokeCallbacks(string callbackName) {
		FindCallbacks ();

		foreach (Type type in callbackRegistry) {
			MethodInfo callbackMethod = type.GetMethod (callbackName);
			if (callbackMethod != null) {
				try {
					callbackMethod.Invoke (type, null);
				} catch (Exception e) {
					Debug.LogError (e);
				}
			}
		}
	}

}