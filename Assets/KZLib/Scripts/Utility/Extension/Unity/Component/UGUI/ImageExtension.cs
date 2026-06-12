using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Extension methods for <see cref="Image"/> assignment, defaults, and color fading.
/// </summary>
public static class ImageExtension
{
	/// <summary>
	/// Sets color to white or transparent depending on <paramref name="isClearColor"/>.
	/// </summary>
	public static void SetDefaultImage(this Image image,bool isClearColor)
	{
		if(!_IsValid(image))
		{
			return;
		}

		image.color = isClearColor ? Color.clear : Color.white;
	}

	/// <summary>
	/// Assigns sprite and material; deactivates the GameObject when <paramref name="sprite"/> is null.
	/// </summary>
	public static void SetSafeImage(this Image image,Sprite sprite,Material material = null,Color? color = null)
	{
		if(!_IsValid(image))
		{
			return;
		}

		if(!sprite)
		{
			image.gameObject.EnsureActive(false);

			return;
		}

		image.gameObject.EnsureActive(true);

		image.sprite = sprite;
		image.material = material;

		if(!color.HasValue)
		{
			return;
		}

		image.color = color.Value;
	}

	public static async UniTask FadeImageAsync(this Image image,float duration,Color prevColor,Color nextColor)
	{
		if(!_IsValid(image))
		{
			return;
		}

		void _ProgressColor(float progress)
		{
			image.color = Color.Lerp(prevColor,nextColor,progress);
		}

		await KZExternalKit.ExecuteProgressAsync(0.0f,1.0f,duration,_ProgressColor,false,null,default);
	}

	private static bool _IsValid(Image image)
	{
		if(!image)
		{
			LogChannel.Kit.E("Image is null");

			return false;
		}

		return true;
	}
}
