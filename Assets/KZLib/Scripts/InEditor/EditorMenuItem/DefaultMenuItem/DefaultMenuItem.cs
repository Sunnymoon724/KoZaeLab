#if UNITY_EDITOR

namespace KZLib.KZMenu
{
	public static partial class DefaultMenuItem
	{
		private static class MenuOrder
		{
			public static class Asset
			{
				private const int DEFAULT			= 0 * Global.MENU_ORDER_MAIN_SPACE;

				public const int TOTAL				= DEFAULT + 0 * Global.MENU_ORDER_SUB_SPACE;
				public const int PREFAB				= DEFAULT + 1 * Global.MENU_ORDER_SUB_SPACE;
				public const int SCRIPT				= DEFAULT + 2 * Global.MENU_ORDER_SUB_SPACE;
				public const int TEXTURE			= DEFAULT + 3 * Global.MENU_ORDER_SUB_SPACE;
				public const int SCRIPTABLE_OBJECT	= DEFAULT + 3 * Global.MENU_ORDER_SUB_SPACE;
			}

			public static class GameObject
			{
				private const int DEFAULT			= 1 * Global.MENU_ORDER_MAIN_SPACE;

				public const int HIERARCHY			= DEFAULT + 0 * Global.MENU_ORDER_SUB_SPACE;
				public const int CATEGORY			= DEFAULT + 1 * Global.MENU_ORDER_SUB_SPACE;
				public const int UI					= DEFAULT + 2 * Global.MENU_ORDER_SUB_SPACE;
			}
		}
	}
}
#endif