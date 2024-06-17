using UnityEngine;

public static partial class CommonUtility
{
	public static void SetCanvasCullTransparentMeshOff(GameObject _panel)
	{
		var canvas = _panel.GetComponent<CanvasRenderer>();

		if(canvas)
		{
			canvas.cullTransparentMesh = false;
		}
	}

	public static TComponent CopyComponent<TComponent>(TComponent _source,GameObject _destination) where TComponent : Component
	{
		var type = _source.GetType();
		var data = _destination.AddComponent(type);

		foreach(var field in type.GetFields())
		{
			field.SetValue(data,field.GetValue(_source));
		}

		return data as TComponent;
	}
}