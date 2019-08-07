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
    public class FacturacionElectronica
    {
        public static void FirmarXml(string origen, string destino, string rutaFirma, string contraseniaFirma)
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

                //Crear datos a firmar  
                DataToSign dataToSign = new DataToSign();
                dataToSign.setXadesFormat(EnumFormatoFirma.XAdES_BES); //XAdES-EPES  
                dataToSign.setEsquema(XAdESSchemas.XAdES_132);
                dataToSign.setPolicyKey("facturae31"); //Da igual lo que pongamos aquí, la política de firma se define arriba  
                dataToSign.setAddPolicy(true);
                dataToSign.setXMLEncoding("UTF-8");
                dataToSign.setEnveloped(true);
                dataToSign.addObject(new ObjectToSign(new AllXMLToSign(), "Descripcion del documento", null, "text/xml", null));
                dataToSign.setDocument(Erp90w(origen));
                //dataToSign.setDocument(LoadXML(origen));

                //Firmar  
                Object[] res = new FirmaXML().signFile(certificate, dataToSign, privateKey, provider);

                // Guardamos la firma a un fichero en el home del usuario  
                UtilidadTratarNodo.saveDocumentToOutputStream((Document)res[0], new FileOutputStream(destino), true);
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