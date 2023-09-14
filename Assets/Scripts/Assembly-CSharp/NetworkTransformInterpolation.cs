using System;
using UnityEngine;

public class NetworkTransformInterpolation : MonoBehaviour
{
	public enum InterpolationMode
	{
		INTERPOLATION = 0,
		EXTRAPOLATION = 1
	}

	public InterpolationMode mode;

	private double interpolationBackTime = 200.0;

	private float extrapolationForwardTime = 1000f;

	private bool running;

	private NetworkTransform[] bufferedStates = new NetworkTransform[20];

	private int statesCount;

	public void StartReceiving()
	{
		running = true;
	}

	public void StopReceiving()
	{
		running = false;
	}

	public void ReceivedTransform(NetworkTransform ntransform)
	{
		if (!running)
		{
			return;
		}
		for (int num = bufferedStates.Length - 1; num >= 1; num--)
		{
			bufferedStates[num] = bufferedStates[num - 1];
		}
		bufferedStates[0] = ntransform;
		statesCount = Mathf.Min(statesCount + 1, bufferedStates.Length);
		for (int i = 0; i < statesCount - 1; i++)
		{
			if (bufferedStates[i].TimeStamp < bufferedStates[i + 1].TimeStamp)
			{
				Debug.Log("State inconsistent");
			}
		}
	}

	private void Update()
	{
		if (!running || statesCount == 0 || TimeManager.Instance == null)
		{
			return;
		}
		UpdateValues();
		double networkTime = TimeManager.Instance.NetworkTime;
		double num = networkTime - interpolationBackTime;
		if (mode == InterpolationMode.INTERPOLATION && bufferedStates[0].TimeStamp > num)
		{
			for (int i = 0; i < statesCount; i++)
			{
				if (bufferedStates[i].TimeStamp <= num || i == statesCount - 1)
				{
					NetworkTransform networkTransform = bufferedStates[Mathf.Max(i - 1, 0)];
					NetworkTransform networkTransform2 = bufferedStates[i];
					double num2 = networkTransform.TimeStamp - networkTransform2.TimeStamp;
					float t = 0f;
					if (num2 > 0.0001)
					{
						t = (float)((num - networkTransform2.TimeStamp) / num2);
					}
					base.transform.position = Vector3.Lerp(networkTransform2.Position, networkTransform.Position, t);
					base.transform.rotation = Quaternion.Slerp(networkTransform2.Rotation, networkTransform.Rotation, t);
					break;
				}
			}
			return;
		}
		float num3 = Convert.ToSingle(networkTime - bufferedStates[0].TimeStamp) / 1000f;
		if (mode == InterpolationMode.EXTRAPOLATION && num3 < extrapolationForwardTime && statesCount > 1)
		{
			Vector3 vector = bufferedStates[0].Position - bufferedStates[1].Position;
			float num4 = Vector3.Distance(bufferedStates[0].Position, bufferedStates[1].Position);
			float num5 = Convert.ToSingle(bufferedStates[0].TimeStamp - bufferedStates[1].TimeStamp) / 1000f;
			if (Mathf.Approximately(num4, 0f) || Mathf.Approximately(num5, 0f))
			{
				base.transform.position = bufferedStates[0].Position;
				base.transform.rotation = bufferedStates[0].Rotation;
				return;
			}
			float num6 = num4 / num5;
			vector = vector.normalized;
			Vector3 b = bufferedStates[0].Position + vector * num3 * num6;
			base.transform.position = Vector3.Lerp(base.transform.position, b, Time.deltaTime * num6);
		}
		else
		{
			base.transform.position = bufferedStates[0].Position;
		}
		base.transform.rotation = bufferedStates[0].Rotation;
	}

	private void UpdateValues()
	{
		double averagePing = TimeManager.Instance.AveragePing;
		if (averagePing < 50.0)
		{
			interpolationBackTime = 50.0;
		}
		else if (averagePing < 100.0)
		{
			interpolationBackTime = 100.0;
		}
		else if (averagePing < 200.0)
		{
			interpolationBackTime = 200.0;
		}
		else if (averagePing < 400.0)
		{
			interpolationBackTime = 400.0;
		}
		else if (averagePing < 600.0)
		{
			interpolationBackTime = 600.0;
		}
		else
		{
			interpolationBackTime = 1000.0;
		}
		interpolationBackTime += 300.0;
	}
}
