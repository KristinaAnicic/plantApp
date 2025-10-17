using Microsoft.VisualBasic.FileIO;

namespace Plant.Domain.Utils;

public static class CsvHelper
{
    public static List<int> ReadIdsFromCsv(string filePath)
    {
        var ids = new List<int>();

        using (var parser = new TextFieldParser(filePath))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                if (fields == null || fields.Length == 0)
                    continue;

                if (int.TryParse(fields[0], out var id))
                    ids.Add(id);
            }
        }

        return ids;
    }

    public static Dictionary<int, string> LoadCsvToDictionary(string filePath)
    {
        var dict = new Dictionary<int, string>();
        if (!File.Exists(filePath))
            return dict;

        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split(',', 2);
            if (parts.Length != 2) continue;

            if (int.TryParse(parts[0], out int key))
                dict[key] = parts[1];
        }
        return dict;
    }
}
