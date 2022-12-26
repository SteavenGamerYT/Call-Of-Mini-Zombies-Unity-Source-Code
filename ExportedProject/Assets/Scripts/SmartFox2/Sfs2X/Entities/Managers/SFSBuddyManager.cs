using System.Collections.Generic;
using Sfs2X.Entities.Variables;

namespace Sfs2X.Entities.Managers
{
	public class SFSBuddyManager : IBuddyManager
	{
		protected Dictionary<string, Buddy> buddiesByName;

		protected Dictionary<string, BuddyVariable> myVariables;

		protected bool myOnlineState;

		protected bool inited;

		private List<string> buddyStates;

		private SmartFox sfs;

		public bool Inited
		{
			get
			{
				return inited;
			}
			set
			{
				inited = value;
			}
		}

		public List<Buddy> BuddyList
		{
			get
			{
				return new List<Buddy>(buddiesByName.Values);
			}
		}

		public List<BuddyVariable> MyVariables
		{
			get
			{
				return new List<BuddyVariable>(myVariables.Values);
			}
			set
			{
				foreach (BuddyVariable item in value)
				{
					SetMyVariable(item);
				}
			}
		}

		public bool MyOnlineState
		{
			get
			{
				if (!inited)
				{
					return false;
				}
				bool result = true;
				BuddyVariable myVariable = GetMyVariable(ReservedBuddyVariables.BV_ONLINE);
				if (myVariable != null)
				{
					result = myVariable.GetBoolValue();
				}
				return result;
			}
			set
			{
				SetMyVariable(new SFSBuddyVariable(ReservedBuddyVariables.BV_ONLINE, value));
			}
		}

		public List<string> BuddyStates
		{
			set
			{
				buddyStates = value;
			}
		}

		public SFSBuddyManager(SmartFox sfs)
		{
			this.sfs = sfs;
			buddiesByName = new Dictionary<string, Buddy>();
			myVariables = new Dictionary<string, BuddyVariable>();
			inited = false;
		}

		public void AddBuddy(Buddy buddy)
		{
			buddiesByName.Add(buddy.Name, buddy);
		}

		public void ClearAll()
		{
			buddiesByName.Clear();
		}

		public Buddy RemoveBuddyByName(string name)
		{
			Buddy buddyByName = GetBuddyByName(name);
			if (buddyByName != null)
			{
				buddiesByName.Remove(name);
			}
			return buddyByName;
		}

		public Buddy GetBuddyById(int id)
		{
			if (id > -1)
			{
				foreach (Buddy value in buddiesByName.Values)
				{
					if (value.Id == id)
					{
						return value;
					}
				}
			}
			return null;
		}

		public bool ContainsBuddy(string name)
		{
			return buddiesByName.ContainsKey(name);
		}

		public Buddy GetBuddyByName(string name)
		{
			if (buddiesByName.ContainsKey(name))
			{
				return buddiesByName[name];
			}
			return null;
		}

		public BuddyVariable GetMyVariable(string varName)
		{
			if (myVariables.ContainsKey(varName))
			{
				return myVariables[varName];
			}
			return null;
		}

		public void SetMyVariable(BuddyVariable bVar)
		{
			myVariables[bVar.Name] = bVar;
		}
	}
}
