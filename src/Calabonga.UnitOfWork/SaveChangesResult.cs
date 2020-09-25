using System;
using System.Collections.Generic;

namespace Calabonga.UnitOfWork
{
    /// <summary>
    /// Represent operation result for SaveChanges.
    /// </summary>
    public class SaveChangesResult
    {
        /// <inheritdoc />
        public SaveChangesResult()
        {
            Messages = new List<string>();
        }

        /// <inheritdoc />
        public SaveChangesResult(string message):this()
        {
            AddMessage(message);
        }

        /// <summary>
        /// List of the error should appear there
        /// </summary>
        public List<string> Messages { get; }

        /// <summary>
        /// Last Exception you can find here
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Is Exception occupied while last operation execution
        /// </summary>
        public bool IsOk => Exception == null;

        /// <summary>
        /// Adds new message to result
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(string message)
        {
            Messages.Add(message);
        }
    }
}
