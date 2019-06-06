# Yaapii.Xml

[![Build status](https://ci.appveyor.com/api/projects/status/3j30j800d7cympsd?svg=true)](https://ci.appveyor.com/project/icarus-consulting/yaapii-xml)

Useful classes for working with XML.

## Usage

Apply XPath queries to an XML document:

## XMLCursor
```csharp
var doc =
    new TextOf(
        new Atoms.IO.InputOf("<root><a><x type='value'>1</x></a><a><x>2</x></a></root>")
    ).AsString(); //you can get the xml string from wherever you want - no need to use atoms, if you don't want to

	//Will give you a list of new XMLCursor objects.
	//The original document is preserved!
	new XMLCursor(doc).Nodes("//a"); 

	//will give you a list of values of the type attributes at <x> elements
	new XMLCursor(doc).Values("//x/@type"); 
```

### *Important*
Point of view: A XML Cursor sits on an XML document. When you call ```xmlCursor.Nodes(xpath)```, this returns new XMLCursors. They are NOT a new document, they sit on the same document.
Therefore, the following xpath has results:
```csharp
new XMLCursor("<root><sub><inner>Hello world</inner></sub>")
    .Nodes("/root/sub")[0]
    .Values("inner/text()");
```

While this one has no results:
```csharp
new XMLCursor("<root><sub><inner>Hello world</inner></sub>")
    .Nodes("/root/sub")[0]
    .Values("/inner/text()"); 
    //a xpath starting with a slash / is pointing to the root of the document. 
    //Because the new XMLCursors of .Nodes(...) preserve the original document,
    // /inner has no results because the root node is still <root>. 
```

### Get current cursor node
```csharp
// returns XNode at the current xpath cursor position.
new XMLCursor("<root><sub>great xml here</sub></root>").AsNode();
```

## XMLSlice
```csharp
var xml =
    new TextOf(
        new Atoms.IO.InputOf("<root><a><x type='value'>1</x></a><a><x>2</x></a></root>")
    ).AsString(); //you can get the xml string from wherever you want - no need to use atoms, if you don't want to
```

### *Important*
Point of view: A XMLSlice represents a sealed XML document. When you call ```xmlSlice.Nodes(xpath)```, this returns new XMLSlices. They are always a new document.
Therefore, the following xpath has results:
```csharp
new XMLSlice("<root><sub><inner>Hello world</inner></sub>")
    .Nodes("/root/sub")[0]
    .Values("/sub/inner/text()");
```

While this one has no results:
```csharp
new XMLSlice("<root><sub><inner>Hello world</inner></sub>")
    .Nodes("/root/sub")[0]
    .Values("inner/text()"); 
    //because .Nodes(...) returns slices of the original document,
    //the xpath must start with root.

```

### Get current cursor node
```csharp
// returns the current xpath slice as XNode.
new XMLCursor("<root><sub>great xml here</sub></root>").Nodes("/root/sub")[0].AsNode(); //<sub>great xml here</sub>
```

### Helpful Wraps
There are a number of helpful wraps which return a specific type:

- XMLString
- XMLStrings
- XMLNumber
- XMLText
- XMLTexts

## Apply XSL transformations to an XML document:

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
