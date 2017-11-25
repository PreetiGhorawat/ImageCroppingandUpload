using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Web.Security;
using System.Collections.Generic;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.Script.Serialization;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Amazon;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;
using System.Text;
using System.Xml;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.html;
using System.Web.UI.HtmlControls;
using iTextSharp.text;
using System.Globalization;
using iTextSharp.text.pdf;

public static class Tools
{
    #region Generic Function
        
    public static string RemoveTag(string StringToRemoveTag)
    {
        if (StringToRemoveTag != "")
        {
            if (StringToRemoveTag.Substring(0, 3) == "<p>")
                StringToRemoveTag = StringToRemoveTag.Substring(3, (StringToRemoveTag.Length - 3));
            if (StringToRemoveTag.Substring((StringToRemoveTag.Length - 4), 4) == "</p>")
                StringToRemoveTag = StringToRemoveTag.Substring(0, (StringToRemoveTag.Length - 4));
        }
        return StringToRemoveTag;
    }

    public static string CompileSQL(string StringToCompile)
    {
        return StringToCompile.Replace("'", "''");
    }

    public static string GetIP()
    {
        return HttpContext.Current.Request.ServerVariables["remote_addr"];
    }

    //Date format for database
    public static string GetDateFormat(DateTime dateValue)
    {
        return dateValue.ToString("yyyy/MM/dd HH:mm:ss");
    }

    public static bool CheckCSV(string name)
    {
        bool value = false;
        string fileExt = name.Substring(name.LastIndexOf(".") + 1, 3);
        if (fileExt.ToLower().Trim() == "csv")
            value = true;
        return value;
    }

    public static string FormatMoney(string value)
    {
        string MoneyValue = "";
        float money;
        float.TryParse(value, out money);
        MoneyValue = String.Format("{0:C2}", money).Replace("$", "");
        return MoneyValue;
    }

    public static bool CreateBucket(IAmazonS3 client, string FolderName)
    {
        try
        {
            bool bucketexists = false;
            ListBucketsResponse response = client.ListBuckets();
            foreach (S3Bucket b in response.Buckets)
            {
                if (b.BucketName == FolderName)
                {
                    bucketexists = true;
                }
            }
            if (!bucketexists)
            {
                PutBucketRequest brequest = new PutBucketRequest();
                brequest.BucketName = FolderName;
                client.PutBucket(brequest);
            }
            return true;
        }
        catch { return false; }
    }

    static IAmazonS3 client;
    public static string SaveImage(string Path, string FolderName, out string err)
    {
        string id = "";
        err = "";
        try
        {
            string filePath = UploadImage(Path, FolderName);

            string saveName = Path.Substring(Path.LastIndexOf(@"\") + 1);
            List<Tools.SqlContainer> SQLCommands = new List<Tools.SqlContainer>();
            {
                Tools.SqlContainer SQLContainer = new Tools.SqlContainer();
                SQLContainer.Query = "INSERT INTO Files ([FilePathInServer],[OriginalFileName]) VALUES (@FilePathInServer, @OriginalFileName)";
                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                SqlParameters.Add(new SqlParameter("FilePathInServer", filePath));
                SqlParameters.Add(new SqlParameter("OriginalFileName", saveName));
                SQLContainer.SqlParameters = SqlParameters;
                SQLCommands.Add(SQLContainer);
            }
            if (Tools.ExecuteSQL(SQLCommands, out err))
            {
                DataSet ds = new DataSet("ds");
                SQLCommands = new List<Tools.SqlContainer>();
                {
                    Tools.SqlContainer SQLContainer = new Tools.SqlContainer();
                    SQLContainer.Query = "SELECT MAX(FileID) AS ID FROM Files";
                    SQLCommands.Add(SQLContainer);
                }
                if (Tools.GetData(SQLCommands, out ds, out err))
                {
                    err = "";
                    id = ds.Tables[0].Rows[0][0].ToString().Trim();
                }
            }
        }
        catch { }
        return id;
    }

    public static string UploadImage(string Path, string FolderName)
    {
        string filepath = "";
        try
        {
            filepath = UploadImagewithLocalCopy(Path, FolderName);
            File.Delete(Path);
        }
        catch { }
        return filepath;
    }

    public static string UploadImagewithLocalCopy(string Path, string FolderName)
    {
        string filepath = "";
        try
        {
            string saveName = Path.Substring(Path.LastIndexOf(@"\") + 1);
            using (client = new AmazonS3Client(Tools.GetConfigValue(Tools.KeyVariables.S3Key), Tools.GetConfigValue(Tools.KeyVariables.S3Secret), RegionEndpoint.APSoutheast1))
            {
                // Creates the bucket.
                CreateBucket(client, FolderName);
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = FolderName,
                    Key = saveName,
                    FilePath = Path
                };
                request.CannedACL = Amazon.S3.S3CannedACL.PublicRead;
                PutObjectResponse response2 = client.PutObject(request);
            }
            filepath = GetConfigValue(KeyVariables.S3Url).Replace("[FOLDERNAME]", FolderName) + saveName;
        }
        catch (Exception ex)
        {
            string err = "";
            List<Tools.SqlContainer> SQLCommands = new List<Tools.SqlContainer>();
            {
                Tools.SqlContainer SQLContainer = new Tools.SqlContainer();
                SQLContainer.Query = "UPDATE Config SET ConfigValue = @ConfigValue WHERE ConfigKey = 'Error'";
                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                SqlParameters.Add(new SqlParameter("ConfigValue", ex.ToString().Trim()));
                SQLContainer.SqlParameters = SqlParameters;
                SQLCommands.Add(SQLContainer);
            }
            ExecuteSQL(SQLCommands, out err);
        }
        return filepath;
    }

    public static byte[] imageToByteArray(System.Drawing.Image imageIn)
    {
        MemoryStream ms = new MemoryStream();
        imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        return ms.ToArray();
    }

    #endregion Generic Funtion

    #region Data Function

    public struct SqlContainer
    {
        public string Query;
        public List<SqlParameter> SqlParameters;
    }

    public static string GetConfigValue(string key)
    {

        string value = "";
        string err = "";
        List<Tools.SqlContainer> SQLCommands = new List<Tools.SqlContainer>();
        {
            Tools.SqlContainer SQLContainer = new Tools.SqlContainer();
            SQLContainer.Query = "Select ConfigValue From Config Where ConfigKey=@ConfigKey";
            List<SqlParameter> SqlParameters = new List<SqlParameter>();
            SqlParameters.Add(new SqlParameter("ConfigKey", key));
            SQLContainer.SqlParameters = SqlParameters;
            SQLCommands.Add(SQLContainer);
        }
        DataSet ds = new DataSet("ds");
        if (GetData(SQLCommands, out ds, out err))
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                value = ds.Tables[0].Rows[0]["ConfigValue"].ToString().Trim();
            }
        }
        return value;
    }

    public static bool GetData(List<SqlContainer> SqlContainers, out DataSet SelectedData, out string ErrorIfAny)
    {
        int MaxRecords = 0;
        SelectedData = new DataSet();
        ErrorIfAny = string.Empty;
        try
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();

            // Validate the query string
            if ((SqlContainers == null) || (SqlContainers.Count == 0))
            {
                ErrorIfAny = "Empty select querys";
                return false;
            }

            // Loop through the select quries and retrive the data
            int TableCount = 0;
            foreach (SqlContainer _SqlContainer in SqlContainers)
            {
                string TableName = "Table_" + TableCount.ToString();
                TableCount++;
                SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter();
                _SqlDataAdapter.SelectCommand = new SqlCommand(_SqlContainer.Query, new SqlConnection(ConnectionString));
                if (_SqlContainer.SqlParameters != null)
                {
                    foreach (SqlParameter _SqlParameter in _SqlContainer.SqlParameters)
                    {
                        _SqlDataAdapter.SelectCommand.Parameters.Add(_SqlParameter);
                    }
                }
                _SqlDataAdapter.Fill(SelectedData, 0, MaxRecords, TableName);
                _SqlDataAdapter.Dispose();
            }
            return true;
        }
        catch (Exception eX)
        {
            ErrorIfAny = eX.Message + Environment.NewLine + eX.StackTrace;
            return false;
        }
    }

    public static bool ExecuteSQL(List<SqlContainer> SqlContainers, out string ErrorOnFailure)
    {
        SqlConnection _SqlConnection = null;
        ErrorOnFailure = string.Empty;
        string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        try
        {
            if ((SqlContainers == null) || (SqlContainers.Count == 0))
            {
                throw new Exception("Empty commands");
            }
            _SqlConnection = new SqlConnection(ConnectionString);
            _SqlConnection.Open();
            foreach (SqlContainer _SqlContainer in SqlContainers)
            {
                SqlCommand _SqlCommand = new SqlCommand(_SqlContainer.Query, _SqlConnection);
                _SqlCommand.CommandTimeout = 600000;
                if (_SqlContainer.SqlParameters != null)
                {
                    foreach (SqlParameter _SqlParameter in _SqlContainer.SqlParameters)
                    {
                        _SqlCommand.Parameters.Add(_SqlParameter);
                    }
                }
                _SqlCommand.ExecuteNonQuery();
            }
            return true;
        }
        catch (Exception eX)
        {
            ErrorOnFailure = eX.Message + Environment.NewLine + eX.StackTrace;
            return false;
        }
        finally
        {
            try
            {
                if ((_SqlConnection != null) && (_SqlConnection.State == ConnectionState.Open))
                {
                    _SqlConnection.Close();
                }
            }
            catch { }
        }
    }

    #endregion Data Funtion
    
    public static class KeyVariables
    {
        public static string AdminEmail = "AdminEmail";
        public static string AdminLogPeriod = "AdminLogPeriod";
        public static string FromMailAddress = "FromMailAddress";
        public static string FromMailName = "FromMailName";
        public static string GoogleCaptchaUrl = "GoogleCaptchaUrl";
        public static string ImageFolderLocation = "ImageFolderLocation";
        public static string ImageRootFolder = "ImageRootFolder";
        public static string MandrillAPIKey = "MandrillAPIKey";
        public static string MarketPlaceMerchantID = "MarketPlaceMerchantID";
        public static string ReplyMailAddress = "ReplyMailAddress";
        public static string ReplyMailName = "ReplyMailName";
        public static string SiteTitle = "SiteTitle";
        public static string S3Key = "S3Key";
        public static string S3Secret = "S3Secret";
        public static string S3Url = "S3Url";
        public static string S3Root = "S3Root";
        public static string TotalAllowedCategories = "TotalAllowedCategories";
        public static string IPayMerchantCode = "IPayMerchantCode";
        public static string IPayMerchantKey = "IPayMerchantKey";
        public static string IPayRequestURL = "IPayRequestURL";
        public static string IPayResponseUrl = "IPayResponseUrl";
        public static string IPayBackendUrl = "IPayBackendUrl";
        public static string ServerUrl = "ServerUrl";
        public static string AlchemyRequestUri = "AlchemyRequestUri";
        public static string AlchemyAPIKey = "AlchemyAPIKey";
        public static string TradeLeadDuration = "TradeLeadDuration";
        public static string EmailFileAttachment = "EmailFileAttachment";
        public static string EnableEnglishCheck = "EnableEnglishCheck";
        public static string MaxBrowsingProducts = "MaxBrowsingProducts";
        public static string GoogleMapURL = "GoogleMapURL";
        public static string PageCount = "PageCount";
        public static string EmptyImageURL = "EmptyImageURL";
        public static string MaxFavoriteProductCategory = "MaxFavoriteProductCategory";
        public static string MaxFavoriteServiceCategory = "MaxFavoriteServiceCategory";
        public static string MaxFavoriteCountries = "MaxFavoriteCountries";
    }

 public static string CsvEscape(string value)
    {
        if (value.Contains(","))
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
        return value;
    }
}


