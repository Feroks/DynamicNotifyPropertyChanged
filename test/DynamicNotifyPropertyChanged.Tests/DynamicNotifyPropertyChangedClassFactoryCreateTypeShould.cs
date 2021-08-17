using FluentAssertions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xunit;

namespace DynamicNotifyPropertyChanged.Tests
{
	public class DynamicNotifyPropertyChangedClassFactoryCreateTypeShould
	{
		private readonly DynamicProperty _property1 = new("Property1", typeof(int));
		private readonly DynamicProperty _property2 = new("Property2", typeof(string));

		[Fact]
		public void CreateClassWithParameterlessConstructor()
		{
			var type = DynamicNotifyPropertyChangedClassFactory.CreateType(new [] { _property1, _property2});

			Activator
				.CreateInstance(type)
				.Should()
				.NotBeNull();
		}
		
		[Fact]
		public void CreateAllProperties()
		{
			var type = DynamicNotifyPropertyChangedClassFactory.CreateType(new[] { _property1, _property2 });

			type
				.Should()
				.HaveProperty(_property1.Type, _property1.Name)
				.And
				.HaveProperty(_property2.Type, _property2.Name);
		}
		
		[Fact]
		public void CreateClassThatInheritsINotifyPropertyChanged()
		{
			var type = DynamicNotifyPropertyChangedClassFactory.CreateType(Array.Empty<DynamicProperty>());

			type
				.Should()
				.BeAssignableTo<INotifyPropertyChanged>();
		}

		[Fact]
		public void CreateTypeWithFunctionalGettersAndSetters()
		{
			var properties = new[] { _property1, _property2 };

			var type = DynamicNotifyPropertyChangedClassFactory.CreateType(properties);
			var instance = Activator.CreateInstance(type);

			foreach (var property in type.GetProperties())
			{
				var value = GetValue(property.PropertyType);
				property.SetValue(instance, value);
				property
					.GetValue(instance)
					.Should()
					.Be(value);
			}
		}

		[Fact]
		public void AddRaisePropertyChangedInSetter()
		{
			var properties = new[] { _property1, _property2 };

			var type = DynamicNotifyPropertyChangedClassFactory.CreateType(properties);
			var instance = Activator.CreateInstance(type);

			var changedProperties = new List<string>(properties.Length);

			var npc = (INotifyPropertyChanged)instance!;
			npc.PropertyChanged += (_, args) => changedProperties.Add(args.PropertyName!);

			foreach (var property in type.GetProperties())
			{
				property.SetValue(instance, GetValue(property.PropertyType));
			}

			changedProperties
				.Should()
				.HaveCount(2)
				.And
				.ContainInOrder(_property1.Name, _property2.Name);
		}

		[Fact]
		public void CacheType()
		{
			var type1 = DynamicNotifyPropertyChangedClassFactory.CreateType(Array.Empty<DynamicProperty>());
			var type2 = DynamicNotifyPropertyChangedClassFactory.CreateType(Array.Empty<DynamicProperty>());

			type1
				.Should()
				.Be(type2);
		}

		[Fact]
		public void ReturnedCachedTypeForDifferentlyOrderedProperties()
		{
			var type1 = DynamicNotifyPropertyChangedClassFactory.CreateType(new[] { _property1, _property2 });
			var type2 = DynamicNotifyPropertyChangedClassFactory.CreateType(new[] { _property2, _property1 });

			type1
				.Should()
				.Be(type2);
		}
		
		[Fact]
		public void BeThreadSafe()
		{
			const int count = 100;

			var types = Enumerable
				.Range(0, count)
				.AsParallel()
				.Select(x =>
				{
					var properties = Enumerable
						.Range(0, x)
						.Select(y => new DynamicProperty($"Property{y}", typeof(int)))
						.ToArray();

					return DynamicNotifyPropertyChangedClassFactory.CreateType(properties);
				})
				.ToArray();

			types
				.Should()
				.OnlyHaveUniqueItems();
		}
		
		private static object GetValue(Type type)
		{
			if (type == typeof(string))
			{
				return "Value";
			}

			if (type == typeof(int))
			{
				return 1;
			}

			throw new ArgumentOutOfRangeException(nameof(type), type, "Value for type is not defined");
		}
	}
}
