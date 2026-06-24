using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.Utilities
{
	/// <summary>
	/// Applies <see cref="StanzaLerp.Progress"/> to a fullscreen <see cref="Image"/> for <see cref="KZLib.UI.TransitionPanel"/>.
	/// Default mode tweens alpha; optional material mode drives <c>KZLib/TextureMask</c> <c>_Range</c>.
	/// </summary>
	public class TransitionStanzaLerp : StanzaLerp
	{
		private enum TransitionType { None, Material }

		/// <summary>Shader property used only in <see cref="TransitionType.Material"/> mode.</summary>
		private static readonly int s_rangeShaderPropertyId = Shader.PropertyToID("_Range");

		[SerializeField,Required]
		private Image m_image = null;

		/// <summary>Runtime clone so <c>_Range</c> updates never write to a shared material asset.</summary>
		private Material m_runtimeMaterial = null;

		[SerializeField,HideInInspector]
		private TransitionType m_currentType = TransitionType.None;

		/// <summary>
		/// Editor-only switch between fade and mask modes.
		/// <c>None</c> = alpha fade; <c>Material</c> = mask shader driven by <c>_Range</c>.
		/// </summary>
		[ShowInInspector]
		private TransitionType CurrentType
		{
			get => m_currentType;
			set
			{
				if(!m_image)
				{
					return;
				}

				m_currentType = value;

				// Fade mode uses the default UI material; mask mode assigns the runtime instance.
				m_image.material = IsFade ? null : _GetOrCreateRuntimeMaterial();

				_SetProgress(Progress);
			}
		}

		private bool IsFade => CurrentType == TransitionType.None;

		protected override void _OnDisable()
		{
			base._OnDisable();

			// Drop the cloned material before the Image is pooled or reused.
			_ReleaseRuntimeMaterial();
		}

		protected override void _SetProgress(float progress)
		{
			if(!m_image)
			{
				return;
			}

			if(IsFade)
			{
				// Keeps RGB (typically black) and drives overlay opacity only.
				m_image.color = m_image.color.MaskAlpha(progress);
			}
			else
			{
				var material = _GetOrCreateRuntimeMaterial();

				if(material)
				{
					material.SetFloat(s_rangeShaderPropertyId,progress);
				}
			}
		}

		/// <summary>Clones <see cref="Image.material"/> once per enable cycle.</summary>
		private Material _GetOrCreateRuntimeMaterial()
		{
			if(!m_image)
			{
				return null;
			}

			if(!m_runtimeMaterial)
			{
				m_runtimeMaterial = new Material(m_image.material);
			}

			return m_runtimeMaterial;
		}

		/// <summary>Restores the default UI material and destroys the runtime clone.</summary>
		private void _ReleaseRuntimeMaterial()
		{
			if(m_image)
			{
				m_image.material = null;
			}

			if(m_runtimeMaterial)
			{
				m_runtimeMaterial.DestroyObject();
				m_runtimeMaterial = null;
			}
		}
	}
}