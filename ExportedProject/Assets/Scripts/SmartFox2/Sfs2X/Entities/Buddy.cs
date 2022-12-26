using System.Collections.Generic;
using Sfs2X.Entities.Variables;

namespace Sfs2X.Entities
{
	public interface Buddy
	{
		int Id { get; set; }

		string Name { get; }

		bool IsBlocked { get; set; }

		bool IsTemp { get; }

		void SetVariable(BuddyVariable bVar);

		void SetVariables(ICollection<BuddyVariable> variables);

		void RemoveVariable(string varName);

		void ClearVolatileVariables();
	}
}
