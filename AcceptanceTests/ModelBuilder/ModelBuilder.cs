using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace AcceptanceTests.ModelBuilder;

public sealed class ModelBuilder<T> where T : class
{
    private readonly Dictionary<string, object?> _propertiesAssignedUsingSet = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, object?> _propertiesAssignedUsingFor = new(StringComparer.OrdinalIgnoreCase);
    private readonly Type _type;
    private readonly ParameterInfo[]? _constructorParameterInfos;

    public ModelBuilder()
    {
        _type = typeof(T);
        var constructors = _type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        foreach (var constructor in constructors)
        {
            var parameterInfos = constructor.GetParameters();
            if (parameterInfos.Length == 0)
            {
                continue;
            }
            
            _constructorParameterInfos = parameterInfos;
            break;
        }
    }

    /// <summary>
    /// At the time of constructing an instance of the Builder, you should call the For method
    /// Each time you call this method you can assign a delegate to a property of your model
    /// This delegate will be called in order to set the value of the property in question
    /// You should call the For method once for each property of your model.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="propertyNameExpression"></param>
    /// <param name="valueGetterDelegate"></param>
    /// <returns></returns>
    public ModelBuilder<T> For<TValue>(Expression<Func<T, TValue>> propertyNameExpression, Func<object> valueGetterDelegate)
    {
        var memberExpression = (MemberExpression)propertyNameExpression.Body;
        var propertyName = memberExpression.Member.Name;
        _propertiesAssignedUsingFor[propertyName] = valueGetterDelegate;
        return this;
    }

    /// <summary>
    /// In your unit test methods you'll generally call the Set method for each property you want to take control of in your test.
    /// Call the Set method for each property whose value you want to change from a randomly generated value.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="propertyNameExpression"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public ModelBuilder<T> Set<TValue>(Expression<Func<T, TValue>> propertyNameExpression, TValue value)
    {
        var memberExpression = (MemberExpression)propertyNameExpression.Body;
        var propertyName = memberExpression.Member.Name;
        _propertiesAssignedUsingSet[propertyName] = value;
        return this;
    }

    /// <summary>
    /// Before calling the Build Method, you may want to do the following:
    /// 1. Use the For method to assign delegates (method that will set the property) for each of the properties of your model
    /// 2. Use the Set method to override/control the random value being set for one or more properties.
    /// 3. Then call the Build Method 
    /// </summary>
    /// <returns></returns>
    public T Build()
    {
        var instance = CreateInstanceUsingSetOrDefaultProperties();
        _propertiesAssignedUsingSet.Clear();
        return instance;
    }

    private T CreateInstanceUsingSetOrDefaultProperties()
    {
        var constructorParams = new object[_constructorParameterInfos!.Length];
        for (var i = 0; i < _constructorParameterInfos.Length; i++)
        {
            var parameterInfo = _constructorParameterInfos[i];
            constructorParams[i] = GetPropertyValue(parameterInfo.Name!, parameterInfo.ParameterType.Name)!;
        }

        var instance = (T)Activator.CreateInstance(_type, constructorParams)!;
        return instance;
    }

    private object? GetPropertyValue(string propertyName, string propertyTypeName)
    {
        if (_propertiesAssignedUsingSet.TryGetValue(propertyName, out var setValue))
        {
            return setValue;
        }

        return _propertiesAssignedUsingFor.TryGetValue(propertyName, out var forValueDelegate)
            ? ((Func<object>)forValueDelegate!)()
            : throw new ModelBuilderPropertyNotSetException($"The builder was not given a value or delegate to set the value of {_type.Name}.{propertyName} property of type {propertyTypeName}. Please use the Set or For methods on the builder to specify a value for the property.");
    }
}