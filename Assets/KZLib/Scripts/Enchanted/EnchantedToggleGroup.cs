using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class EnchantedToggleGroup : ToggleGroup
{
	private const string NoneToggleName = "None";
	
	[PropertySpace(10)]
	[SerializeField,ValueDropdown(nameof(ToggleNameGroup))]
	private string m_startToggleName = null;

	[PropertySpace(5)]

	[ShowInInspector,ReadOnly]
	protected Toggle SelectedToggle => GetFirstActiveToggle();

	protected override void Start()
	{
		_Initialize();
		base.Start();
	}

	protected override void OnEnable()
	{
		_Initialize();
		base.OnEnable();
	}

	private void _Initialize()
	{
		if(m_Toggles.Count == 0)
		{
			return;
		}

		SetAllTogglesOff(true);

		var toggleName = m_toggleNameList.Contains(m_startToggleName) ? m_startToggleName : NoneToggleName;

		if(toggleName.IsEqual(NoneToggleName))
		{
			return;
		}

		var toggle = _GetToggle(toggleName);

		if(toggle == null)
		{
			return;
		}

		toggle.isOn = true;
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

		return toggle != null ? toggle.GetComponent<Toggle>() as TToggle : null;
	}

	public bool ContainToggleUI(BaseToggleUI toggleUI)
	{
		var toggle = toggleUI.GetComponent<Toggle>();

		return m_Toggles.Contains(toggle);
	}

	private Toggle _GetToggle(string toggleName)
	{
		bool _FindGroupIndex(Toggle toggle)
		{
			return toggle.name.IsEqual(toggleName);
		}

		return m_Toggles.Find(_FindGroupIndex);
	}

	private readonly List<string> m_toggleNameList = new();

	private IEnumerable ToggleNameGroup
	{
		get
		{
			if(m_toggleNameList.IsNullOrEmpty())
			{
				m_toggleNameList.AddNotOverlap(NoneToggleName);
				
				foreach(var toggle in m_Toggles)
				{
					if(!toggle.TryGetComponent<BaseToggleUI>(out var _))
					{
						continue;
					}

					m_toggleNameList.AddNotOverlap(toggle.name);
				}
			}

			return m_toggleNameList;
		}
	}
}