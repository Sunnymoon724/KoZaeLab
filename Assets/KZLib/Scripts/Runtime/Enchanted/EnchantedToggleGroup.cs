using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Extends Unity <see cref="ToggleGroup"/>.
/// Initializes from an inspector-assigned start toggle and supports selection by name.
/// Only <see cref="Toggle"/>s with <see cref="BaseToggleUI"/> on the same GameObject are managed.
/// </summary>
public class EnchantedToggleGroup : ToggleGroup
{
	private const string NoneToggleName = "None";

	[PropertySpace(10)]
	[SerializeField,ValueDropdown(nameof(ToggleNameGroup))]
	private string m_startToggleName = null;

	/// <summary>When true, re-applies the start toggle each time the object is re-enabled.</summary>
	[SerializeField,ToggleLeft]
	private bool m_resetOnEnable = true;

	[PropertySpace(5)]
	[ShowInInspector,ReadOnly]
	protected Toggle SelectedToggle => GetFirstActiveToggle();

	/// <summary>Whether Start has run. OnEnable init runs only after that (on re-enable).</summary>
	private bool m_hasStarted = false;

	protected override void Start()
	{
		base.Start();

		m_hasStarted = true;
		_ScheduleInitialize();
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		// First activation is handled by Start; only re-init when the panel is toggled off and on again.
		if(m_hasStarted && m_resetOnEnable)
		{
			_ScheduleInitialize();
		}
	}

	protected override void OnDisable()
	{
		// Cancel any pending init before this object is disabled.
		CancelInvoke(nameof(_Initialize));

		base.OnDisable();
	}

	/// <summary>
	/// Defers initialization to the next frame.
	/// OnEnable runs before children register, so immediate init may see an empty m_Toggles.
	/// </summary>
	private void _ScheduleInitialize()
	{
		if(!isActiveAndEnabled)
		{
			return;
		}

		CancelInvoke(nameof(_Initialize));
		Invoke(nameof(_Initialize),0.0f);
	}

	/// <summary>Turns all toggles off, then turns on m_startToggleName. None leaves all off.</summary>
	private void _Initialize()
	{
		_RefreshToggleNameList();

		if(m_Toggles.Count == 0)
		{
			return;
		}

		SetAllTogglesOff(false);

		var toggleName = m_toggleNameList.Contains(m_startToggleName) ? m_startToggleName : NoneToggleName;

		if(string.Equals(toggleName,NoneToggleName))
		{
			// When allowSwitchOff is false, ToggleGroup requires at least one toggle on.
			if(!allowSwitchOff)
			{
				var firstToggle = _GetFirstToggleUI();

				if(firstToggle != null)
				{
					NotifyToggleOn(firstToggle);
				}
			}

			return;
		}

		var toggle = _GetToggle(toggleName);

		if(toggle == null)
		{
			return;
		}

		NotifyToggleOn(toggle);
	}

	public void Select(BaseToggleUI toggleUI,bool isNotification = true)
	{
		Select(toggleUI.name,isNotification);
	}

	public void Select(string toggleName,bool isNotification = true)
	{
		var toggle = _GetToggle(toggleName);

		if(toggle == null)
		{
			return;
		}

		NotifyToggleOn(toggle,isNotification);
	}

	public TToggle GetToggleUI<TToggle>(string toggleName) where TToggle : BaseToggleUI
	{
		var toggle = _GetToggle(toggleName);

		return toggle != null ? toggle.GetComponent<TToggle>() : null;
	}

	public bool ContainsToggleUI(BaseToggleUI toggleUI)
	{
		if(!toggleUI)
		{
			return false;
		}

		if(!toggleUI.TryGetComponent<Toggle>(out var toggle))
		{
			return false;
		}

		return m_Toggles.Contains(toggle);
	}

	private Toggle _GetFirstToggleUI()
	{
		for(var i=0;i<m_Toggles.Count;i++)
		{
			var toggle = m_Toggles[i];

			if(toggle && toggle.TryGetComponent<BaseToggleUI>(out _))
			{
				return toggle;
			}
		}

		return null;
	}

	/// <summary>Finds a toggle by GameObject name; must have BaseToggleUI.</summary>
	private Toggle _GetToggle(string toggleName)
	{
		for(var i=0;i<m_Toggles.Count;i++)
		{
			var toggle = m_Toggles[i];

			if(!toggle || !toggle.TryGetComponent<BaseToggleUI>(out _))
			{
				continue;
			}

			if(string.Equals(toggle.name,toggleName))
			{
				return toggle;
			}
		}

		return null;
	}

	private readonly List<string> m_toggleNameList = new();

	/// <summary>Inspector ValueDropdown source: None plus BaseToggleUI toggle names.</summary>
	private IEnumerable ToggleNameGroup
	{
		get
		{
			_RefreshToggleNameList();

			return m_toggleNameList;
		}
	}

	private void _RefreshToggleNameList()
	{
		m_toggleNameList.Clear();
		m_toggleNameList.AddIfAbsent(NoneToggleName);

		if(m_Toggles.Count > 0)
		{
			_AppendToggleNames(m_Toggles);

			return;
		}

#if UNITY_EDITOR
		// Before play mode, m_Toggles may be empty; scan child Toggles for the dropdown.
		if(!Application.isPlaying)
		{
			var toggleArray = GetComponentsInChildren<Toggle>(true);

			_AppendToggleNames(toggleArray);
		}
#endif
	}

	private void _AppendToggleNames(IList<Toggle> toggleList)
	{
		for(var i=0;i<toggleList.Count;i++)
		{
			var toggle = toggleList[i];

			if(!toggle || !toggle.TryGetComponent<BaseToggleUI>(out _))
			{
				continue;
			}

			m_toggleNameList.AddIfAbsent(toggle.name);
		}
	}
}
