using CoreMVC.BL.Interfaces;
using CoreMVC.DA;
using CoreMVC.DA.Respository;
using CoreMVC.Models;

namespace CoreMVC.BL.Services
{
    public class MstLanguageService : ILanguageService
    {
        public readonly MstLanguageRespository _respository;
        public MstLanguageService(CoreDbContext context) 
        { 
            _respository = new MstLanguageRespository(context); 
        }

        public IEnumerable<MstLanguage> GetAllLanguages()
        {
            return _respository.GetAllLanguages();
        }

        public string GetLangCodeByCulture(string culture)
        {
            return _respository.GetLangCodeByCulture(culture);
        }

        public MstLanguage GetLanguageByCulture(string culture)
        {
            return _respository.GetLanguageByCulture(culture);
        }

        public List<MstLanguage> GetLanguageError()
        {
            return _respository.GetLanguageError();

        }

        public MstLanguage GetStringResource(string resourceKey, string langcode)
        {
            return _respository.GetStringResource(resourceKey, langcode);
        }

        public void AddLange(MstLanguage language)
        {
            _respository.AddLanguage(language);
        }
    }
}
