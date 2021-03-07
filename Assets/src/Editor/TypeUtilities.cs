using System;
using System.Collections.Generic;
using System.Reflection;

namespace NodeGraph
{
	public class TypeUtilities
	{
		private static Assembly[] _assemblyList;

		public static void GetAllSubclasses(Type type, List<Type> types)
		{
			if (_assemblyList == null || _assemblyList.Length == 0)
			{
				_assemblyList = AppDomain.CurrentDomain.GetAssemblies();
			}

			types.Clear();
			for (int i = 0; i < _assemblyList.Length; i++)
			{
				Assembly assembly = _assemblyList[i];
				Type[] assemblyTypes = assembly.GetTypes();
				for (int j = 0; j < assemblyTypes.Length; j++)
				{
					Type currentType = assemblyTypes[j];
					if (currentType.IsSubclassOf(type))
					{
						types.Add(currentType);
					}
				}
			}
		}
	}
}