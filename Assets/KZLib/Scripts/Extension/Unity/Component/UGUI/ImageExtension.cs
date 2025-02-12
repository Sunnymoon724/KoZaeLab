using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public static class ImageExtension
{
	public static void SetDefaultImage(this Image image,bool isClearColor)
	{
		if(!IsValid(image))
		{
			return;
		}

		image.color = isClearColor ? Color.clear : Color.white;
	}

	public static void SetSafeImage(this Image image,Sprite sprite,Material material = null,Color? color = null)
	{
		if(!IsValid(image))
		{
			return;
		}

		if(!sprite)
		{
			image.gameObject.SetActiveIfDifferent(false);

			return;
		}

		image.gameObject.SetActiveIfDifferent(true);

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

	private static bool IsValid(Image image)
	{
		if(!image)
		{
			LogTag.System.E("Image is null");

			return false;
		}

		return true;
	}
}