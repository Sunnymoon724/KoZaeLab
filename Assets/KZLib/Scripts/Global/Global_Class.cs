using System.ComponentModel;
using System;

namespace System.Runtime.CompilerServices
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class IsExternalInit { }
}

public record MessageInfo(string Header,string Body);

public record NetworkRespondInfo(bool IsUpdate,string Type,string Data);

public record NetworkPacketInfo(int Code,string Message,bool IsEncrypted);

public record UnitStateInfo(Enum PreState,Enum CurState);