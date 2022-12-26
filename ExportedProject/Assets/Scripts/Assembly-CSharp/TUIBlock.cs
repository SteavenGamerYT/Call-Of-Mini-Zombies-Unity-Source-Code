public class TUIBlock : TUIControlImpl
{
	public override bool HandleInput(TUIInput input)
	{
		if (PtInControl(input.position))
		{
			return true;
		}
		return false;
	}
}
