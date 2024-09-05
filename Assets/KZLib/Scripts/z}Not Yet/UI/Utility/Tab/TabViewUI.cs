using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public interface ITabViewUI
{
	void Notify(TabButtonUI _button);
}

public abstract class TabViewUI : BaseComponentUI,ITabViewUI
{
	public abstract void Notify(TabButtonUI _button);
}