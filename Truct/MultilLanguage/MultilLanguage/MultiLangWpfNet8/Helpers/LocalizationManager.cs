using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace MultiLangWpfNet8.Helpers
{
    public static class LocalizationManager
    {
        public static void SetLanguage(string cultureCode)
        {
            var dict = new ResourceDictionary
            {
                Source = new Uri($"/Languages/Strings.{cultureCode}.xaml", UriKind.Relative)
            };

            // Xóa dictionary cũ
            var oldDict = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.StartsWith("/Languages/Strings."));

            if (oldDict != null)
                Application.Current.Resources.MergedDictionaries.Remove(oldDict);

            // Thêm mới
            Application.Current.Resources.MergedDictionaries.Add(dict);

            // Đặt văn hóa cho thread
            var culture = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            // Lưu lại
            MultilLanguage.Properties.Settings.Default.Language = cultureCode;
            MultilLanguage.Properties.Settings.Default.Save();
        }
    }
}
