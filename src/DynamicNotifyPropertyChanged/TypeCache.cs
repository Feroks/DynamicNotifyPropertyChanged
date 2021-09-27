using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace DynamicNotifyPropertyChanged
{
	internal static class TypeCache
	{
		private static readonly ConcurrentDictionary<Type, Lazy<ConcurrentDictionary<string, Lazy<DynamicObjectGetterSetter?>>>> Cache = new();

		internal static bool TryGetObjectGetterSetter(Type type, string propertyName, out DynamicObjectGetterSetter? getterSetter)
		{
			getterSetter = Cache
				.GetOrAdd(type, _ => new(() => new()))
				.Value
#if NETSTANDARD2_1
				.GetOrAdd(propertyName, static (x, t) => CreateLazyGetterSetter(t, x), type)
#else
				.GetOrAdd(propertyName, x =>  CreateLazyGetterSetter(type, x))
#endif
				?.Value;

			return getterSetter != null;
		}

		internal static DynamicObjectGetterSetter GetObjectGetterSetter(Type type, string propertyName)
		{
			return TryGetObjectGetterSetter(type, propertyName, out var getterSetter) && getterSetter != null
				? getterSetter
				: throw new ArgumentException("Property not found on type", nameof(propertyName));
		}

		private static Lazy<DynamicObjectGetterSetter?> CreateLazyGetterSetter(Type type, string propertyName)
		{
			return new(() => type.GetProperty(propertyName) != null
				? new(CreateLazyGetter(type, propertyName), CreateLazySetter(type, propertyName))
				: null);
		}

		private static Lazy<Func<object, object?>> CreateLazyGetter(Type type, string propertyName)
		{
			return new(() => CreateGetter(type, propertyName));
		}

		private static Func<object, object?> CreateGetter(Type objectType, string propertyName)
		{
			var objectParameter = Expression.Parameter(typeof(object), "source");
			var convert = Expression.Convert(objectParameter, objectType);
			var property = Expression.Property(convert, propertyName);
			var convertProperty = Expression.Convert(property, typeof(object));
			var lambda = Expression.Lambda<Func<object, object?>>(convertProperty, objectParameter);

			return lambda.Compile();
		}

		private static Lazy<Action<object, object?>> CreateLazySetter(Type objectType, string propertyName)
		{
			return new(() => CreateSetter(objectType, propertyName));
		}

		private static Action<object, object?> CreateSetter(Type objectType, string propertyName)
		{
			var objectParameter = Expression.Parameter(typeof(object), "source");
			var objectConvert = Expression.Convert(objectParameter, objectType);
			var property = Expression.Property(objectConvert, propertyName);

			var propertyValueParameter = Expression.Parameter(typeof(object), "value");
			var propertyValueConvert = Expression.Convert(propertyValueParameter, property.Type);
			var assign = Expression.Assign(property, propertyValueConvert);
			var lambda = Expression.Lambda<Action<object, object?>>(assign, objectParameter, propertyValueParameter);

			return lambda.Compile();
		}
	}
}
