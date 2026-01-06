using System.Collections.Generic;
using UnityEngine;

public class RepositoryUI2D : RepositoryUI
{
	private readonly Dictionary<UIPriorityType,Transform> m_repositoryDict = new();

	protected override bool IsValid(Window window)
	{
		return window != null && !window.Is3D && window is Window2D;
	}

	public override void Add(Window window)
	{
		if(!IsValid(window))
		{
			return;
		}

		var repository = _GetRepository(window.PriorityType);

		repository.SetUIChild(window.transform);

		_Add(window);
	}

	private Transform _GetRepository(UIPriorityType priorityType)
	{
		if(!m_repositoryDict.ContainsKey(priorityType))
		{
			var repository = transform.Find($"{priorityType}Repository");

			if(repository == null)
			{
				LogSvc.UI.E($"Not Found Repository. PriorityType : {priorityType}");

				return null;
			}

			m_repositoryDict.Add(priorityType,repository);
		}

		return m_repositoryDict[priorityType];
	}
}