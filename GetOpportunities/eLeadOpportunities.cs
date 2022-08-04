using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GetOpportunities
{
    class eLeadOpportunities
    {
        public void GetOpportinityIds(int DmsId)
        {

            List<Opportunities> OpList = BL_Opportunities.GetOpportunityList(DmsId);
            JObject Outcomes = new JObject();
            JArray JArrOutcomes = new JArray();
            JObject Outcome = new JObject();
            foreach(var Op in OpList)
            {
                Console.WriteLine(Op);
                

            
                    //Outcome = getOpportunitiesById(Op);
                    //JArrOutcomes.Add(Outcome);
                }
                Outcomes["outcomes"] = JArrOutcomes;
                XmlDocument outcomesdoc = JsonConvert.DeserializeXmlNode(Outcomes.ToString(), "opportunities");
                var outcomesstringWriter = new StringWriter();
                var outcomesxmlTextWriter = XmlWriter.Create(outcomesstringWriter);
                outcomesdoc.WriteTo(outcomesxmlTextWriter);
                outcomesxmlTextWriter.Flush();
                string outcomesstr = outcomesstringWriter.ToString();
                outcomesstr = outcomesdoc.OuterXml.ToString();
                try
                {
                    string filePath = "D:\\eLeads\\Outcomes\\" + DmsId + ".xml";
                    outcomesdoc.Save(filePath);
                    //  DA_Opportunities.LogInsertion(DmsId, outcomesstr, "0", "", "D", "", "N", dlrTime, Convert.ToString(dlrTime), 8, 0);
                }
                catch (Exception e)
                {
                    // Console.WriteLine(e.ToString());
                }
            

        }
        public void GetHistory(int DmsId)
        {

        }
        public  JObject GenerateHistoryXML(string OppId, int DmsId, string SubscriptionId,string APIKey,string authkey)
        {
            string resultData = "";
            WebRequest request = null;
            WebResponse response = null;
            string url = "https://api.fortellis.io/sales/v1/elead/activities/history/byOpportunityId/{opportunityId}?opportunityId=" + OppId;
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
            Byte[] myData = ReadFully(responseStream);
            resultData = System.Text.ASCIIEncoding.ASCII.GetString(myData);

            XmlDocument doc = JsonConvert.DeserializeXmlNode(resultData, "opportunityhistory");
            var stringWriter = new StringWriter();
            var xmlTextWriter = XmlWriter.Create(stringWriter);
            doc.WriteTo(xmlTextWriter);
            xmlTextWriter.Flush();
            responseStream.Close();
            string str = stringWriter.ToString();
            str = doc.OuterXml.ToString();
            try
            {
                string filePath = "D:\\eLeads\\OpportunityHistory\\" + DmsId + ".xml";
                doc.Save(filePath);
                //  DA_Opportunities.LogInsertion(DmsId, str, "0", "", "D", "", "N", dlrTime, Convert.ToString(dlrTime), 8, 0);
            }
            catch (Exception e)
            {
                // Console.WriteLine(e.ToString());
            }
            var njson = JsonConvert.SerializeObject(resultData);
            var jobj = JsonConvert.DeserializeObject(njson).ToString();
            dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(jobj);
            var arrItems = jsonData.items;

            JObject Joutcome = new JObject();
            JArray JArrOutcome = new JArray();
            foreach (var Item in arrItems)
            {
                //Console.WriteLine("Activity ID------"+Item.id);
                JObject activity = new JObject();
                activity.Add("activityid", Item.id.ToString());
                activity.Add("opportunityid", OppId);
                string outc = ""; // getOutcomesByActivityId(Item.id.ToString());
                if (outc != "")
                {
                    var jObject1 = JObject.Parse(outc);
                    activity.Merge(jObject1);
                    JArrOutcome.Add(new JObject(activity));
                }
            }
            Joutcome["outcome"] = JArrOutcome;
            return Joutcome;

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
}
