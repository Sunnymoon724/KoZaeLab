using System.Threading;
using Cysharp.Threading.Tasks;

namespace KZLib.KZSample
{
	public class Main : MainLib
	{
		protected override void Awake()
		{
			base.Awake();

		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

		}

		protected override async UniTask _InitializeTestMode(CancellationToken token)
		{
			await base._InitializeTestMode(token);
		}

		protected override async UniTask _InitializeNormalMode(CancellationToken token)
		{
			await base._InitializeNormalMode(token);
		}
	}
}
