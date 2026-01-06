using Microsoft.Extensions.Logging;

namespace PlantApp.Domain.Utils.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string entityName, object? key = null, ILogger? logger = null)
        : base(
                userMessage: $"{entityName} not found",
                internalMessage: key != null
                    ? $"{entityName} with id {key} does not exist"
                    : $"{entityName} does not exist",
                logger: logger)
    { }
}
