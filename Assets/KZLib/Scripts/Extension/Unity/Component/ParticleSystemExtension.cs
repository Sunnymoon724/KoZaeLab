using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public static class ParticleSystemExtension
{
	public static async UniTask PlayAndWaitForParticleAsync(this ParticleSystem _particleSystem)
	{
		_particleSystem.Play();

		await  _particleSystem.WaitForParticleAsync();
	}

	public static async UniTask WaitForParticleAsync(this ParticleSystem _particleSystem,CancellationToken _token = default)
	{
		await UniTask.WaitWhile(() => _particleSystem.isPlaying,cancellationToken : _token);
	}
}