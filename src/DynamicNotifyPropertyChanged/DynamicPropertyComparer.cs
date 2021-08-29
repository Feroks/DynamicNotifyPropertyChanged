using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("DynamicNotifyPropertyChanged.Tests")]
namespace DynamicNotifyPropertyChanged
{
	/// <summary>
	/// Class that compares <see cref="DynamicProperty"/> by <see cref="DynamicProperty.Name"/> and then by <see cref="DynamicProperty.Type"/> using <see cref="Type.FullName"/>.
	/// </summary>
	internal class DynamicPropertyComparer : IComparer<DynamicProperty>
	{
		/// <inheritdoc cref="IComparer{T}.Compare"/>.
		public int Compare(DynamicProperty x, DynamicProperty y)
		{
			var value = string.CompareOrdinal(x.Name, y.Name);
			if (value != 0)
			{
				return value;
			}

			value = string.CompareOrdinal(x.Type.FullName, y.Type.FullName);
			if (value != 0)
			{
				return value;
			}

			value = x.RaisePropertyChanged.CompareTo(y.RaisePropertyChanged);
			if (value != 0)
			{
				return value;
			}
			
			return x.RaisePropertyChanging.CompareTo(y.RaisePropertyChanging);
		}
	}
}
