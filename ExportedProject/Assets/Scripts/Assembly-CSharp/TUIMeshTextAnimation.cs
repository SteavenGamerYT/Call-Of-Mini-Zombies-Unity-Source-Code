using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class TUIMeshTextAnimation : TUIMeshText
{
	public Color color_start = new Color32(123, 83, 12, byte.MaxValue);

	public Color color_end = new Color32(247, 219, 57, byte.MaxValue);

	public float rate = 10f;

	protected float lastFrameChangeTime;

	public bool isLoop;

	protected bool is_Start = true;

	public bool is_animate;

	public override void UpdateMesh()
	{
		if (is_animate && Time.time - lastFrameChangeTime > 1f / rate)
		{
			is_Start = !is_Start;
			if (is_Start)
			{
				color = color_start;
			}
			else
			{
				color = color_end;
			}
			lastFrameChangeTime = Time.time;
		}
		base.UpdateMesh();
	}
}
