namespace Wcs.Common.Abstractions.Repositories;

public interface ISettingRepository
{
    Task<string?> GetValueAsync(string key, CancellationToken cancellationToken = default);
    Task SetValueAsync(string key, string value, CancellationToken cancellationToken = default);
}
