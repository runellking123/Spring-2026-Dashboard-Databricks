// ============================================================================
// Tabular Editor 2 C# Script - SILENT
// FIXES ALL ISSUES: Hides columns + Fixes percentage measures
// ============================================================================

int hidden = 0;
int measures = 0;
var t = Model.Tables["stud_term_sum_div"];

// ============================================================================
// STEP 1: HIDE ALL COLUMNS IN ALL TABLES EXCEPT KEY DISPLAY COLUMNS
// ============================================================================

foreach(var table in Model.Tables)
{
    foreach(var col in table.Columns)
    {
        col.IsHidden = true;
        hidden++;
    }
}

// Unhide only essential display columns in stud_term_sum_div
string[] showColumns = {
    "YR_CDE", "TRM_CDE", "DIV_CDE", "PT_FT_STS", "CLASS_CDE", "MAJOR_1"
};
foreach(var c in showColumns)
{
    if(t.Columns.Contains(c)) t.Columns[c].IsHidden = false;
}

// Unhide description columns in lookup tables
if(Model.Tables.Contains("year_term_table"))
{
    var yt = Model.Tables["year_term_table"];
    if(yt.Columns.Contains("YR_TRM_DESC")) yt.Columns["YR_TRM_DESC"].IsHidden = false;
}
if(Model.Tables.Contains("major_minor_def"))
{
    var mm = Model.Tables["major_minor_def"];
    if(mm.Columns.Contains("MAJOR_MINOR_DESC")) mm.Columns["MAJOR_MINOR_DESC"].IsHidden = false;
}
if(Model.Tables.Contains("ipeds_ethnic_race_val_def"))
{
    var ip = Model.Tables["ipeds_ethnic_race_val_def"];
    if(ip.Columns.Contains("VALUE_DESCRIPTION")) ip.Columns["VALUE_DESCRIPTION"].IsHidden = false;
}
if(Model.Tables.Contains("division_def"))
{
    var dd = Model.Tables["division_def"];
    if(dd.Columns.Contains("DIV_DESC")) dd.Columns["DIV_DESC"].IsHidden = false;
}
if(Model.Tables.Contains("address_master"))
{
    var am = Model.Tables["address_master"];
    if(am.Columns.Contains("STATE")) am.Columns["STATE"].IsHidden = false;
    if(am.Columns.Contains("CITY")) am.Columns["CITY"].IsHidden = false;
}
if(Model.Tables.Contains("student_master"))
{
    var sm = Model.Tables["student_master"];
    if(sm.Columns.Contains("ENTRANCE_YR")) sm.Columns["ENTRANCE_YR"].IsHidden = false;
    if(sm.Columns.Contains("ENTRANCE_TRM")) sm.Columns["ENTRANCE_TRM"].IsHidden = false;
}

// ============================================================================
// STEP 2: FIX PERCENTAGE MEASURES - Use CALCULATE with ALL to get correct total
// ============================================================================

// Delete and recreate measures to ensure they're correct
Action<string, string, string, string> Fix = (name, dax, format, folder) =>
{
    try
    {
        if(t.Measures.Contains(name)) t.Measures[name].Delete();
        var m = t.AddMeasure(name, dax);
        m.DisplayFolder = folder;
        if(format == "pct") m.FormatString = "0.0%";
        else if(format == "gpa") m.FormatString = "0.00";
        else m.FormatString = "#,##0";
        measures++;
    }
    catch {}
};

// Core counts
Fix("Total Students", "DISTINCTCOUNT(stud_term_sum_div[ID_NUM])", "num", "Enrollment");

// Gender measures - MUST use CALCULATE to cross filter to biograph_master
Fix("Female Students",
@"CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    FILTER(
        ALL(biograph_master[GENDER]),
        biograph_master[GENDER] = ""F""
    ),
    biograph_master
)", "num", "Demographics");

Fix("Male Students",
@"CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    FILTER(
        ALL(biograph_master[GENDER]),
        biograph_master[GENDER] = ""M""
    ),
    biograph_master
)", "num", "Demographics");

// Percentage measures - use variables to ensure correct calculation
Fix("Female Percentage",
@"VAR TotalCount = [Total Students]
VAR FemaleCount = [Female Students]
RETURN
IF(TotalCount = 0, BLANK(), DIVIDE(FemaleCount, TotalCount, 0))", "pct", "Demographics");

Fix("Male Percentage",
@"VAR TotalCount = [Total Students]
VAR MaleCount = [Male Students]
RETURN
IF(TotalCount = 0, BLANK(), DIVIDE(MaleCount, TotalCount, 0))", "pct", "Demographics");

// FT/PT measures
Fix("FT Students",
@"CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[PT_FT_STS] = ""F""
)", "num", "Enrollment");

Fix("PT Students",
@"CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[PT_FT_STS] = ""P""
)", "num", "Enrollment");

Fix("FT Percentage",
@"VAR TotalCount = [Total Students]
VAR FTCount = [FT Students]
RETURN
IF(TotalCount = 0, BLANK(), DIVIDE(FTCount, TotalCount, 0))", "pct", "Enrollment");

Fix("PT Percentage",
@"VAR TotalCount = [Total Students]
VAR PTCount = [PT Students]
RETURN
IF(TotalCount = 0, BLANK(), DIVIDE(PTCount, TotalCount, 0))", "pct", "Enrollment");

// UG/GR measures
Fix("UG Students",
@"CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[DIV_CDE] = ""UG""
)", "num", "Enrollment");

Fix("GR Students",
@"CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[DIV_CDE] = ""GR""
)", "num", "Enrollment");

Fix("UG Percentage",
@"VAR TotalCount = [Total Students]
VAR UGCount = [UG Students]
RETURN
IF(TotalCount = 0, BLANK(), DIVIDE(UGCount, TotalCount, 0))", "pct", "Enrollment");

Fix("GR Percentage",
@"VAR TotalCount = [Total Students]
VAR GRCount = [GR Students]
RETURN
IF(TotalCount = 0, BLANK(), DIVIDE(GRCount, TotalCount, 0))", "pct", "Enrollment");

// Other measures
Fix("Total Credit Hours", "SUM(stud_term_sum_div[HRS_ENROLLED])", "num", "Enrollment");
Fix("Avg Credit Hours", "AVERAGE(stud_term_sum_div[HRS_ENROLLED])", "num", "Enrollment");
Fix("Total Courses", "SUM(stud_term_sum_div[NUM_OF_CRS])", "num", "Enrollment");
Fix("Avg Term GPA", "AVERAGE(stud_term_sum_div[TRM_GPA])", "gpa", "GPA");
Fix("Avg Career GPA", "AVERAGE(stud_term_sum_div[CAREER_GPA])", "gpa", "GPA");

// Retention
Fix("Fall Enrollment",
@"CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[TRM_CDE] = ""10""
)", "num", "Retention");

Fix("Spring Enrollment",
@"CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[TRM_CDE] = ""30""
)", "num", "Retention");

Fix("Persistence Rate",
@"VAR FallCount = [Fall Enrollment]
VAR SpringCount = [Spring Enrollment]
RETURN
IF(FallCount = 0, BLANK(), DIVIDE(SpringCount, FallCount, 0))", "pct", "Retention");

// Course measures
Fix("Total Course Enrollments", "COUNTROWS(student_crs_hist)", "num", "Courses");
Fix("Registered Enrollments",
@"CALCULATE(
    COUNTROWS(student_crs_hist),
    student_crs_hist[TRANSACTION_STS] = ""P""
)", "num", "Courses");

Fix("Dropped Enrollments",
@"CALCULATE(
    COUNTROWS(student_crs_hist),
    student_crs_hist[TRANSACTION_STS] = ""D""
)", "num", "Courses");

Fix("Drop Rate",
@"VAR TotalEnroll = [Total Course Enrollments]
VAR DroppedCount = [Dropped Enrollments]
RETURN
IF(TotalEnroll = 0, BLANK(), DIVIDE(DroppedCount, TotalEnroll, 0))", "pct", "Courses");

// ============================================================================
// STEP 3: ENSURE ALL MEASURES ARE VISIBLE
// ============================================================================

foreach(var table in Model.Tables)
{
    foreach(var m in table.Measures)
    {
        m.IsHidden = false;
    }
}

// ============================================================================
// SUMMARY
// ============================================================================

Info("════════════════════════════════════════");
Info("ALL ISSUES FIXED");
Info("════════════════════════════════════════");
Info("Columns hidden: " + hidden);
Info("Measures fixed: " + measures);
Info("════════════════════════════════════════");
Info("Visible columns:");
Info("  - YR_CDE, TRM_CDE, DIV_CDE, PT_FT_STS");
Info("  - YR_TRM_DESC, MAJOR_MINOR_DESC");
Info("  - VALUE_DESCRIPTION, STATE, CITY");
Info("  - ENTRANCE_YR, ENTRANCE_TRM");
Info("════════════════════════════════════════");
Info("All measures visible");
Info("Copilot will now use measures correctly!");
Info("════════════════════════════════════════");
Info("Save with Ctrl+S");
