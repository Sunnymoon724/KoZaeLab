
using System;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class NetworkSettings : InSideSingletonSO<NetworkSettings>
{
	[Serializable]
	private class NameIdResultData
	{
		[HorizontalGroup("이름"),HideLabel,SerializeField]
		private readonly string m_Name = null;
		[HorizontalGroup("아이디"),HideLabel,SerializeField]
		private readonly string m_Id = null;

		public NameIdResultData(string _name,string _id) { m_Name = _name; m_Id = _id; }
	}
}