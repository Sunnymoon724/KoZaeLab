using UnityEngine;
using TMPro;
using System.Collections;

public static class ParticleSystemExtension
{
	public static IEnumerator PlayAndWaitForParticleSystem(this ParticleSystem _particleSystem)
	{
		_particleSystem.Play();

		yield return _particleSystem.WaitForParticleSystem();
	}

	public static IEnumerator WaitForParticleSystem(this ParticleSystem _particleSystem)
	{
		yield return new WaitWhile(()=>_particleSystem.isPlaying);
	}
}