using Sfs2X.Entities.Data;

namespace Sfs2X.Entities.Invitation
{
	public class SFSInvitation : Invitation
	{
		protected int id;

		protected User inviter;

		protected User invitee;

		protected int secondsForAnswer;

		protected ISFSObject parameters;

		public int Id
		{
			set
			{
				id = value;
			}
		}

		public SFSInvitation(User inviter, User invitee, int secondsForAnswer, ISFSObject parameters)
		{
			Init(inviter, invitee, secondsForAnswer, parameters);
		}

		private void Init(User inviter, User invitee, int secondsForAnswer, ISFSObject parameters)
		{
			this.inviter = inviter;
			this.invitee = invitee;
			this.secondsForAnswer = secondsForAnswer;
			this.parameters = parameters;
		}
	}
}
