using System;

namespace DynamicNotifyPropertyChanged
{
	/// <summary>
	/// Set of extensions for <see cref="Type"/>.
	/// </summary>
	public static class TypeExtensions
	{
		private static readonly Func<object, object?> EmptyGetter = _ => default;

		private static readonly Action<object, object?> EmptySetter = (_,  _) => { };

		/// <summary>
		/// Get <see cref="Func{Object, Object}"/> that gets value of property called <see cref="propertyName"/> on passed object.
		/// </summary>
		/// <param name="source"><see cref="Type"/> containing property called <paramref name="propertyName"/>.</param>
		/// <param name="propertyName">Name fo the property.</param>
		/// <returns>Func that gets property value.</returns>
		public static Func<object, object?> GetPropertyGetter(this Type source, string propertyName)
		{
			return TypeCache
				.GetObjectGetterSetter(source, propertyName)
				.Getter
				.Value;
		}

		/// <summary>
		/// Get <see cref="Func{Object, Object}"/> that gets value of property called <see cref="propertyName"/> on passed object.
		/// </summary>
		/// <param name="source"><see cref="Type"/> containing property called <paramref name="propertyName"/>.</param>
		/// <param name="propertyName">Name fo the property.</param>
		/// <param name="getter">Func that gets property value.</param>
		/// <returns>True, if <paramref name="propertyName"/> exists on <paramref name="source"/>.</returns>
		public static bool TryGetPropertyGetter(this Type source, string propertyName, out Func<object, object?> getter)
		{
			if (TypeCache.TryGetObjectGetterSetter(source, propertyName, out var getterSetter) && getterSetter != null)
			{
				getter = getterSetter.Getter.Value;
				return true;
			}

			getter = EmptyGetter;
			return false;
		}

		/// <summary>
		/// Get <see cref="Func{Object, Object}"/> that sets value on property called <see cref="propertyName"/> on passed object.
		/// </summary>
		/// <param name="source"><see cref="Type"/> containing property called <paramref name="propertyName"/>.</param>
		/// <param name="propertyName">Name fo the property.</param>
		/// <returns>Action that sets property value.</returns>
		public static Action<object, object?> GetPropertySetter(this Type source, string propertyName)
		{
			return TypeCache
				.GetObjectGetterSetter(source, propertyName)
				.Setter
				.Value;
		}

		/// <summary>
		/// Get <see cref="Func{Object, Object}"/> that sets value on property called <see cref="propertyName"/> on passed object.
		/// </summary>
		/// <param name="source"><see cref="Type"/> containing property called <paramref name="propertyName"/>.</param>
		/// <param name="propertyName">Name fo the property.</param>
		/// <param name="setter">Action that sets property value.</param>
		/// <returns>True, if <paramref name="propertyName"/> exists on <paramref name="source"/>.</returns>
		public static bool TryGetPropertySetter(this Type source, string propertyName, out Action<object, object?> setter)
		{
			if (TypeCache.TryGetObjectGetterSetter(source, propertyName, out var getterSetter) && getterSetter != null)
			{
				setter = getterSetter.Setter.Value;
				return true;
			}

			setter = EmptySetter;
			return false;
		}
	}
}
