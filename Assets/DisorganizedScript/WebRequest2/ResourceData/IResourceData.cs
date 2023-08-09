using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace KZLib.Auth
{
	public interface IResourceData 
	{
		string Kind { get; }
	}
}