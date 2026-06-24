using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.Utilities
{
	/// <summary>
	/// Plays a ordered list of <see cref="Entry"/> blocks sequentially.
	/// Each entry can delay, then play one or more child <see cref="Stanza"/> instances in parallel.
	/// </summary>
	public class StanzaGroup : Stanza
	{
		[SerializeField]
		private List<Entry> m_stanzaList = new();

		/// <summary>One timed block containing child stanzas to run together.</summary>
		[Serializable]
		private class Entry
		{
			[SerializeField]
			private float m_delayTime = 0.0f;

			[SerializeField,ListDrawerSettings(ShowFoldout = false,DraggableItems = false)]
			private List<Stanza> m_stanzaList = new();

			/// <summary>Waits <see cref="m_delayTime"/>, then plays all child stanzas (parallel when count &gt; 1).</summary>
			public async UniTask PlayAsync(Param param,CancellationToken token)
			{
				if(m_stanzaList.IsNullOrEmpty())
				{
					return;
				}

				if(m_delayTime > 0.0f)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(m_delayTime),cancellationToken : token);
				}

				if(m_stanzaList.Count == 1)
				{
					await m_stanzaList[0].PlayAsync(param).AttachExternalCancellation(token);
				}
				else
				{
					var taskList = new List<UniTask>();

					for(var i=0;i<m_stanzaList.Count;i++)
					{
						var stanza = m_stanzaList[i];

						if(stanza == null)
						{
							continue;
						}

						taskList.Add(stanza.PlayAsync(param).AttachExternalCancellation(token));
					}

					await UniTask.WhenAll(taskList).AttachExternalCancellation(token);
				}
			}

		/// <summary>Resets every child stanza in this entry.</summary>
		public void ResetAll()
		{
			for(var i=0;i<m_stanzaList.Count;i++)
			{
				if(m_stanzaList[i] == null)
				{
					continue;
				}

				m_stanzaList[i].ResetAll();
			}
		}
		}

		protected async override UniTask _DoPlayAsync(Param param)
		{
			for(var i=0;i<m_stanzaList.Count;i++)
			{
				await m_stanzaList[i].PlayAsync(param,m_tokenSource.Token);
			}
		}

		public override void ResetAll()
		{
			base.ResetAll();

			for(var i=0;i<m_stanzaList.Count;i++)
			{
				m_stanzaList[i].ResetAll();
			}
		}
	}
}
