using System;

namespace DynamicNotifyPropertyChanged
{
	internal class AnonymousDisposable : IDisposable
	{
		private readonly Action _action;

		internal AnonymousDisposable(Action action)
		{
			_action = action;
		}

		public void Dispose()
		{
			_action();
		}
	}
}
