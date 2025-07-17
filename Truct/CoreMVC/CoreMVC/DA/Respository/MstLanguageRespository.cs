using CoreMVC.Models;
using PdfSharpCore.Pdf.AcroForms;

namespace CoreMVC.DA.Respository
{
    public class MstLanguageRespository
    {
        private readonly CoreDbContext _context;
        public MstLanguageRespository(CoreDbContext context)
        {
            _context = context;
        }
        public IEnumerable<MstLanguage>GetAllLanguages()
        {
            return _context.mstLanguages.ToList();
        }
        public MstLanguage GetLanguageByCulture(string culture)
        {
            return _context.mstLanguages.FirstOrDefault(x => x.lang_code.Trim().ToLower() == culture.Trim().ToLower());
        }

        public string GetLangCodeByCulture(string culture)
        {
            MstLanguage mstLanguage = _context.mstLanguages.FirstOrDefault(x => x.lang_code.Trim().ToLower() == culture.Trim().ToLower());
            return mstLanguage.lang_code;
        }

        public MstLanguage GetStringResource(string resourceKey, string languageCode) 
        {
            return _context.mstLanguages.FirstOrDefault(x => x.item_code.Trim().ToLower() == resourceKey.Trim().ToLower() && x.lang_code == languageCode);
        }


    }
}
