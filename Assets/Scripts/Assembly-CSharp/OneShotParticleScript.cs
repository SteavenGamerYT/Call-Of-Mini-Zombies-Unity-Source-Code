using System.Collections;
using UnityEngine;

public class OneShotParticleScript : MonoBehaviour
{
	private IEnumerator Start()
	{
		yield return new WaitForSeconds(GetComponent<ParticleEmitter>().minEnergy / 2f);
		GetComponent<ParticleEmitter>().emit = false;
	}

	private void Update()
	{
	}
}
