// ============================================================================
// Tabular Editor 2 C# Script
// Count all measures in the Power BI model
// ============================================================================

var totalMeasures = 0;
var measuresByTable = new Dictionary<string, int>();
var measuresByFolder = new Dictionary<string, int>();

// Count all measures across all tables
foreach(var table in Model.Tables)
{
    var tableCount = table.Measures.Count;
    totalMeasures += tableCount;
    
    if(tableCount > 0)
    {
        measuresByTable[table.Name] = tableCount;
        
        // Count by display folder
        foreach(var measure in table.Measures)
        {
            var folder = string.IsNullOrEmpty(measure.DisplayFolder) ? "(No Folder)" : measure.DisplayFolder;
            if(!measuresByFolder.ContainsKey(folder))
                measuresByFolder[folder] = 0;
            measuresByFolder[folder]++;
        }
    }
}

// Output results
var output = new System.Text.StringBuilder();
output.AppendLine("=".PadRight(80, '='));
output.AppendLine("MEASURE COUNT REPORT");
output.AppendLine("=".PadRight(80, '='));
output.AppendLine();
output.AppendLine($"Total Measures in Model: {totalMeasures}");
output.AppendLine();

if(measuresByTable.Count > 0)
{
    output.AppendLine("Measures by Table:");
    output.AppendLine("-".PadRight(80, '-'));
    foreach(var kvp in measuresByTable.OrderByDescending(x => x.Value))
    {
        output.AppendLine($"  {kvp.Key}: {kvp.Value} measures");
    }
    output.AppendLine();
}

if(measuresByFolder.Count > 0)
{
    output.AppendLine("Measures by Display Folder:");
    output.AppendLine("-".PadRight(80, '-'));
    foreach(var kvp in measuresByFolder.OrderByDescending(x => x.Value))
    {
        output.AppendLine($"  {kvp.Key}: {kvp.Value} measures");
    }
    output.AppendLine();
}

output.AppendLine("=".PadRight(80, '='));
output.AppendLine($"Analysis completed at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
output.AppendLine("=".PadRight(80, '='));

// Display in message box and output window
Info(output.ToString());
output.ToString();
