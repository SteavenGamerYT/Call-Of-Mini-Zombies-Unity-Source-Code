using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace IngameDebugConsole.Commands
{
	public class SceneCommands
	{
		[ConsoleMethod("scene.load", "Loads a scene", new string[] { })]
		[Preserve]
		public static void LoadScene(string sceneName)
		{
			LoadSceneInternal(sceneName, false, LoadSceneMode.Single);
		}

		[ConsoleMethod("scene.load", "Loads a scene", new string[] { })]
		[Preserve]
		public static void LoadScene(string sceneName, LoadSceneMode mode)
		{
			LoadSceneInternal(sceneName, false, mode);
		}

		[ConsoleMethod("scene.loadasync", "Loads a scene asynchronously", new string[] { })]
		[Preserve]
		public static void LoadSceneAsync(string sceneName)
		{
			LoadSceneInternal(sceneName, true, LoadSceneMode.Single);
		}

		[ConsoleMethod("scene.loadasync", "Loads a scene asynchronously", new string[] { })]
		[Preserve]
		public static void LoadSceneAsync(string sceneName, LoadSceneMode mode)
		{
			LoadSceneInternal(sceneName, true, mode);
		}

		private static void LoadSceneInternal(string sceneName, bool isAsync, LoadSceneMode mode)
		{
			if (SceneManager.GetSceneByName(sceneName).IsValid())
			{
				Debug.Log("Scene " + sceneName + " is already loaded");
			}
			else if (isAsync)
			{
				SceneManager.LoadSceneAsync(sceneName, mode);
			}
			else
			{
				SceneManager.LoadScene(sceneName, mode);
			}
		}

		[ConsoleMethod("scene.unload", "Unloads a scene", new string[] { })]
		[Preserve]
		public static void UnloadScene(string sceneName)
		{
			SceneManager.UnloadSceneAsync(sceneName);
		}

		[ConsoleMethod("scene.restart", "Restarts the active scene", new string[] { })]
		[Preserve]
		public static void RestartScene()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
		}
	}
}
