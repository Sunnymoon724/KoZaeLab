using System;
using System.Collections.Generic;
using UnityEngine;

namespace KZLib.Data
{
	public partial class DebugConfig : IConfig
	{
		public bool IsCheckProto { get; private set; } = true; // 
		public bool IsOfflineMode { get; private set; } = false; // 오프라인 모드
		public List<string> BindDebugBtnList { get; private set; } = new(); // 
		public List<string> AutoCmdList { get; private set; } = new(); // 
		public bool IsSkipHeroSpawnAni { get; private set; } = false; // 
		public float StartHp { get; private set; } = 100.0f; // 
		public int StartStage { get; private set; } = 10; // 
		public string SelectHierarchyPath { get; private set; } // 
		public List<int> PlayerHeroNumList { get; private set; } // 
		public List<int> PlayerHaveHeroList { get; private set; } = new(); // 
		public List<int> PlayerPetNumList { get; private set; } = new(); // 
		public double PlayerAtkFixDamage { get; private set; } = 10.0d; // 
		public double MonAtkFixDamage { get; private set; } = 10.0d; // 
		public bool IsForceLoadAdFail { get; private set; } = true; // 
		public bool IsWriteBSLog { get; private set; } = false; // 
	}
}