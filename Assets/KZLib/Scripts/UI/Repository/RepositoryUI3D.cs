using UnityEngine;
using System.Collections.Generic;
using System;

public class RepositoryUI3D : RepositoryUI
{
	protected override bool IsValid(WindowUI _window)
	{
		return _window != null && _window.Is3D && _window is WindowUI3D;
	}
}