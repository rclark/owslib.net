# Owslib.Net

## Motivation
- Primarily we're interested in a generic WFS client class that can work for us in a number of Windows-based applications
- Shouldn't there already be something like this? They did it for Python already: [OWSLib for Python](http://owslib.sourceforge.net/)

## Some examples
### Make a connection to a WFS
	
	owslib.WfsClient myWfsConn = new owslib.WfsClient("http://services.usgin.org/geoserver/wfs");

### Find out the WFS's Title
	
	Console.WriteLine(myWfsConn.ServiceIdentification.Title);

### List all the FeatureTypes in the WFS

	foreach (var featureType in myWfsConn.FeatureTypeList)
	{ Console.WriteLine(featureType.Key); }

### Write out all the attributes and their type for a particular featureType
	
	var mapunitpolys = myWfsConn.FeatureTypeList["ncgmp:mapunitpolys"];
	featureType.DescribeFeatureType();
	foreach (var featureDef in mapunitpolys.AttributeList)
	{ Console.WriteLine(featureDef.Key + ": " + featureDef.Value.FieldType.ToString()); }

### Find the System.Type for a particular attribute

	Type thisFieldType = mapunitpolys.AttributeList["datasourceid"].FieldType;
	Console.WriteLine("datasourceid should be of type: " + thisFieldType.ToString());

### Find something else out about an attribute's definition

	var minOccurs = mapunitpolys.AttributeList["identityconfidence"].SchemaDefinition.MinOccurs;
	Console.WriteLine("There has to be at least " + minOccurs.ToString() + " identityconfidence attributes");