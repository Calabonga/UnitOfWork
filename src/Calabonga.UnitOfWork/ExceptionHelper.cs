using System;
using System.Text;

namespace Calabonga.UnitOfWork;

/// <summary>
/// Exception Inner message helper
/// </summary>
public static class ExceptionHelper
{
    /// <summary>
    /// Returns recursively inner exceptions messages if they are exists
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static string GetMessages(Exception? exception) 
        => exception == null ? "Exception is NULL" : GetErrorMessage(exception);

    private static string GetErrorMessage(Exception exception)
    {
        var sb = new StringBuilder();
        sb.AppendLine(exception.Message);
        if (exception.InnerException != null)
        {
            sb.AppendLine(GetErrorMessage(exception.InnerException));
        }
        return sb.ToString();
    }
}