using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;
using PuduIOT.BL.Interfaces;

namespace PuduIOT.DA
{
    public abstract class CustomBaseViewPage<TModel> : Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
        [RazorInject]
        public ILanguageService LanguageService { get; set; }

        public delegate HtmlString Localizer(string resourceKey, params object[] args);
        private Localizer _localizer;

        public Localizer Localize
        {
            get
            {
                if (_localizer == null)
                {
                    var currentCulture = Thread.CurrentThread.CurrentUICulture.Name;

                    var language = LanguageService.GetLanguageByCulture(currentCulture);
                    if (language != null)
                    {
                        _localizer = (resourceKey, args) =>
                        {
                            var stringResource = LanguageService.GetStringResource(resourceKey, language.lang_code);

                            if (stringResource == null || string.IsNullOrEmpty(stringResource.display_name))
                            {
                                return new HtmlString(resourceKey);
                            }

                            return new HtmlString(args == null || args.Length == 0
                                ? stringResource.display_name
                                : string.Format(stringResource.display_name, args));
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
