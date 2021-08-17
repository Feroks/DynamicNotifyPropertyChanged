using System;
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
		private const string OnPropertyChangedName = "OnPropertyChanged";
		private static readonly DynamicPropertyComparer DynamicPropertyComparer = new();
		private static readonly ConcurrentDictionary<string, Lazy<Type>> TypeCache = new();
		private static readonly ConcurrentDictionary<Type, Lazy<Func<object>>> InitializationCache = new();
		private static readonly ConstructorInfo ObjectCtor;
		private static readonly Type BaseClassType;
		private static readonly MethodInfo OnPropertyChangedMethodInfo;
		private static readonly ModuleBuilder ModuleBuilder;
		private static int _index;

		static DynamicNotifyPropertyChangedClassFactory()
		{
			ObjectCtor = typeof(object).GetConstructor(Type.EmptyTypes)!;
			BaseClassType = typeof(BaseNotifyPropertyChangedClass);
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
			return TypeCache
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
				null);

			var ilConstructor = constructor.GetILGenerator();
			ilConstructor.Emit(OpCodes.Ldarg_0);
			ilConstructor.Emit(OpCodes.Call, ObjectCtor);
			ilConstructor.Emit(OpCodes.Ret);

			for (var i = 0; i < properties.Count; i++)
			{
				var property = properties[i];
				var propertyName = property.Name;
				var propertyType = property.Type;

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
					null);

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
				ilSetter.Emit(OpCodes.Ldarg_0);
				ilSetter.Emit(OpCodes.Ldarg_1);
				ilSetter.Emit(OpCodes.Stfld, fieldBuilder);
				ilSetter.Emit(OpCodes.Ldarg_0);
				ilSetter.Emit(OpCodes.Ldstr, propertyName);
				ilSetter.Emit(OpCodes.Callvirt, OnPropertyChangedMethodInfo);
				ilSetter.Emit(OpCodes.Ret);

				propertyBuilder.SetSetMethod(setterBuilder);
			}

			return typeBuilder.CreateTypeInfo()?.AsType() ?? throw new ("Failed to create type");
		}

		private static Func<object> CreateTypeFactoryInternal(Type type)
		{
			var dynamicMethod = new DynamicMethod(
				string.Empty,
				type,
				null,
				ModuleBuilder);

			ILGenerator ilDynamicMethod = dynamicMethod.GetILGenerator();
			ilDynamicMethod.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes)!);
			ilDynamicMethod.Emit(OpCodes.Ret);

			return (Func<object>)dynamicMethod.CreateDelegate(typeof(Func<object>));
		}

		private static Func<object> GetTypeFactory(Type type)
		{
			return InitializationCache
				.GetOrAdd(type, x => new(() => CreateTypeFactoryInternal(x)))
				.Value;
		}

		private static string CreateKey(DynamicProperty[] properties)
		{
			// Apply sort to avoid creating different type with identical properties in different order
			Array.Sort(properties, DynamicPropertyComparer);

			var sb = new StringBuilder();

			for (var i = 0; i < properties.Length; i++)
			{
				var property = properties[i];
				sb.AppendFormat("{0}~{1}|", property.Name, property.Type.FullName);
			}

			return sb.ToString();
		}
	}
}
