using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Data;
using System.Reflection;

namespace Cody_v3.Services.Helpers
{
    public static class FileHelper
    {
        public static readonly string EXCEL_UPLOAD_FILE_PATH = Path.Combine("uploads", "excels");
        public static readonly string SUMMERNOTE_UPLOAD_FILE_PATH = Path.Combine("uploads", "summernote");

        public static async Task<bool> Upload(this IFormFileCollection files, string folderSave, bool generateFileName = false)
        {
            bool isSussess = false;

            if (!Directory.Exists(folderSave)) Directory.CreateDirectory(folderSave);

            foreach (IFormFile file in files)
            {
                string fileName = generateFileName == false ? file.FileName : file.FileName + "_" + new Guid();

                var targetFile = Path.Combine(folderSave, fileName);
                using (var fileStream = new FileStream(targetFile, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    isSussess = true;
                }
            }

            return isSussess;
        }

        public static async Task<string> UploadFile(this IFormFile file, string folderSave, bool generateFileName = true)
        {

            if (!Directory.Exists(folderSave)) Directory.CreateDirectory(folderSave);

            string fileName = file.FileName;
            if (generateFileName)
            {
                fileName=Path.GetFileNameWithoutExtension(file.FileName) +"_"+ Guid.NewGuid() + Path.GetExtension(file.FileName);
            }

            var targetFile = Path.Combine(folderSave, fileName);
            using (var fileStream = new FileStream(targetFile, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }


            return fileName;
        }

        public static async Task<DataSet> GetDataExcel(string filePath, bool firstRowHeader = true)
        {
            return await Task<DataSet>.Factory.StartNew(() =>
            {
                var result = new DataSet();
                var xConfig = new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                };
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        if (reader != null)
                        {
                            result = reader.AsDataSet(xConfig);
                        }
                    }
                }
                File.Delete(filePath);
                return result;
            });
        }


        public static DateTime? ToExacDateTime(this object value)
        {
            if (value == null || Convert.IsDBNull(value))
            {
                return null;
            }
            else
            {
                try
                {
                    return DateTime.ParseExact(value.ToString(), "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (System.Exception)
                {
                    return null;
                }
            }
        }

        public static decimal? ToDecimal(this object value)
        {
            if (value == null || Convert.IsDBNull(value))
            {
                return null;
            }
            else
            {
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch (System.Exception)
                {
                    return int.MinValue;
                }
            }
        }

        public static void CopyPropertiesTo<T, TU>(this T source, TU dest)
        {
            if (source == null || dest == null)
            {
                throw new ArgumentNullException("Source and/or Destination Object are null");
            }

            Type typeDest = dest.GetType();
            Type typeSource = source.GetType();

            PropertyInfo[] scrProps = typeSource.GetProperties();
            foreach (PropertyInfo srcProp in scrProps)
            {
                if (!srcProp.CanRead)
                    continue;

                PropertyInfo destProp = typeDest.GetProperty(srcProp.Name);
                if (destProp == null || !destProp.CanWrite)
                    continue;

                if (destProp.GetSetMethod(true) != null && destProp.GetSetMethod(true).IsPrivate)
                    continue;

                if ((destProp.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                    continue;

                if (!destProp.PropertyType.IsAssignableFrom(srcProp.PropertyType))
                    continue;

                destProp.SetValue(dest, srcProp.GetValue(source, null), null);
            }

        }

        public static T? ToObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string ToJSON<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
