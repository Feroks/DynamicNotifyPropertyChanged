using System;
using System.Collections.Generic;

namespace DynamicNotifyPropertyChanged
{
	/// <summary>
	/// Class that compares <see cref="DynamicPropertyAttributeProperty"/> by <see cref="DynamicPropertyAttributeProperty.Name"/>.
	/// </summary>
	internal class DynamicPropertyAttributePropertyComparer : IComparer<DynamicPropertyAttributeProperty>
	{
		public int Compare(DynamicPropertyAttributeProperty x, DynamicPropertyAttributeProperty y)
		{
			if (ReferenceEquals(x, y))
			{
				return 0;
			}

			if (ReferenceEquals(null, y))
			{
				return 1;
			}

			return ReferenceEquals(null, x) ? -1 : string.Compare(x.Name, y.Name, StringComparison.Ordinal);
		}
	}
}
