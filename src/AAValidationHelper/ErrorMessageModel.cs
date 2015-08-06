using System.Collections.Generic;

namespace AAValidationHelper
{
    public class ErrorMessageModel
    {
        /// <summary>
        /// Name of form being validated
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// Name of control being validated
        /// </summary>
        public string CtrlName { get; set; }

        /// <summary>
        /// Validation error messages
        /// </summary>
        public SortedDictionary<string, string> ErrorMessages { get; set; }

        /// <summary>
        /// Addition html attributes
        /// </summary>
        public string HtmlAttributes { get; set; }
    }
}