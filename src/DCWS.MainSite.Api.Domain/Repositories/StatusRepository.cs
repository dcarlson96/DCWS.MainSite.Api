using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DCWS.MainSite.Api.Domain.Repositories;

public sealed class StatusRepository(AppDbContext context) : IStatusRepository
{
    public Task<StatusTest?> GetLatestAsync()
    {
        return context.StatusEntries
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
    }
}
