namespace DocumentosElectronicos
{
    class Program
    {
        static void Main(string[] args)
        {
            string xmlSinFirmar = @"C:\firmas\XmlSinFirmar\2507201901179232604400110010020000000170000001718.xml";
            string xmlDestino = @"C:\Users\LJChuello\Documents\Sri\Firmados\2507201901179232604400110010020000000170000001718.xml";
            string firmaElectronica = @"C:\firmas\sermatick.p12";
            string contraseniaFirma = "LARA2019";

            FacturacionElectronica.FirmarXml(xmlSinFirmar, xmlDestino, firmaElectronica, contraseniaFirma);
        }
    }
}