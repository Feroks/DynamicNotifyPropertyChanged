using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DynamicNotifyPropertyChanged
{
	/// <summary>
	/// Empty class that implements <see cref="INotifyPropertyChanged"/> and <see cref="INotifyPropertyChanged"/>.
	/// </summary>
	public abstract class BaseNotifyPropertyChangedClass : INotifyPropertyChanging, INotifyPropertyChanged
	{
		public event PropertyChangingEventHandler? PropertyChanging;

		public event PropertyChangedEventHandler? PropertyChanged;

		protected void OnPropertyChanging([CallerMemberName] string? propertyName = null)
		{
			PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
		}

		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
