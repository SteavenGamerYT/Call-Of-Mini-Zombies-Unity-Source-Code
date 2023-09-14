using System;
using Sfs2X.Entities.Data;
using UnityEngine;

public class NetworkTransform
{
	private Vector3 position;

	private Vector3 angleRotation;

	private double timeStamp;

	public Vector3 Position
	{
		get
		{
			return position;
		}
	}

	public Vector3 AngleRotation
	{
		get
		{
			return angleRotation;
		}
	}

	public Vector3 AngleRotationFPS
	{
		get
		{
			return new Vector3(0f, angleRotation.y, 0f);
		}
	}

	public Quaternion Rotation
	{
		get
		{
			return Quaternion.Euler(AngleRotationFPS);
		}
	}

	public double TimeStamp
	{
		get
		{
			return timeStamp;
		}
		set
		{
			timeStamp = value;
		}
	}

	private NetworkTransform()
	{
	}

	public bool IsDifferent(Transform transform, float accuracy)
	{
		float num = Vector3.Distance(position, transform.position);
		float num2 = Vector3.Distance(AngleRotation, transform.localEulerAngles);
		return num > accuracy || num2 > accuracy;
	}

	public void ToSFSObject(ISFSObject data)
	{
		ISFSObject iSFSObject = new SFSObject();
		iSFSObject.PutDouble("x", Convert.ToDouble(position.x));
		iSFSObject.PutDouble("y", Convert.ToDouble(position.y));
		iSFSObject.PutDouble("z", Convert.ToDouble(position.z));
		iSFSObject.PutDouble("rx", Convert.ToDouble(angleRotation.x));
		iSFSObject.PutDouble("ry", Convert.ToDouble(angleRotation.y));
		iSFSObject.PutDouble("rz", Convert.ToDouble(angleRotation.z));
		iSFSObject.PutLong("t", Convert.ToInt64(timeStamp));
		data.PutSFSObject("transform", iSFSObject);
	}

	public void Load(NetworkTransform ntransform)
	{
		position = ntransform.position;
		angleRotation = ntransform.angleRotation;
		timeStamp = ntransform.timeStamp;
	}

	public void Update(Transform trans)
	{
		trans.position = Position;
		trans.localEulerAngles = AngleRotation;
	}

	public static NetworkTransform FromSFSObject(ISFSObject data)
	{
		NetworkTransform networkTransform = new NetworkTransform();
		ISFSObject sFSObject = data.GetSFSObject("transform");
		float x = Convert.ToSingle(sFSObject.GetDouble("x"));
		float y = Convert.ToSingle(sFSObject.GetDouble("y"));
		float z = Convert.ToSingle(sFSObject.GetDouble("z"));
		float x2 = Convert.ToSingle(sFSObject.GetDouble("rx"));
		float y2 = Convert.ToSingle(sFSObject.GetDouble("ry"));
		float z2 = Convert.ToSingle(sFSObject.GetDouble("rz"));
		networkTransform.position = new Vector3(x, y, z);
		networkTransform.angleRotation = new Vector3(x2, y2, z2);
		if (sFSObject.ContainsKey("t"))
		{
			networkTransform.TimeStamp = Convert.ToDouble(sFSObject.GetLong("t"));
		}
		else
		{
			networkTransform.TimeStamp = 0.0;
		}
		return networkTransform;
	}

	public static NetworkTransform FromTransform(Transform transform)
	{
		NetworkTransform networkTransform = new NetworkTransform();
		networkTransform.position = transform.position;
		networkTransform.angleRotation = transform.localEulerAngles;
		return networkTransform;
	}

	public static NetworkTransform Clone(NetworkTransform ntransform)
	{
		NetworkTransform networkTransform = new NetworkTransform();
		networkTransform.Load(ntransform);
		return networkTransform;
	}
}
