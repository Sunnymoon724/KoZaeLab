using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	public abstract class CarouselNavigator : BaseComponent
	{
		public abstract void SetNavigator(bool selected);
	}
}