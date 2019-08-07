using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DocumentosElectronicos
{
    class Program
    {
        static void Main(string[] args)
        {
            string xmlSinFirmar = @"C:\firmas\XmlSinFirmar\0708201901179232604400110010020000000180000001819.xml";
            string xmlDestino = @"C:\Users\LJChuello\Documents\Sri\Firmados\0708201901179232604400110010020000000180000001819.xml";
            string firmaElectronica = @"C:\firmas\sermatick.p12";
            string contraseniaFirma = "LARA2019";

            // Eliminamos todos los restos de los xml
            var aux = Directory.GetFiles(@"C:\Users\LJChuello\Documents\Sri", "*.xml", SearchOption.AllDirectories).ToList();

            foreach (var row in aux)
            {
                File.Delete(row);
            }

            FacturacionElectronica.FirmarXml(xmlSinFirmar, xmlDestino, firmaElectronica, contraseniaFirma);
        }
    }
}