// ============================================================================
// Tabular Editor 2 C# Script - SILENT
// Hides raw columns so Copilot uses MEASURES instead
// ============================================================================

int hidden = 0;

// ============================================================================
// HIDE RAW NUMERIC COLUMNS IN FACT TABLES
// (Keep only ID/Key columns and text columns visible)
// ============================================================================

// stud_term_sum_div - Hide numeric columns (measures will replace them)
if(Model.Tables.Contains("stud_term_sum_div"))
{
    var t = Model.Tables["stud_term_sum_div"];
    string[] hideInFact = {
        // Credit hours - use measures instead
        "HRS_ENROLLED",
        "NUM_OF_CRS",
        "PT_FT_HRS",
        "TRM_HRS_ATTEMPT",
        "TRM_HRS_EARNED",
        "TRM_HRS_GPA",
        "CAREER_HRS_ATTEMPT",
        "CAREER_HRS_EARNED",
        "CAREER_HRS_GPA",
        "LOCAL_HRS_ATTEMPT",
        "LOCAL_HRS_EARNED",
        "LOCAL_HRS_GPA",
        "XFER_HRS_ATTEMPTED",
        "XFER_HRS_EARNED",
        "XFER_HRS_GPA",
        // GPA - use measures instead
        "TRM_GPA",
        "CAREER_GPA",
        "LOCAL_GPA",
        "XFER_GPA",
        "TRM_QUAL_PTS",
        "CAREER_QUAL_PTS",
        "LOCAL_QUAL_PTS",
        "XFER_QUAL_PTS",
        // Other numeric
        "PROBATION_HR_TOTAL",
        "HONORS_HR_TOTAL",
        "ALT1_HRS_ATTEMPTED",
        "ALT1_HRS_EARNED",
        "ALT1_HRS_GPA",
        "ALT1_QUAL_PTS",
        "ALT1_GPA",
        "ALT2_HRS_ATTEMPTED",
        "ALT2_HRS_EARNED",
        "ALT2_HRS_GPA",
        "ALT2_QUAL_PTS",
        "ALT2_GPA",
        // Grade hours
        "CAREER_CRTYPE1_HRS",
        "CAREER_CRTYPE2_HRS",
        "CAREER_CRTYPE3_HRS",
        "TRM_CRTYPE1_HRS",
        "TRM_CRTYPE2_HRS",
        "TRM_CRTYPE3_HRS",
        "CAREER_GRADE_1_HRS",
        "CAREER_GRADE_2_HRS",
        "CAREER_GRADE_3_HRS",
        "CAREER_GRADE_4_HRS",
        "CAREER_GRADE_5_HRS",
        "CAREER_GRADE_6_HRS",
        "TRM_GRADE_1_HRS",
        "TRM_GRADE_2_HRS",
        "TRM_GRADE_3_HRS",
        "TRM_GRADE_4_HRS",
        "TRM_GRADE_5_HRS",
        "TRM_GRADE_6_HRS"
    };

    foreach(var c in hideInFact)
        if(t.Columns.Contains(c)) { t.Columns[c].IsHidden = true; hidden++; }
}

// student_crs_hist - Hide numeric columns
if(Model.Tables.Contains("student_crs_hist"))
{
    var t = Model.Tables["student_crs_hist"];
    string[] hideInCrs = {
        "CREDIT_HRS",
        "TUITION_HRS",
        "HRS_ATTEMPTED",
        "HRS_EARNED",
        "HRS_GPA",
        "QUAL_PTS",
        "ABSENCES",
        "REPEAT_COUNT",
        "MIDTRM_HRS_ATTEMPT",
        "MIDTRM_HRS_EARNED",
        "MIDTRM_HRS_GPA",
        "MIDTRM_QUAL_PTS",
        "STUDENT_POINTS",
        "CRS_CLOCK_HRS",
        "FLAT_FEE",
        "GRADE_NUMERIC"
    };

    foreach(var c in hideInCrs)
        if(t.Columns.Contains(c)) { t.Columns[c].IsHidden = true; hidden++; }
}

// ============================================================================
// HIDE ID_NUM IN ALL TABLES (Copilot shouldn't count IDs directly)
// ============================================================================

foreach(var table in Model.Tables)
{
    if(table.Columns.Contains("ID_NUM"))
    {
        table.Columns["ID_NUM"].IsHidden = true;
        hidden++;
    }
}

// ============================================================================
// HIDE ADVISOR ID COLUMNS
// ============================================================================

string[] advisorCols = {"ADVISOR_ID_NUM_1", "ADVISOR_ID_NUM_2", "ADVISOR_ID_NUM_3"};
foreach(var table in Model.Tables)
{
    foreach(var c in advisorCols)
        if(table.Columns.Contains(c)) { table.Columns[c].IsHidden = true; hidden++; }
}

// ============================================================================
// MAKE SURE MEASURES ARE VISIBLE
// ============================================================================

int measuresVisible = 0;
foreach(var table in Model.Tables)
{
    foreach(var m in table.Measures)
    {
        m.IsHidden = false;
        measuresVisible++;
    }
}

// ============================================================================
// SUMMARY
// ============================================================================

Info("════════════════════════════════════════");
Info("COPILOT OPTIMIZATION COMPLETE");
Info("════════════════════════════════════════");
Info("Columns hidden: " + hidden);
Info("Measures visible: " + measuresVisible);
Info("════════════════════════════════════════");
Info("Copilot will now use your MEASURES!");
Info("Save with Ctrl+S");
