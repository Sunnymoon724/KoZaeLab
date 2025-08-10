
namespace KZLib.KZNetwork
{
	public abstract class AccountCredential
	{
		private const string c_accountId = "AccountId";

		public virtual bool HasAccountToken => m_accountId.IsEmpty();

		private string m_accountId = null;
		public string AccountId => m_accountId;

		public AccountCredential()
		{
			m_accountId = PlayerPrefsMgr.In.TryGetString( c_accountId,out var id ) ? id : string.Empty;
		}

		public virtual void Clear()
		{
			SetAccountId(string.Empty);
		}

		public void SetAccountId(string profileId)
		{
			if(m_accountId == profileId)
			{
				return;
			}

			m_accountId = profileId;

			PlayerPrefsMgr.In.SetString(c_accountId,profileId);
		}
	}
#if KZLIB_PLAY_FAB
	public class PlayFabAccountCredential : AccountCredential
	{
		private const string c_loginOptionType = "LoginOptionType";

		private PlayFabLoginOptionType m_loginOptionType = PlayFabLoginOptionType.None;
		public PlayFabLoginOptionType LoginOptionType => m_loginOptionType;

		public PlayFabAccountCredential() : base()
		{
			m_loginOptionType = PlayerPrefsMgr.In.TryGetEnum<PlayFabLoginOptionType>( c_loginOptionType,out var type ) ? type : PlayFabLoginOptionType.None;
		}

		public override void Clear()
		{
			base.Clear();
			
			SetLoginType(PlayFabLoginOptionType.None);
		}

		public void SetLoginType(PlayFabLoginOptionType loginOptionType)
		{
			if(m_loginOptionType == loginOptionType)
			{
				return;
			}

			m_loginOptionType = loginOptionType;

			PlayerPrefsMgr.In.SetEnum(c_loginOptionType,loginOptionType);
		}
	}
#endif
}
