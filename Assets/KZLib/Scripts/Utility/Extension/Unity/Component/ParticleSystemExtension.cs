using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// Extension methods for async <see cref="ParticleSystem"/> playback and completion waiting.
/// </summary>
public static class ParticleSystemExtension
{
	/// <summary>
	/// Starts the particle system and awaits until all particles have finished.
	/// </summary>
	public static async UniTask PlayAndWaitForParticleAsync(this ParticleSystem particleSystem)
	{
		if(!_IsValid(particleSystem))
		{
			return;
		}

		particleSystem.Play();

		await particleSystem.WaitForParticleAsync();
	}

	/// <summary>
	/// Awaits while the particle system is playing. Looping systems wait until cancelled.
	/// </summary>
	public static async UniTask WaitForParticleAsync(this ParticleSystem particleSystem,CancellationToken token = default)
	{
		if(!_IsValid(particleSystem))
		{
			return;
		}

		if(particleSystem.main.loop)
		{
			LogChannel.Kit.W("ParticleSystem is set to loop. WaitForParticleAsync will wait indefinitely unless cancelled.");
		}

		bool _IsParticlePlaying()
		{
			return particleSystem.isPlaying;
		}

		await UniTask.WaitWhile(_IsParticlePlaying,cancellationToken : token).SuppressCancellationThrow();
	}

	private static bool _IsValid(ParticleSystem particleSystem)
	{
		if(!particleSystem)
		{
			LogChannel.Kit.E("ParticleSystem is null");

			return false;
		}

		return true;
	}
}