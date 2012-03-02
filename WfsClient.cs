using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;

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
            nsManager = new XmlNamespaceManager(capabilitiesXmlReader.NameTable);
            nsManager.AddNamespace("wfs", "http://www.opengis.net/wfs");
            nsManager.AddNamespace("ogc", "http://www.opengis.net/ogc");
            nsManager.AddNamespace("gml", "http://www.opengis.net/gml");
            nsManager.AddNamespace("ows", "http://www.opengis.net/ows");
            nsManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            // Gather ServiceIdentification Information
            XElement serviceIdentificationXml = capabilitiesXml.XPathSelectElement("/ows:ServiceIdentification", nsManager);
            serviceIdentification = new ServiceIdentificationClass(serviceIdentificationXml, nsManager);

            // TODO: Gather OperationsMetadata someday.

            // Gather FeatureTypeList information
            IEnumerable<XElement> featureTypes = capabilitiesXml.XPathSelectElements("/wfs:FeatureTypeList/wfs:FeatureType", nsManager);
            featureTypeList = new FeatureTypeListClass(featureTypes, nsManager);

            // TODO: Gather FilterCapabilities someday.
        }

        // Classes to house information from the service's GetCapabilities
        public class ServiceIdentificationClass
        {
            public ServiceIdentificationClass(XElement serviceIndentificationXml, XmlNamespaceManager nsManager)
            {
                // Grab all the various bits of info from the XML snippet
                title = serviceIndentificationXml.XPathSelectElement("ows:Title", nsManager).Value;
                description = serviceIndentificationXml.XPathSelectElement("ows:Abstract", nsManager).Value;
                IEnumerable<XElement> keywordEles = serviceIndentificationXml.XPathSelectElements("ows:Keywords/ows:Keyword", nsManager);
                foreach (XElement ele in keywordEles) { keywords.Add(ele.Value); }
                serviceType = serviceIndentificationXml.XPathSelectElement("ows:ServiceType", nsManager).Value;
                serviceTypeVersion = serviceIndentificationXml.XPathSelectElement("ows:ServiceTypeVersion", nsManager).Value;
                fees = serviceIndentificationXml.XPathSelectElement("ows:Fees", nsManager).Value;
                accessConstraints = serviceIndentificationXml.XPathSelectElement("ows:AccessConstraints", nsManager).Value;
            }

            // ServiceIdentificationClass fields
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

        public class ServiceProviderClass 
        {
            // TODO: Implement this someday!!!
        }

        public class OperationsMetadataClass 
        {
            // TODO: Implement this someday!!!
        }

        public class FeatureTypeClass 
        {
            public FeatureTypeClass(XElement featureTypeXml, XmlNamespaceManager nsManager)
            {
                // Grab all the various bits of info from the XML snippet
                name = featureTypeXml.XPathSelectElement("wfs:Name", nsManager).Value;
                title = featureTypeXml.XPathSelectElement("wfs:Title", nsManager).Value;
                description = featureTypeXml.XPathSelectElement("wfs:Abstract", nsManager).Value;
                defaultSrs = featureTypeXml.XPathSelectElement("wfs:DefaultSRS", nsManager).Value;
                foreach (XElement ele in featureTypeXml.XPathSelectElements("wfs:OutputFormats/wfs:Format", nsManager)) { outputFormats.Add(ele.Value); }
                boundingBox = new BoundingBoxClass(featureTypeXml.XPathSelectElement("ows:WGS84BoundingBox", nsManager), nsManager);

                // TODO: Read DescribeFeatureType results for this FeatureType. Top priority!
            }

            public class BoundingBoxClass
            {
                public BoundingBoxClass(XElement boundingBoxXml, XmlNamespaceManager nsManager)
                {
                    // Grab all the various bits of info from the XML snippet
                    string lowerCorner = boundingBoxXml.XPathSelectElement("ows:LowerCorner", nsManager).Value;
                    string upperCorner = boundingBoxXml.XPathSelectElement("ows:UpperCorner", nsManager).Value;
                    string[] lowerCoords = lowerCorner.Split(new Char[] { ' ' });
                    string[] upperCoords = upperCorner.Split(new Char[] { ' ' });
                    northBound = float.Parse(upperCoords[1]);
                    eastBound = float.Parse(upperCoords[0]);
                    southBound = float.Parse(lowerCoords[1]);
                    westBound = float.Parse(lowerCoords[0]);
                }

                // BoundingBoxClass fields
                private float northBound;
                private float southBound;
                private float eastBound;
                private float westBound;

                // Expose fields as read-only properties
                public float NorthBound { get { return northBound; } }                
                public float SouthBound { get { return southBound; } }                
                public float EastBound { get { return eastBound; } }                
                public float WestBound { get { return westBound; } }
            }

            // FeatureTypeClass fields
            private string name;
            private string title;
            private string description;
            private string defaultSrs;
            private List<string> outputFormats = new List<string>();            
            private BoundingBoxClass boundingBox;

            // Expose fields as read-only properties
            public string Name { get { return name; } }            
            public string Title { get { return title; } }            
            public string Description { get { return description; } }            
            public string DefaultSrs { get { return defaultSrs; } }
            public List<string> OutputFormats { get { return outputFormats; } }
            public BoundingBoxClass BoundingBox { get { return boundingBox; } }
        }

        public class FeatureTypeListClass : Dictionary<string, FeatureTypeClass>
        {
            public FeatureTypeListClass(IEnumerable<XElement> featureTypes, XmlNamespaceManager nsManager)
            {                
                // All we have to do here is loop through the elements and create new FeatureTypeClass instances
                foreach (XElement ele in featureTypes) {
                    FeatureTypeClass thisFeature = new FeatureTypeClass(ele, nsManager);
                    this[thisFeature.Name] = thisFeature;
                }
            }
        }

        public class FilterCapabilitiesClass { }

        // Fields of the WfsClient
        private XmlNamespaceManager nsManager;
        private ServiceIdentificationClass serviceIdentification;
        private OperationsMetadataClass operationsMetadata;
        private FeatureTypeListClass featureTypeList;
        private FilterCapabilitiesClass filterCapabilities;

        // Read-only properties of the WfsClient
        public ServiceIdentificationClass ServiceIdentification { get { return serviceIdentification; } }
        public OperationsMetadataClass OperationsMetadata { get { return operationsMetadata; } }
        public FeatureTypeListClass FeatureTypeList { get { return featureTypeList; } }
        public FilterCapabilitiesClass FilterCapabilities { get { return filterCapabilities; } }

        // Methods of the WfsClient
        public void GetFeature() 
        {
            // TODO: Implement this soon!
        }

    }
}
