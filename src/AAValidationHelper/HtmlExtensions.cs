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
            return " " + string.Join(" ", dict.Select(kv => string.Format("{0}=\"{1}\"", kv.Key, kv.Value)));
        }

        public static MvcHtmlString NgMessageFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            string templateName,
            string formName,
            string ctrlName,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes = null
            )
        {
            var validationRules = GetValidationRules(expression);
            var dictErrorMessage = GetNgMessages(validationRules);

            return htmlHelper.Partial(templateName, new ErrorMessageModel
            {
                FormName = formName,
                CtrlName = ctrlName,
                ErrorMessages = dictErrorMessage,
                HtmlAttributes = htmlHelper.Raw(GetHtmlAttributesString(htmlAttributes)).ToHtmlString(),
            });
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