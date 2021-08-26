using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;

namespace DynamicNotifyPropertyChanged
{
	/// <summary>
	/// Empty class that implements <see cref="INotifyPropertyChanged"/> and <see cref="INotifyPropertyChanged"/>.
	/// </summary>
	public abstract class BaseNotifyPropertyChangedClass : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private readonly ConcurrentQueue<string> _pendingChangingNotifications = new();
		private readonly ConcurrentQueue<string> _pendingChangedNotifications = new();
		private bool _suspendNotifications;
		private bool _suspendRaisePropertyChangingNotifications;
		private bool _suspendRaisePropertyChangedNotifications;

		public event PropertyChangingEventHandler? PropertyChanging;

		public event PropertyChangedEventHandler? PropertyChanged;

		/// <summary>
		/// Suspend <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> until disposed.
		/// </summary>
		/// <returns><see cref="IDisposable"/> that enables and fires <see cref="PropertyChanged"/> and <see cref="PropertyChanging"/> notifications after <see cref="IDisposable.Dispose"/> is called.</returns>
		public IDisposable SuspendNotifications()
		{
			_suspendNotifications = true;
			return new AnonymousDisposable(() =>
			{
				_suspendNotifications = false;

				if (!_suspendRaisePropertyChangingNotifications)
				{
					RaisePendingChangingNotifications();
				}

				if (!_suspendRaisePropertyChangedNotifications)
				{
					RaisePendingChangedNotifications();
				}
			});
		}

		/// <summary>
		/// Suspend <see cref="PropertyChanging"/> until disposed.
		/// </summary>
		/// <returns><see cref="IDisposable"/> that enables and fires <see cref="PropertyChanging"/> notifications after <see cref="IDisposable.Dispose"/> is called.</returns>
		public IDisposable SuspendPropertyChangingNotifications()
		{
			_suspendRaisePropertyChangingNotifications = true;
			return new AnonymousDisposable(() =>
			{
				_suspendRaisePropertyChangingNotifications = false;

				if (!_suspendNotifications)
				{
					RaisePendingChangingNotifications();
				}
			});
		}

		/// <summary>
		/// Suspend <see cref="PropertyChanged"/> until disposed.
		/// </summary>
		/// <returns><see cref="IDisposable"/> that enables and fires <see cref="PropertyChanged"/> notifications after <see cref="IDisposable.Dispose"/> is called.</returns>
		public IDisposable SuspendPropertyChangedNotifications()
		{
			_suspendRaisePropertyChangedNotifications = true;
			return new AnonymousDisposable(() =>
			{
				_suspendRaisePropertyChangedNotifications = false;

				if (!_suspendNotifications)
				{
					RaisePendingChangedNotifications();
				}
			});
		}

		protected void OnPropertyChanging(string propertyName)
		{
			if (_suspendRaisePropertyChangingNotifications || _suspendNotifications)
			{
				_pendingChangingNotifications.Enqueue(propertyName);
			}
			else
			{
				PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
			}
		}

		protected void OnPropertyChanged(string propertyName)
		{
			if (_suspendRaisePropertyChangedNotifications || _suspendNotifications)
			{
				_pendingChangedNotifications.Enqueue(propertyName);
			}
			else
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private void RaisePendingChangingNotifications()
		{
			RaisePendingNotifications(_pendingChangingNotifications, OnPropertyChanging);
		}

		private void RaisePendingChangedNotifications()
		{
			RaisePendingNotifications(_pendingChangedNotifications, OnPropertyChanged);
		}

		private static void RaisePendingNotifications(ConcurrentQueue<string> queue, Action<string> callback)
		{
			// Trigger in sequence and avoid firing multiple times for same property.
			var properties = new HashSet<string>();
			while (queue.TryDequeue(out var propertyName) && !properties.Contains(propertyName))
			{
				properties.Add(propertyName);
				callback(propertyName);
			}
		}
	}
}
