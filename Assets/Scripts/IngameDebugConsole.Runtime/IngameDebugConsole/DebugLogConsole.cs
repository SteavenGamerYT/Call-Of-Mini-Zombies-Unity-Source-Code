using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace IngameDebugConsole
{
	public static class DebugLogConsole
	{
		public delegate bool ParseFunction(string input, out object output);

		private static readonly List<ConsoleMethodInfo> methods;

		private static readonly List<ConsoleMethodInfo> matchingMethods;

		private static readonly Dictionary<Type, ParseFunction> parseFunctions;

		private static readonly Dictionary<Type, string> typeReadableNames;

		private static readonly List<string> commandArguments;

		private static readonly string[] inputDelimiters;

		internal static readonly CompareInfo caseInsensitiveComparer;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache2;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache3;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache4;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache5;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache6;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache7;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache8;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache9;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cacheA;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cacheB;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cacheC;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cacheD;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cacheE;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cacheF;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache10;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache11;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache12;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache13;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache14;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache15;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache16;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache17;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache18;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache19;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache1A;

		[CompilerGenerated]
		private static ParseFunction _003C_003Ef__mg_0024cache1B;

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache1C;

		[CompilerGenerated]
		private static Action<string> _003C_003Ef__mg_0024cache1D;

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache1E;

		static DebugLogConsole()
		{
			methods = new List<ConsoleMethodInfo>();
			matchingMethods = new List<ConsoleMethodInfo>(4);
			Dictionary<Type, ParseFunction> dictionary = new Dictionary<Type, ParseFunction>();
			Type typeFromHandle = typeof(string);
			if (_003C_003Ef__mg_0024cache0 == null)
			{
				_003C_003Ef__mg_0024cache0 = ParseString;
			}
			dictionary.Add(typeFromHandle, _003C_003Ef__mg_0024cache0);
			Type typeFromHandle2 = typeof(bool);
			if (_003C_003Ef__mg_0024cache1 == null)
			{
				_003C_003Ef__mg_0024cache1 = ParseBool;
			}
			dictionary.Add(typeFromHandle2, _003C_003Ef__mg_0024cache1);
			Type typeFromHandle3 = typeof(int);
			if (_003C_003Ef__mg_0024cache2 == null)
			{
				_003C_003Ef__mg_0024cache2 = ParseInt;
			}
			dictionary.Add(typeFromHandle3, _003C_003Ef__mg_0024cache2);
			Type typeFromHandle4 = typeof(uint);
			if (_003C_003Ef__mg_0024cache3 == null)
			{
				_003C_003Ef__mg_0024cache3 = ParseUInt;
			}
			dictionary.Add(typeFromHandle4, _003C_003Ef__mg_0024cache3);
			Type typeFromHandle5 = typeof(long);
			if (_003C_003Ef__mg_0024cache4 == null)
			{
				_003C_003Ef__mg_0024cache4 = ParseLong;
			}
			dictionary.Add(typeFromHandle5, _003C_003Ef__mg_0024cache4);
			Type typeFromHandle6 = typeof(ulong);
			if (_003C_003Ef__mg_0024cache5 == null)
			{
				_003C_003Ef__mg_0024cache5 = ParseULong;
			}
			dictionary.Add(typeFromHandle6, _003C_003Ef__mg_0024cache5);
			Type typeFromHandle7 = typeof(byte);
			if (_003C_003Ef__mg_0024cache6 == null)
			{
				_003C_003Ef__mg_0024cache6 = ParseByte;
			}
			dictionary.Add(typeFromHandle7, _003C_003Ef__mg_0024cache6);
			Type typeFromHandle8 = typeof(sbyte);
			if (_003C_003Ef__mg_0024cache7 == null)
			{
				_003C_003Ef__mg_0024cache7 = ParseSByte;
			}
			dictionary.Add(typeFromHandle8, _003C_003Ef__mg_0024cache7);
			Type typeFromHandle9 = typeof(short);
			if (_003C_003Ef__mg_0024cache8 == null)
			{
				_003C_003Ef__mg_0024cache8 = ParseShort;
			}
			dictionary.Add(typeFromHandle9, _003C_003Ef__mg_0024cache8);
			Type typeFromHandle10 = typeof(ushort);
			if (_003C_003Ef__mg_0024cache9 == null)
			{
				_003C_003Ef__mg_0024cache9 = ParseUShort;
			}
			dictionary.Add(typeFromHandle10, _003C_003Ef__mg_0024cache9);
			Type typeFromHandle11 = typeof(char);
			if (_003C_003Ef__mg_0024cacheA == null)
			{
				_003C_003Ef__mg_0024cacheA = ParseChar;
			}
			dictionary.Add(typeFromHandle11, _003C_003Ef__mg_0024cacheA);
			Type typeFromHandle12 = typeof(float);
			if (_003C_003Ef__mg_0024cacheB == null)
			{
				_003C_003Ef__mg_0024cacheB = ParseFloat;
			}
			dictionary.Add(typeFromHandle12, _003C_003Ef__mg_0024cacheB);
			Type typeFromHandle13 = typeof(double);
			if (_003C_003Ef__mg_0024cacheC == null)
			{
				_003C_003Ef__mg_0024cacheC = ParseDouble;
			}
			dictionary.Add(typeFromHandle13, _003C_003Ef__mg_0024cacheC);
			Type typeFromHandle14 = typeof(decimal);
			if (_003C_003Ef__mg_0024cacheD == null)
			{
				_003C_003Ef__mg_0024cacheD = ParseDecimal;
			}
			dictionary.Add(typeFromHandle14, _003C_003Ef__mg_0024cacheD);
			Type typeFromHandle15 = typeof(Vector2);
			if (_003C_003Ef__mg_0024cacheE == null)
			{
				_003C_003Ef__mg_0024cacheE = ParseVector2;
			}
			dictionary.Add(typeFromHandle15, _003C_003Ef__mg_0024cacheE);
			Type typeFromHandle16 = typeof(Vector3);
			if (_003C_003Ef__mg_0024cacheF == null)
			{
				_003C_003Ef__mg_0024cacheF = ParseVector3;
			}
			dictionary.Add(typeFromHandle16, _003C_003Ef__mg_0024cacheF);
			Type typeFromHandle17 = typeof(Vector4);
			if (_003C_003Ef__mg_0024cache10 == null)
			{
				_003C_003Ef__mg_0024cache10 = ParseVector4;
			}
			dictionary.Add(typeFromHandle17, _003C_003Ef__mg_0024cache10);
			Type typeFromHandle18 = typeof(Quaternion);
			if (_003C_003Ef__mg_0024cache11 == null)
			{
				_003C_003Ef__mg_0024cache11 = ParseQuaternion;
			}
			dictionary.Add(typeFromHandle18, _003C_003Ef__mg_0024cache11);
			Type typeFromHandle19 = typeof(Color);
			if (_003C_003Ef__mg_0024cache12 == null)
			{
				_003C_003Ef__mg_0024cache12 = ParseColor;
			}
			dictionary.Add(typeFromHandle19, _003C_003Ef__mg_0024cache12);
			Type typeFromHandle20 = typeof(Color32);
			if (_003C_003Ef__mg_0024cache13 == null)
			{
				_003C_003Ef__mg_0024cache13 = ParseColor32;
			}
			dictionary.Add(typeFromHandle20, _003C_003Ef__mg_0024cache13);
			Type typeFromHandle21 = typeof(Rect);
			if (_003C_003Ef__mg_0024cache14 == null)
			{
				_003C_003Ef__mg_0024cache14 = ParseRect;
			}
			dictionary.Add(typeFromHandle21, _003C_003Ef__mg_0024cache14);
			Type typeFromHandle22 = typeof(RectOffset);
			if (_003C_003Ef__mg_0024cache15 == null)
			{
				_003C_003Ef__mg_0024cache15 = ParseRectOffset;
			}
			dictionary.Add(typeFromHandle22, _003C_003Ef__mg_0024cache15);
			Type typeFromHandle23 = typeof(Bounds);
			if (_003C_003Ef__mg_0024cache16 == null)
			{
				_003C_003Ef__mg_0024cache16 = ParseBounds;
			}
			dictionary.Add(typeFromHandle23, _003C_003Ef__mg_0024cache16);
			Type typeFromHandle24 = typeof(GameObject);
			if (_003C_003Ef__mg_0024cache17 == null)
			{
				_003C_003Ef__mg_0024cache17 = ParseGameObject;
			}
			dictionary.Add(typeFromHandle24, _003C_003Ef__mg_0024cache17);
			Type typeFromHandle25 = typeof(Vector2Int);
			if (_003C_003Ef__mg_0024cache18 == null)
			{
				_003C_003Ef__mg_0024cache18 = ParseVector2Int;
			}
			dictionary.Add(typeFromHandle25, _003C_003Ef__mg_0024cache18);
			Type typeFromHandle26 = typeof(Vector3Int);
			if (_003C_003Ef__mg_0024cache19 == null)
			{
				_003C_003Ef__mg_0024cache19 = ParseVector3Int;
			}
			dictionary.Add(typeFromHandle26, _003C_003Ef__mg_0024cache19);
			Type typeFromHandle27 = typeof(RectInt);
			if (_003C_003Ef__mg_0024cache1A == null)
			{
				_003C_003Ef__mg_0024cache1A = ParseRectInt;
			}
			dictionary.Add(typeFromHandle27, _003C_003Ef__mg_0024cache1A);
			Type typeFromHandle28 = typeof(BoundsInt);
			if (_003C_003Ef__mg_0024cache1B == null)
			{
				_003C_003Ef__mg_0024cache1B = ParseBoundsInt;
			}
			dictionary.Add(typeFromHandle28, _003C_003Ef__mg_0024cache1B);
			parseFunctions = dictionary;
			typeReadableNames = new Dictionary<Type, string>
			{
				{
					typeof(string),
					"String"
				},
				{
					typeof(bool),
					"Boolean"
				},
				{
					typeof(int),
					"Integer"
				},
				{
					typeof(uint),
					"Unsigned Integer"
				},
				{
					typeof(long),
					"Long"
				},
				{
					typeof(ulong),
					"Unsigned Long"
				},
				{
					typeof(byte),
					"Byte"
				},
				{
					typeof(sbyte),
					"Short Byte"
				},
				{
					typeof(short),
					"Short"
				},
				{
					typeof(ushort),
					"Unsigned Short"
				},
				{
					typeof(char),
					"Char"
				},
				{
					typeof(float),
					"Float"
				},
				{
					typeof(double),
					"Double"
				},
				{
					typeof(decimal),
					"Decimal"
				}
			};
			commandArguments = new List<string>(8);
			inputDelimiters = new string[5] { "\"\"", "''", "{}", "()", "[]" };
			caseInsensitiveComparer = new CultureInfo("en-US").CompareInfo;
			if (_003C_003Ef__mg_0024cache1C == null)
			{
				_003C_003Ef__mg_0024cache1C = LogAllCommands;
			}
			AddCommand("help", "Prints all commands", _003C_003Ef__mg_0024cache1C);
			if (_003C_003Ef__mg_0024cache1D == null)
			{
				_003C_003Ef__mg_0024cache1D = LogAllCommandsWithName;
			}
			AddCommand("help", "Prints all matching commands", _003C_003Ef__mg_0024cache1D);
			if (_003C_003Ef__mg_0024cache1E == null)
			{
				_003C_003Ef__mg_0024cache1E = LogSystemInfo;
			}
			AddCommand("sysinfo", "Prints system information", _003C_003Ef__mg_0024cache1E);
			string[] array = new string[12]
			{
				"Unity", "System", "Mono.", "mscorlib", "netstandard", "TextMeshPro", "Microsoft.GeneratedCode", "I18N", "Boo.", "UnityScript.",
				"ICSharpCode.", "ExCSS.Unity"
			};
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				string name = assembly.GetName().Name;
				bool flag = false;
				for (int j = 0; j < array.Length; j++)
				{
					if (caseInsensitiveComparer.IsPrefix(name, array[j], CompareOptions.IgnoreCase))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
				try
				{
					Type[] exportedTypes = assembly.GetExportedTypes();
					foreach (Type type in exportedTypes)
					{
						MethodInfo[] array2 = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
						foreach (MethodInfo methodInfo in array2)
						{
							object[] customAttributes = methodInfo.GetCustomAttributes(typeof(ConsoleMethodAttribute), false);
							foreach (object obj in customAttributes)
							{
								ConsoleMethodAttribute consoleMethodAttribute = obj as ConsoleMethodAttribute;
								if (consoleMethodAttribute != null)
								{
									AddCommand(consoleMethodAttribute.Command, consoleMethodAttribute.Description, methodInfo, null, consoleMethodAttribute.ParameterNames);
								}
							}
						}
					}
				}
				catch (NotSupportedException)
				{
				}
				catch (FileNotFoundException)
				{
				}
				catch (ReflectionTypeLoadException)
				{
				}
				catch (Exception ex4)
				{
					Debug.LogError("Couldn't search assembly for [ConsoleMethod] attributes: " + name + "\n" + ex4.ToString());
				}
			}
		}

		public static void LogAllCommands()
		{
			int num = 25;
			for (int i = 0; i < methods.Count; i++)
			{
				if (methods[i].IsValid())
				{
					num += methods[i].signature.Length + 7;
				}
			}
			StringBuilder stringBuilder = new StringBuilder(num);
			stringBuilder.Append("Available commands:");
			for (int j = 0; j < methods.Count; j++)
			{
				if (methods[j].IsValid())
				{
					stringBuilder.Append("\n    - ").Append(methods[j].signature);
				}
			}
			Debug.Log(stringBuilder.ToString());
			if ((bool)DebugLogManager.Instance)
			{
				DebugLogManager.Instance.AdjustLatestPendingLog(true, true);
			}
		}

		public static void LogAllCommandsWithName(string commandName)
		{
			matchingMethods.Clear();
			FindCommands(commandName, false, matchingMethods);
			if (matchingMethods.Count == 0)
			{
				FindCommands(commandName, true, matchingMethods);
			}
			if (matchingMethods.Count == 0)
			{
				Debug.LogWarning("ERROR: can't find command '" + commandName + "'");
				return;
			}
			int num = 25;
			for (int i = 0; i < matchingMethods.Count; i++)
			{
				num += matchingMethods[i].signature.Length + 7;
			}
			StringBuilder stringBuilder = new StringBuilder(num);
			stringBuilder.Append("Matching commands:");
			for (int j = 0; j < matchingMethods.Count; j++)
			{
				stringBuilder.Append("\n    - ").Append(matchingMethods[j].signature);
			}
			Debug.Log(stringBuilder.ToString());
			if ((bool)DebugLogManager.Instance)
			{
				DebugLogManager.Instance.AdjustLatestPendingLog(true, true);
			}
		}

		public static void LogSystemInfo()
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			stringBuilder.Append("Rig: ").AppendSysInfoIfPresent(SystemInfo.deviceModel).AppendSysInfoIfPresent(SystemInfo.processorType)
				.AppendSysInfoIfPresent(SystemInfo.systemMemorySize, "MB RAM")
				.Append(SystemInfo.processorCount)
				.Append(" cores\n");
			stringBuilder.Append("OS: ").Append(SystemInfo.operatingSystem).Append("\n");
			stringBuilder.Append("GPU: ").Append(SystemInfo.graphicsDeviceName).Append(" ")
				.Append(SystemInfo.graphicsMemorySize)
				.Append("MB ")
				.Append(SystemInfo.graphicsDeviceVersion)
				.Append((!SystemInfo.graphicsMultiThreaded) ? "\n" : " multi-threaded\n");
			stringBuilder.Append("Data Path: ").Append(Application.dataPath).Append("\n");
			stringBuilder.Append("Persistent Data Path: ").Append(Application.persistentDataPath).Append("\n");
			stringBuilder.Append("StreamingAssets Path: ").Append(Application.streamingAssetsPath).Append("\n");
			stringBuilder.Append("Temporary Cache Path: ").Append(Application.temporaryCachePath).Append("\n");
			stringBuilder.Append("Device ID: ").Append(SystemInfo.deviceUniqueIdentifier).Append("\n");
			stringBuilder.Append("Max Texture Size: ").Append(SystemInfo.maxTextureSize).Append("\n");
			stringBuilder.Append("Max Cubemap Size: ").Append(SystemInfo.maxCubemapSize).Append("\n");
			stringBuilder.Append("Accelerometer: ").Append((!SystemInfo.supportsAccelerometer) ? "not supported\n" : "supported\n");
			stringBuilder.Append("Gyro: ").Append((!SystemInfo.supportsGyroscope) ? "not supported\n" : "supported\n");
			stringBuilder.Append("Location Service: ").Append((!SystemInfo.supportsLocationService) ? "not supported\n" : "supported\n");
			stringBuilder.Append("Image Effects: ").Append((!SystemInfo.supportsImageEffects) ? "not supported\n" : "supported\n");
			stringBuilder.Append("RenderToCubemap: ").Append((!SystemInfo.supportsRenderToCubemap) ? "not supported\n" : "supported\n");
			stringBuilder.Append("Compute Shaders: ").Append((!SystemInfo.supportsComputeShaders) ? "not supported\n" : "supported\n");
			stringBuilder.Append("Shadows: ").Append((!SystemInfo.supportsShadows) ? "not supported\n" : "supported\n");
			stringBuilder.Append("Instancing: ").Append((!SystemInfo.supportsInstancing) ? "not supported\n" : "supported\n");
			stringBuilder.Append("Motion Vectors: ").Append((!SystemInfo.supportsMotionVectors) ? "not supported\n" : "supported\n");
			stringBuilder.Append("3D Textures: ").Append((!SystemInfo.supports3DTextures) ? "not supported\n" : "supported\n");
			stringBuilder.Append("3D Render Textures: ").Append((!SystemInfo.supports3DRenderTextures) ? "not supported\n" : "supported\n");
			stringBuilder.Append("2D Array Textures: ").Append((!SystemInfo.supports2DArrayTextures) ? "not supported\n" : "supported\n");
			stringBuilder.Append("Cubemap Array Textures: ").Append((!SystemInfo.supportsCubemapArrayTextures) ? "not supported" : "supported");
			Debug.Log(stringBuilder.ToString());
			if ((bool)DebugLogManager.Instance)
			{
				DebugLogManager.Instance.AdjustLatestPendingLog(true, true);
			}
		}

		private static StringBuilder AppendSysInfoIfPresent(this StringBuilder sb, string info, string postfix = null)
		{
			if (info != "n/a")
			{
				sb.Append(info);
				if (postfix != null)
				{
					sb.Append(postfix);
				}
				sb.Append(" ");
			}
			return sb;
		}

		private static StringBuilder AppendSysInfoIfPresent(this StringBuilder sb, int info, string postfix = null)
		{
			if (info > 0)
			{
				sb.Append(info);
				if (postfix != null)
				{
					sb.Append(postfix);
				}
				sb.Append(" ");
			}
			return sb;
		}

		public static void AddCustomParameterType(Type type, ParseFunction parseFunction, string typeReadableName = null)
		{
			if (type == null)
			{
				Debug.LogError("Parameter type can't be null!");
				return;
			}
			if (parseFunction == null)
			{
				Debug.LogError("Parameter parseFunction can't be null!");
				return;
			}
			parseFunctions[type] = parseFunction;
			if (!string.IsNullOrEmpty(typeReadableName))
			{
				typeReadableNames[type] = typeReadableName;
			}
		}

		public static void RemoveCustomParameterType(Type type)
		{
			parseFunctions.Remove(type);
			typeReadableNames.Remove(type);
		}

		public static void AddCommandInstance(string command, string description, string methodName, object instance, params string[] parameterNames)
		{
			if (instance == null)
			{
				Debug.LogError("Instance can't be null!");
			}
			else
			{
				AddCommand(command, description, methodName, instance.GetType(), instance, parameterNames);
			}
		}

		public static void AddCommandStatic(string command, string description, string methodName, Type ownerType, params string[] parameterNames)
		{
			AddCommand(command, description, methodName, ownerType, null, parameterNames);
		}

		public static void AddCommand(string command, string description, Action method)
		{
			AddCommand(command, description, method.Method, method.Target, null);
		}

		public static void AddCommand<T1>(string command, string description, Action<T1> method)
		{
			AddCommand(command, description, method.Method, method.Target, null);
		}

		public static void AddCommand<T1>(string command, string description, Func<T1> method)
		{
			AddCommand(command, description, method.Method, method.Target, null);
		}

		public static void AddCommand<T1, T2>(string command, string description, Action<T1, T2> method)
		{
			AddCommand(command, description, method.Method, method.Target, null);
		}

		public static void AddCommand<T1, T2>(string command, string description, Func<T1, T2> method)
		{
			AddCommand(command, description, method.Method, method.Target, null);
		}

		public static void AddCommand<T1, T2, T3>(string command, string description, Action<T1, T2, T3> method)
		{
			AddCommand(command, description, method.Method, method.Target, null);
		}

		public static void AddCommand<T1, T2, T3>(string command, string description, Func<T1, T2, T3> method)
		{
			AddCommand(command, description, method.Method, method.Target, null);
		}

		public static void AddCommand<T1, T2, T3, T4>(string command, string description, Action<T1, T2, T3, T4> method)
		{
			AddCommand(command, description, method.Method, method.Target, null);
		}

		public static void AddCommand<T1, T2, T3, T4>(string command, string description, Func<T1, T2, T3, T4> method)
		{
			AddCommand(command, description, method.Method, method.Target, null);
		}

		public static void AddCommand<T1, T2, T3, T4, T5>(string command, string description, Func<T1, T2, T3, T4, T5> method)
		{
			AddCommand(command, description, method.Method, method.Target, null);
		}

		public static void AddCommand(string command, string description, Delegate method)
		{
			AddCommand(command, description, method.Method, method.Target, null);
		}

		public static void AddCommand<T1>(string command, string description, Action<T1> method, string parameterName)
		{
			AddCommand(command, description, method.Method, method.Target, new string[1] { parameterName });
		}

		public static void AddCommand<T1, T2>(string command, string description, Action<T1, T2> method, string parameterName1, string parameterName2)
		{
			AddCommand(command, description, method.Method, method.Target, new string[2] { parameterName1, parameterName2 });
		}

		public static void AddCommand<T1, T2>(string command, string description, Func<T1, T2> method, string parameterName)
		{
			AddCommand(command, description, method.Method, method.Target, new string[1] { parameterName });
		}

		public static void AddCommand<T1, T2, T3>(string command, string description, Action<T1, T2, T3> method, string parameterName1, string parameterName2, string parameterName3)
		{
			AddCommand(command, description, method.Method, method.Target, new string[3] { parameterName1, parameterName2, parameterName3 });
		}

		public static void AddCommand<T1, T2, T3>(string command, string description, Func<T1, T2, T3> method, string parameterName1, string parameterName2)
		{
			AddCommand(command, description, method.Method, method.Target, new string[2] { parameterName1, parameterName2 });
		}

		public static void AddCommand<T1, T2, T3, T4>(string command, string description, Action<T1, T2, T3, T4> method, string parameterName1, string parameterName2, string parameterName3, string parameterName4)
		{
			AddCommand(command, description, method.Method, method.Target, new string[4] { parameterName1, parameterName2, parameterName3, parameterName4 });
		}

		public static void AddCommand<T1, T2, T3, T4>(string command, string description, Func<T1, T2, T3, T4> method, string parameterName1, string parameterName2, string parameterName3)
		{
			AddCommand(command, description, method.Method, method.Target, new string[3] { parameterName1, parameterName2, parameterName3 });
		}

		public static void AddCommand<T1, T2, T3, T4, T5>(string command, string description, Func<T1, T2, T3, T4, T5> method, string parameterName1, string parameterName2, string parameterName3, string parameterName4)
		{
			AddCommand(command, description, method.Method, method.Target, new string[4] { parameterName1, parameterName2, parameterName3, parameterName4 });
		}

		public static void AddCommand(string command, string description, Delegate method, params string[] parameterNames)
		{
			AddCommand(command, description, method.Method, method.Target, parameterNames);
		}

		private static void AddCommand(string command, string description, string methodName, Type ownerType, object instance, string[] parameterNames)
		{
			MethodInfo method = ownerType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | ((instance == null) ? BindingFlags.Static : BindingFlags.Instance));
			if (method == null)
			{
				Debug.LogError(methodName + " does not exist in " + ownerType);
			}
			else
			{
				AddCommand(command, description, method, instance, parameterNames);
			}
		}

		private static void AddCommand(string command, string description, MethodInfo method, object instance, string[] parameterNames)
		{
			if (string.IsNullOrEmpty(command))
			{
				Debug.LogError("Command name can't be empty!");
				return;
			}
			command = command.Trim();
			if (command.IndexOf(' ') >= 0)
			{
				Debug.LogError("Command name can't contain whitespace: " + command);
				return;
			}
			ParameterInfo[] array = method.GetParameters();
			if (array == null)
			{
				array = new ParameterInfo[0];
			}
			Type[] array2 = new Type[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].ParameterType.IsByRef)
				{
					Debug.LogError("Command can't have 'out' or 'ref' parameters");
					return;
				}
				Type parameterType = array[i].ParameterType;
				if (parseFunctions.ContainsKey(parameterType) || typeof(Component).IsAssignableFrom(parameterType) || parameterType.IsEnum || IsSupportedArrayType(parameterType))
				{
					array2[i] = parameterType;
					continue;
				}
				Debug.LogError(string.Concat("Parameter ", array[i].Name, "'s Type ", parameterType, " isn't supported"));
				return;
			}
			int num = FindCommandIndex(command);
			if (num < 0)
			{
				num = ~num;
			}
			else
			{
				int num2 = num;
				int j = num;
				while (num2 > 0 && caseInsensitiveComparer.Compare(methods[num2 - 1].command, command, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) == 0)
				{
					num2--;
				}
				for (; j < methods.Count - 1 && caseInsensitiveComparer.Compare(methods[j + 1].command, command, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) == 0; j++)
				{
				}
				num = num2;
				for (int k = num2; k <= j; k++)
				{
					int num3 = methods[k].parameterTypes.Length - array2.Length;
					if (num3 > 0)
					{
						continue;
					}
					num = k + 1;
					if (num3 == 0)
					{
						int l;
						for (l = 0; l < array2.Length && array2[l] == methods[k].parameterTypes[l]; l++)
						{
						}
						if (l >= array2.Length)
						{
							num = k;
							j--;
							methods.RemoveAt(k--);
						}
					}
				}
			}
			StringBuilder stringBuilder = new StringBuilder(256);
			string[] array3 = new string[array2.Length];
			stringBuilder.Append("<b>");
			stringBuilder.Append(command);
			if (array2.Length > 0)
			{
				stringBuilder.Append(" ");
				for (int m = 0; m < array2.Length; m++)
				{
					int length = stringBuilder.Length;
					stringBuilder.Append("[").Append(GetTypeReadableName(array2[m])).Append(" ")
						.Append((parameterNames == null || m >= parameterNames.Length || string.IsNullOrEmpty(parameterNames[m])) ? array[m].Name : parameterNames[m])
						.Append("]");
					if (m < array2.Length - 1)
					{
						stringBuilder.Append(" ");
					}
					array3[m] = stringBuilder.ToString(length, stringBuilder.Length - length);
				}
			}
			stringBuilder.Append("</b>");
			if (!string.IsNullOrEmpty(description))
			{
				stringBuilder.Append(": ").Append(description);
			}
			methods.Insert(num, new ConsoleMethodInfo(method, array2, instance, command, stringBuilder.ToString(), array3));
		}

		public static void RemoveCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				return;
			}
			for (int num = methods.Count - 1; num >= 0; num--)
			{
				if (caseInsensitiveComparer.Compare(methods[num].command, command, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) == 0)
				{
					methods.RemoveAt(num);
				}
			}
		}

		public static void RemoveCommand(Action method)
		{
			RemoveCommand(method.Method);
		}

		public static void RemoveCommand<T1>(Action<T1> method)
		{
			RemoveCommand(method.Method);
		}

		public static void RemoveCommand<T1>(Func<T1> method)
		{
			RemoveCommand(method.Method);
		}

		public static void RemoveCommand<T1, T2>(Action<T1, T2> method)
		{
			RemoveCommand(method.Method);
		}

		public static void RemoveCommand<T1, T2>(Func<T1, T2> method)
		{
			RemoveCommand(method.Method);
		}

		public static void RemoveCommand<T1, T2, T3>(Action<T1, T2, T3> method)
		{
			RemoveCommand(method.Method);
		}

		public static void RemoveCommand<T1, T2, T3>(Func<T1, T2, T3> method)
		{
			RemoveCommand(method.Method);
		}

		public static void RemoveCommand<T1, T2, T3, T4>(Action<T1, T2, T3, T4> method)
		{
			RemoveCommand(method.Method);
		}

		public static void RemoveCommand<T1, T2, T3, T4>(Func<T1, T2, T3, T4> method)
		{
			RemoveCommand(method.Method);
		}

		public static void RemoveCommand<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5> method)
		{
			RemoveCommand(method.Method);
		}

		public static void RemoveCommand(Delegate method)
		{
			RemoveCommand(method.Method);
		}

		public static void RemoveCommand(MethodInfo method)
		{
			if (method == null)
			{
				return;
			}
			for (int num = methods.Count - 1; num >= 0; num--)
			{
				if (methods[num].method == method)
				{
					methods.RemoveAt(num);
				}
			}
		}

		public static string GetAutoCompleteCommand(string commandStart, string previousSuggestion)
		{
			int num = FindCommandIndex(string.IsNullOrEmpty(previousSuggestion) ? commandStart : previousSuggestion);
			if (num < 0)
			{
				num = ~num;
				return (num >= methods.Count || !caseInsensitiveComparer.IsPrefix(methods[num].command, commandStart, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace)) ? null : methods[num].command;
			}
			for (int i = num + 1; i < methods.Count; i++)
			{
				if (caseInsensitiveComparer.Compare(methods[i].command, previousSuggestion, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) != 0)
				{
					if (caseInsensitiveComparer.IsPrefix(methods[i].command, commandStart, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace))
					{
						return methods[i].command;
					}
					break;
				}
			}
			string result = null;
			int num2 = num - 1;
			while (num2 >= 0 && caseInsensitiveComparer.IsPrefix(methods[num2].command, commandStart, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace))
			{
				result = methods[num2].command;
				num2--;
			}
			return result;
		}

		public static void ExecuteCommand(string command)
		{
			if (command == null)
			{
				return;
			}
			command = command.Trim();
			if (command.Length == 0)
			{
				return;
			}
			commandArguments.Clear();
			FetchArgumentsFromCommand(command, commandArguments);
			matchingMethods.Clear();
			bool flag = false;
			int num = FindCommandIndex(commandArguments[0]);
			if (num >= 0)
			{
				string @string = commandArguments[0];
				int i = num;
				while (num > 0 && caseInsensitiveComparer.Compare(methods[num - 1].command, @string, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) == 0)
				{
					num--;
				}
				for (; i < methods.Count - 1 && caseInsensitiveComparer.Compare(methods[i + 1].command, @string, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) == 0; i++)
				{
				}
				while (num <= i)
				{
					if (!methods[num].IsValid())
					{
						methods.RemoveAt(num);
						i--;
						continue;
					}
					if (methods[num].parameterTypes.Length == commandArguments.Count - 1)
					{
						matchingMethods.Add(methods[num]);
					}
					else
					{
						flag = true;
					}
					num++;
				}
			}
			if (matchingMethods.Count == 0)
			{
				string text = commandArguments[0];
				FindCommands(text, !flag, matchingMethods);
				if (matchingMethods.Count == 0)
				{
					Debug.LogWarning("ERROR: can't find command '" + text + "'");
					return;
				}
				int num2 = text.Length + 75;
				for (int j = 0; j < matchingMethods.Count; j++)
				{
					num2 += matchingMethods[j].signature.Length + 7;
				}
				StringBuilder stringBuilder = new StringBuilder(num2);
				if (flag)
				{
					stringBuilder.Append("ERROR: '").Append(text).Append("' doesn't take ")
						.Append(commandArguments.Count - 1)
						.Append(" parameter(s). Available command(s):");
				}
				else
				{
					stringBuilder.Append("ERROR: can't find command '").Append(text).Append("'. Did you mean:");
				}
				for (int k = 0; k < matchingMethods.Count; k++)
				{
					stringBuilder.Append("\n    - ").Append(matchingMethods[k].signature);
				}
				Debug.LogWarning(stringBuilder.ToString());
				if ((bool)DebugLogManager.Instance)
				{
					DebugLogManager.Instance.AdjustLatestPendingLog(true, true);
				}
				return;
			}
			ConsoleMethodInfo consoleMethodInfo = null;
			object[] array = new object[commandArguments.Count - 1];
			string text2 = null;
			for (int l = 0; l < matchingMethods.Count; l++)
			{
				if (consoleMethodInfo != null)
				{
					break;
				}
				ConsoleMethodInfo consoleMethodInfo2 = matchingMethods[l];
				bool flag2 = true;
				for (int m = 0; m < consoleMethodInfo2.parameterTypes.Length; m++)
				{
					if (!flag2)
					{
						break;
					}
					try
					{
						string text3 = commandArguments[m + 1];
						Type type = consoleMethodInfo2.parameterTypes[m];
						object output;
						if (ParseArgument(text3, type, out output))
						{
							array[m] = output;
							continue;
						}
						flag2 = false;
						text2 = "ERROR: couldn't parse " + text3 + " to " + GetTypeReadableName(type);
					}
					catch (Exception ex)
					{
						flag2 = false;
						text2 = "ERROR: " + ex.ToString();
					}
				}
				if (flag2)
				{
					consoleMethodInfo = consoleMethodInfo2;
				}
			}
			if (consoleMethodInfo == null)
			{
				Debug.LogWarning(string.IsNullOrEmpty(text2) ? "ERROR: something went wrong" : text2);
				return;
			}
			object obj = consoleMethodInfo.method.Invoke(consoleMethodInfo.instance, array);
			if (consoleMethodInfo.method.ReturnType != typeof(void))
			{
				if (obj == null || obj.Equals(null))
				{
					Debug.Log("Returned: null");
				}
				else
				{
					Debug.Log("Returned: " + obj.ToString());
				}
			}
		}

		public static void FetchArgumentsFromCommand(string command, List<string> commandArguments)
		{
			for (int i = 0; i < command.Length; i++)
			{
				if (!char.IsWhiteSpace(command[i]))
				{
					int num = IndexOfDelimiterGroup(command[i]);
					if (num >= 0)
					{
						int num2 = IndexOfDelimiterGroupEnd(command, num, i + 1);
						commandArguments.Add(command.Substring(i + 1, num2 - i - 1));
						i = ((num2 >= command.Length - 1 || command[num2 + 1] != ',') ? num2 : (num2 + 1));
					}
					else
					{
						int num3 = IndexOfChar(command, ' ', i + 1);
						commandArguments.Add(command.Substring(i, (command[num3 - 1] != ',') ? (num3 - i) : (num3 - 1 - i)));
						i = num3;
					}
				}
			}
		}

		public static void FindCommands(string commandName, bool allowSubstringMatching, List<ConsoleMethodInfo> matchingCommands)
		{
			if (allowSubstringMatching)
			{
				for (int i = 0; i < methods.Count; i++)
				{
					if (methods[i].IsValid() && caseInsensitiveComparer.IndexOf(methods[i].command, commandName, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0)
					{
						matchingCommands.Add(methods[i]);
					}
				}
				return;
			}
			for (int j = 0; j < methods.Count; j++)
			{
				if (methods[j].IsValid() && caseInsensitiveComparer.Compare(methods[j].command, commandName, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) == 0)
				{
					matchingCommands.Add(methods[j]);
				}
			}
		}

		internal static void GetCommandSuggestions(string command, List<ConsoleMethodInfo> matchingCommands, List<int> caretIndexIncrements, ref string commandName, out int numberOfParameters)
		{
			bool flag = false;
			bool flag2 = false;
			numberOfParameters = -1;
			for (int i = 0; i < command.Length; i++)
			{
				if (char.IsWhiteSpace(command[i]))
				{
					continue;
				}
				int num = IndexOfDelimiterGroup(command[i]);
				if (num >= 0)
				{
					int num2 = IndexOfDelimiterGroupEnd(command, num, i + 1);
					if (!flag)
					{
						flag = true;
						flag2 = command.Length > num2;
						int num3 = num2 - i - 1;
						if (commandName == null || num3 == 0 || commandName.Length != num3 || caseInsensitiveComparer.IndexOf(command, commandName, i + 1, num3, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) != i + 1)
						{
							commandName = command.Substring(i + 1, num3);
						}
					}
					i = ((num2 >= command.Length - 1 || command[num2 + 1] != ',') ? num2 : (num2 + 1));
					caretIndexIncrements.Add(i + 1);
				}
				else
				{
					int num4 = IndexOfChar(command, ' ', i + 1);
					if (!flag)
					{
						flag = true;
						flag2 = command.Length > num4;
						int num5 = ((command[num4 - 1] != ',') ? (num4 - i) : (num4 - 1 - i));
						if (commandName == null || num5 == 0 || commandName.Length != num5 || caseInsensitiveComparer.IndexOf(command, commandName, i, num5, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) != i)
						{
							commandName = command.Substring(i, num5);
						}
					}
					i = num4;
					caretIndexIncrements.Add(i);
				}
				numberOfParameters++;
			}
			if (!flag)
			{
				commandName = string.Empty;
			}
			if (string.IsNullOrEmpty(commandName))
			{
				return;
			}
			int j = FindCommandIndex(commandName);
			if (j < 0)
			{
				j = ~j;
			}
			int k = j;
			if (!flag2)
			{
				if (j < methods.Count && caseInsensitiveComparer.IsPrefix(methods[j].command, commandName, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace))
				{
					while (j > 0 && caseInsensitiveComparer.IsPrefix(methods[j - 1].command, commandName, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace))
					{
						j--;
					}
					for (; k < methods.Count - 1 && caseInsensitiveComparer.IsPrefix(methods[k + 1].command, commandName, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace); k++)
					{
					}
				}
				else
				{
					k = -1;
				}
			}
			else if (j < methods.Count && caseInsensitiveComparer.Compare(methods[j].command, commandName, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) == 0)
			{
				while (j > 0 && caseInsensitiveComparer.Compare(methods[j - 1].command, commandName, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) == 0)
				{
					j--;
				}
				for (; k < methods.Count - 1 && caseInsensitiveComparer.Compare(methods[k + 1].command, commandName, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) == 0; k++)
				{
				}
			}
			else
			{
				k = -1;
			}
			for (; j <= k; j++)
			{
				if (methods[j].parameterTypes.Length >= numberOfParameters)
				{
					matchingCommands.Add(methods[j]);
				}
			}
		}

		private static int IndexOfDelimiterGroup(char c)
		{
			for (int i = 0; i < inputDelimiters.Length; i++)
			{
				if (c == inputDelimiters[i][0])
				{
					return i;
				}
			}
			return -1;
		}

		private static int IndexOfDelimiterGroupEnd(string command, int delimiterIndex, int startIndex)
		{
			char c = inputDelimiters[delimiterIndex][0];
			char c2 = inputDelimiters[delimiterIndex][1];
			int num = 1;
			for (int i = startIndex; i < command.Length; i++)
			{
				char c3 = command[i];
				if (c3 == c2 && --num <= 0)
				{
					return i;
				}
				if (c3 == c)
				{
					num++;
				}
			}
			return command.Length;
		}

		private static int IndexOfChar(string command, char c, int startIndex)
		{
			int num = command.IndexOf(c, startIndex);
			if (num < 0)
			{
				num = command.Length;
			}
			return num;
		}

		private static int FindCommandIndex(string command)
		{
			int num = 0;
			int num2 = methods.Count - 1;
			while (num <= num2)
			{
				int num3 = (num + num2) / 2;
				int num4 = caseInsensitiveComparer.Compare(command, methods[num3].command, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);
				if (num4 == 0)
				{
					return num3;
				}
				if (num4 < 0)
				{
					num2 = num3 - 1;
				}
				else
				{
					num = num3 + 1;
				}
			}
			return ~num;
		}

		public static bool IsSupportedArrayType(Type type)
		{
			if (type.IsArray)
			{
				if (type.GetArrayRank() != 1)
				{
					return false;
				}
				type = type.GetElementType();
			}
			else
			{
				if (!type.IsGenericType)
				{
					return false;
				}
				if (type.GetGenericTypeDefinition() != typeof(List<>))
				{
					return false;
				}
				type = type.GetGenericArguments()[0];
			}
			return parseFunctions.ContainsKey(type) || typeof(Component).IsAssignableFrom(type) || type.IsEnum;
		}

		public static string GetTypeReadableName(Type type)
		{
			string value;
			if (typeReadableNames.TryGetValue(type, out value))
			{
				return value;
			}
			if (IsSupportedArrayType(type))
			{
				Type type2 = ((!type.IsArray) ? type.GetGenericArguments()[0] : type.GetElementType());
				if (typeReadableNames.TryGetValue(type2, out value))
				{
					return value + "[]";
				}
				return type2.Name + "[]";
			}
			return type.Name;
		}

		public static bool ParseArgument(string input, Type argumentType, out object output)
		{
			ParseFunction value;
			if (parseFunctions.TryGetValue(argumentType, out value))
			{
				return value(input, out output);
			}
			if (typeof(Component).IsAssignableFrom(argumentType))
			{
				return ParseComponent(input, argumentType, out output);
			}
			if (argumentType.IsEnum)
			{
				return ParseEnum(input, argumentType, out output);
			}
			if (IsSupportedArrayType(argumentType))
			{
				return ParseArray(input, argumentType, out output);
			}
			output = null;
			return false;
		}

		public static bool ParseString(string input, out object output)
		{
			output = input;
			return true;
		}

		public static bool ParseBool(string input, out object output)
		{
			if (input == "1" || input.ToLowerInvariant() == "true")
			{
				output = true;
				return true;
			}
			if (input == "0" || input.ToLowerInvariant() == "false")
			{
				output = false;
				return true;
			}
			output = false;
			return false;
		}

		public static bool ParseInt(string input, out object output)
		{
			int result;
			bool result2 = int.TryParse(input, out result);
			output = result;
			return result2;
		}

		public static bool ParseUInt(string input, out object output)
		{
			uint result;
			bool result2 = uint.TryParse(input, out result);
			output = result;
			return result2;
		}

		public static bool ParseLong(string input, out object output)
		{
			long result;
			bool result2 = long.TryParse(input.EndsWith("L", StringComparison.OrdinalIgnoreCase) ? input.Substring(0, input.Length - 1) : input, out result);
			output = result;
			return result2;
		}

		public static bool ParseULong(string input, out object output)
		{
			ulong result;
			bool result2 = ulong.TryParse(input.EndsWith("L", StringComparison.OrdinalIgnoreCase) ? input.Substring(0, input.Length - 1) : input, out result);
			output = result;
			return result2;
		}

		public static bool ParseByte(string input, out object output)
		{
			byte result;
			bool result2 = byte.TryParse(input, out result);
			output = result;
			return result2;
		}

		public static bool ParseSByte(string input, out object output)
		{
			sbyte result;
			bool result2 = sbyte.TryParse(input, out result);
			output = result;
			return result2;
		}

		public static bool ParseShort(string input, out object output)
		{
			short result;
			bool result2 = short.TryParse(input, out result);
			output = result;
			return result2;
		}

		public static bool ParseUShort(string input, out object output)
		{
			ushort result;
			bool result2 = ushort.TryParse(input, out result);
			output = result;
			return result2;
		}

		public static bool ParseChar(string input, out object output)
		{
			char result;
			bool result2 = char.TryParse(input, out result);
			output = result;
			return result2;
		}

		public static bool ParseFloat(string input, out object output)
		{
			float result;
			bool result2 = float.TryParse(input.EndsWith("f", StringComparison.OrdinalIgnoreCase) ? input.Substring(0, input.Length - 1) : input, out result);
			output = result;
			return result2;
		}

		public static bool ParseDouble(string input, out object output)
		{
			double result;
			bool result2 = double.TryParse(input.EndsWith("f", StringComparison.OrdinalIgnoreCase) ? input.Substring(0, input.Length - 1) : input, out result);
			output = result;
			return result2;
		}

		public static bool ParseDecimal(string input, out object output)
		{
			decimal result;
			bool result2 = decimal.TryParse(input.EndsWith("f", StringComparison.OrdinalIgnoreCase) ? input.Substring(0, input.Length - 1) : input, out result);
			output = result;
			return result2;
		}

		public static bool ParseVector2(string input, out object output)
		{
			return ParseVector(input, typeof(Vector2), out output);
		}

		public static bool ParseVector3(string input, out object output)
		{
			return ParseVector(input, typeof(Vector3), out output);
		}

		public static bool ParseVector4(string input, out object output)
		{
			return ParseVector(input, typeof(Vector4), out output);
		}

		public static bool ParseQuaternion(string input, out object output)
		{
			return ParseVector(input, typeof(Quaternion), out output);
		}

		public static bool ParseColor(string input, out object output)
		{
			return ParseVector(input, typeof(Color), out output);
		}

		public static bool ParseColor32(string input, out object output)
		{
			return ParseVector(input, typeof(Color32), out output);
		}

		public static bool ParseRect(string input, out object output)
		{
			return ParseVector(input, typeof(Rect), out output);
		}

		public static bool ParseRectOffset(string input, out object output)
		{
			return ParseVector(input, typeof(RectOffset), out output);
		}

		public static bool ParseBounds(string input, out object output)
		{
			return ParseVector(input, typeof(Bounds), out output);
		}

		public static bool ParseVector2Int(string input, out object output)
		{
			return ParseVector(input, typeof(Vector2Int), out output);
		}

		public static bool ParseVector3Int(string input, out object output)
		{
			return ParseVector(input, typeof(Vector3Int), out output);
		}

		public static bool ParseRectInt(string input, out object output)
		{
			return ParseVector(input, typeof(RectInt), out output);
		}

		public static bool ParseBoundsInt(string input, out object output)
		{
			return ParseVector(input, typeof(BoundsInt), out output);
		}

		public static bool ParseGameObject(string input, out object output)
		{
			output = ((!(input == "null")) ? GameObject.Find(input) : null);
			return true;
		}

		public static bool ParseComponent(string input, Type componentType, out object output)
		{
			GameObject gameObject = ((!(input == "null")) ? GameObject.Find(input) : null);
			output = ((!gameObject) ? null : gameObject.GetComponent(componentType));
			return true;
		}

		public static bool ParseEnum(string input, Type enumType, out object output)
		{
			int num = 0;
			int num2 = 0;
			int num3;
			for (num3 = 0; num3 < input.Length; num3++)
			{
				int num4 = input.IndexOf('|', num3);
				int num5 = input.IndexOf('&', num3);
				string text = ((num4 >= 0) ? input.Substring(num3, ((num5 >= 0) ? Mathf.Min(num5, num4) : num4) - num3).Trim() : input.Substring(num3, ((num5 >= 0) ? num5 : input.Length) - num3).Trim());
				int result;
				if (!int.TryParse(text, out result))
				{
					try
					{
						result = Convert.ToInt32(Enum.Parse(enumType, text, true));
					}
					catch
					{
						output = null;
						return false;
					}
				}
				switch (num2)
				{
				case 0:
					num = result;
					break;
				case 1:
					num |= result;
					break;
				default:
					num &= result;
					break;
				}
				if (num4 >= 0)
				{
					if (num5 > num4)
					{
						num2 = 2;
						num3 = num5;
					}
					else
					{
						num2 = 1;
						num3 = num4;
					}
				}
				else if (num5 >= 0)
				{
					num2 = 2;
					num3 = num5;
				}
				else
				{
					num3 = input.Length;
				}
			}
			output = Enum.ToObject(enumType, num);
			return true;
		}

		public static bool ParseArray(string input, Type arrayType, out object output)
		{
			List<string> list = new List<string>(2);
			FetchArgumentsFromCommand(input, list);
			IList list2 = (IList)(output = (IList)Activator.CreateInstance(arrayType, list.Count));
			if (arrayType.IsArray)
			{
				Type elementType = arrayType.GetElementType();
				for (int i = 0; i < list.Count; i++)
				{
					object output2;
					if (!ParseArgument(list[i], elementType, out output2))
					{
						return false;
					}
					list2[i] = output2;
				}
			}
			else
			{
				Type argumentType = arrayType.GetGenericArguments()[0];
				for (int j = 0; j < list.Count; j++)
				{
					object output3;
					if (!ParseArgument(list[j], argumentType, out output3))
					{
						return false;
					}
					list2.Add(output3);
				}
			}
			return true;
		}

		private static bool ParseVector(string input, Type vectorType, out object output)
		{
			List<string> list = new List<string>(input.Replace(',', ' ').Trim().Split(' '));
			for (int num = list.Count - 1; num >= 0; num--)
			{
				list[num] = list[num].Trim();
				if (list[num].Length == 0)
				{
					list.RemoveAt(num);
				}
			}
			float[] array = new float[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				object output2;
				if (!ParseFloat(list[i], out output2))
				{
					if (vectorType == typeof(Vector3))
					{
						output = Vector3.zero;
					}
					else if (vectorType == typeof(Vector2))
					{
						output = Vector2.zero;
					}
					else
					{
						output = Vector4.zero;
					}
					return false;
				}
				array[i] = (float)output2;
			}
			if (vectorType == typeof(Vector3))
			{
				Vector3 zero = Vector3.zero;
				for (int j = 0; j < array.Length && j < 3; j++)
				{
					zero[j] = array[j];
				}
				output = zero;
			}
			else if (vectorType == typeof(Vector2))
			{
				Vector2 zero2 = Vector2.zero;
				for (int k = 0; k < array.Length && k < 2; k++)
				{
					zero2[k] = array[k];
				}
				output = zero2;
			}
			else if (vectorType == typeof(Vector4))
			{
				Vector4 zero3 = Vector4.zero;
				for (int l = 0; l < array.Length && l < 4; l++)
				{
					zero3[l] = array[l];
				}
				output = zero3;
			}
			else if (vectorType == typeof(Quaternion))
			{
				Quaternion identity = Quaternion.identity;
				for (int m = 0; m < array.Length && m < 4; m++)
				{
					identity[m] = array[m];
				}
				output = identity;
			}
			else if (vectorType == typeof(Color))
			{
				Color black = Color.black;
				for (int n = 0; n < array.Length && n < 4; n++)
				{
					black[n] = array[n];
				}
				output = black;
			}
			else if (vectorType == typeof(Color32))
			{
				Color32 color = new Color32(0, 0, 0, byte.MaxValue);
				if (array.Length > 0)
				{
					color.r = (byte)Mathf.RoundToInt(array[0]);
				}
				if (array.Length > 1)
				{
					color.g = (byte)Mathf.RoundToInt(array[1]);
				}
				if (array.Length > 2)
				{
					color.b = (byte)Mathf.RoundToInt(array[2]);
				}
				if (array.Length > 3)
				{
					color.a = (byte)Mathf.RoundToInt(array[3]);
				}
				output = color;
			}
			else if (vectorType == typeof(Rect))
			{
				Rect zero4 = Rect.zero;
				if (array.Length > 0)
				{
					zero4.x = array[0];
				}
				if (array.Length > 1)
				{
					zero4.y = array[1];
				}
				if (array.Length > 2)
				{
					zero4.width = array[2];
				}
				if (array.Length > 3)
				{
					zero4.height = array[3];
				}
				output = zero4;
			}
			else if (vectorType == typeof(RectOffset))
			{
				RectOffset rectOffset = new RectOffset();
				if (array.Length > 0)
				{
					rectOffset.left = Mathf.RoundToInt(array[0]);
				}
				if (array.Length > 1)
				{
					rectOffset.right = Mathf.RoundToInt(array[1]);
				}
				if (array.Length > 2)
				{
					rectOffset.top = Mathf.RoundToInt(array[2]);
				}
				if (array.Length > 3)
				{
					rectOffset.bottom = Mathf.RoundToInt(array[3]);
				}
				output = rectOffset;
			}
			else if (vectorType == typeof(Bounds))
			{
				Vector3 zero5 = Vector3.zero;
				for (int num2 = 0; num2 < array.Length && num2 < 3; num2++)
				{
					zero5[num2] = array[num2];
				}
				Vector3 zero6 = Vector3.zero;
				for (int num3 = 3; num3 < array.Length && num3 < 6; num3++)
				{
					zero6[num3 - 3] = array[num3];
				}
				output = new Bounds(zero5, zero6);
			}
			else if (vectorType == typeof(Vector3Int))
			{
				Vector3Int zero7 = Vector3Int.zero;
				for (int num4 = 0; num4 < array.Length && num4 < 3; num4++)
				{
					zero7[num4] = Mathf.RoundToInt(array[num4]);
				}
				output = zero7;
			}
			else if (vectorType == typeof(Vector2Int))
			{
				Vector2Int zero8 = Vector2Int.zero;
				for (int num5 = 0; num5 < array.Length && num5 < 2; num5++)
				{
					zero8[num5] = Mathf.RoundToInt(array[num5]);
				}
				output = zero8;
			}
			else if (vectorType == typeof(RectInt))
			{
				RectInt rectInt = default(RectInt);
				if (array.Length > 0)
				{
					rectInt.x = Mathf.RoundToInt(array[0]);
				}
				if (array.Length > 1)
				{
					rectInt.y = Mathf.RoundToInt(array[1]);
				}
				if (array.Length > 2)
				{
					rectInt.width = Mathf.RoundToInt(array[2]);
				}
				if (array.Length > 3)
				{
					rectInt.height = Mathf.RoundToInt(array[3]);
				}
				output = rectInt;
			}
			else
			{
				if (vectorType != typeof(BoundsInt))
				{
					output = null;
					return false;
				}
				Vector3Int zero9 = Vector3Int.zero;
				for (int num6 = 0; num6 < array.Length && num6 < 3; num6++)
				{
					zero9[num6] = Mathf.RoundToInt(array[num6]);
				}
				Vector3Int zero10 = Vector3Int.zero;
				for (int num7 = 3; num7 < array.Length && num7 < 6; num7++)
				{
					zero10[num7 - 3] = Mathf.RoundToInt(array[num7]);
				}
				output = new BoundsInt(zero9, zero10);
			}
			return true;
		}
	}
}
