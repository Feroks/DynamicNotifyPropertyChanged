using FluentAssertions;
using Xunit;

namespace DynamicNotifyPropertyChanged.Tests
{
	public class BaseNotifyPropertyChangedClassSuspendPropertyChangingNotificationsShould
	{
		[Fact]
		public void NotRaisePropertyChangingUntilDisposed()
		{
			var isEventRaised = false;

			var fixture = new SuspendNotificationTestClass();
			fixture.SuspendPropertyChangingNotifications();
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
			fixture.SuspendNotifications();
			using (fixture.SuspendPropertyChangingNotifications())
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
			using (fixture.SuspendPropertyChangingNotifications())
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
			using (fixture.SuspendPropertyChangingNotifications())
			{
				fixture.PropertyChanging += (_, _) => ++callCount;
				fixture.Value = 1;
				fixture.Value = 2;
			}

			callCount
				.Should()
				.Be(1);
		}
	}
}
