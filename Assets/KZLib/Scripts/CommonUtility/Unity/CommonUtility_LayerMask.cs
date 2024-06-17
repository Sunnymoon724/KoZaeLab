using UnityEngine;

public static partial class CommonUtility
{
	public static int GetLayerByName(string _layerName,bool _autoCreate = false)
	{
		var layer = LayerMask.NameToLayer(_layerName);

#if UNITY_EDITOR
		if(layer == -1 && _autoCreate)
		{
			AddLayer(_layerName);
		}
#endif

		return layer;
	}
}