using CheckerboardGameApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CheckerboardGameApp.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class RequireGameStartedAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var gameService = context.HttpContext.RequestServices.GetRequiredService<IGameService>();

        if (gameService.WhitePlayer == null || gameService.BlackPlayer == null)
        {
            context.Result = new BadRequestObjectResult(new
            {
                Message = "Permainan belum dimulai. Silakan mulai game terlebih dahulu."
            });
            return;
        }

        await next();
    }
}
