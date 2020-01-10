namespace kwd.ConsoleAssist
{
    /// <summary>
    /// Report error to user.
    /// </summary>
    public class ReportError
    {
        /// <summary>
        /// Create new error report details.
        /// </summary>
        public ReportError(int code, string message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>Returned error code</summary>
        public int Code { get; }

        /// <summary>Displayed error message</summary>
        public string Message { get; }
    }
}