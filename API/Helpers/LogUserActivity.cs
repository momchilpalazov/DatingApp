using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
   
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

        var userId = context.HttpContext.User.GetUserId();
        var uow = resultContext.HttpContext.RequestServices.GetService<IUnitOfWorkRepository>();
        var user = await uow.UserRepository.GetUserByIdAsync(int.Parse(userId));
        user.LastActive = DateTime.UtcNow;
        await uow.Complete();
       
    }
}
