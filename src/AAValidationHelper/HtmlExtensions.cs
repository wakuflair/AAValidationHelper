using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace AAValidationHelper
{
    public static class HtmlExtensions
    {
        /// <summary>
        /// HTML Helper extension method to generate AngularJS ngMessages html code.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="expression">expression</param>
        /// <param name="formName">name of form being validated</param>
        /// <param name="ctrlName">name of control being validated. if null, the name of the Property with first letter lowered will be used</param>
        /// <param name="templateName">error template file name</param>
        /// <param name="htmlAttributes">additional html attributes</param>
        /// <returns>ngMessages html code</returns>
        public static MvcHtmlString NgMessageFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string formName,
            string ctrlName = null,
            object htmlAttributes = null,
            string templateName = "ErrorTemplate"
            )
        {
            var validationRules = GetValidationRules(expression);
            var dictErrorMessage = GetNgMessages(validationRules);

            return htmlHelper.Partial(templateName, new ErrorMessageModel
            {
                FormName = formName,
                CtrlName = ctrlName ?? ExpressionHelper.GetExpressionText(expression).LowerFirstLetter(),
                ErrorMessages = dictErrorMessage,
                HtmlAttributes = htmlHelper.Raw(GetHtmlAttributesString(htmlAttributes)).ToHtmlString(),
            });
        }

        /// <summary>
        /// HTML Helper extension method to generate AngularJS validation directives.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="expression">expression</param>
        /// <param name="htmlAttributes">additional html attributes</param>
        /// <returns></returns>
        public static MvcHtmlString NgValidationFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes = null)
        {
            var validationRules = GetValidationRules(expression);
            string result = GetNgDirective(validationRules);
            result += GetHtmlAttributesString(htmlAttributes);
            return new MvcHtmlString(result);
        }

        private static string GetHtmlAttributesString(object htmlAttributes)
        {
            if (htmlAttributes == null)
            {
                return string.Empty;
            }
            var dict = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return " " + string.Join(" ", dict.Select(kv => $"{kv.Key}=\"{kv.Value}\""));
        }

        private static IEnumerable<ModelClientValidationRule> GetValidationRules<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, new ViewDataDictionary<TModel>());
            var name = ExpressionHelper.GetExpressionText(expression);
            var validationRules = ModelValidatorProviders.Providers.GetValidators(
                metadata ?? ModelMetadata.FromStringExpression(name, new ViewDataDictionary()),
                new ControllerContext())
                .SelectMany(v => v.GetClientValidationRules());
            return validationRules;
        }

        private static SortedDictionary<string, string> GetNgMessages(IEnumerable<ModelClientValidationRule> validationRules)
        {
            var dict = new SortedDictionary<string, string>();
            foreach (var rule in validationRules)
            {
                switch (rule.ValidationType)
                {
                    case "range":
                        dict.Add("min", rule.ErrorMessage);
                        dict.Add("max", rule.ErrorMessage);
                        break;
                    case "regex":
                        dict.Add("pattern", rule.ErrorMessage);
                        break;
                    case "equalto":
                        dict.Add("equalTo", rule.ErrorMessage);
                        break;
                    default:
                        dict.Add(rule.ValidationType, rule.ErrorMessage);
                        break;
                }
            }
            return dict;
        }

        private static string GetNgDirective(IEnumerable<ModelClientValidationRule> ruleidationRules)
        {
            return string.Join(" ", ruleidationRules.Select(rule =>
            {
                switch (rule.ValidationType.ToLower())
                {
                    case "required":
                        return "required";
                    case "range":
                        return string.Format("min=\"{0}\" max=\"{1}\"", rule.ValidationParameters["min"], rule.ValidationParameters["max"]);
                    case "regex":
                        return string.Format("ng-pattern=\"/{0}/\"", rule.ValidationParameters["pattern"]);
                    case "minlength":
                        return string.Format("ng-minlength={0}", rule.ValidationParameters["min"]);
                    case "maxlength":
                        return string.Format("ng-maxlength={0}", rule.ValidationParameters["max"]);
                    default:
                        return string.Format("{0}=\"{1}\"", rule.ValidationType, Json.Encode(rule.ValidationParameters));
                }
            }));
        }
    }
}