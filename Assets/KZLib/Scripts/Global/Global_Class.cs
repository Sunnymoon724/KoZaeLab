using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class IsExternalInit { }
}

public record MessageData(string Header,string Body);

public record RespondData(bool IsUpdate,string Type,string Data);

public record NetworkPacket(int Code,string Message,bool IsEncrypted);