using Microsoft.Extensions.Logging;

namespace PlantApp.Domain.Utils.Exceptions;

public class DtoIdMismatchException : AppException
{
    public DtoIdMismatchException(string entityName, int dtoId, int paramId, ILogger logger)
        : base(
            userMessage: $"The submitted data is invalid.",
            internalMessage: $"DTO ID {dtoId} does not match parameter ID {paramId} for {entityName}.",
            logger: logger
        )
    {
    }
}
