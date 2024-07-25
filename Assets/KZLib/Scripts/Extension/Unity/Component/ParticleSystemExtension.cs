using UnityEngine;
using Cysharp.Threading.Tasks;

public static class ParticleSystemExtension
{
	public static async UniTask PlayAndWaitForParticleAsync(this ParticleSystem _particleSystem)
	{
		_particleSystem.Play();

		await  _particleSystem.WaitForParticleAsync();
	}

	public static async UniTask WaitForParticleAsync(this ParticleSystem _particleSystem)
	{
		await UniTask.WaitWhile(() => _particleSystem.isPlaying);
	}
}