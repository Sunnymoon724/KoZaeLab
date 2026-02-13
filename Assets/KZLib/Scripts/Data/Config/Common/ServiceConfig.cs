using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace KZLib.Data
{
	public class ServiceConfig : IConfig
	{
		private Dictionary<string,string> DiscordLinkDict { get; set; } = new() { { "Bug Report", " " }, };
		private Dictionary<string,string> GoogleSheetFileIdDict { get; set; } = new() { { " ", " " }, };
		private Dictionary<string,string> GoogleDriveFolderIdDict { get; set; } = new() { { " ", " " }, };

		private string TrelloApiKey { get; set; } = "";
		private string TrelloToken { get; set; } = "";

		public string GoogleSheetURL { get; private set; } = "";
		public string GoogleDriveURL { get; private set; } = "";

		public List<string> BugReportPostList { get; private set; } = new() { "Discord", "Trello" };

		public string GetDiscordLink(string key)
		{
			return _FindKey(DiscordLinkDict,key);
		}

		public string GetGoogleSheetFileId(string key)
		{
			return _FindKey(GoogleSheetFileIdDict,key);
		}

		public string GetGoogleDriveFolderId(string key)
		{
			return _FindKey(GoogleDriveFolderIdDict,key);
		}

		[YamlIgnore]
		public string TrelloKey => TrelloApiKey.IsEmpty() || TrelloToken.IsEmpty() ? null : $"key={TrelloApiKey}&token={TrelloToken}";

		private string _FindKey(Dictionary<string,string> dictionary,string key)
		{
			bool _IsKey(KeyValuePair<string,string> pair)
			{
				return pair.Key.Contains(key);
			}

			return dictionary.FindOrFirst(_IsKey).Value;
		}
	}
}