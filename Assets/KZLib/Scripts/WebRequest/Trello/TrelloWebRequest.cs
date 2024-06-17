using System;
using Newtonsoft.Json;

namespace KZLib.KZNetwork
{
	public abstract class TrelloWebRequest : BaseWebRequest
	{
		[Serializable]
		protected record TrelloData
		{
			[JsonProperty("id")] public string Id { get; set; }
			[JsonProperty("boards")] public BoardData[] BoardArray { get; set; }
		}

		[Serializable]
		protected record BoardData
		{
			[JsonProperty("id")] public string Id { get; set; }
			[JsonProperty("name")] public string Name { get; set; }
			[JsonProperty("closed")] public bool IsClosed { get; set; }
			[JsonProperty("idOrganization")] public string OrganizationId { get; set; }
			[JsonProperty("lists")] public ListData[] ListArray { get; set; }
		}

		[Serializable]
		protected record ListData
		{
			[JsonProperty("id")] public string Id { get; set; }
			[JsonProperty("name")] public string Name { get; set; }
			[JsonProperty("closed")] public bool IsClosed { get; set; }
			[JsonProperty("idBoard")] public string BoardId { get; set; }
			[JsonProperty("cards")] public CardData[] CardArray { get; set; }
		}

		[Serializable]
		protected record CardData
		{
			[JsonProperty("id")] public string Id { get; set; }
			[JsonProperty("name")] public string Name { get; set; }
			[JsonProperty("closed")] public bool IsClosed { get; set; }
			[JsonProperty("dueComplete")] public bool IsCompleted { get; set; }
			[JsonProperty("desc")] public string Description { get; set; }
			[JsonProperty("idBoard")] public string BoardId { get; set; }
			[JsonProperty("idList")] public string ListId { get; set; }
		}

		protected TrelloWebRequest(string _uri,string _method) : base(_uri,_method) { }
	}
}