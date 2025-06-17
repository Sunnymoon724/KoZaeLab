using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public static class ParticleSystemExtension
{
	public static async UniTask PlayAndWaitForParticleAsync(this ParticleSystem particleSystem)
	{
		if(!_IsValid(particleSystem))
		{
			return;
		}

		particleSystem.Play();

		await  particleSystem.WaitForParticleAsync();
	}

	public static async UniTask WaitForParticleAsync(this ParticleSystem particleSystem,CancellationToken cancellationToken = default)
	{
		if(!_IsValid(particleSystem))
		{
			return;
		}

		await UniTask.WaitWhile(() => particleSystem.isPlaying,cancellationToken : cancellationToken);
	}

	private static bool _IsValid(ParticleSystem particleSystem)
	{
		if(!particleSystem)
		{
			Logger.System.E("ParticleSystem is null");

			return false;
		}

		return true;
	}
}