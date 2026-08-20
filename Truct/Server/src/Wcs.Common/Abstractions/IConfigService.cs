namespace Wcs.Common.Abstractions;

public interface IConfigService
{   
    T GetFromFile<T>(string fileName) where T : class, new();
}
