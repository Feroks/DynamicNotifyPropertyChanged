using System;
using System.Collections.Generic;

namespace DynamicNotifyPropertyChanged
{
	/// <summary>
	/// Class describing attribute used on property.
	/// </summary>
	public class DynamicPropertyAttribute
	{
		/// <param name="type">Type of the attribute.</param>		
		public DynamicPropertyAttribute(Type type)
		{
			Type = type;
		}

		/// <param name="type">Type of the attribute.</param>		
		/// <param name="constructorArgs">Array of parameters passed to constructor.</param>		
		/// <param name="constructorArgsTypes">Collection of property setters for attribute.</param>		
		public DynamicPropertyAttribute(Type type, object?[] constructorArgs, Type[] constructorArgsTypes)
			: this(type)
		{
			if (constructorArgs.Length != constructorArgsTypes.Length)
			{
				throw new ArgumentException($"{nameof(constructorArgs)} length must be equal to {nameof(constructorArgsTypes)}");
			}

			ConstructorArgs = constructorArgs;
			ConstructorArgsTypes = constructorArgsTypes;
		}

		/// <summary>
		/// Attribute type.
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Array of parameters passed to constructor.
		/// </summary>
		public object?[] ConstructorArgs { get; } = Array.Empty<object?>();

		/// <summary>
		/// Array of parameter types passed to constructor.
		/// </summary>
		public Type[] ConstructorArgsTypes { get; } = Array.Empty<Type>();

		/// <summary>
		/// Collection of property setters for attribute.
		/// </summary>
		public IReadOnlyList<DynamicPropertyAttributeProperty> Properties { get; set; } = Array.Empty<DynamicPropertyAttributeProperty>();
	}
}
