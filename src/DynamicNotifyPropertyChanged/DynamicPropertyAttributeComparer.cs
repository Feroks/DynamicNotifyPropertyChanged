using System;
using System.Collections.Generic;

namespace DynamicNotifyPropertyChanged
{
	/// <summary>
	/// Class that compares <see cref="DynamicPropertyAttribute"/> by <see cref="DynamicPropertyAttribute.Type"/> name.
	/// </summary>
	internal class DynamicPropertyAttributeComparer : IComparer<DynamicPropertyAttribute>
	{
		public int Compare(DynamicPropertyAttribute x, DynamicPropertyAttribute y)
		{
			var xName = x.Type.FullName;
			var yName = y.Type.FullName;

			if (ReferenceEquals(xName, yName))
			{
				return 0;
			}

			if (ReferenceEquals(null, yName))
			{
				return 1;
			}

			return ReferenceEquals(null, xName) ? -1 : string.Compare(xName, yName, StringComparison.Ordinal);
		}
	}
}
