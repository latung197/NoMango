//==================================================================================================
// System  : DenKa
// File    : Utility.cs
// Author  : Tan Vu (tanvd@robotcom-fa.com)
// Updated : 05/06/2024
// Note    : Copyright 2018-2024, RFVN, All rights reserved
//
// A collection of utility methods.
//
// Modification Log:
// Version  Date        By       Notes
// -------  ----------  -------  --------
// 1.0.0    05/06/2024  Tan Vu   Created the code
//==================================================================================================

using PlastMB.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PlastMB
{
    internal class Utility
    {
        public static Dictionary<string, string> COLORS = new Dictionary<string, string>
        {
            { "-A-", "Amber" },
            { "-BE-", "Beige" },
            { "-B-", "Black" },
            { "-L-", "Blue" },
            { "-BG-", "Bright Green" },
            { "-BR-", "Brown" },
            { "-C-", "Cream" },
            { "-GR-", "Gray" },
            { "-G-", "Green" },
            { "-I-", "Ivory" },
            { "-O-", "Orange" },
            { "-P-", "Pink" },
            { "-PU-", "Purple" },
            { "-R-", "Red" },
            { "-SB-", "Sky Blue" },
            { "-T-", "Transparent" },
            { "-W-", "White" },
            { "-Y-", "Yellow" },
            { " A-", "Amber" },
            { " BE-", "Beige" },
            { " B-", "Black" },
            { " L-", "Blue" },
            { " BG-", "Bright Green" },
            { " BR-", "Brown" },
            { " C-", "Cream" },
            { " GR-", "Gray" },
            { " G-", "Green" },
            { " I-", "Ivory" },
            { " O-", "Orange" },
            { " P-", "Pink" },
            { " PU-", "Purple" },
            { " R-", "Red" },
            { " SB-", "Sky Blue" },
            { " T-", "Transparent" },
            { " W-", "White" },
            { " Y-", "Yellow" },
            { "-Be-", "Beige" },
            { "-Bg-", "Bright Green" },
            { "-Br-", "Brown" },
            { "-Gr-", "Gray" },
            { "-Pu-", "Purple" },
            { "-Sb-", "Sky Blue" },
            { " Be-", "Beige" },
            { " Bg-", "Bright Green" },
            { " Br-", "Brown" },
            { " Gr-", "Gray" },
            { " Pu-", "Purple" },
            { " Sb-", "Sky Blue" },
            { "-bE-", "Beige" },
            { "-bG-", "Bright Green" },
            { "-bR-", "Brown" },
            { "-gR-", "Gray" },
            { "-pU-", "Purple" },
            { "-sB-", "Sky Blue" },
            { " bE-", "Beige" },
            { " bG-", "Bright Green" },
            { " bR-", "Brown" },
            { " gR-", "Gray" },
            { " pU-", "Purple" },
            { " sB-", "Sky Blue" },
            { "-a-", "Amber" },
            { "-be-", "Beige" },
            { "-b-", "Black" },
            { "-l-", "Blue" },
            { "-bg-", "Bright Green" },
            { "-br-", "Brown" },
            { "-c-", "Cream" },
            { "-gr-", "Gray" },
            { "-g-", "Green" },
            { "-i-", "Ivory" },
            { "-o-", "Orange" },
            { "-p-", "Pink" },
            { "-pu-", "Purple" },
            { "-r-", "Red" },
            { "-sb-", "Sky Blue" },
            { "-t-", "Transparent" },
            { "-w-", "White" },
            { "-y-", "Yellow" },
            { " a-", "Amber" },
            { " be-", "Beige" },
            { " b-", "Black" },
            { " l-", "Blue" },
            { " bg-", "Bright Green" },
            { " br-", "Brown" },
            { " c-", "Cream" },
            { " gr-", "Gray" },
            { " g-", "Green" },
            { " i-", "Ivory" },
            { " o-", "Orange" },
            { " p-", "Pink" },
            { " pu-", "Purple" },
            { " r-", "Red" },
            { " sb-", "Sky Blue" },
            { " t-", "Transparent" },
            { " w-", "White" },
            { " y-", "Yellow" },
        };

        public static string FOLDER_CSV_HA = "";
        public static string FOLDER_CSV_EA = "";
        public static string FOLDER_EXCEL = "";

        public static Form FindForm(string title)
        {
            var frm = Application.OpenForms.Cast<Form>().Where(x => x.Text == title).FirstOrDefault();
            return frm;
        }

        public static void ShowForm(string title)
        {
            var frm = Application.OpenForms.Cast<Form>().Where(x => x.Text == title).FirstOrDefault();
            if (null != frm)
            {
                frm.Show();
            }
        }

        public static string RemoveUnicode(string text)
        {
            var arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
            "đ",
            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
            "í","ì","ỉ","ĩ","ị",
            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
            "ý","ỳ","ỷ","ỹ","ỵ"
        };
            var arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
            "d",
            "e","e","e","e","e","e","e","e","e","e","e",
            "i","i","i","i","i",
            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
            "u","u","u","u","u","u","u","u","u","u","u",
            "y","y","y","y","y"
        };
            for (var i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            //return text;
            return text.Replace(" ", "");
        }

        public static DateTime? GetDate(string dateString, string format = "yyyy/MM/dd")
        {
            try
            {
                if (string.IsNullOrEmpty(format))
                {
                    format = "yyyy/MM/dd";
                }

                return new DateTime?(DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture));
            }
            catch
            {
                return null;
            }
        }

        public static string GetExceptionInfo(Exception ex, string className)
        {
            var lineNumber = 0;
            const string lineSearch = ":line ";
            var index = ex.StackTrace.LastIndexOf(lineSearch);
            if (index != -1)
            {
                var lineNumberText = ex.StackTrace.Substring(index + lineSearch.Length);
                if (int.TryParse(lineNumberText, out lineNumber))
                {
                }
            }
            return $"{className}:line {lineNumber} | {ex.GetType()}: {ex.Message}";
        }

        public static void BackupFileCSV(FileInfo file)
        {
            try
            {
                string folder_name = Settings.Default.FolderNameBkCSV;
                string folderBK_CSV = Path.Combine(Utility.FOLDER_CSV_HA, folder_name);
                string[] folders = file.Name.Split('_');
                if (folders.Length > 0)
                {
                    for (int i = 0; i < folders.Length - 1; i++)
                    {
                        var folder = folders[i];
                        folderBK_CSV = Path.Combine(folderBK_CSV, folder);
                    }
                }
                if (!Directory.Exists(folderBK_CSV))
                {
                    Directory.CreateDirectory(folderBK_CSV);
                }
                string path = $@"{folderBK_CSV}\{file.Name}";
                if (File.Exists(path))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file.Name);
                    var dtime = DateTime.Now.ToString("yyyyMMddHHmmss");
                    path = $@"{folderBK_CSV}\{fileName}_{dtime}.csv";
                }
                file.MoveTo(path);
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

    }
}
