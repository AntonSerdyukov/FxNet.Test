using FxNet.Test.Data;
using FxNet.Test.Exceptions;
using FxNet.Test.Helpers;
using FxNet.Test.Models;
using System.Text.Json;
using System.Text;

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

                var queryParams = context.Request.Query
                    .ToDictionary(q => q.Key, q => q.Value.ToString());

                context.Request.EnableBuffering();

                string body = string.Empty;
                if (context.Request.ContentLength > 0)
                {
                    context.Request.Body.Position = 0;
                    using var reader = new StreamReader(
                        context.Request.Body,
                        Encoding.UTF8,
                        detectEncodingFromByteOrderMarks: false,
                        leaveOpen: true);

                    body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }

                var journalText = JsonSerializer.Serialize(new
                {
                    request = new
                    {
                        method = context.Request.Method,
                        path = context.Request.Path,
                        query = queryParams,
                        body = string.IsNullOrWhiteSpace(body) ? null : body
                    },
                    exception = new
                    {
                        type = ex.GetType().FullName,
                        message = ex.Message,
                        stackTrace = ex.StackTrace
                    }
                }, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                if (ex is SecureException secureEx)
                {
                    await LogToJournalAsync(db, eventId, journalText, isSecure: true);

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

                await LogToJournalAsync(db, eventId, journalText, isSecure: false);

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
            string journalText,
            bool isSecure)
        {
            db.Journals.Add(new Journal
            {
                EventId = eventId,
                Text = journalText,
                CreatedAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
        }
    }
}
