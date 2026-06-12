#if UNITY_EDITOR

namespace KZLib.EditorInternal.Menus
{
	public static partial class DefaultMenuItem
	{
		private static class MenuOrder
		{
			public static class Asset
			{
				private const int DEFAULT			= 0 * Global.MenuOrderMainSpace;

				public const int TOTAL				= DEFAULT + 0 * Global.MenuOrderSubSpace;
				public const int PREFAB				= DEFAULT + 1 * Global.MenuOrderSubSpace;
				public const int SCRIPT				= DEFAULT + 2 * Global.MenuOrderSubSpace;
				public const int TEXTURE			= DEFAULT + 3 * Global.MenuOrderSubSpace;
				public const int SCRIPTABLE_OBJECT	= DEFAULT + 4 * Global.MenuOrderSubSpace;
				public const int MESH				= DEFAULT + 5 * Global.MenuOrderSubSpace;
			}

			public static class GameObject
			{
				private const int DEFAULT			= 1 * Global.MenuOrderMainSpace;

				public const int HIERARCHY			= DEFAULT + 0 * Global.MenuOrderSubSpace;
				public const int CATEGORY			= DEFAULT + 1 * Global.MenuOrderSubSpace;
				public const int UI					= DEFAULT + 2 * Global.MenuOrderSubSpace;
			}
		}
	}
}
#endif