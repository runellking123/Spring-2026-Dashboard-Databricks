// ============================================================================
// Tabular Editor 2 C# Script - SILENT
// Hides ALL columns in ALL tables EXCEPT stud_term_sum_div
// ============================================================================

int hidden = 0;
int kept = 0;

foreach(var table in Model.Tables)
{
    if(table.Name == "stud_term_sum_div")
    {
        // Keep stud_term_sum_div columns visible
        foreach(var col in table.Columns)
        {
            col.IsHidden = false;
            kept++;
        }
    }
    else
    {
        // Hide all columns in other tables
        foreach(var col in table.Columns)
        {
            col.IsHidden = true;
            hidden++;
        }
    }
}

// Make sure all measures stay visible
int measures = 0;
foreach(var table in Model.Tables)
{
    foreach(var m in table.Measures)
    {
        m.IsHidden = false;
        measures++;
    }
}

// ============================================================================
// SUMMARY
// ============================================================================

Info("════════════════════════════════════════");
Info("COLUMN VISIBILITY UPDATE COMPLETE");
Info("════════════════════════════════════════");
Info("Columns hidden (other tables): " + hidden);
Info("Columns visible (stud_term_sum_div): " + kept);
Info("Measures visible: " + measures);
Info("════════════════════════════════════════");
Info("Save with Ctrl+S");
