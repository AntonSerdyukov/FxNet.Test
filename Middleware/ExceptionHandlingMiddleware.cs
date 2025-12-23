using FxNet.Test.Data;
using FxNet.Test.Exceptions;
using FxNet.Test.Helpers;
using FxNet.Test.Models;

namespace FxNet.Test.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate _next, ILogger<ExceptionHandlingMiddleware> _logger)
    {
        public async Task InvokeAsync(HttpContext context, AppDbContext db)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var eventId = EventIdGenerator.NewId();

                if (ex is SecureException secureEx)
                {
                    await LogToJournalAsync(db, eventId, secureEx, isSecure: true);

                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(new
                    {
                        type = "Secure",
                        id = eventId.ToString(),
                        data = new
                        {
                            message = secureEx.Message
                        }
                    });

                    return;
                }

                await LogToJournalAsync(db, eventId, ex, isSecure: false);

                _logger.LogError(ex, "Unhandled exception, EventId={EventId}", eventId);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    type = "Exception",
                    id = eventId.ToString(),
                    data = new
                    {
                        message = $"Internal server error ID = {eventId}"
                    }
                });
            }
        }

        private static async Task LogToJournalAsync(
            AppDbContext db,
            long eventId,
            Exception ex,
            bool isSecure)
        {
            var text = isSecure ? ex.Message : ex.ToString();

            db.Journals.Add(new Journal
            {
                EventId = eventId,
                Text = text,
                CreatedAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
        }
    }
}
