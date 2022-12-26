using UnityEngine;

public class TUIMoveCallBack : TUIMove
{
	public override bool HandleInput(TUIInput input)
	{
		if (input.inputType == TUIInputType.Began)
		{
			if (PtInControl(input.position))
			{
				if (move)
				{
					fingerId = -1;
					position = Vector2.zero;
					move = false;
					PostEvent(this, 3, 0f, 0f, null);
				}
				fingerId = input.fingerId;
				position = input.position;
				move = false;
			}
			return false;
		}
		if (input.fingerId != fingerId)
		{
			return false;
		}
		if (input.inputType == TUIInputType.Moved)
		{
			if (!PtInControl(input.position))
			{
				return false;
			}
			float num = input.position.x - position.x;
			float num2 = input.position.y - position.y;
			if (move)
			{
				position = input.position;
				PostEvent(this, 2, num, num2, null);
				return true;
			}
			float num3 = ((num >= 0f) ? num : (0f - num));
			float num4 = ((num2 >= 0f) ? num2 : (0f - num2));
			if (num3 > minX || num4 > minY)
			{
				position = input.position;
				move = true;
				PostEvent(this, 1, 0f, 0f, null);
				PostEvent(this, 2, num, num2, null);
				base.gameObject.SendMessage("OnTUIMoveBegin", input, SendMessageOptions.DontRequireReceiver);
				return true;
			}
			return false;
		}
		if (input.inputType == TUIInputType.Ended)
		{
			bool flag = move;
			fingerId = -1;
			position = Vector2.zero;
			move = false;
			base.gameObject.SendMessage("OnTUIMoveEnd", input, SendMessageOptions.DontRequireReceiver);
			if (flag)
			{
				PostEvent(this, 3, 0f, 0f, null);
				return true;
			}
			return false;
		}
		return false;
	}
}
