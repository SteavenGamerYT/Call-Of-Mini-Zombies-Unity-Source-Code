using System.Collections.Generic;
using Sfs2X.Entities.Variables;

namespace Sfs2X.Entities.Managers
{
	public interface IBuddyManager
	{
		bool Inited { get; set; }

		List<Buddy> BuddyList { get; }

		List<string> BuddyStates { set; }

		List<BuddyVariable> MyVariables { get; set; }

		bool MyOnlineState { get; set; }

		void AddBuddy(Buddy buddy);

		Buddy RemoveBuddyByName(string name);

		bool ContainsBuddy(string name);

		Buddy GetBuddyById(int id);

		Buddy GetBuddyByName(string name);

		BuddyVariable GetMyVariable(string varName);

		void SetMyVariable(BuddyVariable bVar);

		void ClearAll();
	}
}
