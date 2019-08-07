using System;
using com.sun.org.apache.xerces.@internal.jaxp;
using es.mityc.firmaJava.libreria.utilidades;
using es.mityc.firmaJava.libreria.xades;
using es.mityc.javasign.pkstore;
using es.mityc.javasign.pkstore.keystore;
using es.mityc.javasign.trust;
using es.mityc.javasign.xml.refs;
using es.mityc.javasign.xml.xades.policy;
using es.mityc.javasign.xml.xades.policy.facturae;
using java.io;
using java.security;
using javax.xml.parsers;
using org.w3c.dom;
using sviudes.blogspot.com;
using X509Certificate = java.security.cert.X509Certificate;

namespace DocumentosElectronicos
{
    public class FirmarXml
    {
        public static void Firmar(string origen, string destino, string rutaFirma, string contraseniaFirma)
        {
            PrivateKey privateKey;
            Provider provider;
            X509Certificate certificate = LoadCertificate(rutaFirma, contraseniaFirma, out privateKey, out provider);

            //Si encontramos el certificado...  
            if (certificate != null)
            {
                //Política de firma (Con las librerías JAVA, esto se define en tiempo de ejecución)  
                TrustFactory.instance = TrustExtendFactory.newInstance();
                TrustFactory.truster = MyPropsTruster.getInstance();
                PoliciesManager.POLICY_SIGN = new Facturae31Manager();
                PoliciesManager.POLICY_VALIDATION = new Facturae31Manager();
                TrustFactory.instance = TrustFactory.newInstance();
                TrustFactory.truster = PropsTruster.getInstance();
                PoliciesManager.POLICY_SIGN = new Facturae31Manager();
                PoliciesManager.POLICY_VALIDATION = new Facturae31Manager();
                DataToSign dataToSign = new DataToSign();
                dataToSign.setXadesFormat(EnumFormatoFirma.XAdES_BES);
                dataToSign.setEsquema(XAdESSchemas.XAdES_132);
                dataToSign.setXMLEncoding("UTF-8");
                dataToSign.setEnveloped(true);
                dataToSign.setParentSignNode("comprobante");
                dataToSign.addObject(new ObjectToSign(new InternObjectToSign("comprobante"), "contenido comprobante", null, "text/xml", null));
                dataToSign.setDocument(Erp90w(origen));
                object[] objArray = (new FirmaXML()).signFile(certificate, dataToSign, privateKey, provider);
                FileOutputStream fileOutputStream = new FileOutputStream(destino);
                UtilidadTratarNodo.saveDocumentToOutputStream((Document)objArray[0], fileOutputStream, true);
                fileOutputStream.close();
            }
        }

        private static Document LoadXML(string path)
        {
            DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
            dbf.setNamespaceAware(true);
            return dbf.newDocumentBuilder().parse(new BufferedInputStream(new FileInputStream(path)));
        }

        private static X509Certificate LoadCertificate(string path, string password, out PrivateKey privateKey, out Provider provider)
        {
            X509Certificate certificate = null;
            provider = null;
            privateKey = null;

            //Cargar certificado de fichero PFX  
            KeyStore ks = KeyStore.getInstance("PKCS12");
            ks.load(new BufferedInputStream(new FileInputStream(path)), password.ToCharArray());
            IPKStoreManager storeManager = new KSStore(ks, new PassStoreKS(password));
            var certificates = storeManager.getSignCertificates();

            //Si encontramos el certificado...  
            if (certificates.size() == 1)
            {
                certificate = (X509Certificate)certificates.get(0);

                // Obtención de la clave privada asociada al certificado  
                privateKey = storeManager.getPrivateKey(certificate);

                // Obtención del provider encargado de las labores criptográficas  
                provider = storeManager.getProvider(certificate);
            }

            return certificate;
        }

        public static Document Erp90w(string path)
        {
            SAXParserFactoryImpl sAXParserFactoryImpl = new SAXParserFactoryImpl();
            DocumentBuilderFactory documentBuilderFactory = DocumentBuilderFactory.newInstance();
            documentBuilderFactory.setNamespaceAware(true);
            Document document = documentBuilderFactory.newDocumentBuilder().parse(new BufferedInputStream(new FileInputStream(path)));
            return document;
        }
    }
}