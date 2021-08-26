using FluentAssertions;
using Xunit;

namespace DynamicNotifyPropertyChanged.Tests
{
	public class BaseNotifyPropertyChangedClassSuspendNotificationsShould
	{
		[Fact]
		public void NotRaisePropertyChangingUntilDisposed()
		{
			var isEventRaised = false;

			var fixture = new SuspendNotificationTestClass();
			fixture.SuspendNotifications();
			fixture.PropertyChanging += (_, _) => isEventRaised = true;

			fixture.Value = 1;

			isEventRaised
				.Should()
				.BeFalse();
		}
		
		[Fact]
		public void NotRaisePropertyChangingIfBlocked()
		{
			var isEventRaised = false;

			var fixture = new SuspendNotificationTestClass();
			fixture.SuspendPropertyChangingNotifications();
			using (fixture.SuspendNotifications())
			{
				fixture.PropertyChanging += (_, _) => isEventRaised = true;
				fixture.Value = 1;
			}

			fixture.Value = 1;

			isEventRaised
				.Should()
				.BeFalse();
		}

		[Fact]
		public void RaisePropertyChangingAfterDisposed()
		{
			var isEventRaised = false;

			var fixture = new SuspendNotificationTestClass();
			using (fixture.SuspendNotifications())
			{
				fixture.PropertyChanging += (_, _) => isEventRaised = true;
				fixture.Value = 1;
			}

			isEventRaised
				.Should()
				.BeTrue();
		}

		[Fact]
		public void RaisePropertyChangingAfterDisposedOnlyOnce()
		{
			var callCount = 0;

			var fixture = new SuspendNotificationTestClass();
			using (fixture.SuspendNotifications())
			{
				fixture.PropertyChanging += (_, _) => ++callCount;
				fixture.Value = 1;
				fixture.Value = 2;
			}

			callCount
				.Should()
				.Be(1);
		}
		
		[Fact]
		public void NotRaisePropertyChangedUntilDisposed()
		{
			var isEventRaised = false;

			var fixture = new SuspendNotificationTestClass();
			fixture.SuspendNotifications();
			fixture.PropertyChanged += (_, _) => isEventRaised = true;

			fixture.Value = 1;

			isEventRaised
				.Should()
				.BeFalse();
		}
		
		[Fact]
		public void NotRaisePropertyChangedIfBlocked()
		{
			var isEventRaised = false;

			var fixture = new SuspendNotificationTestClass();
			fixture.SuspendPropertyChangedNotifications();
			using (fixture.SuspendNotifications())
			{
				fixture.PropertyChanged += (_, _) => isEventRaised = true;
				fixture.Value = 1;
			}

			fixture.Value = 1;

			isEventRaised
				.Should()
				.BeFalse();
		}

		[Fact]
		public void RaisePropertyChangedAfterDisposed()
		{
			var isEventRaised = false;

			var fixture = new SuspendNotificationTestClass();
			using (fixture.SuspendNotifications())
			{
				fixture.PropertyChanged += (_, _) => isEventRaised = true;
				fixture.Value = 1;
			}

			isEventRaised
				.Should()
				.BeTrue();
		}

		[Fact]
		public void RaisePropertyChangedAfterDisposedOnlyOnce()
		{
			var callCount = 0;

			var fixture = new SuspendNotificationTestClass();
			using (fixture.SuspendNotifications())
			{
				fixture.PropertyChanged += (_, _) => ++callCount;
				fixture.Value = 1;
				fixture.Value = 2;
			}

			callCount
				.Should()
				.Be(1);
		}
	}
}
