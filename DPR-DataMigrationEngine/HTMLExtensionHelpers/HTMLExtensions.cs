using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace DPR_DataMigrationEngine.HTMLExtensionHelpers
{

    public static class HtmlExtensions
    {
        public static IHtmlString MyRadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes, object booleanHtmlAttributes)
        {
            var attributes = new RouteValueDictionary(htmlAttributes);

            //Reflect over the properties of the newly added booleanHtmlAttributes object
            foreach (var prop in booleanHtmlAttributes.GetType().GetProperties())
            {
                //Find only the properties that are true and inject into the main attributes.
                //and discard the rest.
                if (ValueIsTrue(prop.GetValue(booleanHtmlAttributes, null)))
                {
                    attributes[prop.Name] = prop.Name;
                }
            }

            return htmlHelper.TextBoxFor(expression, attributes);
        }

        private static bool ValueIsTrue(object obj)
        {
            bool res;
            try
            {
                res = Convert.ToBoolean(obj);
            }
            catch (FormatException)
            {
                res = false;
            }
            catch (InvalidCastException)
            {
                res = false;
            }
            return res;
        }

        
    }
}