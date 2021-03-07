using System;
using System.Collections.Generic;

public class TypeUtilities
{
	private static System.Reflection.Assembly[] _assemblyList;

	public static void GetAllSubclasses(System.Type _class, List<System.Type> _subclassList)
	{
		if (_assemblyList == null || _assemblyList.Length == 0)
		{
			_assemblyList = System.AppDomain.CurrentDomain.GetAssemblies();
		}

		_subclassList.Clear();

		foreach (var assembly in _assemblyList)
		{
			System.Type[] types = assembly.GetTypes();
			foreach (var currentType in types)
			{
				if (currentType.IsSubclassOf(_class))
				{
					_subclassList.Add(currentType);
				}
			}
		}
	}
}
