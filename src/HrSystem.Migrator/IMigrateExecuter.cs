using Microsoft.Extensions.Logging;
using HRSystem.Common.DIContracts;

namespace HrSystem.Migrator;

public interface IMigrateExecuter
{
    public Task RunAsync();
}