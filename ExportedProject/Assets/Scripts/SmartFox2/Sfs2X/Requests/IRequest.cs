using Sfs2X.Bitswarm;

namespace Sfs2X.Requests
{
	public interface IRequest
	{
		IMessage Message { get; }

		void Validate(SmartFox sfs);

		void Execute(SmartFox sfs);
	}
}
