// ============================================================================
// Tabular Editor 2 C# Script - SILENT
// Fixes broken measures (Graduation Rate, etc.)
// ============================================================================

int updated = 0;
int created = 0;
var errors = new List<string>();

// Target table
var t = Model.Tables["stud_term_sum_div"];

// Helper to update or create measure
Action<string, string, string, string> Fix = (name, dax, format, folder) =>
{
    try
    {
        if(t.Measures.Contains(name))
        {
            t.Measures[name].Expression = dax;
            t.Measures[name].DisplayFolder = folder;
            if(format == "pct") t.Measures[name].FormatString = "0.0%";
            else if(format == "gpa") t.Measures[name].FormatString = "0.00";
            else if(format == "dec") t.Measures[name].FormatString = "#,##0.00";
            else t.Measures[name].FormatString = "#,##0";
            updated++;
        }
        else
        {
            var m = t.AddMeasure(name, dax);
            m.DisplayFolder = folder;
            if(format == "pct") m.FormatString = "0.0%";
            else if(format == "gpa") m.FormatString = "0.00";
            else if(format == "dec") m.FormatString = "#,##0.00";
            else m.FormatString = "#,##0";
            created++;
        }
    }
    catch(Exception ex) { errors.Add(name + ": " + ex.Message); }
};

// ============================================================================
// FIX GRADUATION MEASURES - Must respect filter context
// ============================================================================

// Graduated Students - Only count graduates who are in current filter context
Fix("Graduated Students",
@"VAR StudentsInContext = VALUES(stud_term_sum_div[ID_NUM])
RETURN
CALCULATE(
    DISTINCTCOUNT(degree_history[ID_NUM]),
    degree_history[ID_NUM] IN StudentsInContext,
    NOT(ISBLANK(degree_history[DTE_DEGR_CONFERRED]))
)", "num", "Graduation");

// Active Students - Only count active who are in current filter context
Fix("Active Students",
@"VAR StudentsInContext = VALUES(stud_term_sum_div[ID_NUM])
RETURN
CALCULATE(
    DISTINCTCOUNT(degree_history[ID_NUM]),
    degree_history[ID_NUM] IN StudentsInContext,
    degree_history[ACTIVE] = ""Y""
)", "num", "Graduation");

// Exited Students - Only count exited who are in current filter context
Fix("Exited Students",
@"VAR StudentsInContext = VALUES(stud_term_sum_div[ID_NUM]),
    NOT(ISBLANK(degree_history[DTE_DEGR_CONFERRED]))
RETURN
CALCULATE(
    DISTINCTCOUNT(degree_history[ID_NUM]),
    degree_history[ID_NUM] IN StudentsInContext,
    NOT(ISBLANK(degree_history[EXIT_DTE])),
    ISBLANK(degree_history[DTE_DEGR_CONFERRED])
)", "num", "Graduation");

// Graduation Rate - Now uses filtered Graduated Students
Fix("Graduation Rate",
@"VAR GradCount = [Graduated Students]
VAR TotalCount = [Total Students]
RETURN
IF(TotalCount = 0, BLANK(), DIVIDE(GradCount, TotalCount, 0))", "pct", "Graduation");

// ============================================================================
// FIX COHORT-BASED MEASURES
// ============================================================================

// Cohort Size - Count students by their entrance year/term
Fix("Cohort Size",
@"CALCULATE(
    DISTINCTCOUNT(student_master[ID_NUM]),
    ALLEXCEPT(student_master, student_master[ENTRANCE_YR], student_master[ENTRANCE_TRM])
)", "num", "Retention");

// Cohort Graduation Rate - For cohort analysis
Fix("Cohort Graduation Rate",
@"VAR CohortStudents =
    CALCULATETABLE(
        VALUES(student_master[ID_NUM]),
        ALLEXCEPT(student_master, student_master[ENTRANCE_YR], student_master[ENTRANCE_TRM])
    )
VAR GraduatedFromCohort =
    CALCULATE(
        DISTINCTCOUNT(degree_history[ID_NUM]),
        degree_history[ID_NUM] IN CohortStudents,
        NOT(ISBLANK(degree_history[DTE_DEGR_CONFERRED]))
    )
VAR CohortTotal = [Cohort Size]
RETURN
IF(CohortTotal = 0, BLANK(), DIVIDE(GraduatedFromCohort, CohortTotal, 0))", "pct", "Graduation");

// ============================================================================
// FIX PERSISTENCE RATE - Ensure same academic year
// ============================================================================

Fix("Persistence Rate",
@"VAR CurrentYr = SELECTEDVALUE(stud_term_sum_div[YR_CDE])
VAR FallCount =
    CALCULATE(
        DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
        stud_term_sum_div[TRM_CDE] = ""10"",
        stud_term_sum_div[YR_CDE] = CurrentYr
    )
VAR SpringCount =
    CALCULATE(
        DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
        stud_term_sum_div[TRM_CDE] = ""30"",
        stud_term_sum_div[YR_CDE] = CurrentYr
    )
RETURN
IF(FallCount = 0, BLANK(), DIVIDE(SpringCount, FallCount, 0))", "pct", "Retention");

// ============================================================================
// FIX FIRST GEN - Handle 'U' values
// ============================================================================

Fix("First Gen Students",
@"CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    student_master[FIRST_GENERATION] IN {""Y"", ""1"", ""Yes"", ""TRUE""}
)", "num", "Demographics");

Fix("First Gen Percentage",
@"DIVIDE([First Gen Students], [Total Students], 0)", "pct", "Demographics");

// ============================================================================
// ADD NEW USEFUL MEASURES
// ============================================================================

// Retention Rate (Year over Year) - Students returning next fall
Fix("Retention Rate YoY",
@"VAR CurrentYr = SELECTEDVALUE(student_master[ENTRANCE_YR])
VAR CohortStudents =
    CALCULATETABLE(
        VALUES(student_master[ID_NUM]),
        student_master[ENTRANCE_YR] = CurrentYr,
        student_master[ENTRANCE_TRM] = ""10""
    )
VAR NextYrFall =
    CALCULATE(
        DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
        stud_term_sum_div[ID_NUM] IN CohortStudents,
        stud_term_sum_div[YR_CDE] = FORMAT(VALUE(CurrentYr) + 1, ""0""),
        stud_term_sum_div[TRM_CDE] = ""10""
    )
VAR CohortTotal = COUNTROWS(CohortStudents)
RETURN
IF(CohortTotal = 0, BLANK(), DIVIDE(NextYrFall, CohortTotal, 0))", "pct", "Retention");

// Not Graduated Not Enrolled
Fix("Still Enrolled",
@"VAR StudentsInContext = VALUES(stud_term_sum_div[ID_NUM])
VAR GraduatedStudents =
    CALCULATETABLE(
        VALUES(degree_history[ID_NUM]),
        NOT(ISBLANK(degree_history[DTE_DEGR_CONFERRED]))
    )
RETURN
CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[ID_NUM] IN StudentsInContext,
    NOT(stud_term_sum_div[ID_NUM] IN GraduatedStudents)
)", "num", "Graduation");

// ============================================================================
// SUMMARY
// ============================================================================

Info("════════════════════════════════════════");
Info("MEASURE FIXES COMPLETE");
Info("════════════════════════════════════════");
Info("Updated: " + updated);
Info("Created: " + created);
Info("Errors: " + errors.Count);
if(errors.Count > 0) foreach(var e in errors) Info("  - " + e);
Info("════════════════════════════════════════");
Info("Fixed measures:");
Info("  - Graduated Students (now filtered)");
Info("  - Active Students (now filtered)");
Info("  - Exited Students (now filtered)");
Info("  - Graduation Rate (now accurate)");
Info("  - Cohort Graduation Rate (new)");
Info("  - Persistence Rate (same year)");
Info("  - Retention Rate YoY (new)");
Info("  - First Gen Students (handles U)");
Info("  - Still Enrolled (new)");
Info("════════════════════════════════════════");
Info("Save with Ctrl+S");
