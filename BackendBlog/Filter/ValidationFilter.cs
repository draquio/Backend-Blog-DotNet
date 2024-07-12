using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using BackendBlog.DTO.Response;

namespace BackendBlog.Filter
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Keys
                    .SelectMany(key => context.ModelState[key].Errors.Select(x => new { key, x.ErrorMessage }))
                    .GroupBy(x => x.key, x => x.ErrorMessage, (key, errors) => new { key, errors = errors.ToList() })
                    .ToDictionary(x => x.key, x => x.errors);

                var response = new Response<Dictionary<string, List<string>>>
                {
                    status = false,
                    errors = errors,
                    msg = "Validation errors occurred."
                };

                context.Result = new BadRequestObjectResult(response);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
