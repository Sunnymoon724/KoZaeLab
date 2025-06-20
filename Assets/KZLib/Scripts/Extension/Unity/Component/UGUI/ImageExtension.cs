using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public static class ImageExtension
{
	public static void SetDefaultImage(this Image image,bool isClearColor)
	{
		if(!_IsValid(image))
		{
			return;
		}

		image.color = isClearColor ? Color.clear : Color.white;
	}

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
		await CommonUtility.ExecuteProgressAsync(0.0f,1.0f,duration,(progress)=>
		{
			image.color = Color.Lerp(prevColor,nextColor,progress);
		});
	}

	private static bool _IsValid(Image image)
	{
		if(!image)
		{
			LogSvc.System.E("Image is null");

			return false;
		}

		return true;
	}
}