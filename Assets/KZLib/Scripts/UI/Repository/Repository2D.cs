using System;
using System.Collections.Generic;
using UnityEngine;
using KZLib.UI;

/// <summary>
/// Screen-space overlay repository.
/// Parents each window under a priority layer child (Backmost, Back, Middle, Fore, Foremost).
/// </summary>
public class Repository2D : Repository
{
	// Cached lookup: WindowPriorityType -> "{PriorityType}Repository" child transform.
	[SerializeField]
	private Dictionary<WindowPriorityType,Transform> m_repositoryDict = new();

	protected override bool IsValid(Window window)
	{
		return window != null && !window.Is3D && window is Window2D;
	}

	/// <summary>
	/// Places the window under its priority layer and registers it as open.
	/// </summary>
	public override void Add(Window window)
	{
		if(!IsValid(window))
		{
			throw new InvalidOperationException($"{window?.NameTag} is not a valid 2D window for {nameof(Repository2D)}.");
		}

		var repository = _GetRepository(window.PriorityType);

		repository.SetChild(window.transform,false);

		_Add(window);
	}

	private Transform _GetRepository(WindowPriorityType priorityType)
	{
		if(!m_repositoryDict.TryGetValue(priorityType,out var repository))
		{
			repository = transform.Find($"{priorityType}Repository");

			if(!repository)
			{
				throw new InvalidOperationException($"Not Found Repository. PriorityType : {priorityType}");
			}

			m_repositoryDict.Add(priorityType,repository);
		}

		return repository;
	}
}
