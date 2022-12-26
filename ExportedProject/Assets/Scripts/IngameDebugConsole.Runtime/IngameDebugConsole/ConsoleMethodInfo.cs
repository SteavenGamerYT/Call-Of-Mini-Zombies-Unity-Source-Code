using System;
using System.Reflection;

namespace IngameDebugConsole
{
	public class ConsoleMethodInfo
	{
		public readonly MethodInfo method;

		public readonly Type[] parameterTypes;

		public readonly object instance;

		public readonly string command;

		public readonly string signature;

		public readonly string[] parameters;

		public ConsoleMethodInfo(MethodInfo method, Type[] parameterTypes, object instance, string command, string signature, string[] parameters)
		{
			this.method = method;
			this.parameterTypes = parameterTypes;
			this.instance = instance;
			this.command = command;
			this.signature = signature;
			this.parameters = parameters;
		}

		public bool IsValid()
		{
			if (!method.IsStatic && (instance == null || instance.Equals(null)))
			{
				return false;
			}
			return true;
		}
	}
}
