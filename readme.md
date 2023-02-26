[![Nuget](https://img.shields.io/nuget/v/DynamicNotifyPropertyChanged)](https://www.nuget.org/packages/DynamicNotifyPropertyChanged/)

This library allows you to create types at runtime that implement `INotifyPropertyChanging` and `INotifyPropertyChanged` interfaces. Each property calls `OnPropertyChanging` and `OnPropertyChanged` in its setter.

## Creating Type
Dynamic class can be created by calling:
```csharp
Type DynamicNotifyPropertyChangedClassFactory.CreateType(DynamicProperty[] properties)
```

`DynamicProperty` contains following properties:
- Name
- Type
- RaisePropertyChanging
- RaisePropertyChanged

By default both `OnPropertyChanging` and `OnPropertyChanged` are injected into property setter, but you can disable it by changing `RaisePropertyChanging` and `RaisePropertyChanged` values.

You must ensure that **duplicate properties are not present**.
Created type is cached, e.g. if you pass same set of `DynamicProperty` (even in different order) new type will not be created.

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

## Getting/Setting Property Value
Use [DynamicPropertyAccess](https://github.com/Feroks/DynamicPropertyAccess) to get or set property values without reflection. 

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