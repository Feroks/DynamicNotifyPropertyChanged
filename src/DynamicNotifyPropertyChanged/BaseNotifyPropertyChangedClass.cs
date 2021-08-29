using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;

namespace DynamicNotifyPropertyChanged
{
	/// <summary>
	/// Empty class that implements <see cref="INotifyPropertyChanging"/> and <see cref="INotifyPropertyChanged"/>.
	/// </summary>
	public abstract class BaseNotifyPropertyChangedClass : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private readonly ConcurrentQueue<string> _pendingChangedNotifications = new();
		private bool _suspendNotifications;
		private bool _suspendRaisePropertyChangingNotifications;
		private bool _suspendRaisePropertyChangedNotifications;

		public event PropertyChangingEventHandler? PropertyChanging;

		public event PropertyChangedEventHandler? PropertyChanged;

		/// <summary>
		/// Suspend <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> until disposed.
		/// </summary>
		/// <param name="raisePropertyChangedOnDispose">True, if <see cref="PropertyChanged"/> should be called for each changed property when <see cref="IDisposable.Dispose"/> is called.</param>
		/// <returns><see cref="IDisposable"/> that enables <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> notifications when <see cref="IDisposable.Dispose"/> is called and raises <see cref="PropertyChanged"/> for each changed property.</returns>
		public IDisposable SuspendNotifications(bool raisePropertyChangedOnDispose)
		{
			_suspendNotifications = true;
			return new AnonymousDisposable(() =>
			{
				_suspendNotifications = false;

				if (_suspendRaisePropertyChangedNotifications)
				{
					return;
				}

				if (raisePropertyChangedOnDispose)
				{
					RaisePendingChangedNotifications();
				}
				else
				{
					ClearPendingChangedNotifications();
				}
			});
		}

		/// <summary>
		/// Suspend <see cref="PropertyChanging"/> until disposed.
		/// </summary>
		/// <returns><see cref="IDisposable"/> that enables <see cref="PropertyChanging"/> notifications after <see cref="IDisposable.Dispose"/> is called.</returns>
		public IDisposable SuspendPropertyChangingNotifications()
		{
			// There is no point in firing OnPropertyChanging after notifications are enabled again, because value is already changed.
			_suspendRaisePropertyChangingNotifications = true;
			return new AnonymousDisposable(() => _suspendRaisePropertyChangingNotifications = false);
		}

		/// <summary>
		/// Suspend <see cref="PropertyChanged"/> until disposed.
		/// </summary>
		/// <param name="raisePropertyChangedOnDispose">True, if <see cref="PropertyChanged"/> should be called for each changed property when <see cref="IDisposable.Dispose"/> is called.</param>
		/// <returns><see cref="IDisposable"/> that enables and raises <see cref="PropertyChanged"/> notifications after <see cref="IDisposable.Dispose"/> is called.</returns>
		public IDisposable SuspendPropertyChangedNotifications(bool raisePropertyChangedOnDispose)
		{
			_suspendRaisePropertyChangedNotifications = true;
			return new AnonymousDisposable(() =>
			{
				_suspendRaisePropertyChangedNotifications = false;

				if (_suspendNotifications)
				{
					return;
				}

				if (raisePropertyChangedOnDispose)
				{
					RaisePendingChangedNotifications();
				}
				else
				{
					ClearPendingChangedNotifications();
				}
			});
		}

		protected void OnPropertyChanging(string propertyName)
		{
			if (!_suspendRaisePropertyChangingNotifications && !_suspendNotifications)
			{
				PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
			}
		}

		protected void OnPropertyChanged(string propertyName)
		{
			if (!_suspendRaisePropertyChangedNotifications && !_suspendNotifications)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
			else
			{
				_pendingChangedNotifications.Enqueue(propertyName);
			}
		}

		private void RaisePendingChangedNotifications()
		{
			// Raise in sequence and avoid firing multiple times for same property.
			var properties = new HashSet<string>();
			while (_pendingChangedNotifications.TryDequeue(out var propertyName) && !properties.Contains(propertyName))
			{
				properties.Add(propertyName);
				OnPropertyChanged(propertyName);
			}
		}

		private void ClearPendingChangedNotifications()
		{
#if NETSTANDARD2_1
			_pendingChangedNotifications.Clear();
#else
			while (_pendingChangedNotifications.TryDequeue(out _))
			{
			}
#endif
		}
	}
}
