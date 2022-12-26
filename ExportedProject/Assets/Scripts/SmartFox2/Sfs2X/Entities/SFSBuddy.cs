using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;

namespace Sfs2X.Entities
{
	public class SFSBuddy : Buddy
	{
		protected string name;

		protected int id;

		protected bool isBlocked;

		protected Dictionary<string, BuddyVariable> variables = new Dictionary<string, BuddyVariable>();

		protected bool isTemp;

		public int Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		public bool IsBlocked
		{
			get
			{
				return isBlocked;
			}
			set
			{
				isBlocked = value;
			}
		}

		public bool IsTemp
		{
			get
			{
				return isTemp;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public SFSBuddy(int id, string name, bool isBlocked, bool isTemp)
		{
			this.id = id;
			this.name = name;
			this.isBlocked = isBlocked;
			variables = new Dictionary<string, BuddyVariable>();
			this.isTemp = isTemp;
		}

		public static Buddy FromSFSArray(ISFSArray arr)
		{
			Buddy buddy = new SFSBuddy(arr.GetInt(0), arr.GetUtfString(1), arr.GetBool(2), arr.Size() > 4 && arr.GetBool(4));
			ISFSArray sFSArray = arr.GetSFSArray(3);
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				BuddyVariable variable = SFSBuddyVariable.FromSFSArray(sFSArray.GetSFSArray(i));
				buddy.SetVariable(variable);
			}
			return buddy;
		}

		public void SetVariable(BuddyVariable bVar)
		{
			variables[bVar.Name] = bVar;
		}

		public void SetVariables(ICollection<BuddyVariable> variables)
		{
			foreach (BuddyVariable variable in variables)
			{
				SetVariable(variable);
			}
		}

		public void RemoveVariable(string varName)
		{
			variables.Remove(varName);
		}

		public void ClearVolatileVariables()
		{
			foreach (BuddyVariable value in variables.Values)
			{
				if (value.Name[0] != Convert.ToChar(SFSBuddyVariable.OFFLINE_PREFIX))
				{
					RemoveVariable(value.Name);
				}
			}
		}

		public override string ToString()
		{
			return "[Buddy: " + name + ", id: " + id + "]";
		}
	}
}
