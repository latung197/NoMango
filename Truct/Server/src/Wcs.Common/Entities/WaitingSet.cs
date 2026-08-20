using System.Text.Json.Serialization;

namespace Wcs.Common.Entities;

public sealed class WaitingSet
{
    private readonly HashSet<string> _needs = new();
    private readonly HashSet<string> _met = new();

    [JsonPropertyName("needs")]
    public IEnumerable<string> Needs => _needs.ToList();

    [JsonPropertyName("met")]
    public IEnumerable<string> Met => _met.ToList();
    
    public static WaitingSet For(params string[] needs) 
    { 
        var w = new WaitingSet(); 
        foreach (var n in needs) w._needs.Add(n); 
        return w; 
    }
    
    public static WaitingSet For<T>() where T : class
    {
        return For(typeof(T).Name);
    }
    
    public static WaitingSet For<T1, T2>() where T1 : class where T2 : class
    {
        return For(typeof(T1).Name, typeof(T2).Name);
    }
    
    public static WaitingSet ForTypes(params Type[] types)
    {
        var names = types.Select(t => t.Name).ToArray();
        return For(names);
    }
    
    public bool Mark(string key) 
    { 
        if (!_needs.Contains(key)) return false; 
        _met.Add(key); 
        return IsAllMet; 
    }
    
    public bool Mark<T>() where T : class
    {
        return Mark(typeof(T).Name);
    }
    
    [JsonPropertyName("isAllMet")]
    public bool IsAllMet => _met.SetEquals(_needs);
    
    /// <summary>
    /// Lấy danh sách các needs (for database operations)
    /// </summary>
    public IEnumerable<string> GetNeeds() => _needs.ToList();
    
    /// <summary>
    /// Lấy danh sách các conditions đã được thỏa mãn
    /// </summary>
    public IEnumerable<string> GetMet() => _met.ToList();
}