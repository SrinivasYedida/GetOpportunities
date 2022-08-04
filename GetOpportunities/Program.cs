using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
//using System.Web.Extensions;
using System.Xml.Linq;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GetOpportunities
{
    class Program
    {
        static string serializedEst;
        static DateTime dlrTime;
        static DateTime utcDateStart;
        static DateTime utcDateEnd;
        static string deltaDateStart;
        static string deltaDateEnd;
        static int Identity = 0;
        static string iHrs;
        static string iMins;
        static string iDay;
        public static string authkey = "";
        public static int DmsId = 0;
        public static int EndUserId = 0;
        public static string EndUserTitle = "";
        public static string ServiceURL = "";
        public static string APIKey = "";
        public static string SecretKey = "";
        public static string SubscriptionId = "";
        public static int SrcType = 0;

        static void Main(string[] args)
        {
            SrcType = 3;
            GetVSToken();
            string time_zone = String.Empty;
            List<CdkDealers> dlrList = BL_Opportunities.GetDMSActiveDealers(1, SrcType, ""); // 1 refers DMS Type Source Name is "CDK"

            foreach (CdkDealers row in dlrList)
            {
                DmsId = row.DMS_ID;
                EndUserId = row.DMS_ENDUSER_ID;
                EndUserTitle = row.DMS_ENDUSER_TITLE;
                ServiceURL = row.DSTDP_SERVICE_URL;
                APIKey = row.DS_APIKEY;
                SecretKey = row.DS_SECRETKEY;
                SubscriptionId = row.DED_SUBSCRIPTIONID;

               
                dlrTime = row.ZONETIME;
                int dlrTm = dlrTime.Hour;
                int dlrMin = dlrTime.Minute;
                int dlrDay = dlrTime.Day;
                iDay = dlrDay.ToString();

                iHrs = dlrTm.ToString();
                if (Convert.ToInt32(iHrs) <= 9)
                    iHrs = "0" + iHrs;

                iMins = dlrMin.ToString();
                if (Convert.ToInt32(iMins) <= 9)
                    iMins = "0" + iMins;


                if (dlrTm == 8)
                    GetOpportunities(dlrTm);
                else if (dlrTm == 13)
                    GetOpportunities(dlrTm);
                else if (dlrTm == 21)
                    GetOpportunities(dlrTm);
                else if (args[0] == "Y")
                    GetOpportunities(dlrTm);               

            }
            List<OpportunitiesDeltaAction> Opportunities = BL_Opportunities.OpportunitiesDeltaAction("D", "", 59);

        }
        public static void GetVSToken()
        {
            string[] TokenExpire = new string[2];
            TokenExpire = DA_Opportunities.CheckVSTokenExpiry();
            if (TokenExpire[0] == "0")
                authkey = GetBearerToken();
            else
                authkey = TokenExpire[1];
        }

        public static string GetBearerToken()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string resultData = "";
            WebRequest request = null;
            WebResponse response = null;
            Stream stream = null;
            try
            {
                string url = "https://identity.fortellis.io/oauth2/aus1p1ixy7YL8cMq02p7/v1/token";
                string postData = "grant_type=client_credentials&client_secret=fakAFPIArhO2pp3p&scope=anonymous&client_id=heInundwhF8iHEByMAHwBFxkGGAc1rPk";
                //string postData = "grant_type=client_credentials&client_secret="+SecretKey+"&scope=anonymous&client_id="+APIKey+"";
                //encode post data     
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(postData);
                request = HttpWebRequest.Create(url);
                request.Method = "POST";
                request.ContentLength = data.Length;
                request.ContentType = "application/x-www-form-urlencoded";
                CredentialCache mycache = new CredentialCache();
                request.Credentials = mycache;
                Stream newStream = request.GetRequestStream();
                // Send the post data.     
                newStream.Write(data, 0, data.Length);
                newStream.Close();
                response = request.GetResponse();
                stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                resultData = sr.ReadToEnd();
                stream.Close();
                stream.Dispose();
                Item Item = JsonConvert.DeserializeObject<Item>(resultData);
                authkey = Item.access_token.ToString();
                int err_no = DA_Opportunities.InsertVSToken(Item.access_token, Item.expires_in, Item.token_type);
            }
            catch (Exception e)
            {
            }
            return authkey;
        }


        public static void GetOpportunities(int dlrTm)
        {
            restartOpportunities:
            string resultData = "";
            WebRequest request = null;
            WebResponse response = null;
            int Page = 1;
            int PageSize = 10;
            string Date = DateTime.Today.AddDays(-1).ToString("MM-dd-yyyy");
            
            //try
            //{
                int totalRecCnt = GetTotalOpportunities();
            if (totalRecCnt > 0) {
                try {
                    string url = "https://api.fortellis.io/sales/v2/elead/opportunities/searchDelta?page=" + Convert.ToString(Page) + "&pageSize=" + Convert.ToString(totalRecCnt) + "&DateFrom=" + Date;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    request = HttpWebRequest.Create(url);
                    request.Method = "GET";
                    //request.ContentType = "application/json";
                    CredentialCache mycache = new CredentialCache();
                    mycache.Add(new Uri(url), "No", new NetworkCredential());
                    request.Credentials = mycache;
                    request.Headers.Add("Subscription-Id", SubscriptionId);
                    request.Headers.Add("Request-Id", "owned-vehicles");
                    request.Headers.Add("api_key", APIKey);
                    request.Headers.Add("Authorization", "Bearer " + authkey);
                    response = request.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    // Do whatever you need with the response
                    Byte[] myData = ReadFully(responseStream);
                    resultData = System.Text.ASCIIEncoding.ASCII.GetString(myData);
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(resultData, "opportunities");
                    //XmlNode root = doc.DocumentElement;
                    //XmlElement elem = doc.CreateElement("positionName");
                    //elem.InnerText = positionName;
                    //root.AppendChild(elem);
                    var stringWriter = new StringWriter();
                    var xmlTextWriter = XmlWriter.Create(stringWriter);
                    doc.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    responseStream.Close();
                    string str = stringWriter.ToString();
                    str = doc.OuterXml.ToString();
                    //  try
                    //{
                    string filePath = "D:\\DMS\\eLeads\\GetOpportunity\\" + DmsId + ".xml";
                    doc.Save(filePath);
                    DA_Opportunities.LogInsertion(DmsId, str, "0", "", "D", "", "N", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                    // }              
                    //catch (Exception e)
                    //{
                    //var w32ex = e as Win32Exception;
                    //string ErrorCode = "";
                    //if (w32ex == null)
                    //{
                    //    w32ex = e.InnerException as Win32Exception;
                    //}
                    //if (w32ex != null)
                    //{
                    //    ErrorCode = Convert.ToString(w32ex.ErrorCode);
                    //    // do stuff
                    //}
                    //    DA_Opportunities. DA_Opportunities.ErrorLogInsertion(DmsId, e.HResult.ToString(), "Error2 :" + e.Message, "D", "", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                    //    DA_Opportunities.LogInsertion(DmsId, str, ErrorCode, e.Message, "D", "", "N", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                    //}
                }
                catch (WebException e)
                {
                    if (e.Status == WebExceptionStatus.ProtocolError)
                    {
                        // protocol errors find the statuscode in the Response
                        // the enum statuscode can be cast to an int.
                        int code = (int)((HttpWebResponse)e.Response).StatusCode;
                        string content;
                        using (var reader = new StreamReader(e.Response.GetResponseStream()))
                        {
                            content = reader.ReadToEnd();
                        }
                        // do what ever you want to store and return to your callers
                        dynamic errObj = JObject.Parse(content);
                        string errMsg = errObj.code + " - " + errObj.message;
                        if (code == 401 && errObj.message == "Invalid Bearer Token - Token Expired")
                        {
                            authkey = GetBearerToken();
                            goto restartOpportunities;
                        }
                        else
                        {
                            DA_Opportunities.ErrorLogInsertion(DmsId, code.ToString(), "Error1 :" + errMsg, "D", "", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                            DA_Opportunities.LogInsertion(DmsId, "", code.ToString(), "Error1 :" + errMsg, "D", "", "N", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                        }
                    }
                    else
                    {
                        DA_Opportunities.ErrorLogInsertion(DmsId, e.HResult.ToString(), "Error2 :" + e.Message, "D", "", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                        DA_Opportunities.LogInsertion(DmsId, "", e.HResult.ToString(), "Error2 :" + e.Message, "D", "", "N", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                    }

                }
            }
           
        }

        public static int GetTotalOpportunities()
        {
                restartTotalOpportunities:
                string resultData = "";
                WebRequest request = null;
                WebResponse response = null;
                int Page = 1;
                int PageSize = 10;
                string Date = DateTime.Today.AddDays(-1).ToString("MM-dd-yyyy");

                try
                {
                    string url = "https://api.fortellis.io/sales/v2/elead/opportunities/searchDelta?page=" + Convert.ToString(Page) + "&pageSize=" + Convert.ToString(PageSize) + "&DateFrom=" + Date;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    request = HttpWebRequest.Create(url);
                    request.Method = "GET";
                    //request.ContentType = "application/json";
                    CredentialCache mycache = new CredentialCache();
                    mycache.Add(new Uri(url), "No", new NetworkCredential());
                    request.Credentials = mycache;
                    request.Headers.Add("Subscription-Id", SubscriptionId);
                    request.Headers.Add("Request-Id", "owned-vehicles");
                    request.Headers.Add("api_key", APIKey);
                    request.Headers.Add("Authorization", "Bearer " + authkey);
                    response = request.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    // Do whatever you need with the response
                    Byte[] myData = ReadFully(responseStream);
                    resultData = System.Text.ASCIIEncoding.ASCII.GetString(myData);
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(resultData, "opportunities");
                    //XmlNode root = doc.DocumentElement;
                    //XmlElement elem = doc.CreateElement("positionName");
                    //elem.InnerText = positionName;
                    //root.AppendChild(elem);
                    var stringWriter = new StringWriter();
                    var xmlTextWriter = XmlWriter.Create(stringWriter);
                    doc.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    responseStream.Close();
                    string str = stringWriter.ToString();
                    str = doc.OuterXml.ToString();
                    PageSize = Convert.ToInt32(doc.DocumentElement.SelectSingleNode("/opportunities/totalItems").InnerText);
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    // protocol errors find the statuscode in the Response
                    // the enum statuscode can be cast to an int.
                    string code = ((HttpWebResponse)e.Response).StatusCode.ToString();
                    string content;
                    using (var reader = new StreamReader(e.Response.GetResponseStream()))
                    {
                        content = reader.ReadToEnd();
                    }
                    // do what ever you want to store and return to your callers
                    dynamic errObj = JObject.Parse(content);
                    string errMsg = errObj.code + " - " + errObj.message;
                    if (code == "401" && errObj.message == "Invalid Bearer Token - Token Expired")
                    {
                        authkey = GetBearerToken();
                        goto restartTotalOpportunities;
                    }                  
                    else
                    {
                        DA_Opportunities.ErrorLogInsertion(DmsId, code.ToString(), "Error1 :" + errMsg, "D", "", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                    }
                }
                else
                {
                         DA_Opportunities.ErrorLogInsertion(DmsId, e.HResult.ToString(), "Error2 :" + e.Message, "D", "", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                }
                PageSize = 0;
            }
            catch (Exception e)
            {
                DA_Opportunities.ErrorLogInsertion(DmsId, e.HResult.ToString(), "Error2 :" + e.Message, "D", "", dlrTime, Convert.ToString(dlrTime), SrcType, 0);
                PageSize = 0;
            }
            return PageSize;
        }


        public static byte[] ReadFully(Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }
    }
    public class Item
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string token_type { get; set; }
    }
}