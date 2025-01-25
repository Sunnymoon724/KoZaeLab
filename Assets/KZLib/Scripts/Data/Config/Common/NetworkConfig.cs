using System.Collections.Generic;
using KZLib.KZData;
using YamlDotNet.Serialization;

namespace ConfigData
{
	public class NetworkConfig : IConfig
	{
		private Dictionary<string,string> DiscordLinkDict { get; set; } = new() { { "Bug Report", " " }, };
		private Dictionary<string,string> GoogleSheetFileIdDict { get; set; } = new() { { " ", " " }, };
		private Dictionary<string,string> GoogleDriveFolderIdDict { get; set; } = new() { { " ", " " }, };

		private string TrelloApiKey { get; set; } = "";
		private string TrelloToken { get; set; } = "";

		public List<string> BugReportPostList { get; private set; } = new() { "Discord", "Trello" };

		public string GetDiscordLink(string key)
		{
			return DiscordLinkDict.FindOrFirst(x=>x.Key.Contains(key)).Value;
		}

		public string GetGoogleSheetFileId(string key)
		{
			return GoogleSheetFileIdDict.FindOrFirst(x=>x.Key.Contains(key)).Value;
		}

		public string GetGoogleDriveFolderId(string key)
		{
			return GoogleDriveFolderIdDict.FindOrFirst(x=>x.Key.Contains(key)).Value;
		}

		[YamlIgnore]
		public string TrelloCoreKey => TrelloApiKey.IsEmpty() || TrelloToken.IsEmpty() ? null : $"key={TrelloApiKey}&token={TrelloToken}";
	}
}