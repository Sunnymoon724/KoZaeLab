
namespace UnityEngine.UI
{
	public class MenuSlot : Slot
	{
		private string m_menuText = null;

		protected override bool UseGizmos => true;
		protected override string GizmosText => m_menuText ?? string.Empty;

		public override void SetEntryInfo(IEntryInfo entryInfo)
		{
			m_menuText = entryInfo.Name;

			base.SetEntryInfo(entryInfo);
		}
	}
}