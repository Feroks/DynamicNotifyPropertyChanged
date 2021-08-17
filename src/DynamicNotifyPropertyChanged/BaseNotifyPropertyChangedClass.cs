using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DynamicNotifyPropertyChanged
{
	/// <summary>
	/// Empty class that implements <see cref="INotifyPropertyChanged"/>.
	/// </summary>
	public abstract class BaseNotifyPropertyChangedClass : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
