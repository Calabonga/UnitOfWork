using System;
using System.Text;

namespace Calabonga.UnitOfWork
{
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
        public static string GetMessages(Exception exception)
        {
            if (exception == null) return "Exception is NULL";
            return GetErrorMessage(exception);
        }

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
}
