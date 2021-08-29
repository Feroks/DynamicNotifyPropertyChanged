namespace DynamicNotifyPropertyChanged.Tests
{
	public class SuspendNotificationTestClass : BaseNotifyPropertyChangedClass
	{
		private int _value;

		public int Value
		{
			get => _value;
			set
			{
				OnPropertyChanging(nameof(Value));
				_value = value;
				OnPropertyChanged(nameof(Value));
			}
		}
	}
}
