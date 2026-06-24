namespace KZLib.UI
{
	/// <summary>
	/// <see cref="FocusSlot"/> variant used by <see cref="Carousel"/> prefabs.
	/// </summary>
	/// <remarks>
	/// Inherits center-only button interactability from <see cref="FocusSlot"/>.
	/// <see cref="Carousel"/> calls <see cref="IFocusSlot.RefreshLocation"/> while scrolling when the pivot uses this type.
	/// </remarks>
	public class CarouselSlot : FocusSlot { }
}
