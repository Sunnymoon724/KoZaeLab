#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;

namespace KZLib.EditorInternal.Menus
{
	/// <summary>
	/// Extensions for Unity built-in context menus (<c>Assets</c>, <c>GameObject</c>).
	/// Split into partial files by domain; <see cref="MenuOrder"/> lives here.
	/// </summary>
	public static partial class DefaultMenuItem
	{
		private static class MenuOrder
		{
			public static class KZSubMenu
			{
				private const int DEFAULT			= 0 * Global.MenuOrderMainSpace;

				public const int SCRIPT				= DEFAULT + 0 * Global.MenuOrderSubSpace;
				public const int TEXTURE			= DEFAULT + 1 * Global.MenuOrderSubSpace;
				public const int SCRIPTABLE_OBJECT	= DEFAULT + 2 * Global.MenuOrderSubSpace;
				public const int PREFAB				= DEFAULT + 3 * Global.MenuOrderSubSpace;
				public const int PREFAB_SHOW_MESH_NAME = PREFAB + 0;
			}

			public static class Hierarchy
			{
				private const int DEFAULT			= 1 * Global.MenuOrderMainSpace;

				public const int COPY				= DEFAULT + 0 * Global.MenuOrderSubSpace;
				public const int CATEGORY_LINE		= DEFAULT + 1 * Global.MenuOrderSubSpace;
			}

			public static class Create
			{
				private const int DEFAULT			= 2 * Global.MenuOrderMainSpace;

				public const int PATH_CREATOR		= DEFAULT + 0 * Global.MenuOrderSubSpace;

				public static class UI
				{
					private const int OFFSET		= DEFAULT + 1 * Global.MenuOrderSubSpace;

					public const int EMPTY_PANEL	= OFFSET + 0;
					public const int SHAPE			= OFFSET + 1;
					public const int CAROUSEL		= OFFSET + 2;
				}
			}
		}

		internal static void _LogGroupedPathList(Dictionary<string,List<string>> groupDict)
		{
			var builder = new StringBuilder();

			foreach(var pair in groupDict)
			{
				builder.Clear();
				builder.Append($"{pair.Key}\n");

				var pathList = pair.Value;

				for(var i=0;i<pathList.Count;i++)
				{
					builder.Append($" -[{pathList[i]}]\n");
				}

				builder.AppendLine();

				LogChannel.Editor.I(builder.ToString());
			}
		}
	}
}
#endif
