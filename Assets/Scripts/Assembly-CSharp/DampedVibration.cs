using System;
using UnityEngine;

public class DampedVibration
{
	private float e;

	private float m_A;

	private float m_beta;

	private float m_omega;

	private float m_alpha;

	public void SetParameter(float A, float beta, float omega, float alpha)
	{
		m_A = A;
		m_beta = beta;
		m_omega = omega;
		m_alpha = alpha;
	}

	public float CalculateDistance(float time)
	{
		return m_A * Mathf.Pow(e, (0f - m_beta) * time) * Mathf.Cos(m_omega * time + m_alpha);
	}

	public float CalculateZeroTime(int n)
	{
		float f = m_alpha / (float)System.Math.PI - 0.5f;
		int num = ((m_omega > 0f) ? (Mathf.CeilToInt(f) + (n - 1)) : (Mathf.FloorToInt(f) - (n - 1)));
		return ((float)System.Math.PI / 2f * (float)(1 + 2 * num) - m_alpha) / m_omega;
	}
}
