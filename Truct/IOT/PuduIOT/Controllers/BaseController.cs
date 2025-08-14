using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PuduIOT.BL.Commons;
using PuduIOT.BL.Interfaces;
using PuduIOT.BL.Services;
using PuduIOT.DA.Middleware;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace PuduIOT.Controllers
{
    [KeepAlive]
    public class BaseController : Controller
    {
        private readonly ILanguageService _languageService;
        private readonly IMstConfigurationService _configurationService;


        private readonly IBreadcrumbService _breadcrumbService;
        public BaseController(ILanguageService languageService, IBreadcrumbService breadcrumbService, IMstConfigurationService configurationService)
        {
            _languageService = languageService;
            _breadcrumbService = breadcrumbService;
            _configurationService = configurationService;
        }

        public HtmlString Localize(string resourceKey, params object[] args)
        {
            var currentCulture = Thread.CurrentThread.CurrentUICulture.Name;

            var language = _languageService.GetLanguageByCulture(currentCulture);
            if (language != null)
            {
                var stringResource = _languageService.GetStringResource(resourceKey, language.lang_code);
                if (stringResource == null || string.IsNullOrEmpty(stringResource.display_name))
                {
                    return new HtmlString(resourceKey);
                }

                return new HtmlString((args == null || args.Length == 0)
                    ? stringResource.display_name
                    : string.Format(stringResource.display_name, args));
            }

            return new HtmlString(resourceKey);
        }

        [HttpPost, Route("change-language")]
        public IActionResult ChangeLanguage([FromBody] string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                }
            );
            
            return Ok(new { });
        }

        [HttpGet, Route("language-error")]
        public IActionResult LanguageErrorNotice([FromBody] string culture)
        {
            var errors = _languageService.GetLanguageError();
            return Ok( errors );
        }

        [HttpGet, Route("configuration")]
        public IActionResult GetConfigValueByKey([FromBody] string key)
        {
            string cost = _configurationService.GetValueByKey(key);

            return Ok(cost);
        }
    }
}
