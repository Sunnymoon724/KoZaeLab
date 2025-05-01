// using System;
// using KZLib.KZData;
// using KZLib.KZUtility;
// using UnityEngine.Events;

// namespace KZLib.KZNetwork
// {
// 	public partial class NetworkMgr : Singleton<NetworkMgr>
// 	{
// 		public void RequestNetworkPacket<TAffix>(Action trelloKey,Action<bool,object> onResponse) where TAffix : IAffix
// 		{
// 			if( trelloKey == null )
// 			{
// 				LogTag.Network.E("TrelloKey is null");

// 				return;
// 			}

// 			if( onResponse == null )
// 			{
// 				LogTag.Network.E("OnResponse is null");

// 				return;
// 			}

// 			var request = new NetworkPacketRequest( trelloKey, onResponse );

// 			request.SendRequest();
// 		}
// 		{

// 			// Log.log( "#가이드 퀘스트 보상 요청 : " + mLogInService.platform );

// 			// NetworkPacketRequest request = helper.CREATE_PACKET_REQUEST_QUEST_GUIDE_REWARD( mLogInService.token );
// 			// int apiID = request.apiID;
// 			// CLocalServer.instance.gameData.playerData.playerInventory.Stamp( apiID, eClientInventoryAction.SPAWN_MERGE_FOOD );
// 			// CLocalServer.instance.gameData.playerData.playerInventory.Stamp( apiID, eClientInventoryAction.PURCHASE_MERGE_FEVER );
// 			// helper.SendRequest( request, _callback, _ResponseNetworkPacket );
// 		}

// 		private void _ResponseNetworkPacket<TAffix>(ResponseType responseType) where TAffix : IAffix
// 		{
// 			try
// 			{
// 				var response = PARSE_PACKET_RESPONSE(responseType, _req, _res );

// 				if( response == null ) { return; }

// 				CLocalServer.Instance.IncreaseGuideQuest();
// 				CLocalServer.instance.UpdateContentsUnlockSystem();
// 				CLocalServer.instance.gameData.playerData.playerInventory.Reset( _req.apiID );
// 				CLocalServer.Instance.reddotSystem.UpdateReddotAll();

// 				if( _req.callBackResponse != null )
// 				{
// 					_req.callBackResponse( ( int )response.result, response );
// 				}

// 				helper.RequestNextAPI();

// 			}
// 			catch(Exception exception)
// 			{
// 				LogTag.System.E($"Exception : {exception.Message}");

// 				return _WriteDump(new ResponseInfo(false,c_internalServerError,string.Empty,exception.Message));
// 			}
// 			finally
// 			{
// 				// _req.Dispose();
// 				// _res.Dispose();
// 			}
// 		}

// 		public NetworkPacketResponse PARSE_PACKET_RESPONSE(ResponseType responseType,object data)
// 		{
// 			var response = data as NetworkPacketResponse;

// 			this.SetTokenAPI( response.apiToken );


// 			if( response.Validate() == false )
// 			{
// 				Log.logError( "요청 응답 오류 발생 : " + response.code );
// 				NetworkManager.Instance.SendNetworkError( response.code );
// 				this.RequestNextAPI();
// 				return response;
// 			}

// 			setPacketData( _apiType, response );

// 			return response;
// 		}
// 	}
// }