
// namespace KZLib.KZNetwork
// {
// 	public class NetworkPacketResponse
// 	{
// 		public string code;
// 		public string encrypted;
// 		public string result;
// 		public virtual bool Validate()
// 		{
// 			try
// 			{
// 				result = CNUtils.ConvertEnumData<eServerResult>( code );
// 				return ( result == eServerResult.SUCCESS );	
// 			}
// 			catch
// 			{
// 				Log.logError( "# 등록되지 않은 에러코드 : " + code );
// 				result = eServerResult.FAIL;
// 				return false;
// 			}

// 		}
// 	}



// 	/// <summary>
// 	/// 로그인/생성데이터 응답정보
// 	/// </summary>
// 	public class NetworkPacketResponseLogIn : NetworkPacketResponse
// 	{
// 		public DataPacketLogIn plain;
// 	}




// 	public class NetworkPacketResponseServerConfig : NetworkPacketResponse
// 	{
// 		public DataPacketServerConfig plain;
// 	}




// 	/// <summary>
// 	/// 게임데이터 응답정보
// 	/// </summary>
// 	public class NetworkPacketResponseGame : NetworkPacketResponse
// 	{
// 		public DataPacketGame plain;
// 	}



// }