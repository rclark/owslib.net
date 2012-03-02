using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Net;

namespace owslib
{
    public class WfsClient
    {
        // Constructor
        public WfsClient(string baseServiceUrl)
        {
            // Load the GetCapabilities XML Document
            string capabilitiesUrl = baseServiceUrl + "?request=GetCapabilities&service=WFS&version=1.1.0";
            XmlReader capabilitiesXmlReader = XmlReader.Create(capabilitiesUrl);
            XElement capabilitiesXml = XElement.Load(capabilitiesXmlReader);

            // XmlNamespaceManager has to be populated before it is useful in resolving XPath expressions
            this.nsManager = new XmlNamespaceManager(capabilitiesXmlReader.NameTable);
            this.nsManager.AddNamespace("wfs", "http://www.opengis.net/wfs");
            this.nsManager.AddNamespace("ogc", "http://www.opengis.net/ogc");
            this.nsManager.AddNamespace("gml", "http://www.opengis.net/gml");
            this.nsManager.AddNamespace("ows", "http://www.opengis.net/ows");
            this.nsManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            // Gather ServiceIdentification Information
            XElement serviceIdentificationXml = capabilitiesXml.XPathSelectElement("/ows:ServiceIdentification", this.nsManager);
            this.serviceIdentification = new ServiceIdentification(serviceIdentificationXml, this.nsManager);

            // TODO: Gather OperationsMetadata

            // TODO: Gather FeatureTypeList information

            // TODO: Gather FilterCapabilities
        }

        // Classes to house information from the service's GetCapabilities
        class ServiceIdentification
        {
            public ServiceIdentification(XElement serviceIndentificationXml, XmlNamespaceManager nsManager)
            {
                title = serviceIndentificationXml.XPathSelectElement("ows:Title", nsManager).Value;
                description = serviceIndentificationXml.XPathSelectElement("ows:Abstract", nsManager).Value;
                IEnumerable<XElement> keywordEles = serviceIndentificationXml.XPathSelectElements("ows:Keywords/ows:Keyword", nsManager);
                foreach (XElement ele in keywordEles) { keywords.Add(ele.Value); }
                serviceType = serviceIndentificationXml.XPathSelectElement("ows:ServiceType", nsManager).Value;
                serviceTypeVersion = serviceIndentificationXml.XPathSelectElement("ows:ServiceTypeVersion", nsManager).Value;
                fees = serviceIndentificationXml.XPathSelectElement("ows:Fees", nsManager).Value;
                accessConstraints = serviceIndentificationXml.XPathSelectElement("ows:AccessConstraints", nsManager).Value;
            }

            private string title;
            private string description;
            private List<string> keywords = new List<string>();
            private string serviceType;
            private string serviceTypeVersion;
            private string fees;
            private string accessConstraints;

            // Expose fields as read-only properties
            public string Title { get { return title; } }            
            public string Description { get { return description; } }
            public List<string> Keywords { get { return keywords; } }            
            public string ServiceType { get { return serviceType; } }            
            public string ServiceTypeVersion { get { return serviceTypeVersion; } }            
            public string Fees { get { return fees; } }            
            public string AccessConstraints { get { return accessConstraints; } }
        }

        class ServiceProvider { }

        class OperationsMetadata { }

        class FeatureTypeList { }

        class FilterCapabilities { }

        // Fields of the WfsClient
        XmlNamespaceManager nsManager;
        private ServiceIdentification serviceIdentification;

    }
}
