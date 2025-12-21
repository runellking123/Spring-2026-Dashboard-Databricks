// ============================================================================
// Tabular Editor 2 C# Script - COMPLETE FIX
// Deletes ALL existing measures and recreates them correctly
// Uses ID-based filtering that doesn't depend on relationships
// ============================================================================

var t = Model.Tables["stud_term_sum_div"];
var errors = new List<string>();
int deleted = 0;
int created = 0;

// ============================================================================
// STEP 1: DELETE ALL EXISTING MEASURES IN stud_term_sum_div
// ============================================================================

var measuresToDelete = t.Measures.ToList();
foreach(var m in measuresToDelete)
{
    try { m.Delete(); deleted++; }
    catch {}
}

// ============================================================================
// STEP 2: CREATE MEASURES WITH CORRECT DAX
// ============================================================================

Action<string, string, string, string> Create = (name, dax, format, folder) =>
{
    try
    {
        var m = t.AddMeasure(name, dax);
        m.DisplayFolder = folder;
        if(format == "pct") m.FormatString = "0.0%";
        else if(format == "gpa") m.FormatString = "0.00";
        else m.FormatString = "#,##0";
        m.IsHidden = false;
        created++;
    }
    catch(Exception ex) { errors.Add(name + ": " + ex.Message); }
};

// ============================================================================
// ENROLLMENT MEASURES - Base counts from fact table
// ============================================================================

Create("Total Students",
    "DISTINCTCOUNT(stud_term_sum_div[ID_NUM])",
    "num", "Enrollment");

Create("Total Credit Hours",
    "SUM(stud_term_sum_div[HRS_ENROLLED])",
    "num", "Enrollment");

Create("Avg Credit Hours",
    "AVERAGE(stud_term_sum_div[HRS_ENROLLED])",
    "num", "Enrollment");

Create("Total Courses",
    "SUM(stud_term_sum_div[NUM_OF_CRS])",
    "num", "Enrollment");

// ============================================================================
// FT/PT MEASURES - Filter on fact table column (works directly)
// ============================================================================

Create("FT Students",
@"CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[PT_FT_STS] = ""F""
)", "num", "Enrollment");

Create("PT Students",
@"CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[PT_FT_STS] = ""P""
)", "num", "Enrollment");

Create("FT Percentage",
@"DIVIDE([FT Students], [Total Students], 0)", "pct", "Enrollment");

Create("PT Percentage",
@"DIVIDE([PT Students], [Total Students], 0)", "pct", "Enrollment");

// ============================================================================
// UG/GR MEASURES - Filter on fact table column (works directly)
// ============================================================================

Create("UG Students",
@"CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[DIV_CDE] = ""UG""
)", "num", "Enrollment");

Create("GR Students",
@"CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[DIV_CDE] = ""GR""
)", "num", "Enrollment");

Create("UG Percentage",
@"DIVIDE([UG Students], [Total Students], 0)", "pct", "Enrollment");

Create("GR Percentage",
@"DIVIDE([GR Students], [Total Students], 0)", "pct", "Enrollment");

// ============================================================================
// GENDER MEASURES - Must use ID-based lookup (relationship independent)
// ============================================================================

Create("Female Students",
@"VAR FemaleIDs =
    CALCULATETABLE(
        VALUES(biograph_master[ID_NUM]),
        biograph_master[GENDER] = ""F"",
        ALL(biograph_master)
    )
RETURN
CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[ID_NUM] IN FemaleIDs
)", "num", "Demographics");

Create("Male Students",
@"VAR MaleIDs =
    CALCULATETABLE(
        VALUES(biograph_master[ID_NUM]),
        biograph_master[GENDER] = ""M"",
        ALL(biograph_master)
    )
RETURN
CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[ID_NUM] IN MaleIDs
)", "num", "Demographics");

Create("Female Percentage",
@"DIVIDE([Female Students], [Total Students], 0)", "pct", "Demographics");

Create("Male Percentage",
@"DIVIDE([Male Students], [Total Students], 0)", "pct", "Demographics");

// ============================================================================
// FIRST GEN MEASURES - ID-based lookup
// ============================================================================

Create("First Gen Students",
@"VAR FirstGenIDs =
    CALCULATETABLE(
        VALUES(student_master[ID_NUM]),
        student_master[FIRST_GENERATION] IN {""Y"", ""1"", ""Yes"", ""TRUE""},
        ALL(student_master)
    )
RETURN
CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[ID_NUM] IN FirstGenIDs
)", "num", "Demographics");

Create("First Gen Percentage",
@"DIVIDE([First Gen Students], [Total Students], 0)", "pct", "Demographics");

// ============================================================================
// GPA MEASURES - Direct aggregation on fact table
// ============================================================================

Create("Avg Term GPA",
    "AVERAGE(stud_term_sum_div[TRM_GPA])",
    "gpa", "GPA");

Create("Avg Career GPA",
    "AVERAGE(stud_term_sum_div[CAREER_GPA])",
    "gpa", "GPA");

// ============================================================================
// RETENTION MEASURES - Filter on fact table columns
// ============================================================================

Create("Fall Enrollment",
@"CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[TRM_CDE] = ""10""
)", "num", "Retention");

Create("Spring Enrollment",
@"CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[TRM_CDE] = ""30""
)", "num", "Retention");

Create("Persistence Rate",
@"VAR CurrentYr = SELECTEDVALUE(stud_term_sum_div[YR_CDE])
VAR FallStudents =
    CALCULATETABLE(
        VALUES(stud_term_sum_div[ID_NUM]),
        stud_term_sum_div[TRM_CDE] = ""10"",
        stud_term_sum_div[YR_CDE] = CurrentYr,
        ALL(stud_term_sum_div)
    )
VAR SpringStudents =
    CALCULATE(
        DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
        stud_term_sum_div[TRM_CDE] = ""30"",
        stud_term_sum_div[YR_CDE] = CurrentYr,
        stud_term_sum_div[ID_NUM] IN FallStudents
    )
VAR FallCount = COUNTROWS(FallStudents)
RETURN
IF(FallCount = 0, BLANK(), DIVIDE(SpringStudents, FallCount, 0))", "pct", "Retention");

// ============================================================================
// GRADUATION MEASURES - ID-based lookup
// ============================================================================

Create("Graduated Students",
@"VAR StudentsInContext = VALUES(stud_term_sum_div[ID_NUM])
VAR GraduatedIDs =
    CALCULATETABLE(
        VALUES(degree_history[ID_NUM]),
        NOT(ISBLANK(degree_history[DTE_DEGR_CONFERRED])),
        ALL(degree_history)
    )
RETURN
CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[ID_NUM] IN GraduatedIDs,
    stud_term_sum_div[ID_NUM] IN StudentsInContext
)", "num", "Graduation");

Create("Graduation Rate",
@"DIVIDE([Graduated Students], [Total Students], 0)", "pct", "Graduation");

// ============================================================================
// COURSE MEASURES - From student_crs_hist table
// ============================================================================

Create("Total Course Enrollments",
    "COUNTROWS(student_crs_hist)",
    "num", "Courses");

Create("Registered Enrollments",
@"CALCULATE(
    COUNTROWS(student_crs_hist),
    student_crs_hist[TRANSACTION_STS] = ""P""
)", "num", "Courses");

Create("Dropped Enrollments",
@"CALCULATE(
    COUNTROWS(student_crs_hist),
    student_crs_hist[TRANSACTION_STS] = ""D""
)", "num", "Courses");

Create("Drop Rate",
@"DIVIDE([Dropped Enrollments], [Total Course Enrollments], 0)", "pct", "Courses");

// ============================================================================
// STEP 3: HIDE ALL COLUMNS, SHOW ONLY ESSENTIALS
// ============================================================================

int hidden = 0;
foreach(var table in Model.Tables)
{
    foreach(var col in table.Columns)
    {
        col.IsHidden = true;
        hidden++;
    }
}

// Unhide essential columns for filtering/display
string[] showCols = {"YR_CDE", "TRM_CDE", "DIV_CDE", "PT_FT_STS", "CLASS_CDE", "MAJOR_1"};
foreach(var c in showCols)
    if(t.Columns.Contains(c)) t.Columns[c].IsHidden = false;

// Unhide lookup descriptions
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

// ============================================================================
// SUMMARY
// ============================================================================

Info("════════════════════════════════════════════════════════════════");
Info("COMPLETE FIX APPLIED");
Info("════════════════════════════════════════════════════════════════");
Info("Measures deleted: " + deleted);
Info("Measures created: " + created);
Info("Columns hidden: " + hidden);
Info("Errors: " + errors.Count);
if(errors.Count > 0) foreach(var e in errors) Info("  ERROR: " + e);
Info("════════════════════════════════════════════════════════════════");
Info("Expected values after save:");
Info("  Total Students = 969");
Info("  Female Students = 536 (55.3%)");
Info("  Male Students = 426 (44.0%)");
Info("════════════════════════════════════════════════════════════════");
Info("SAVE WITH CTRL+S");
Info("════════════════════════════════════════════════════════════════");
