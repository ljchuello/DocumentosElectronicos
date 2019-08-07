using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace DocumentosElectronicos
{
    class Program
    {
        static void Main(string[] args)
        {
            string xmlSinFirmar = @"C:\firmas\XmlSinFirmar\0708201901179232604400110010020000000210000002110.xml";
            string xmlDestino = @"C:\Users\LJChuello\Documents\Sri\Firmados\0708201901179232604400110010020000000210000002110.xml";
            string firmaElectronica = @"C:\firmas\sermatick.p12";
            string contraseniaFirma = "LARA2019";

            // Eliminamos todos los restos de los xml
            var aux = Directory.GetFiles(@"C:\Users\LJChuello\Documents\Sri", "*.xml", SearchOption.AllDirectories).ToList();

            foreach (var row in aux)
            {
                File.Delete(row);
            }

            FirmarXml.Firmar(xmlSinFirmar, xmlDestino, firmaElectronica, contraseniaFirma);

            string asd = SendXMLFile(xmlDestino, "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline?wsdl", 600);
        }

        public static string SendXMLFile(string xmlFilepath, string uri, int timeout)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            request.KeepAlive = false;
            //request.ProtocolVersion = HttpVersion.Version10;
            request.ContentType = "application/xml";
            request.Method = "POST";

            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(xmlFilepath))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
                byte[] postBytes = Encoding.UTF8.GetBytes(sb.ToString());

                if (timeout < 0)
                {
                    request.ReadWriteTimeout = timeout;
                    request.Timeout = timeout;
                }

                request.ContentLength = postBytes.Length;

                try
                {
                    Stream requestStream = request.GetRequestStream();

                    requestStream.Write(postBytes, 0, postBytes.Length);
                    requestStream.Close();

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        return response.ToString();
                    }
                }
                catch (Exception ex)
                {
                    request.Abort();
                    return string.Empty;
                }
            }
        }
    }
}