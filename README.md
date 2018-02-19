# Yaapii.Xml

Useful classes for working with XML.

## Usage

Apply XPath queries to an XML document:
```csharp
var xml =
    new TextOf(
        new Atoms.IO.InputOf("<root><a><x type='value'>1</x></a><a><x>2</x></a></root>")
    ).AsString(); //you can get the xml string from wherever you want - no need to use atoms, if you don't want to

//Get <a> elements 
new XMLQuery(doc).Nodes("//a"); //Will give you a list of new XMLQuery objects which contain the <a>

//Get "type" of <x> elements
new XMLQuery(doc).Values("//x/@type"); //will give you a list of values of the type attributes at <x> elements
```

Apply XSL transformations to an XML document:

```csharp
IXSL xsl = 
    new XSLDocument(
        @"<xsl:stylesheet 
            xmlns:xsl='http://www.w3.org/1999/XSL/Transform'  
            version='2.0'><xsl:output method='text'/>
        <xsl:template match='/'>hello</xsl:template></xsl:stylesheet>"
    );

Assert.Equal(
    "hello",
    xsl.TransformedToText(
        new XMLQuery("<something/>")
    )
);
```

### Code Coverage
To generate a code coverage report on your local dev machine
```ps
PS> .\build.ps1 -report=true
```
This will generate a code coverage report in `.\artifacts/coverage-report`. Open the `index.html` with your browser of choice.
