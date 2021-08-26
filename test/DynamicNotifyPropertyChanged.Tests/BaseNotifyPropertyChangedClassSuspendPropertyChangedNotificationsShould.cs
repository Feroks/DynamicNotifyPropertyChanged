using FluentAssertions;
using Xunit;

namespace DynamicNotifyPropertyChanged.Tests
{
	public class BaseNotifyPropertyChangedClassSuspendPropertyChangedNotificationsShould
	{
		[Fact]
		public void NotRaisePropertyChangedUntilDisposed()
		{
			var isEventRaised = false;

			var fixture = new SuspendNotificationTestClass();
			fixture.SuspendPropertyChangedNotifications();
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
			fixture.SuspendNotifications();
			using (fixture.SuspendPropertyChangedNotifications())
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
			using (fixture.SuspendPropertyChangedNotifications())
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
			using (fixture.SuspendPropertyChangedNotifications())
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
