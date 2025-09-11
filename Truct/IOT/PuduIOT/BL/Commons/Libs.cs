using PuduIOT.BL;
using PuduIOT.BL.Commons;
using PuduIOT.BL.Interfaces;
using PuduIOT.DA;
using PuduIOT.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using Npgsql;
using NuGet.Protocol;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;
using System;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using static PuduIOT.BL.Commons.Constants;
using static System.Reflection.Metadata.BlobBuilder;
using System.Net;
using System.Security.Cryptography;
using System.Web;

public class Libs : ILibs
{
    private readonly PuduIotDbContext _dbContext;
    private readonly ILanguageService _language;
    private readonly ILoggers _logger;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IMstUnitService _unitService;

    public Libs(PuduIotDbContext context, ILoggers loggers, ILanguageService language, IWebHostEnvironment hostingEnvironment, IMstUnitService unitService)
    {
        _logger = loggers;
        _dbContext = context;
        _language = language;
        _hostingEnvironment = hostingEnvironment;
        _unitService = unitService;
    }

    private string Localize(string resourceKey, params object[] args)
    {

        var currentCulture = Thread.CurrentThread.CurrentUICulture.Name;

        var language = _language.GetLanguageByCulture(currentCulture);
        if (language != null)
        {
            var stringResource = _language.GetStringResource(resourceKey, language.lang_code);
            if (stringResource == null || string.IsNullOrEmpty(stringResource.display_name))
            {
                return new string(resourceKey);
            }

            return new string((args == null || args.Length == 0)
                ? stringResource.display_name
                : string.Format(stringResource.display_name, args));
        }

        return new string(resourceKey);
    }


    /// <summary>
    /// Maps a .NET type to a corresponding DbType for PostgreSQL.
    /// </summary>
    /// <param name="type">The .NET type to be mapped.</param>
    /// <returns>The corresponding DbType for PostgreSQL.</returns>
    public DbType GetDbType(Type type)
    {
        try
        {
            if (type == typeof(int))
                return DbType.Int32;
            else if (type == typeof(string))
                return DbType.String;
            else if (type == typeof(DateTime))
                return DbType.DateTime;
            else if (type == typeof(decimal))
                return DbType.Decimal;
            else if (type == typeof(double))
                return DbType.Double;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        throw new ArgumentException($"No DbType mapping found for type {type.Name}");
    }

    /// <summary>
    /// Concatenates an array of parameter names for use in a PostgreSQL function call.
    /// </summary>
    /// <param name="paramName">An array of parameter names.</param>
    /// <returns>A string containing the concatenated parameter names with "@" prefix.</returns>
    string ConcatenateParamNames(string[] paramName)
    {
        if (paramName == null || paramName.Length == 0)
        {
            return string.Empty;
        }

        // Sử dụng phương thức Join để nối các chuỗi và thêm "@"
        string result = "@" + string.Join(", @", paramName);

        return result;
    }

    public async Task<string> GetDataPudu(string pathAndQuery)
    {
        string HTTPMethod = "GET";
        string Accept = "application/json";
        string ContentType = "application/json";

        // 应用 ApiAppKey
        string ApiAppKey = "APIDdPWPWY2EVEPTKWFOd5NNkEpuHRSb9FPjn3n8h";
        string ApiAppSecret = "aeipzmPCHWrVthBfbeHDPwnyiNpx32efBB4foCn9Z";

        // string url = "https://css-open-platform.pudutech.com/pudu-entry/data-open-platform-service/v1/api/robot?limit=2&offset=0&shop_id=526150005";

        // Base URL node Mỹ (thay theo khu vực bạn)
        string baseUrl = "https://css-open-platform.pudutech.com/pudu-entry";
        // API healthCheck + query string

        string url = baseUrl + pathAndQuery;

        Uri uri = new Uri(url);
        string host = uri.Host;
        string path = uri.AbsolutePath;
        Console.WriteLine("Url:{0}", url);
        Console.WriteLine("Host:{0}", host);

        // Without environmental information
        if (path.StartsWith("/release"))
        {
            path = path.Substring("/release".Length);

        }
        else if (path.StartsWith("/test"))
        {
            path = path.Substring("/test".Length);
        }
        else if (path.StartsWith("/prepub"))
        {
            path = path.Substring("/prepub".Length);
        }
        if (path == "")
        {
            path = "/";
        }
        //query sort
        if (uri.Query.Length > 0)
        {
            var queryString = HttpUtility.ParseQueryString(uri.Query);
            List<string> lstQuery = new List<string>();
            foreach (var key in queryString.AllKeys)
            {
                lstQuery.Add(key);
            }
            lstQuery.Sort();
            StringBuilder sbQuery = new StringBuilder();
            foreach (string q in lstQuery)
            {
                if (queryString[q] != "")
                {
                    sbQuery = sbQuery.Append("&").Append(q).Append("=").Append(queryString[q]);
                }
                else
                {
                    sbQuery = sbQuery.Append("&").Append(q);

                }
            }
            path += "?" + sbQuery.ToString().TrimStart('&');
        }

        var xDate = DateTime.UtcNow.ToUniversalTime().ToString("r");
        string contentMd5 = "";
        string bodyStr = "{\"b\":\"2\", \"a\":\"###特殊字符测试\", \"c\": \"3\"}";
        if (HTTPMethod == "POST")
        {
            //Content-MD5
            byte[] result = Encoding.UTF8.GetBytes(bodyStr);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string hexString = BitConverter.ToString(output).Replace("-", "").ToLower();
            byte[] bs = System.Text.Encoding.ASCII.GetBytes(hexString);
            contentMd5 = Convert.ToBase64String(bs);
        }
        string signingStr = string.Format("x-date: {0}\n{1}\n{2}\n{3}\n{4}\n{5}", xDate, HTTPMethod, Accept, ContentType, contentMd5, path);

        //HMACSHA1
        HMACSHA1 hmacsha1 = new HMACSHA1();
        hmacsha1.Key = System.Text.Encoding.UTF8.GetBytes(ApiAppSecret);
        byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(signingStr);
        byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);
        string signature = Convert.ToBase64String(hashBytes);

        //get authorization
        string sign = string.Format("hmac id=\"{0}\", algorithm=\"hmac-sha1\", headers=\"x-date\", signature=\"{1}\"", ApiAppKey, signature);
        Console.WriteLine("sign:" + sign);


        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = HTTPMethod;
        request.Host = host;
        request.ContentType = ContentType;
        request.Accept = Accept;
        request.Headers.Add("x-date", xDate);
        request.Headers.Add("Authorization", sign);
        request.Headers.Add("Content-MD5", contentMd5);
        try
        {
            if (HTTPMethod == "POST")
            {
                //post request body
                byte[] byteData = Encoding.UTF8.GetBytes(bodyStr);
                int length = byteData.Length;
                request.ContentLength = length;
                Stream writer = request.GetRequestStream();
                writer.Close();
                return null;
            }
            //get response
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;
            }

        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public string[] ProcessingParam(List<MstUnit> units)
    {
        List<string> lstUnits = new List<string>();
        List<string> lstUnitsCol = new List<string>();

        foreach (var item in units)
        {
            lstUnits.Add($"{item.unit_code.Trim()}");
            lstUnitsCol.Add($"'{item.unit_code.Trim()}'");
        }

        return new string[] { string.Join(",", lstUnits.ToArray()), string.Join(",", lstUnitsCol.ToArray()) };
    }

    public string[] getStringTimeType(string timeType, string langcode)
    {
        string timeFormatStr = "";
        string timeFormatCSV = "";

        try
        {
            switch (timeType)
            {
                case "0":
                    timeFormatStr = langcode == "vi-VN" ? "DD/MM/YYYY" : "YYYY/MM/DD";
                    timeFormatCSV = langcode == "vi_VN" ? "DD/MM/YYYY HH24:00" : "YYYY/MM/DD HH24:00";
                    break;

                case "1":
                    timeFormatStr = langcode == "vi-VN" ? "MM/YYYY" : "YYYY/MM";
                    timeFormatCSV = langcode == "vi-VN" ? "DD/MM/YYYY" : "YYYY/MM/DD";
                    break;

                case "2":
                    timeFormatStr = "YYYY";
                    timeFormatCSV = langcode == "vi-VN" ? "MM/YYYY" : "YYYY/MM";
                    break;
                case "3":
                    timeFormatStr = langcode == "vi-VN" ? "HH24:MI" : "HH24:MI";
                    timeFormatCSV = langcode == "vi_VN" ? "HH24:MI" : "HH24:MI";
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return new string[] { timeFormatStr, timeFormatCSV };

    }


    /// <summary>
    /// Executes a PostgreSQL stored function.
    /// </summary>
    /// <param name="func_name">The name of the stored function.</param>
    /// <param name="paramName">An array of parameter names.</param>
    /// <param name="paramValue">An array of parameter values corresponding to the names.</param>
    /// <returns>A DataTable containing the result of the stored function.</returns>
    public DataTable callFuncPostgre(string func_name, string[] paramName, object[] paramValue)
    {
        DataTable dt = new DataTable();
        try
        {

            using (NpgsqlConnection connection = new NpgsqlConnection(_dbContext.Database.GetConnectionString()))
            {
                connection.Open();

                // Create a command with a parameter
                using (NpgsqlCommand command = new NpgsqlCommand($"SELECT * FROM public.{func_name}({ConcatenateParamNames(paramName)})", connection))
                {
                    // Set parameter values
                    if (paramName != null)
                    {
                        for (int i = 0; i < paramName.Length; i++)
                        {
                            command.Parameters.AddWithValue($"@{paramName[i]}", paramValue[i]);
                        }
                    }
                    // Execute the query
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        dt.Load(reader);

                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return dt;
    }

    /// <summary>
    /// Executes a PostgreSQL stored function without parameters.
    /// </summary>
    /// <param name="func_name">The name of the stored function.</param>
    public DataTable callFuncPostgre(string func_name)
    {
        DataTable dt = new DataTable();
        using (NpgsqlConnection connection = new NpgsqlConnection(_dbContext.Database.GetConnectionString()))
        {
            connection.Open();

            // Create a command with a parameter
            using (NpgsqlCommand command = new NpgsqlCommand($"SELECT * FROM public.{func_name}()", connection))
            {
                // Execute the query
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }
        }

        return dt;
    }

    /// <summary>
    /// Executes a custom SQL query and returns the result as a DataTable.
    /// </summary>
    /// <param name="sql">The SQL query to be executed.</param>
    /// <returns>A DataTable containing the result of the SQL query.</returns>
    public DataTable ExecuteFunction(string sql)
    {
        DataTable dataTable = new DataTable();
        try
        {
            using (DbCommand cmd = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                _dbContext.Database.OpenConnection();
                using DbDataReader reader = cmd.ExecuteReader();
                dataTable.Load(reader);
            }
            _dbContext.Database.CloseConnection();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return dataTable;
    }


    /// <summary>
    /// Retrieves formatted date and time strings in different languages.
    /// </summary>
    /// <param name="langcode">The language code for formatting.</param>
    /// <returns>A list of formatted date and time strings.</returns>
    public List<string> GetStringDateTimeByLangCode(string langcode)
    {
        List<string> dateTimeFormat = new List<string>();
        DateTime time = new DateTime();
        try
        {
            time = DateTime.Now;

            dateTimeFormat.Add(time.ToString("'Ngày' dd 'Tháng' MM 'Năm' yyyy"));
            dateTimeFormat.Add(time.ToString("hh:mm tt", new CultureInfo("en-US")));


            dateTimeFormat.Add(time.ToString("yyyy年MM月dd日"));
            dateTimeFormat.Add(time.ToString("hh:mm tt", new CultureInfo("en-US")));

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return dateTimeFormat;
    }

    public DataTable ConvertListToDataTable<T>(List<T> list)
    {
        DataTable table = new DataTable();

        // Create columns for the DataTable based on the properties of T
        foreach (var prop in typeof(T).GetProperties())
        {
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        // Add data from the list to the DataTable
        foreach (var item in list)
        {
            DataRow row = table.NewRow();
            foreach (var prop in typeof(T).GetProperties())
            {
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            }
            table.Rows.Add(row);
        }

        return table;
    }

    public byte[] ExportDataTableToCsv(DataTable dataTable, string timeType)
    {
        byte[] csvBytes = { };

        try
        {
            string[] header = new string[] { "No.", Localize("DAS-LBL-014"), Localize("EXP-LBL-003"), Localize("EXP-LBL-004"), Localize("EXP-LBL-005"), Localize("EXP-LBL-006"), Localize("EXP-LBL-007") };
            StringBuilder sb = new StringBuilder();
            string[] arrTimeType = { Localize("DAS-OPT-001"), Localize("DAS-OPT-002"), Localize("DAS-OPT-003"), Localize("DAS-OPT-004") };
            // Write header row
            for (int i = 0; i < header.Length; i++)
            {
                sb.Append(header[i]);
                if (i < header.Length - 1)
                {
                    sb.Append(",");
                }
            }
            sb.AppendLine();
            string unit = Localize("COM-LBL-015");
            // Write data rows
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = -1; j < header.Length - 3; j++)
                {
                    if (j == -1)
                    {
                        sb.Append($"{i + 1}");
                    }

                    else
                    {
                        if (j == 1) //resource
                        {
                            sb.Append($"{unit}"); sb.Append(",");
                            sb.Append($"{arrTimeType[Convert.ToInt16(timeType)]}"); sb.Append(",");
                        }
                        if (dataTable.Columns[j].ColumnName == "total_value")
                        {
                            sb.Append("\"");
                            sb.Append(dataTable.Rows[i][j] != DBNull.Value ? Convert.ToDecimal(dataTable.Rows[i][j]).ToString("n2", new CultureInfo("en-US")) : "0");
                            sb.Append("\"");
                        }
                        else
                            sb.Append(dataTable.Rows[i][j].ToString());
                    }

                    if (j < header.Length - 3)
                    {
                        sb.Append(",");
                    }
                }
                sb.AppendLine();
            }

            csvBytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return csvBytes;
    }

    public string ConvertUnitToFormatQuery(string units)
    {
        string[] arrUnits = units.Split(',');
        for (int i = 0; i < arrUnits.Length; i++)
        {
            arrUnits[i] = $"'{arrUnits[i].Trim()}'";
        }
        string strUnits = string.Join(", ", arrUnits);
        return strUnits;
    }

    public DateTime[] GenerateDateTimeByTimeType(string timeType)
    {
        DateTime startTime;
        DateTime endTime;
        switch (timeType)
        {
            case "0":
                startTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd 00:00:00"));
                endTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd 23:59:59"));
                break;
            case "1":
                startTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/01 00:00:00"));
                endTime = Convert.ToDateTime(DateTime.Now.ToString($"yyyy/MM/{DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)} 23:59:59"));
                break;
            case "2":
                startTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/01/01 00:00:00"));
                endTime = Convert.ToDateTime(DateTime.Now.ToString($"yyyy/12/{DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)} 23:59:59"));
                break;
            case "3":
                startTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd HH:00:00"));
                endTime = Convert.ToDateTime(DateTime.Now.AddHours(1).ToString("yyyy/MM/dd HH:00:00"));
                break;
            default:
                startTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd 00:00:00"));
                endTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd 23:59:59"));
                break;
        }
        return new DateTime[] { startTime, endTime };
    }

    public byte[] ExportDataTableToPdf(DataTable dataTable, MyData mData, string langcode)
    {
        byte[] pdfBytes = { };
        string fontNameVi = "Tahoma";
        string fontNameJp = "Yu Mincho";
        string svgFilePath = "";
        string pngFilePath = "";
        string pathTemps = "";
        string startTime, endTime, dateExport;
        DataTable dt = dataTable;

        List<string> arrPngFilePath = new List<string>();
        try
        {

            string[] unitCodeArray = dataTable.AsEnumerable()
                                   .Select(row => row.Field<string>("display_name"))
                                   .Distinct()
                                   .ToArray();


            string[] header = new string[] { "No.", Localize("EXP-LBL-010"), Localize("EXP-LBL-005"), Localize("EXP-LBL-011"), Localize("EXP-LBL-007"), Localize("EXP-LBL-012"), Localize("EXP-LBL-013"), Localize("EXP-LBL-014") };
            XFont fontTableHeader = new XFont(fontNameVi, 10.0, XFontStyle.Bold);
            XFont fontHeader = new XFont(fontNameVi, 20.0, XFontStyle.Bold);
            XFont fontTableContent = new XFont(fontNameVi, 8.0, XFontStyle.Regular);

            DateTime startDate = Convert.ToDateTime(mData.startTime);
            DateTime endDate = Convert.ToDateTime(mData.endTime);
            if (mData.startTime == null)
            {
                startDate = DateTime.Today;
                endDate = DateTime.Today.AddDays(1).AddTicks(-1);
            }

            if (langcode == Constants.JP_CULTURE)
            {
                fontTableHeader = new XFont(fontNameJp, 10.0, XFontStyle.Bold);
                fontHeader = new XFont(fontNameJp, 20.0, XFontStyle.Bold);
                fontTableContent = new XFont(fontNameJp, 8.0, XFontStyle.Regular);

                startTime = startDate.ToString("yyyy/MM/dd HH:mm");
                endTime = endDate.ToString("yyyy/MM/dd HH:mm");
                dateExport = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            }
            else
            {

                startTime = startDate.ToString("dd/MM/yyyy HH:mm");
                endTime = endDate.ToString("dd/MM/yyyy HH:mm");
                dateExport = DateTime.Now.ToString();
            }

            XSolidBrush brushHead = new XSolidBrush(XColor.FromArgb(0xC9, 0xAB, 0x24));
            XSolidBrush brush = XBrushes.Black;
            string[] arrTimeType = { Localize("DAS-OPT-001"), Localize("DAS-OPT-002"), Localize("DAS-OPT-003"), Localize("DAS-OPT-004") };
            var top = new XUnit(0.75, XGraphicsUnit.Inch);
            var bot = new XUnit(0.75, XGraphicsUnit.Inch);
            var left = new XUnit(0.25, XGraphicsUnit.Inch);
            var right = new XUnit(0.25, XGraphicsUnit.Inch);
            var head = new XUnit(0.3, XGraphicsUnit.Inch);
            var foot = new XUnit(0.3, XGraphicsUnit.Inch);

            if (mData.html != null)
                for (int i = 0; i < mData.html.Length; i++)
                {
                    pathTemps = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temps");
                    svgFilePath = Path.Combine(pathTemps, $"chart.svg");
                    pngFilePath = Path.Combine(pathTemps, $"chart{DateTime.Now.ToString($"dd_MM_yyyy_HH_mm_ss_{i.ToString()}")}.png");
                    arrPngFilePath.Add(pngFilePath);
                    if (!File.Exists(pathTemps))
                    {
                        Directory.CreateDirectory(pathTemps);
                    }
                    var bytes = Convert.FromBase64String(mData.html[i].Substring(mData.html[i].IndexOf(',') + 1));
                    using (var imageFile = new FileStream(pngFilePath, FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }
                }

            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();

            page.Width = new XUnit(8.27, XGraphicsUnit.Inch);
            page.Height = new XUnit(11.69, XGraphicsUnit.Inch);

            var yPos = top - head;
            var aria = page.Width - left - right;
            int dem = 0;
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XPen pen = new XPen(XColor.FromKnownColor(XKnownColor.Black), 1);
            XRect rect = new XRect(left, yPos - head, aria, head + 10);
            XBrush brushHeader = new XSolidBrush(XColor.FromArgb(13, 59, 128));
            gfx.DrawRectangle(brushHeader, rect);
            gfx.DrawRectangle(pen, rect);
            var tf = new XTextFormatter(gfx);
            string headStr;

            headStr = Localize("EXP-LBL-008");

            var xHead = gfx.MeasureString(headStr, fontHeader).Width;
            gfx.DrawString(Localize(headStr), fontHeader, brushHead, (page.Width - left - right - xHead) / 2, yPos);
            yPos += 40;

            gfx.DrawString(Localize("EXP-LBL-004") + ": " + arrTimeType[Convert.ToInt16(mData.timeType)], fontTableContent, brush, left + 50f, yPos);
            gfx.DrawString(Localize("EXP-LBL-009") + ": " + dateExport, fontTableContent, brush, left + 25f + aria / 2, yPos);

            yPos += 20;

            if (mData.timeType == "3")
                gfx.DrawString(Localize("EXP-LBL-005") + ": " + startDate.ToString("yyyy/MM/dd HH:00"), fontTableContent, brush, left + 50f, yPos);
            else
                gfx.DrawString(Localize("DAS-LBL-002") + startTime, fontTableContent, brush, left + 50f, yPos);
            tf.DrawString(Localize("EXP-LBL-010") + ": " + String.Join(", ", unitCodeArray), fontTableContent, brush, new XRect(left + 25f + aria / 2, yPos - 10, aria - (left + 25f + aria / 2), 100));

            yPos += 20;
            if (mData.timeType != "3")
                gfx.DrawString(Localize("DAS-LBL-003") + endTime, fontTableContent, brush, left + 50f, yPos);
            yPos += 60;



            var height = 0.0;
            var width = (page.Width - left - right);

            XImage image = XImage.FromFile(arrPngFilePath[dem]);
            XImage imgIcon = XImage.FromFile(Path.Combine(_hostingEnvironment.WebRootPath, "assets", "img", "upload.jpg"));
            height = width * Convert.ToDouble(image.PixelHeight) / Convert.ToDouble(image.PixelWidth);
            gfx.DrawImage(image, left, yPos, width, height);

            dem++;

            yPos += height;

            yPos += 40;
            XPen xpen = new XPen(XColors.Navy, 0.4);


            XStringFormat formatH = new XStringFormat();
            formatH.LineAlignment = XLineAlignment.Near;
            formatH.Alignment = XStringAlignment.Near;

            var noColumn = 30f;
            var columnWidth = (aria - noColumn) / (header.Length - 1);
            var headerPen = XPens.Black;
            var xPos = left;
            for (int j = 0; j < header.Length; j++)
            {
                var cellRect = new XRect(left + (j * columnWidth) - (columnWidth - noColumn), yPos, columnWidth, 30);
                var textLocation = new XRect(left + (j * columnWidth) - (columnWidth - noColumn) + 5, yPos + 5, columnWidth - 10, 30);
                if (j == 0)
                {
                    cellRect = new XRect(left + j * columnWidth, yPos, noColumn, 30);
                    textLocation = new XRect(left + j * columnWidth + 5, yPos + 5, noColumn, 30);
                }
                gfx.DrawRectangle(brushHeader, cellRect);
                gfx.DrawRectangle(headerPen, cellRect);

                var dataText = header[j];
                var dataSize = gfx.MeasureString(dataText, fontTableHeader);
                var dataCenter = cellRect.Left + (cellRect.Width - dataSize.Width) / 2;
                var dataYPos = cellRect.Top + (cellRect.Height - dataSize.Height) / 2;
                if (dataSize.Width > columnWidth)
                    tf.DrawString(header[j], fontTableHeader, brushHead, textLocation);
                else
                    tf.DrawString(header[j], fontTableHeader, brushHead, new XRect(dataCenter, dataYPos, columnWidth, 30));
                if (j == 0)
                {
                    xPos += noColumn;
                }
                else
                    xPos += columnWidth;

            }
            XStringFormat formatStringR = new XStringFormat();
            formatStringR.Alignment = XStringAlignment.Near;
            formatStringR.LineAlignment = XLineAlignment.Near;

            XStringFormat formatNumberR = new XStringFormat();
            formatNumberR.Alignment = XStringAlignment.Far;
            formatNumberR.LineAlignment = XLineAlignment.Near;
            int pageNumber = 1;
            yPos += 30;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                xPos = left;
                for (int m = 0; m < header.Length; m++)
                {
                    var cellRect = new XRect(xPos, yPos, columnWidth, 20);
                    if (m == 0)
                        cellRect = new XRect(xPos, yPos, noColumn, 20);
                    gfx.DrawRectangle(XBrushes.White, cellRect);
                    gfx.DrawRectangle(headerPen, cellRect);

                    var dataXPos = cellRect.Left + 2f;

                    var dataYPos = cellRect.Top + 5f;


                    if (m == 0)
                    {
                        gfx.DrawString((i + 1).ToString(), fontTableContent, brush, new XRect(cellRect.Left - 2f, dataYPos, noColumn, 30), formatNumberR);
                        xPos += noColumn;
                    }
                    else
                    {
                        if (dt.Columns[m - 1].ColumnName == "total_value" || dt.Columns[m - 1].ColumnName == "avg_value" || dt.Columns[m - 1].ColumnName == "standard_value")
                            gfx.DrawString(dt.Rows[i][m - 1] != DBNull.Value ? Convert.ToDecimal(dt.Rows[i][m - 1]).ToString("n2", new CultureInfo("en-US")) : "0", fontTableContent, brush, new XRect(cellRect.Left - 2f, dataYPos, columnWidth, 60), formatNumberR);
                        else if (dt.Columns[m - 1].ColumnName == "transdate")
                            gfx.DrawString(dt.Rows[i][m - 1].ToString(), fontTableContent, brush, new XRect(cellRect.Left - 2f, dataYPos, columnWidth, 60), formatNumberR);
                        else if (dt.Columns[m - 1].ColumnName == "performance")
                            gfx.DrawString(Convert.ToDecimal(dt.Rows[i][m - 1]).ToString("n2", new CultureInfo("en-US")) + "%", fontTableContent, brush, new XRect(cellRect.Left - 2f, dataYPos, columnWidth, 60), formatNumberR);
                        else
                            gfx.DrawString(dt.Rows[i][m - 1].ToString(), fontTableContent, brush, new XRect(dataXPos, dataYPos, columnWidth, 60), formatStringR);
                        xPos += columnWidth;
                    }

                }
                yPos += 20;


                if (yPos >= (page.Height - bot))
                {
                    gfx.DrawString(pageNumber.ToString(), fontTableContent, brush, page.Width - right - 10, page.Height - 20);
                    pageNumber++;
                    page = document.AddPage();
                    page.Width = new XUnit(8.27, XGraphicsUnit.Inch);
                    page.Height = new XUnit(11.69, XGraphicsUnit.Inch);
                    xPos = left;
                    yPos = top;
                    gfx = XGraphics.FromPdfPage(page);
                    tf = new XTextFormatter(gfx);
                    for (int j = 0; j < header.Length; j++)
                    {
                        var cellRect = new XRect(left + (j * columnWidth) - (columnWidth - noColumn), yPos, columnWidth, 30);
                        var textLocation = new XRect(left + (j * columnWidth) - (columnWidth - noColumn) + 5, yPos + 5, columnWidth - 10, 30);
                        if (j == 0)
                        {
                            cellRect = new XRect(left + j * columnWidth, yPos, noColumn, 30);
                            textLocation = new XRect(left + j * columnWidth + 5, yPos + 5, noColumn, 30);
                        }
                        gfx.DrawRectangle(brushHeader, cellRect);
                        gfx.DrawRectangle(headerPen, cellRect);

                        var dataText = header[j];
                        var dataSize = gfx.MeasureString(dataText, fontTableHeader);
                        var dataCenter = cellRect.Left + (cellRect.Width - dataSize.Width) / 2;
                        var dataYPos = cellRect.Top + (cellRect.Height - dataSize.Height) / 2;
                        if (dataSize.Width > columnWidth)
                            tf.DrawString(header[j], fontTableHeader, brushHead, textLocation);
                        else
                            tf.DrawString(header[j], fontTableHeader, brushHead, new XRect(dataCenter, dataYPos, columnWidth, 30));
                        if (j == 0)
                        {
                            xPos += noColumn;
                        }
                        else
                            xPos += columnWidth;
                    }
                    yPos += 30;
                }
                if (i == dt.Rows.Count - 1)
                {
                    gfx.DrawString(pageNumber.ToString(), fontTableContent, brush, page.Width - right - 10, page.Height - 20);
                }
            }
            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream);
                pdfBytes = stream.ToArray();

            }
            if (Directory.Exists(pathTemps))
            {
                Directory.Delete(pathTemps, true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return pdfBytes;
    }

    string[] GetTableNameByUnitType(int dashboardType)
    {
        string tableName = "";
        string unit_type = "";
        string tableNameRealTime = "";
        try
        {
            switch ((Constants.Dashboard)dashboardType)
            {

                case Dashboard.Line:
                    tableName = Constants.ELECTRIC_CABINET_CONSUMPTION;
                    unit_type = Constants.ELECTRIC_CABINET_TYPE;
                    tableNameRealTime = Constants.ELECTRIC_CABINET_REAL_TIME;
                    break;
                case Dashboard.Pac:
                    tableName = Constants.CONDITIONER_CONSUMPTION;
                    unit_type = Constants.CONDITIONER_TYPE;
                    tableNameRealTime = Constants.CONDITIONER_REAL_TIME;
                    break;
                case Dashboard.Air:
                    tableName = Constants.COMPRESSOR_CONSUMPTION;
                    unit_type = Constants.COMPRESSOR_TYPE;
                    tableNameRealTime = Constants.COMPRESSOR_REAL_TIME;
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
        return new string[] { tableName, unit_type, tableNameRealTime };
    }

    public byte[] ExportPdfApi(MyData mdata)
    {
        byte[] pdfBytes = null;
        try
        {
            string[] rs = GetTableNameByUnitType(mdata.eDashboard);
            string tableName = rs[0];
            string unit_type = rs[1];


            DateTime startTime = Convert.ToDateTime(mdata.startTime);
            DateTime endTime = Convert.ToDateTime(mdata.endTime);
            if (mdata.startTime == null)
            {
                var time = GenerateDateTimeByTimeType(mdata.timeType);
                startTime = time[0];
                endTime = time[1];
            }
            if (mdata.timeType == "3")
            {
                startTime = startTime.AddMinutes(10);
                endTime = endTime.AddMinutes(10);
            }
            string langcode = Thread.CurrentThread.CurrentUICulture.Name;
            string[] formatDate = getStringTimeType(mdata.timeType, langcode);
            string[] strUnits = ConvertUnitsToFormatParam1(mdata.unitCode);
            DataTable dt = callFuncPostgre("get_data_export_pdf", new string[] { "pr_table_name", "pr_query", "pr_lang_code", "pr_format_date", "pr_units", "pr_units_name", "pr_avg_value", "pr_unit_type", "pr_start_time", "pr_end_time" }, new object[] { tableName, strUnits[0], langcode, formatDate[0], mdata.unitCode, strUnits[1], strUnits[2], unit_type, startTime, endTime });
            mdata.startTime = startTime.ToString();
            mdata.endTime = endTime.ToString();
            if (mdata.timeType == "3")
                dt = RoundDownTime(dt, "transdate");

            pdfBytes = ExportDataTableToPdf(dt, mdata, langcode);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
        return pdfBytes;
    }
    public byte[] ExportCsvApi(MyData mdata)
    {
        byte[] pdfBytes = null;
        try
        {
            string[] rs = GetTableNameByUnitType(mdata.eDashboard);
            string tableName = rs[0];
            string unit_type = rs[1];

            string langcode = Thread.CurrentThread.CurrentUICulture.Name;
            DateTime startTime = Convert.ToDateTime(mdata.startTime);
            DateTime endTime = Convert.ToDateTime(mdata.endTime);
            if (mdata.startTime == null)
            {
                var time = GenerateDateTimeByTimeType(mdata.timeType);
                startTime = time[0];
                endTime = time[1];
            }
            if (mdata.timeType == "3")
            {
                startTime = startTime.AddMinutes(10);
                endTime = endTime.AddMinutes(10);
            }


            string[] formatDate = getStringTimeType(mdata.timeType, langcode);
            string[] strUnits = ConvertUnitsToFormatParam1(mdata.unitCode);
            DataTable dt = callFuncPostgre("get_data_export_csv", new string[] { "pr_table_name", "pr_query", "pr_format_date", "pr_lang_code", "pr_units", "pr_units_name", "pr_avg_value", "pr_unit_type", "pr_start_time", "pr_end_time" }, new object[] { tableName, strUnits[0], formatDate[1], langcode, mdata.unitCode, strUnits[1], strUnits[2], unit_type, startTime, endTime });
            if (mdata.timeType == "3")
                dt = RoundDownTime(dt, "transdate");
            pdfBytes = ExportDataTableToCsv(dt, mdata.timeType);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
        return pdfBytes;
    }

    public DataTable SearchDataApi(int type, string units, DateTime startTime, DateTime endTime, int eDashboard)
    {
        DataTable dtData = new DataTable();
        try
        {
            string[] rs = GetTableNameByUnitType(eDashboard);
            string tableName = rs[0];
            string unit_type = rs[1];
            string formatTime;

            string[] strUnits = ConvertUnitsToFormatParam1(units);


            switch (type)
            {
                case 0:
                    if (startTime.Day != endTime.Day)
                        formatTime = Constants.DAY_FORMAT;
                    else
                        formatTime = Constants.HOUR_FORMAT;
                    break;
                case 1:
                    if (startTime.Month != endTime.Month)
                        formatTime = Constants.MONTH_FORMAT;
                    else
                        formatTime = Constants.DAY_FORMAT;
                    break;
                case 2:
                    if (startTime.Year != endTime.Year)
                        formatTime = Constants.YEAR_FORMAT;
                    else
                        formatTime = Constants.MONTH_FORMAT;
                    break;
                case 3:
                    startTime = startTime.AddMinutes(10);
                    endTime = endTime.AddMinutes(10);
                    formatTime = Constants.MINUTE_FORMAT;
                    break;
                default:
                    formatTime = Constants.DAY_FORMAT;
                    break;
            }

            string langcode = Thread.CurrentThread.CurrentUICulture.Name;


            dtData = callFuncPostgre("get_data_consumption", new string[] { "pr_table_name", "pr_query", "pr_units", "pr_units_value", "pr_avg_value", "pr_unit_type", "pr_lang_code", "pr_format", "pr_start_time", "pr_end_time" }, new object[] { tableName, strUnits[0], strUnits[1], units, strUnits[2], unit_type, langcode, formatTime, startTime, endTime });

            if (type == 3)
                dtData = RoundDownTime(dtData, "datetime");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
        return dtData;
    }

    public string LoadDataApi(string units, int eDashboard)
    {
        string json = "";
        try
        {
            string[] rs = GetTableNameByUnitType(eDashboard);
            string tableName = rs[0];
            string unit_type = rs[1];
            string tableNameRealTime = rs[2];
            string langcode = Thread.CurrentThread.CurrentUICulture.Name;

            DataSet ds = new DataSet();
            DataTable dtPerformance = new DataTable();
            string formatTime = "";
            string[] strUnits = ConvertUnitsToFormatParam1(units);


            for (int i = 0; i < 4; i++)
            {
                string startTime = "";
                string endTime = "";
                switch (i)
                {
                    case 0:
                        startTime = DateTime.Now.ToString("yyyy/MM/dd 00:00:00");
                        endTime = DateTime.Now.ToString("yyyy/MM/dd 23:59:59");
                        formatTime = Constants.HOUR_FORMAT;
                        break;
                    case 1:
                        startTime = DateTime.Now.ToString("yyyy/MM/01 00:00:00");
                        endTime = DateTime.Now.ToString($"yyyy/MM/{DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)} 23:59:59");
                        formatTime = Constants.DAY_FORMAT;
                        break;
                    case 2:
                        startTime = DateTime.Now.ToString("yyyy/01/01 00:00:00");
                        endTime = DateTime.Now.ToString($"yyyy/12/{DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)} 23:59:59");
                        formatTime = Constants.MONTH_FORMAT;
                        break;
                    case 3:
                        startTime = DateTime.Now.ToString("yyyy/MM/dd HH:10:00");
                        endTime = DateTime.Now.ToString($"yyyy/MM/dd {DateTime.Now.Hour + 1}:10:00");
                        formatTime = Constants.MINUTE_FORMAT;
                        break;
                    default:

                        break;
                }
                ds.Tables.Add(callFuncPostgre("get_data_consumption", new string[] { "pr_table_name", "pr_query", "pr_units", "pr_units_value", "pr_avg_value", "pr_unit_type", "pr_lang_code", "pr_format", "pr_start_time", "pr_end_time" }, new object[] { tableName, strUnits[0], strUnits[1], units, strUnits[2], unit_type, langcode, formatTime, Convert.ToDateTime(startTime), Convert.ToDateTime(endTime) }));

            }
            dtPerformance = callFuncPostgre("get_data_real_time_one_hour_prev", new string[] { "pr_table_name", "pr_units", "pr_units_name", "pr_unit_type" }, new object[] { tableName.Replace("consumption", "real_time"), units, strUnits[1], unit_type });

            var realTimeData = _unitService.GetUnitsByType(unit_type);
            var data = new
            {

                DayChart = ds.Tables[0],
                MonthChart = ds.Tables[1],
                YearChart = ds.Tables[2],
                HourChart = RoundDownTime(ds.Tables[3], "datetime"),
                Performance = dtPerformance

            };

            json = data.ToJson();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
        return json;
    }



    string[] ConvertUnitsToFormatParam1(string units)
    {
        string strUnits = "";
        string[] arrUnits = units.Split(',');
        string[] arrUnitsAvg = units.Split(',');
        string[] arrUnitsSum = units.Split(',');
        for (int i = 0; i < arrUnits.Length; i++)
        {
            arrUnits[i] = $"'{arrUnits[i].Trim()}'";
            arrUnitsAvg[i] = $"avg_{arrUnitsAvg[i].Trim()}";
            arrUnitsSum[i] = $"sum({arrUnitsSum[i].Trim()}) As {arrUnitsSum[i].Trim()}, avg({arrUnitsSum[i].Trim()}) As avg_{arrUnitsSum[i].Trim()}";
        }
        strUnits = string.Join(", ", arrUnits);

        return new string[] { string.Join(", ", arrUnitsSum), strUnits, string.Join(", ", arrUnitsAvg) };
    }

    public DataTable SumData(DataTable dt, DataTable dtPerformance)
    {

        DataTable groupedData = new DataTable();
        try
        {
            DataView dv = new DataView(dt);
            foreach (DataRow row in dt.Rows)
            {
                if (row["unit_value"] == DBNull.Value)
                {
                    row["unit_value"] = 0;
                }
            }
            groupedData = dv.ToTable(true, "unit_name", "display_name", "standard_value");
            groupedData.Columns.Add("unit_value", typeof(decimal));
            groupedData.Columns.Add("performance", typeof(decimal));

            foreach (DataRow row in groupedData.Rows)
            {
                string unit = (string)row["unit_name"];
                object value = row["standard_value"];

                decimal standardValue = value != DBNull.Value ? Convert.ToDecimal(row["standard_value"]) : 0;
                decimal sumValue = dt.AsEnumerable()
                                 .Where(r => r.Field<string>("unit_name") == unit)
                                 .Sum(r => r.Field<decimal>("unit_value"));
                row["unit_value"] = sumValue;

                decimal per = dtPerformance.AsEnumerable()
                                .Where(r => r.Field<string>("unit_name") == unit)
                                .Select(r => r.Field<decimal>("performance")).FirstOrDefault();
                row["performance"] = per;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }

        return groupedData;
    }

    public DataTable SumData(DataTable dt)
    {
        DataTable groupedData = new DataTable();
        try
        {
            DataView dv = new DataView(dt);

            groupedData = dv.ToTable(true, "unit_name", "display_name", "standard_value");
            groupedData.Columns.Add("unit_value", typeof(decimal));

            foreach (DataRow row in groupedData.Rows)
            {
                string unit = (string)row["unit_name"];
                object value = row["standard_value"];

                decimal standardValue = value != DBNull.Value ? (decimal)row["standard_value"] : 0;
                decimal sumValue = dt.AsEnumerable()
                                 .Where(r => r.Field<string>("unit_name") == unit)
                                 .Sum(r => r.Field<decimal>("unit_value"));
                row["unit_value"] = sumValue;


            }
        }
        catch (Exception)
        {

            throw;
        }

        return groupedData;
    }

    public DataTable RoundDownTime(DataTable dt, string columnName)
    {
        DataTable groupedData = dt;
        try
        {



            foreach (DataRow row in groupedData.Rows)
            {
                string transdate = (string)row[columnName];
                string[] time = transdate.Split(':');
                int minute = Convert.ToInt16(time[1]);
                minute = minute / 10 * 10;

                row[columnName] = time[0] + ':' + minute.ToString("00");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }

        return groupedData;
    }
}
