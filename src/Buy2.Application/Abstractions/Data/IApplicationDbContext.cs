using System;
using System.Collections.Generic;
using System.Text;

namespace Buy2.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
