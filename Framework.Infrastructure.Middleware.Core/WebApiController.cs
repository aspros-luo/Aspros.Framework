using Microsoft.AspNetCore.Mvc;

namespace Framework.Infrastructure.Middleware.Core
{
    public class WebApiController : Controller
    {
        protected ObjectResult Fail(string message)
        {
            return new ObjectResult(new { msg = message, is_success = false });
        }

        protected ObjectResult Success(object obj)
        {
            return new ObjectResult(new { data = obj, is_success = true });
        }
    }
}
