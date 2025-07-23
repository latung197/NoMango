using CoreMVC.BL.Interfaces;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace CoreMVC.DA
{
    public abstract class CustomBaseViewPage<TModel> : Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
        [RazorInject]
        public ILanguageService languageService { get; set; }
        public delegate HtmlString Localizer(string resourceKey, params object[] parameters);
        private Localizer _localizer;
        public Localizer Localize
        {
            get
            {
                if (_localizer == null)
                {
                    var currentCulture = Thread.CurrentThread.CurrentUICulture.Name;

                    var language = languageService.GetLanguageByCulture(currentCulture);
                    if (language != null)
                    {
                        _localizer = (resourceKey, parameters) =>
                        {
                            var stringResource = languageService.GetStringResource(resourceKey, language.lang_code);

                            if (stringResource == null || string.IsNullOrEmpty(stringResource.item_name))
                            {
                                return new HtmlString(resourceKey);
                            }

                            return new HtmlString(parameters == null || parameters.Length == 0
                                ? stringResource.item_name
                                : string.Format(stringResource.item_name, parameters));
                        };
                    }
                }
                return _localizer;
            }
        }
    }
    public abstract class CustomBaseViewPage : CustomBaseViewPage<dynamic>
    {
    }
}
