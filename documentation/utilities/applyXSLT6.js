var adTypeBinary = 1;
var adSaveCreateOverWrite = 2;
var adSaveCreateNotExist = 1;

try 
{
    var args = WScript.Arguments;

    if(args.length < 3)
    {
        WScript.Echo("Usage:\n\tapplyXSLT6.js <input.xmlFile> <output> <stylesheet.xslFile>");
        WScript.Quit(1);
    }
    else
    {
        var xmlFile = args(0);
        var outFile = args(1);
        var xslFile = args(2);

        var inputXML = new ActiveXObject("Msxml2.DOMDocument.6.0");
        var xslt = new ActiveXObject("Msxml2.DOMDocument.6.0");

        inputXML.setProperty("AllowDocumentFunction", true);
        inputXML.resolveExternals = true;
        inputXML.preserveWhiteSpace = true;

        xslt.setProperty("AllowDocumentFunction", true);
        xslt.preserveWhiteSpace = true;
        //xslt.resolveExternals = true;

        /* Create a binary IStream */
        var outDoc = new ActiveXObject("ADODB.Stream");
        outDoc.type = adTypeBinary;
        outDoc.open();

        if(inputXML.load(xmlFile) == false)
        {
            throw new Error("Could not load XML document " + inputXML.parseError.reason);
        }

        if(xslt.load(xslFile) == false)
        {
            throw new Error("Could not load XSL document " + xslt.parseError.reason);         
        }

        inputXML.transformNodeToObject(xslt, outDoc);
        outDoc.SaveToFile(outFile, adSaveCreateOverWrite);
    }
}
catch(e)
{
    WScript.Echo(e.message);
    WScript.Quit(1);
}