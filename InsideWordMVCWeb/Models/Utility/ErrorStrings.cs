using System;

namespace InsideWordMVCWeb.Models.Utility
{
    /// <summary>
    /// Summary description for ErrorStrings
    /// </summary>
    static public class ErrorStrings
    {
        static public string HtmlErrorCode(int errorCode)
        {
            string retVal = "Unknown error";
            switch (errorCode)
            {
                case 404:
                    retVal = "Page not found";
                    break;
                case 401:
                    retVal = "You do not have the rights to view this page";
                    break;
            }

            return retVal;
        }

        static public string TITLE_ERROR { get { return "ERROR"; } }

        static public string TITLE_WARNING { get { return "WARNING"; } }

        static public string OPERATION_UNKNOWN(string operation)
        {
            return "Unknown operation \"" + operation + "\"";
        }

        static public string OPERATION_FAILED { get { return "Operation failed."; } }

        static public string OPERATION_NO_RIGHTS { get { return "You do not have the rights to perform this operation."; } }

        static public string INVALID_INPUT { get { return "Invalid input."; } }

        static public string ACCESS_DENIED { get { return "ACCESS DENIED"; } }

        static public string FAILED_ACTIVATION { get { return "Failed to activate user account."; } }

        static public string INVALID_QUERY { get { return "Invalid webpage address."; } }

        static public string FAILED_TO_FLAG { get { return "Failed to Flag. Administrator has been notified."; } }

        static public string INVALID_IMAGE { get { return "Image can be no larger than 4 megs and must be a png, gif or jpeg"; } }
    }
}
