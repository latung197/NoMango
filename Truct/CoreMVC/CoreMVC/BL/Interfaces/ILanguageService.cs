using CoreMVC.Models;

namespace CoreMVC.BL.Interfaces
{
    public interface ILanguageService
    {
        IEnumerable<MstLanguage> GetAllLanguages();
        MstLanguage GetLanguageByCulture(string culture);
        string GetLangCodeByCulture(string culture);
        MstLanguage GetStringResource(string resourceKey, string langcode);
        List<MstLanguage> GetLanguageError();
    }
}
