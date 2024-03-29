﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace DynamicNotifyPropertyChanged
{
	
	/// <summary>
	/// Factory capable of creating <see cref="Type"/> at runtime that implement <see cref="INotifyPropertyChanged"/>.
	/// </summary>
	[SuppressMessage("ReSharper", "ForCanBeConvertedToForeach", Justification = "Performance")]
	public static class DynamicNotifyPropertyChangedClassFactory
	{
		private const string OnPropertyChangingName = "OnPropertyChanging";
		private const string OnPropertyChangedName = "OnPropertyChanged";
		private static readonly DynamicPropertyComparer DynamicPropertyComparer = new();
		private static readonly DynamicPropertyAttributeComparer DynamicPropertyAttributeComparer = new();
		private static readonly DynamicPropertyAttributePropertyComparer DynamicPropertyAttributePropertyComparer = new();
		private static readonly ConcurrentDictionary<string, Lazy<Type>> DynamicTypeCache = new();
		private static readonly ConcurrentDictionary<Type, Lazy<Func<object>>> InitializationCache = new();
		private static readonly ConcurrentDictionary<string, Lazy<ConstructorInfo>> TypeConstructorInfoCache = new();
		private static readonly ConcurrentDictionary<Type, Lazy<PropertyInfo[]>> TypePropertyInfoCache = new();
		private static readonly ConstructorInfo ObjectCtor;
		private static readonly Type BaseClassType;
		private static readonly MethodInfo OnPropertyChangingMethodInfo;
		private static readonly MethodInfo OnPropertyChangedMethodInfo;
		private static readonly ModuleBuilder ModuleBuilder;
		private static int _index;

		static DynamicNotifyPropertyChangedClassFactory()
		{
			ObjectCtor = typeof(object).GetConstructor(Type.EmptyTypes)!;
			BaseClassType = typeof(BaseNotifyPropertyChangedClass);
			
			OnPropertyChangingMethodInfo = BaseClassType.GetMethod(
				OnPropertyChangingName,
				BindingFlags.Instance | BindingFlags.NonPublic)!;
			OnPropertyChangedMethodInfo = BaseClassType.GetMethod(
				OnPropertyChangedName,
				BindingFlags.Instance | BindingFlags.NonPublic)!;

			ModuleBuilder = AssemblyBuilder
				.DefineDynamicAssembly(
					new AssemblyName("DynamicNotifyPropertyChanged, Version=1.0.0.0"),
					AssemblyBuilderAccess.Run)
				.DefineDynamicModule("DynamicNotifyPropertyChanged");
		}

		/// <summary>
		/// Create <see cref="Type"/> that implements <see cref="INotifyPropertyChanged"/> for <paramref name="properties"/> and cache it.
		/// </summary>
		/// <param name="properties">List of properties to define on <see cref="Type"/>.</param>
		/// <remarks>Use <see cref="CreateTypeFactory"/> or <see cref="CreateTypeInstance"/> to initialize object via IL.</remarks>
		/// <returns>Dynamically created type that implements <see cref="INotifyPropertyChanged"/>.</returns>
		public static Type CreateType(DynamicProperty[] properties)
		{
			return DynamicTypeCache
				.GetOrAdd(CreateKey(properties), _ => new(() => CreateTypeInternal(properties)))
				.Value;
		}

		/// <summary>
		/// Create Func that creates <paramref name="type"/> by generating and caching IL code.
		/// </summary>
		/// <param name="type">Type of object to create that has parameterless constructor.</param>
		/// <returns>Func that creates instance of <paramref name="type"/>.</returns>
		public static Func<object> CreateTypeFactory(Type type)
		{
			return GetTypeFactory(type);
		}

		/// <summary>
		/// Create instance of <paramref name="type"/> by generating and caching IL code.
		/// </summary>
		/// <param name="type">Type of object to create that has parameterless constructor.</param>
		/// <returns>Instance of <paramref name="type"/>.</returns>
		public static object CreateTypeInstance(Type type)
		{
			return GetTypeFactory(type)();
		}

		private static Type CreateTypeInternal(IReadOnlyList<DynamicProperty> properties)
		{
			// Properties must have unique case insensitive names
			var createdProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			// Name must be unique
			var index = Interlocked.Increment(ref _index);
			var name = $"<>f__AnonymousNotifyPropertyChangedType{index.ToString()}";

			var typeBuilder = ModuleBuilder.DefineType(
				name,
				TypeAttributes.AnsiClass | TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoLayout | TypeAttributes.BeforeFieldInit, 
				BaseClassType);

			// Constructor
			var constructor = typeBuilder.DefineConstructor(
				MethodAttributes.Public,
				CallingConventions.Standard,
				Type.EmptyTypes);

			var ilConstructor = constructor.GetILGenerator();
			ilConstructor.Emit(OpCodes.Ldarg_0);
			ilConstructor.Emit(OpCodes.Call, ObjectCtor);
			ilConstructor.Emit(OpCodes.Ret);

			for (var i = 0; i < properties.Count; i++)
			{
				var property = properties[i];
				var propertyName = property.Name;
				var propertyType = property.Type;

				if (!createdProperties.Add(propertyName))
				{
					throw new($"{propertyName} is defined twice. Properties on class must have unique name.");
				}

				// Backing Field
				var fieldBuilder = typeBuilder.DefineField(
					$"_{propertyName}",
					propertyType,
					FieldAttributes.Private);

				// Property
				var propertyBuilder = typeBuilder.DefineProperty(
					propertyName,
					PropertyAttributes.None,
					propertyType,
					Type.EmptyTypes);

				// Attributes
				foreach (var attribute in property.Attributes)
				{
					var customAttributeBuilder = CreateCustomAttributeBuilder(attribute);
					propertyBuilder.SetCustomAttribute(customAttributeBuilder);
				}
				
				// Getter
				var getterBuilder = typeBuilder.DefineMethod(
					$"get_{propertyName}",
					MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
					propertyType,
					Type.EmptyTypes);

				var ilGetter = getterBuilder.GetILGenerator();
				ilGetter.Emit(OpCodes.Ldarg_0);
				ilGetter.Emit(OpCodes.Ldfld, fieldBuilder);
				ilGetter.Emit(OpCodes.Ret);
				propertyBuilder.SetGetMethod(getterBuilder);

				// Setter
				var setterBuilder = typeBuilder.DefineMethod(
					$"set_{propertyName}",
					MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
					null,
					new[] { propertyType });

				var ilSetter = setterBuilder.GetILGenerator();
				
				// Property Changing
				if (property.RaisePropertyChanging)
				{
					ilSetter.Emit(OpCodes.Ldarg_0);
					ilSetter.Emit(OpCodes.Ldstr, propertyName);
					ilSetter.Emit(OpCodes.Callvirt, OnPropertyChangingMethodInfo);
				}
				
				// Property value set
				ilSetter.Emit(OpCodes.Ldarg_0);
				ilSetter.Emit(OpCodes.Ldarg_1);
				ilSetter.Emit(OpCodes.Stfld, fieldBuilder);
				
				// Property Changed
				if (property.RaisePropertyChanged)
				{
					ilSetter.Emit(OpCodes.Ldarg_0);
					ilSetter.Emit(OpCodes.Ldstr, propertyName);
					ilSetter.Emit(OpCodes.Callvirt, OnPropertyChangedMethodInfo);
				}
				ilSetter.Emit(OpCodes.Ret);

				propertyBuilder.SetSetMethod(setterBuilder);
			}

			return typeBuilder.CreateTypeInfo()?.AsType() ?? throw new ("Failed to create type");
		}
		
		/// <summary>
		/// Clear all cached data.
		/// </summary>
		public static void ClearCache()
		{
			DynamicTypeCache.Clear();
			InitializationCache.Clear();
			TypeConstructorInfoCache.Clear();
			TypePropertyInfoCache.Clear();
		}

		private static CustomAttributeBuilder CreateCustomAttributeBuilder(DynamicPropertyAttribute attribute)
		{
			var type = attribute.Type;
			
			// Constructor
			var constructor = TypeConstructorInfoCache
				.GetOrAdd(
					CreateKey(attribute),
					_ => new(() => type.GetConstructor(attribute.ConstructorArgsTypes) ?? throw new Exception($"Failed to resolve constructor for {attribute}")))
				.Value;
			
			// Property set
			var attributeProperties = attribute.Properties;
			var properties = new PropertyInfo[attributeProperties.Length];
			var propertyValues = new object[attributeProperties.Length];

			var propertyInfos = attributeProperties.Length > 0
				? TypePropertyInfoCache
					.GetOrAdd(
						type,
						x => new (() => x.GetProperties(BindingFlags.Public | BindingFlags.Instance)))
					.Value
				: Array.Empty<PropertyInfo>();

			for (var i = 0; i < attributeProperties.Length; i++)
			{
				var attributeProperty = attributeProperties[i];
				var propertyInfo = Array.Find(propertyInfos, x => x.Name == attributeProperty.Name) ?? throw new Exception($"Failed to find property: {attributeProperty.Name} on attribute: {attribute.Type}");

				properties[i] = propertyInfo;
				propertyValues[i] = attributeProperty.Value;
			}

			return new CustomAttributeBuilder(
				constructor,
				attribute.ConstructorArgs,
				properties,
				propertyValues);
		}

		private static Func<object> CreateTypeFactoryInternal(Type type)
		{
			var dynamicMethod = new DynamicMethod(
				string.Empty,
				type,
				Type.EmptyTypes,
				ModuleBuilder);

			var ilDynamicMethod = dynamicMethod.GetILGenerator();
			ilDynamicMethod.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes)!);
			ilDynamicMethod.Emit(OpCodes.Ret);

			return (Func<object>)dynamicMethod.CreateDelegate(typeof(Func<object>));
		}

		private static Func<object> GetTypeFactory(Type type)
		{
			return InitializationCache
				.GetOrAdd(type, static x => new(() => CreateTypeFactoryInternal(x)))
				.Value;
		}

		private static string CreateKey(DynamicProperty[] properties)
		{
			var sb = new StringBuilder();

			// Apply sort to avoid creating different type with identical properties in different order
			Array.Sort(properties, DynamicPropertyComparer);
			for (var i = 0; i < properties.Length; i++)
			{
				var property = properties[i];
				sb.AppendFormat(
					"{0}~{1}_{2}_{3}|",
					property.Name,
					property.Type.FullName,
					property.RaisePropertyChanging.ToString(),
					property.RaisePropertyChanged.ToString());
				
				Array.Sort(property.Attributes, DynamicPropertyAttributeComparer);
				for (var j = 0; j < property.Attributes.Length; j++)
				{
					var attribute = property.Attributes[j];
					sb.AppendFormat("|Attribute_{0}", attribute.Type.FullName);
					
					// Do not sort, because order is important for constructor arguments
					for (var k = 0; k < attribute.ConstructorArgsTypes.Length; k++)
					{
						var argType = attribute.ConstructorArgsTypes[k];
						var arg = attribute.ConstructorArgs[k];
						sb.AppendFormat("|AttributeConstructorArg_{0}_{1}", argType.FullName, arg);
					}

					Array.Sort(attribute.Properties, DynamicPropertyAttributePropertyComparer);
					for (var k = 0; k < attribute.Properties.Length; k++)
					{
						var attributeProperty = attribute.Properties[k];
						sb.AppendFormat("|AttributeProperty_{0}_{1}", attributeProperty.Name, attributeProperty.Value);
					}
				}

				sb.Append('|');
			}

			return sb.ToString();
		}
		
		private static string CreateKey(DynamicPropertyAttribute attribute)
		{
			var sb = new StringBuilder(attribute.Type.FullName);

			// Do not sort, because order is important for constructor arguments
			for (var i = 0; i < attribute.ConstructorArgsTypes.Length; i++)
			{
				var argType = attribute.ConstructorArgsTypes[i];
				var arg = attribute.ConstructorArgs[i];
				sb.AppendFormat("_{0}_{1}|", argType.FullName, arg);
			}

			return sb.ToString();
		}
	}
}
