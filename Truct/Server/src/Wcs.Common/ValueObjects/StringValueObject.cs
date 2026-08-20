using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Wcs.Common.ValueObjects;

/// <summary>
/// Abstract base class for string-based value objects with predefined constants.
/// Provides pattern for creating type-safe value objects that map to strings in database.
/// </summary>
/// <typeparam name="T">The derived value object type</typeparam>
public abstract class StringValueObject<T>(string value) where T : StringValueObject<T>
{
    protected static Dictionary<string, T> _values = new(StringComparer.OrdinalIgnoreCase);

    public string Value { get; protected init; } = value ?? throw new ArgumentNullException(nameof(value));

    /// <summary>
    /// Registers a value in the dictionary. Should be called in static constructor of derived class.
    /// </summary>
    protected static void Register(T t)
    {
        _values[t.Value] = t;
    }

    /// <summary>
    /// Gets all registered values of this value object type
    /// </summary>
    public static IEnumerable<T> GetAll()
    {
        RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
        return _values.Values;
    }

    /// <summary>
    /// Converts a string to the value object instance
    /// </summary>
    /// <param name="value">String value to convert</param>
    /// <returns>Value object instance</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not found</exception>
    public static T FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null or whitespace", nameof(value));
        }

        RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);

        var normalizedValue = value.Trim();

        if (_values.TryGetValue(normalizedValue, out var instance))
        {
            return instance;
        }

        throw new ArgumentException($"Invalid {typeof(T).Name} value: {value}", nameof(value));
    }

    /// <summary>
    /// Tries to convert a string to the value object instance
    /// </summary>
    /// <param name="value">String value to convert</param>
    /// <param name="instance">The value object instance if found</param>
    /// <returns>True if conversion succeeded, false otherwise</returns>
    public static bool TryFromString(string? value, out T? instance)
    {
        instance = null;
        
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);

        var normalizedValue = value.Trim();

        return _values.TryGetValue(normalizedValue, out instance);
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        if (obj is T other)
        {
            return Value == other.Value;
        }
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(StringValueObject<T>? left, StringValueObject<T>? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Value == right.Value;
    }

    public static bool operator !=(StringValueObject<T>? left, StringValueObject<T>? right)
    {
        return !(left == right);
    }
}
