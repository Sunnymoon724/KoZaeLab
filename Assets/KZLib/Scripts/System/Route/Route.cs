using System.IO;

namespace KZLib.Data
{
	/// <summary>
	/// project folder path
	/// </summary>
	public readonly struct Route
	{
		private const string c_assets = "Assets";

		private readonly string m_localPath;
		private readonly string m_extension;

		public string Extension => m_extension;

		public Route(string path)
		{
			if(path.IsEmpty())
			{
				LogChannel.Data.E("Path cannot be null or empty.");

				m_localPath = null;
				m_extension = null;

				return;
			}

			var projectPath = Global.ProjectPath;

			if(path.Contains(c_assets))
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

				path = Path.Combine(projectPath,c_assets,path);
			}

			path = Path.GetFullPath(path);

			var localPath = path.Replace($"{Path.Combine(projectPath,c_assets)}{Path.DirectorySeparatorChar}","");

			m_localPath = Path.Combine(KZFileKit.GetParentPath(localPath),KZFileKit.GetFileName(localPath));
			m_extension = KZFileKit.GetExtension(localPath).TrimStart('.');
		}

		public string AssetPath => Path.Combine(c_assets,LocalPath);
		public string LocalPath => m_localPath;

		public string AbsolutePath => Path.GetFullPath(Path.Combine(Global.ProjectPath,AssetPath));
	}
}