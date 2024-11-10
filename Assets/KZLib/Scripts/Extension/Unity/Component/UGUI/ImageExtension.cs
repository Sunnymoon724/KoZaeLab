using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public static class ImageExtension
{
	public static void SetDefaultImage(this Image _image,bool _isClearColor)
	{
		if(!_image)
		{
			LogTag.System.E("Image is null.");

			return;
		}

		_image.color = _isClearColor ? Color.clear : Color.white;
	}

	public static void SetSafeImage(this Image _image,Sprite _sprite,Material _material = null,Color? _color = null)
	{
		if(!_image)
		{
			LogTag.System.E("Image is null.");

			return;
		}

		if(!_sprite)
		{
			_image.gameObject.SetActiveSelf(false);

			return;
		}

		_image.gameObject.SetActiveSelf(true);

		_image.sprite = _sprite;
		_image.material = _material;

		if(!_color.HasValue)
		{
			return;
		}

		_image.color = _color.Value;
	}

	public static async UniTask FadeImageAsync(this Image _image,float _duration,Color _prev,Color _next)
	{
		await CommonUtility.ExecuteOverTimeAsync(0.0f,1.0f,_duration,(progress)=>
		{
			_image.color = Color.Lerp(_prev,_next,progress);
		});
	}
}