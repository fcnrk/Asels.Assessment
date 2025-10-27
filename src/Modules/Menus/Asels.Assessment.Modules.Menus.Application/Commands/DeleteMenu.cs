using Asels.Assessment.Modules.Menus.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Asels.Assessment.Modules.Menus.Application.Commands;

public static class DeleteMenu
{
    public sealed record Command(Guid MenuId) : IRequest<bool>;

    public sealed class Handler(IMenuRepository menus, ILogger<Handler> logger) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken ct)
        {
            logger.LogInformation($"Handling {nameof(DeleteMenu)} for Item Id: {request.MenuId}");
            try
            {
                var entity = await menus.GetByIdAsync(request.MenuId, ct);
                if (entity is null)
                    throw new KeyNotFoundException($"Menu {request.MenuId} not found.");

                var deleted = await menus.DeleteAsync(request.MenuId, ct);
                if (!deleted) return false;
                await menus.SaveChangesAsync(ct);
                logger.LogInformation($"Deleted Menu with Id: {entity.Id}");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error handling {nameof(DeleteMenu)} for Menu Id: {request.MenuId}");
                throw;
            }
        }
    }
}