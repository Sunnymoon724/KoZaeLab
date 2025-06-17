using System.IO;
using KZLib.KZUtility;

namespace KZLib.KZData
{
	/// <summary>
	/// project folder path
	/// </summary>
	public readonly struct Route
	{
		private readonly string m_localPath;
		private readonly string m_extension;

		public string Extension => m_extension;

		public Route(string path)
		{
			if(path.IsEmpty())
			{
				LogSvc.System.E("Path cannot be null or empty.");

				m_localPath = null;
				m_extension = null;

				return;
			}

			var projectPath = Global.PROJECT_PATH;

			if(path.Contains(Global.ASSETS_TEXT))
			{
				if(!Path.IsPathRooted(path))
				{
					path = Path.Combine(projectPath,path);
				}
			}
			else
			{
				if(Path.IsPathRooted(path))
				{
					throw new InvalidDataException("Path is not in the project.");
				}

				path = Path.Combine(projectPath,Global.ASSETS_TEXT,path);
			}

			path = Path.GetFullPath(path);

			var localPath = path.Replace($"{Path.Combine(projectPath,Global.ASSETS_TEXT)}{Path.DirectorySeparatorChar}","");

			m_localPath = Path.Combine(FileUtility.GetParentPath(localPath),FileUtility.GetFileName(localPath));
			m_extension = FileUtility.GetExtension(localPath).TrimStart('.');
		}

		public string AssetPath => Path.Combine(Global.ASSETS_TEXT,LocalPath);
		public string LocalPath => m_localPath;

		public string AbsolutePath => Path.GetFullPath(AssetPath);
	}
}