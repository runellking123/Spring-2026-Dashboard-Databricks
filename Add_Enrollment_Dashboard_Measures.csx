// ============================================================================
// Tabular Editor 2 C# Script
// Adds 42 DAX measures for Power BI Enrollment Dashboard
// Covers: Course Schedule, Faculty, Course Performance, Academic Standing,
//         Time Comparisons, Section Utilization, and Major Analysis
// ============================================================================

int updated = 0;
int created = 0;
var errors = new List<string>();

// Target table - use stud_term_sum_div as base table for all measures
var t = Model.Tables["stud_term_sum_div"];

// Helper function to update or create measure with description
Action<string, string, string, string, string> AddMeasure = (name, dax, format, folder, description) =>
{
    try
    {
        if(t.Measures.Contains(name))
        {
            t.Measures[name].Expression = dax;
            t.Measures[name].DisplayFolder = folder;
            t.Measures[name].Description = description;
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
            m.Description = description;
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
// CATEGORY 1: COURSE SCHEDULE ANALYSIS (12 measures)
// Display Folder: "Schedule"
// ============================================================================

AddMeasure("Total Sections",
@"COUNTROWS(section_master)",
"num", "Schedule",
"Total count of course sections offered in selected term(s)");

AddMeasure("Total Capacity",
@"SUM(section_master[CRS_CAPACITY])",
"num", "Schedule",
"Total seat capacity across all sections");

AddMeasure("Total Section Enrollment",
@"SUM(section_master[CRS_ENROLLMENT])",
"num", "Schedule",
"Total students enrolled across all sections");

AddMeasure("Overall Capacity Fill Rate",
@"VAR TotalEnrollment = [Total Section Enrollment]
VAR TotalCap = [Total Capacity]
RETURN
DIVIDE(TotalEnrollment, TotalCap, 0)",
"pct", "Schedule",
"Percentage of total capacity filled (Total Section Enrollment / Total Capacity)");

AddMeasure("Open Sections",
@"CALCULATE(
    COUNTROWS(section_master),
    section_master[SECTION_STS] = ""O""
)",
"num", "Schedule",
"Count of sections with status 'O' (Open)");

AddMeasure("Full Sections",
@"VAR FullStatus = 
    CALCULATE(
        COUNTROWS(section_master),
        section_master[SECTION_STS] = ""F""
    )
VAR FullByRate = 
    CALCULATE(
        COUNTROWS(section_master),
        DIVIDE(section_master[CRS_ENROLLMENT], section_master[CRS_CAPACITY], 0) >= 1,
        NOT(ISBLANK(section_master[CRS_CAPACITY])),
        section_master[CRS_CAPACITY] > 0
    )
RETURN
FullStatus + FullByRate",
"num", "Schedule",
"Count of sections that are full (status 'F' or fill rate >= 100%)");

AddMeasure("Cancelled Sections",
@"CALCULATE(
    COUNTROWS(section_master),
    section_master[SECTION_STS] = ""C""
)",
"num", "Schedule",
"Count of sections with status 'C' (Cancelled)");

AddMeasure("Seats Available",
@"VAR TotalCap = [Total Capacity]
VAR TotalEnroll = [Total Section Enrollment]
RETURN
TotalCap - TotalEnroll",
"num", "Schedule",
"Number of available seats (Total Capacity - Total Section Enrollment)");

AddMeasure("Average Section Size",
@"AVERAGE(section_master[CRS_ENROLLMENT])",
"dec", "Schedule",
"Average number of students enrolled per section");

AddMeasure("Sections Near Capacity",
@"CALCULATE(
    COUNTROWS(section_master),
    VAR FillRate = DIVIDE(section_master[CRS_ENROLLMENT], section_master[CRS_CAPACITY], 0)
    RETURN
    FillRate >= 0.8 && FillRate < 1.0 && NOT(ISBLANK(section_master[CRS_CAPACITY])) && section_master[CRS_CAPACITY] > 0
)",
"num", "Schedule",
"Count of sections with fill rate between 80% and 99%");

AddMeasure("Low Enrollment Sections",
@"CALCULATE(
    COUNTROWS(section_master),
    VAR FillRate = DIVIDE(section_master[CRS_ENROLLMENT], section_master[CRS_CAPACITY], 0)
    RETURN
    FillRate < 0.25 && NOT(ISBLANK(section_master[CRS_CAPACITY])) && section_master[CRS_CAPACITY] > 0
)",
"num", "Schedule",
"Count of sections with fill rate less than 25%");

AddMeasure("Sections by Fill Category",
@"VAR FillRate = DIVIDE(SUM(section_master[CRS_ENROLLMENT]), SUM(section_master[CRS_CAPACITY]), 0)
RETURN
SWITCH(
    TRUE(),
    FillRate >= 1.0, ""Full (100%+)"",
    FillRate >= 0.8, ""Near Full (80-99%)"",
    FillRate >= 0.5, ""Moderate (50-79%)"",
    FillRate >= 0.25, ""Low (25-49%)"",
    ""Very Low (<25%)""
)",
"text", "Schedule",
"Categorizes section fill rates: Full, Near Full, Moderate, Low, Very Low");

// ============================================================================
// CATEGORY 2: FACULTY WORKLOAD (4 measures)
// Display Folder: "Faculty"
// ============================================================================

AddMeasure("Total Faculty Teaching",
@"DISTINCTCOUNT(section_master[LEAD_INSTRUCTR_ID])",
"num", "Faculty",
"Count of unique faculty members teaching sections");

AddMeasure("Average Sections per Faculty",
@"VAR TotalSections = [Total Sections]
VAR TotalFaculty = [Total Faculty Teaching]
RETURN
DIVIDE(TotalSections, TotalFaculty, 0)",
"dec", "Faculty",
"Average number of sections taught per faculty member");

AddMeasure("Faculty with Overload",
@"VAR FacultyLoad = 
    ADDCOLUMNS(
        VALUES(section_master[LEAD_INSTRUCTR_ID]),
        ""SectionCount"", 
        CALCULATE(COUNTROWS(section_master))
    )
RETURN
COUNTROWS(
    FILTER(
        FacultyLoad,
        [SectionCount] > 4
    )
)",
"num", "Faculty",
"Count of faculty teaching more than 4 sections (overload threshold)");

AddMeasure("Sections Without Instructor",
@"CALCULATE(
    COUNTROWS(section_master),
    ISBLANK(section_master[LEAD_INSTRUCTR_ID])
)",
"num", "Faculty",
"Count of sections without an assigned lead instructor");

// ============================================================================
// CATEGORY 3: COURSE PERFORMANCE (8 measures)
// Display Folder: "Courses"
// ============================================================================

AddMeasure("Average Course Grade",
@"VAR TotalQualityPoints = 
    CALCULATE(
        SUM(student_crs_hist[QUAL_PTS]),
        student_crs_hist[TRANSACTION_STS] = ""P"",
        NOT(ISBLANK(student_crs_hist[QUAL_PTS]))
    )
VAR TotalGPAHours = 
    CALCULATE(
        SUM(student_crs_hist[HRS_GPA]),
        student_crs_hist[TRANSACTION_STS] = ""P"",
        NOT(ISBLANK(student_crs_hist[HRS_GPA])),
        student_crs_hist[HRS_GPA] > 0
    )
RETURN
DIVIDE(TotalQualityPoints, TotalGPAHours, BLANK())",
"gpa", "Courses",
"Average GPA for completed courses (QUAL_PTS / HRS_GPA where TRANSACTION_STS = 'P')");

AddMeasure("Course Pass Rate",
@"VAR CompletedCourses = 
    CALCULATE(
        COUNTROWS(student_crs_hist),
        student_crs_hist[TRANSACTION_STS] = ""P"",
        NOT(ISBLANK(student_crs_hist[GRADE_CDE]))
    )
VAR PassedCourses = 
    CALCULATE(
        COUNTROWS(student_crs_hist),
        student_crs_hist[TRANSACTION_STS] = ""P"",
        NOT(student_crs_hist[GRADE_CDE] IN {""F"", ""D"", ""W"", ""I"", ""U""})
    )
RETURN
DIVIDE(PassedCourses, CompletedCourses, 0)",
"pct", "Courses",
"Percentage of completed courses with passing grades (excludes D, F, W, I, U)");

AddMeasure("Course Withdrawal Rate",
@"VAR TotalRegistrations = COUNTROWS(student_crs_hist)
VAR Withdrawals = 
    CALCULATE(
        COUNTROWS(student_crs_hist),
        student_crs_hist[TRANSACTION_STS] IN {""D"", ""W""}
    )
RETURN
DIVIDE(Withdrawals, TotalRegistrations, 0)",
"pct", "Courses",
"Percentage of course registrations that were dropped or withdrawn");

AddMeasure("DFW Rate",
@"VAR GradedCourses = 
    CALCULATE(
        COUNTROWS(student_crs_hist),
        student_crs_hist[TRANSACTION_STS] = ""P"",
        NOT(ISBLANK(student_crs_hist[GRADE_CDE]))
    )
VAR DFWCourses = 
    CALCULATE(
        COUNTROWS(student_crs_hist),
        student_crs_hist[GRADE_CDE] IN {""D"", ""F"", ""W""}
    )
RETURN
DIVIDE(DFWCourses, GradedCourses, 0)",
"pct", "Courses",
"Percentage of graded courses with D, F, or W grades");

AddMeasure("Credit Hours Attempted",
@"SUM(student_crs_hist[HRS_ATTEMPTED])",
"num", "Courses",
"Total credit hours attempted across all course registrations");

AddMeasure("Credit Hours Earned",
@"SUM(student_crs_hist[HRS_EARNED])",
"num", "Courses",
"Total credit hours successfully earned");

AddMeasure("Credit Completion Rate",
@"VAR Earned = [Credit Hours Earned]
VAR Attempted = [Credit Hours Attempted]
RETURN
DIVIDE(Earned, Attempted, 0)",
"pct", "Courses",
"Percentage of attempted credit hours that were successfully earned");

AddMeasure("Repeat Course Count",
@"CALCULATE(
    COUNTROWS(student_crs_hist),
    student_crs_hist[REPEAT_FLAG] = ""Y""
)",
"num", "Courses",
"Count of course registrations where student is repeating the course");

// ============================================================================
// CATEGORY 4: ACADEMIC STANDING (5 measures)
// Display Folder: "GPA"
// ============================================================================

AddMeasure("Dean's List Eligible",
@"CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[TRM_GPA] >= 3.5,
    stud_term_sum_div[HRS_ENROLLED] >= 12,
    NOT(ISBLANK(stud_term_sum_div[TRM_GPA]))
)",
"num", "GPA",
"Students with Term GPA >= 3.5 and enrolled >= 12 credit hours");

AddMeasure("Academic Warning",
@"CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[CAREER_GPA] >= 1.5,
    stud_term_sum_div[CAREER_GPA] < 2.0,
    NOT(ISBLANK(stud_term_sum_div[CAREER_GPA]))
)",
"num", "GPA",
"Students with Career GPA between 1.5 and 2.0 (academic warning range)");

AddMeasure("Academic Suspension Risk",
@"CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[CAREER_GPA] < 1.5,
    NOT(ISBLANK(stud_term_sum_div[CAREER_GPA]))
)",
"num", "GPA",
"Students with Career GPA below 1.5 (at risk of academic suspension)");

AddMeasure("Average Credits to Graduate",
@"VAR GraduateIDs = 
    CALCULATETABLE(
        VALUES(degree_history[ID_NUM]),
        NOT(ISBLANK(degree_history[DTE_DEGR_CONFERRED]))
    )
VAR TotalCredits = 
    CALCULATE(
        SUM(stud_term_sum_div[HRS_EARNED]),
        stud_term_sum_div[ID_NUM] IN GraduateIDs
    )
VAR GraduateCount = COUNTROWS(GraduateIDs)
RETURN
DIVIDE(TotalCredits, GraduateCount, BLANK())",
"dec", "GPA",
"Average total credit hours earned by students who graduated");

AddMeasure("On-Track Students",
@"VAR OnTrack = 
    CALCULATETABLE(
        VALUES(stud_term_sum_div[ID_NUM]),
        VAR ClassCode = stud_term_sum_div[CLASS_CDE]
        VAR EarnedCredits = stud_term_sum_div[HRS_EARNED]
        RETURN
        SWITCH(
            ClassCode,
            ""01"", EarnedCredits >= 0,
            ""02"", EarnedCredits >= 30,
            ""03"", EarnedCredits >= 60,
            ""04"", EarnedCredits >= 90,
            ""GR"", TRUE(),
            FALSE()
        )
    )
RETURN
CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    stud_term_sum_div[ID_NUM] IN OnTrack
)",
"num", "GPA",
"Students with appropriate credit hours for their classification level");

// ============================================================================
// CATEGORY 5: TIME COMPARISONS (4 measures)
// Display Folder: "Trends"
// ============================================================================

AddMeasure("Prior Term Enrollment",
@"VAR CurrentYr = SELECTEDVALUE(stud_term_sum_div[YR_CDE])
VAR CurrentTrm = SELECTEDVALUE(stud_term_sum_div[TRM_CDE])
VAR PriorYr = 
    IF(
        CurrentTrm = ""30"", 
        CurrentYr,
        IF(NOT(ISERROR(VALUE(CurrentYr))), FORMAT(VALUE(CurrentYr) - 1, ""0""), BLANK())
    )
VAR PriorTrm = 
    IF(
        CurrentTrm = ""30"", 
        ""10"",
        IF(CurrentTrm = ""10"", ""30"", BLANK())
    )
RETURN
IF(
    ISBLANK(PriorYr) || ISBLANK(PriorTrm),
    BLANK(),
    CALCULATE(
        [Total Students],
        stud_term_sum_div[YR_CDE] = PriorYr,
        stud_term_sum_div[TRM_CDE] = PriorTrm,
        ALL(stud_term_sum_div[YR_CDE], stud_term_sum_div[TRM_CDE])
    )
)",
"num", "Trends",
"Student enrollment from the prior term (Fall to Spring or Spring to previous Fall)");

AddMeasure("Term-over-Term Change",
@"VAR CurrentEnrollment = [Total Students]
VAR PriorEnrollment = [Prior Term Enrollment]
RETURN
IF(
    ISBLANK(PriorEnrollment) || PriorEnrollment = 0,
    BLANK(),
    DIVIDE(CurrentEnrollment - PriorEnrollment, PriorEnrollment, 0)
)",
"pct", "Trends",
"Percentage change in enrollment from prior term");

AddMeasure("3-Year Enrollment Trend",
@"VAR CurrentYearText = SELECTEDVALUE(stud_term_sum_div[YR_CDE])
VAR CurrentTerm = SELECTEDVALUE(stud_term_sum_div[TRM_CDE])
VAR IsNumeric = NOT(ISERROR(VALUE(CurrentYearText)))
VAR ThreeYearsAgoText = IF(IsNumeric, FORMAT(VALUE(CurrentYearText) - 3, ""0""), BLANK())
VAR CurrentEnrollment = [Total Students]
VAR ThreeYearsAgo = 
    IF(
        ISBLANK(ThreeYearsAgoText),
        BLANK(),
        CALCULATE(
            [Total Students],
            stud_term_sum_div[YR_CDE] = ThreeYearsAgoText,
            stud_term_sum_div[TRM_CDE] = CurrentTerm,
            ALL(stud_term_sum_div[YR_CDE])
        )
    )
RETURN
IF(
    ISBLANK(ThreeYearsAgo) || ThreeYearsAgo = 0,
    BLANK(),
    DIVIDE(CurrentEnrollment - ThreeYearsAgo, ThreeYearsAgo, 0)
)",
"pct", "Trends",
"Percentage change in enrollment compared to 3 years ago (same term)");

AddMeasure("Same Term Last Year",
@"VAR CurrentYearText = SELECTEDVALUE(stud_term_sum_div[YR_CDE])
VAR CurrentTerm = SELECTEDVALUE(stud_term_sum_div[TRM_CDE])
VAR IsNumeric = NOT(ISERROR(VALUE(CurrentYearText)))
VAR LastYearText = IF(IsNumeric, FORMAT(VALUE(CurrentYearText) - 1, ""0""), BLANK())
RETURN
IF(
    ISBLANK(LastYearText),
    BLANK(),
    CALCULATE(
        [Total Students],
        stud_term_sum_div[YR_CDE] = LastYearText,
        stud_term_sum_div[TRM_CDE] = CurrentTerm,
        ALL(stud_term_sum_div[YR_CDE])
    )
)",
"num", "Trends",
"Student enrollment from same term in previous year");

// ============================================================================
// CATEGORY 6: SECTION UTILIZATION (5 measures)
// Display Folder: "Schedule"
// ============================================================================

AddMeasure("Room Utilization Rate",
@"VAR SectionsWithRooms = 
    CALCULATE(
        COUNTROWS(section_schedules),
        NOT(ISBLANK(section_schedules[ROOM_CDE]))
    )
VAR TotalSections = COUNTROWS(section_schedules)
RETURN
DIVIDE(SectionsWithRooms, TotalSections, 0)",
"pct", "Schedule",
"Percentage of section schedules with assigned rooms");

AddMeasure("Prime Time Sections",
@"CALCULATE(
    COUNTROWS(section_schedules),
    section_schedules[BEGIN_TIM] >= TIME(9, 0, 0),
    section_schedules[BEGIN_TIM] < TIME(15, 0, 0),
    NOT(ISBLANK(section_schedules[BEGIN_TIM]))
)",
"num", "Schedule",
"Count of sections meeting between 9:00 AM and 3:00 PM");

AddMeasure("Evening Sections",
@"CALCULATE(
    COUNTROWS(section_schedules),
    section_schedules[BEGIN_TIM] >= TIME(17, 0, 0),
    NOT(ISBLANK(section_schedules[BEGIN_TIM]))
)",
"num", "Schedule",
"Count of sections meeting at or after 5:00 PM");

AddMeasure("Online/Hybrid Sections",
@"CALCULATE(
    COUNTROWS(section_master),
    OR(
        CONTAINSSTRING(UPPER(section_master[CRS_CDE]), ""OL""),
        CONTAINSSTRING(UPPER(section_master[CRS_CDE]), ""HY"")
    )
)",
"num", "Schedule",
"Count of sections with OL (Online) or HY (Hybrid) in course code");

AddMeasure("MWF vs TR Distribution",
@"VAR MWF = 
    CALCULATE(
        COUNTROWS(section_schedules),
        OR(
            OR(
                section_schedules[MONDAY_CDE] = ""M"",
                section_schedules[WEDNESDAY_CDE] = ""W""
            ),
            section_schedules[FRIDAY_CDE] = ""F""
        )
    )
VAR TR = 
    CALCULATE(
        COUNTROWS(section_schedules),
        OR(
            section_schedules[TUESDAY_CDE] = ""T"",
            section_schedules[THURSDAY_CDE] = ""R""
        )
    )
RETURN
""MWF: "" & MWF & "" | TR: "" & TR",
"text", "Schedule",
"Distribution of sections by meeting pattern (MWF vs Tuesday/Thursday)");

// ============================================================================
// CATEGORY 7: MAJOR ANALYSIS (4 measures)
// Display Folder: "Demographics"
// ============================================================================

AddMeasure("Students by Major",
@"DISTINCTCOUNT(stud_term_sum_div[MAJOR_1])",
"num", "Demographics",
"Count of unique students by their primary major");

AddMeasure("Graduates by Major",
@"VAR GraduatesInContext = 
    CALCULATETABLE(
        VALUES(degree_history[ID_NUM]),
        NOT(ISBLANK(degree_history[DTE_DEGR_CONFERRED]))
    )
RETURN
CALCULATE(
    DISTINCTCOUNT(degree_history[ID_NUM]),
    degree_history[ID_NUM] IN GraduatesInContext
)",
"num", "Demographics",
"Count of graduates grouped by major");

AddMeasure("Most Popular Majors",
@"VAR MajorEnrollments = 
    ADDCOLUMNS(
        VALUES(stud_term_sum_div[MAJOR_1]),
        ""StudentCount"",
        CALCULATE(DISTINCTCOUNT(stud_term_sum_div[ID_NUM]))
    )
VAR TopMajor = 
    TOPN(1, MajorEnrollments, [StudentCount], DESC)
RETURN
MAXX(TopMajor, stud_term_sum_div[MAJOR_1])",
"text", "Demographics",
"Major with the highest enrollment");

AddMeasure("Major Completion Rate",
@"VAR StudentsWithMajor = 
    CALCULATE(
        DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
        NOT(ISBLANK(stud_term_sum_div[MAJOR_1]))
    )
VAR GraduatesInMajor = [Graduates by Major]
RETURN
DIVIDE(GraduatesInMajor, StudentsWithMajor, 0)",
"pct", "Demographics",
"Percentage of students in major who have graduated");

// ============================================================================
// REPORT RESULTS
// ============================================================================

var summary = string.Format(
    "Measures Added: {0} created, {1} updated",
    created, updated
);

if(errors.Count > 0)
{
    summary += "\n\nErrors:\n" + string.Join("\n", errors);
}

Info(summary);
