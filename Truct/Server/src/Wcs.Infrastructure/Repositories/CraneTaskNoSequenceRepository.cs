using System.Data;
using Microsoft.EntityFrameworkCore;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Infrastructure.Data;
using Wcs.Infrastructure.Data.Models;

namespace Wcs.Infrastructure.Repositories;

public sealed class CraneTaskNoSequenceRepository(WcsDbContext context) : ICraneTaskNoSequenceRepository
{
    public const string SettingKey = "CraneTaskNoSequence";

    private readonly WcsDbContext _context = context;

    public async Task<int> GetNextAsync(int minValue, int maxValue, CancellationToken cancellationToken = default)
    {
        for (var attempt = 0; attempt < 3; attempt++)
        {
            try
            {
                return await GetNextCoreAsync(minValue, maxValue, cancellationToken);
            }
            catch (DbUpdateException) when (attempt < 2)
            {
                _context.ChangeTracker.Clear();
            }
        }

        return await GetNextCoreAsync(minValue, maxValue, cancellationToken);
    }

    private async Task<int> GetNextCoreAsync(int minValue, int maxValue, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var setting = await _context.Settings
            .FirstOrDefaultAsync(s => s.Key == SettingKey, cancellationToken);

        int next;
        if (setting is null)
        {
            var seed = await _context.CraneTaskDispatches
                .MaxAsync(d => (int?)d.TaskNo, cancellationToken) ?? 0;
            next = WrapNext(seed, minValue, maxValue);
            _context.Settings.Add(new SettingDbModel
            {
                Key = SettingKey,
                Value = next.ToString(),
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            var current = int.TryParse(setting.Value, out var parsed) ? parsed : 0;
            next = WrapNext(current, minValue, maxValue);
            setting.Value = next.ToString();
            setting.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return next;
    }

    private static int WrapNext(int current, int minValue, int maxValue)
    {
        if (current < minValue || current >= maxValue)
        {
            return minValue;
        }

        return current + 1;
    }
}
