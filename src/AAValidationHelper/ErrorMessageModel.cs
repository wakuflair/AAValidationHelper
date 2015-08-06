using System.Collections.Generic;

namespace AAValidationHelper
{
    public class ErrorMessageModel
    {
        public string FormName { get; set; }
        public string CtrlName { get; set; }
        public SortedDictionary<string, string> ErrorMessages { get; set; }
        public string HtmlAttributes { get; set; }
    }
}