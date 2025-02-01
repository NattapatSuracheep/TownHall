using System.Collections.Generic;

public class GoogleSheetData
{
    public string typeName;
    public string sheetId;
    public List<string> sheetNames;
    public string writeFilePath;
}

public class RawSheetData
{
    public string range;
    public string majorDimension;
    public List<List<string>> values;
}