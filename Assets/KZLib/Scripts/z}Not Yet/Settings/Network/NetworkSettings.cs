using System;
using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZUtility;

public partial class NetworkSettings : InnerBaseSettings<NetworkSettings>
{
	[Serializable]
	private class NameIdResultData
	{
		[HorizontalGroup("Name"),HideLabel,SerializeField]
		private string m_Name = null;
		[HorizontalGroup("Id"),HideLabel,SerializeField]
		private string m_Id = null;

		public NameIdResultData(string _name,string _id) { m_Name = _name; m_Id = _id; }
	}

	[Serializable]
	private record ResultData
	{
		[HorizontalGroup("Name"),HideLabel,ShowInInspector]
		public string Name { get; init; }
		[HorizontalGroup("Id"),HideLabel,ShowInInspector]
		public string Id { get; init; }

		public ResultData(string _name,string _id) { Name = _name; Id = _id; }
	}
}