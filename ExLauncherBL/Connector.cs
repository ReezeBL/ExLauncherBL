using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExLauncherBL
{
    class Connector
    {     
        public static XDocument POSTurl(String url, String parameters = "")
        {
            WebRequest req = WebRequest.Create(url);
            req.Method = "POST";
            req.Timeout = 100000;
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] sentData = Encoding.GetEncoding(1251).GetBytes(parameters);
            req.ContentLength = sentData.Length;
            Stream sendStream = req.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);
            sendStream.Close();

            StreamReader sr = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.UTF8);
            XDocument doc = XDocument.Load(sr);
            return doc;
        }
        public static XDocument GETurl(String url, String parameters = "")
        {
            WebRequest req = WebRequest.Create(url);
            StreamReader sr = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.UTF8);
            XDocument doc = XDocument.Load(sr);
            return doc;
        }
    }
}
