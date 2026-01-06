using Microsoft.Extensions.Logging;

namespace PlantApp.Domain.Utils.Exceptions;

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string action, string? entityName = null, ILogger? logger = null)
            : base(
                  userMessage: "You are not authorized", // korisniku
                  internalMessage: entityName != null
                      ? $"Unauthorized attempt to {action} {entityName}" // log
                      : $"Unauthorized attempt to {action}",
                  logger: logger)
    { }
}
