[![Nuget](https://img.shields.io/nuget/v/DynamicNotifyPropertyChanged)](https://www.nuget.org/packages/DynamicNotifyPropertyChanged/)

This library allows you to create types at runtime that implement `INotifyPropertyChanged` interface. Each property calls `OnPropertyChanged` when its value is changed.

## Creating Type
Dynamic class can be created by calling:
```csharp
Type DynamicNotifyPropertyChangedClassFactory.CreateType(DynamicProperty[] properties)
```

`DynamicProperty` contains two properties:
- Name
- Type

You must ensure that **duplicate properties are not present**. Created type is cached. If you pass same set of dynamic properties (even in different order) new type will not be created.

## Initializing Created Type
You can create new instance by calling:
```csharp
object DynamicNotifyPropertyChangedClassFactory.CreateTypeInstance(Type type)
```

Alternatively, you can create `Func` that returns new instance by calling:
```csharp
Func<object> DynamicNotifyPropertyChangedClassFactory.CreateTypeFactory(Type type)
```

Both methods use caching and `Reflection.Emit` internally for [better performance](https://andrewlock.net/benchmarking-4-reflection-methods-for-calling-a-constructor-in-dotnet/#the-results).

## Getting Property Value
You can get property value by calling:
```csharp
T ObjectExtensions.GetPropertyValue<T>(this object source, string propertyName)
```

Alternatively, you can create `Func` that gets property value from object by calling:
```csharp
Func<object, object> TypeExtensions.GetPropertyGetter(this Type type, string propertyName)
```

## Setting Property Value
You can set property value by calling:
```csharp
void ObjectExtensions.SetPropertyValue<T>(this object source, string propertyName, T value)
```

Alternatively, you can create `Func` that sets property value on object by calling:
```csharp
Action<object, object> TypeExtensions.GetPropertySetter(this Type type, string propertyName)
```

All methods described above use caching and compiled expressions for better performance.

## Batch changes

To improve performance during batch changes you can suspend `PropertyChanging`, `PropertyChanged` or both by calling:
```csharp
IDisposable SuspendPropertyChangingNotifications()
```

```csharp
IDisposable SuspendPropertyChangedNotifications(bool raisePropertyChangedOnDispose)
```

```csharp
IDisposable SuspendNotifications(bool raisePropertyChangedOnDispose)
```

After `IDisposable` is disposed, events will start firing again.
If you wish to trigger `PropertyChanged` for all changed properties after `Dispose` is called, then set `raisePropertyChangedOnDispose` to `true`.