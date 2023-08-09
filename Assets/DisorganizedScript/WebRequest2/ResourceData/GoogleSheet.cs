using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace KZLib.Auth
{
	[Serializable]
	public class GoogleSheet : IResourceData
	{
		public string range;
		public string majorDimension;

		public string[][] values;

		public string Kind => null;
	}
}