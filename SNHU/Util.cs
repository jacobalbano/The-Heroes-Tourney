
using System;

namespace SNHU
{
	/// <summary>
	/// Description of Util.
	/// </summary>
	public static class Util
	{
		public static int GetEnumFromName(Type enumType, string name)
		{
			var modeNames = Enum.GetNames(enumType);
			var modeValues = (int[]) Enum.GetValues(enumType);
			
			for (int i = 0; i < modeNames.Length; i++)
			{
				if (modeNames[i] == name)
					return modeValues[i];
			}
			
			throw new Exception(string.Format("{0} is not a valid value for enum {1}.", name, enumType.Name));
		}
	}
}
