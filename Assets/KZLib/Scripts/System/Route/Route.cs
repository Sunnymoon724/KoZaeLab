using System;
using System.IO;

namespace KZLib.Utilities
{
	/// <summary>
	/// Normalized project asset path (local, asset-relative, and absolute forms).
	/// </summary>
	/// <remarks>
	/// Lives under <c>System/Route</c> as cross-cutting path infrastructure (config, lingo, proto, editor tools, etc.),
	/// not under <c>Data/</c>. Resolved via <see cref="RouteManager.Fetch"/>.
	/// </remarks>
	public readonly struct Route
	{
		private const string c_assets = "Assets";

		private readonly string m_localPath;
		private readonly string m_extension;

		/// <summary>File extension without the leading dot.</summary>
		public string Extension => m_extension;

		public string AssetPath => Path.Combine(c_assets,LocalPath);
		public string LocalPath => m_localPath;
		public string AbsolutePath => KZFileKit.GetAbsolutePath(AssetPath,true);

		public Route(string path)
		{
			if(path.IsEmpty())
			{
				throw new ArgumentException($"{nameof(path)} is empty.");
			}

			var absolutePath = KZFileKit.GetAbsolutePath(path,true);
			var localPath = _GetLocalPathFromAbsolute(absolutePath);

			m_localPath = KZFileKit.NormalizePath(localPath);
			m_extension = KZFileKit.GetExtension(localPath).TrimStart('.');
		}

		private static string _GetLocalPathFromAbsolute(string absolutePath)
		{
			var assetsRoot = KZFileKit.GetAbsolutePath(c_assets,true);
			var normalizedAbsolutePath = KZFileKit.NormalizePath(Path.GetFullPath(absolutePath));

			if(!normalizedAbsolutePath.StartsWith(assetsRoot,StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidDataException($"Path is not in the project. [{normalizedAbsolutePath}]");
			}

			var suffixStart = assetsRoot.Length;

			if(suffixStart == normalizedAbsolutePath.Length)
			{
				return string.Empty;
			}

			// Boundary check avoids "Assets" matching "AssetsExtra" without allocating a trailing-separator prefix string.
			if(normalizedAbsolutePath[suffixStart] != Path.DirectorySeparatorChar)
			{
				throw new InvalidDataException($"Path is not in the project. [{normalizedAbsolutePath}]");
			}

			return normalizedAbsolutePath[(suffixStart+1)..];
		}
	}
}