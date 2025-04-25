using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using KZLib.KZUtility;
using KZLib.KZData;
using System.IO;
using ConfigData;

namespace KZLib
{
	public class LingoMgr : Singleton<LingoMgr>
	{
		private bool m_disposed = false;

		protected override void Initialize()
		{
			base.Initialize();


		}

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				
			}

			m_disposed = true;

			base.Release(disposing);
		}
	}
}