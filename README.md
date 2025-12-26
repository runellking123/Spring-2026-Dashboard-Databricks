# Power BI Enrollment Dashboard - Complete Setup Guide

## Overview
Spring 2026 Enrollment Dashboard connected to the Jenzabar Data Lake (Databricks) for higher education institutional research and analytics.

## Quick Stats
- **Total Measures**: 57 ([see details](HOW_MANY_MEASURES.md))
- **Dashboard Pages**: 17
- **Model Size**: 316.09 MB (65% reduction from 908.74 MB)

---

## Model Optimization

### Size Reduction
- **Before**: 908.74 MB
- **After**: 316.09 MB (65% reduction)

### Changes Made
- Disabled auto date/time (removed 24 LocalDateTable tables)
- Removed address_master table (unused, 109 MB)
- Removed NULL columns from all tables
- Removed UDEF columns from all tables

---

## Lookup Tables

| Table | Code Column | Name Column | Relationship To |
|-------|-------------|-------------|-----------------|
| Division Lookup | Division Code | Division Name | stud_term_sum_div[DIV_CDE] |
| Enrollment Status Lookup | Status Code | Status Name | stud_term_sum_div[PT_FT_STS] |
| Gender Lookup | Gender Code | Gender Name | biograph_master[GENDER] |
| Classification Lookup | Class Code | Classification | stud_term_sum_div[CLASS_CDE] |

### Classification Codes
| Code | Description |
|------|-------------|
| 00 | Freshman (First Time) |
| 01 | Freshman |
| 02 | Sophomore |
| 03 | Junior |
| 04 | Senior |
| 05 | Special - No Degree |
| 07 | Junior - WMI |
| 08 | Senior - WMI |
| GR | Graduate |

### IPEDS Ethnicity Codes
| Code | Description |
|------|-------------|
| 1 | Nonresident Alien |
| 2 | Race/Ethnicity Unknown |
| 3 | Hispanic/Latino |
| 4 | American Indian/Alaska Native |
| 5 | Asian |
| 6 | Black/African American |
| 7 | Native Hawaiian/Pacific Islander |
| 8 | White |
| 9 | Two or More Races |

---

## DAX Measures (57 Total)

### Core Enrollment (4)
- Total Students
- Total Credit Hours
- Avg Credit Hours
- Total Courses

### Year-over-Year (3)
- YoY Student Change
- YoY Indicator
- Prior Year Students

### FTE (2)
- FTE (Total Credit Hours / 12)
- FTE Display

### Student Categories (3)
- New Students
- Returning Students
- Avg Course Load

### Enrollment Status (4)
- FT Students
- PT Students
- FT Percentage
- PT Percentage

### Division (4)
- UG Students
- GR Students
- UG Percentage
- GR Percentage

### Demographics (4)
- Female Students
- Male Students
- Female Percentage
- Male Percentage

### First Generation (2)
- First Gen Students
- First Gen Percentage

### Academic Standing (4)
- High GPA Students
- High GPA Percentage
- Probation Students
- Probation Percentage

### GPA (2)
- Avg Term GPA
- Avg Career GPA

### Retention (3)
- Fall Enrollment
- Spring Enrollment
- Persistence Rate

### Graduation (2)
- Graduated Students
- Graduation Rate

### Trends (2)
- Enrollment Trend
- Credit Hours Trend

### Ethnicity (11)
- Students by Ethnicity
- Ethnicity Percentage
- Hispanic Latino Students
- Black African American Students
- White Students
- Asian Students
- Native American Students
- Pacific Islander Students
- Two or More Races Students
- Nonresident Alien Students
- Unknown Ethnicity Students

### Formatting (3)
- GPA Color
- Enrollment Change Color
- Selected Term

### Courses (4)
- Total Course Enrollments
- Course Credit Hours
- Registered Enrollments
- Dropped Enrollments

---

## Fixed Measures

### YoY Student Change (handles non-numeric years)
```dax
YoY Student Change =
VAR CurrentYearText = MAX(stud_term_sum_div[YR_CDE])
VAR IsNumeric = NOT(ISERROR(VALUE(CurrentYearText)))
VAR CurrentYear = IF(IsNumeric, VALUE(CurrentYearText), BLANK())
VAR PriorYearText = IF(IsNumeric, FORMAT(CurrentYear - 1, "0"), BLANK())
VAR CurrentStudents =
    CALCULATE(
        [Total Students],
        stud_term_sum_div[YR_CDE] = CurrentYearText
    )
VAR PriorStudents =
    CALCULATE(
        [Total Students],
        stud_term_sum_div[YR_CDE] = PriorYearText
    )
RETURN
    IF(
        ISBLANK(CurrentYear) || ISBLANK(PriorStudents) || PriorStudents = 0,
        BLANK(),
        DIVIDE(CurrentStudents - PriorStudents, PriorStudents, 0)
    )
```

### Prior Year Students (handles non-numeric years)
```dax
Prior Year Students =
VAR CurrentYearText = SELECTEDVALUE(stud_term_sum_div[YR_CDE])
VAR CurrentTerm = SELECTEDVALUE(stud_term_sum_div[TRM_CDE])
VAR IsNumeric = NOT(ISERROR(VALUE(CurrentYearText)))
VAR PriorYearText = IF(IsNumeric, FORMAT(VALUE(CurrentYearText) - 1, "0"), BLANK())
RETURN
    IF(
        ISBLANK(PriorYearText),
        BLANK(),
        CALCULATE(
            [Total Students],
            stud_term_sum_div[YR_CDE] = PriorYearText,
            stud_term_sum_div[TRM_CDE] = CurrentTerm,
            ALL(stud_term_sum_div[YR_CDE])
        )
    )
```

### FTE (12-hour full-time standard)
```dax
FTE = DIVIDE([Total Credit Hours], 12, 0)
```

### Students by Ethnicity (many-to-many handling)
```dax
Students by Ethnicity =
VAR EthnicityStudents =
    CALCULATETABLE(
        VALUES(ethnic_race_report[ID_NUM]),
        ethnic_race_report
    )
RETURN
    CALCULATE(
        DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
        TREATAS(EthnicityStudents, stud_term_sum_div[ID_NUM])
    )
```

---

## Dashboard Pages (17 Total)

### Page 1: Executive Summary
- KPI Cards: Total Students, FTE, Avg Term GPA, YoY Student Change
- Donut: Students by Classification
- Donut: Students by Enrollment Status
- Line: Enrollment by Year/Term
- Slicers: YR_CDE, TRM_CDE, Classification

### Page 2: Enrollment Trends
- Cards: Total Students, YoY Change
- Line: Enrollment by Year and Term
- Column: Current vs Prior Year
- Slicers: YR_CDE, Classification

### Page 3: Student Demographics
- Cards: Female, Male, First Gen counts
- Donut: Students by Gender
- Bar: Students by Classification
- Stacked Bar: Classification by Gender
- Slicers: YR_CDE, TRM_CDE, Classification

### Page 4: Academic Performance
- Cards: Avg Term GPA, Avg Career GPA, High GPA, Probation
- Bar: GPA by Classification
- Line: GPA Trend
- Slicers: YR_CDE, TRM_CDE, Classification

### Page 5: Ethnicity Breakdown
- Cards: Total Students, Students by Ethnicity
- Bar: Students by VALUE_DESCRIPTION
- Table: Ethnicity, Count, Percentage
- Slicers: YR_CDE, TRM_CDE, Classification

### Page 6: Graduation Analysis
- Cards: Graduated Students, Graduation Rate
- Bar: Graduates by Classification
- Bar: Graduates by Degree
- Table: Graduates by Major
- Slicers: YR_CDE, DEGR_CDE

### Page 7: Enrollment Status (FT/PT)
- Cards: FT Students, PT Students, FT%, PT%
- Donut: Students by Status
- Bar: Classification by Status
- Line: FT vs PT Trend
- Slicers: YR_CDE, TRM_CDE

### Page 8: Retention Analysis
- Cards: Fall Enrollment, Spring Enrollment, Persistence Rate
- Bar: Persistence by Classification
- Line: Retention Trend
- Table: Year, Fall, Spring, Persistence, Lost Students
- Slicers: YR_CDE, Classification

### Page 9: Credit Hour Analysis
- Cards: Total Credit Hours, Avg Credit Hours, FTE
- Bar: Credit Hours by Classification
- Bar: Avg Load by Classification
- Line: Credit Hour Trend
- Table: Classification, Hours, FTE
- Slicers: YR_CDE, TRM_CDE, Status Name

### Page 10: Major/Program Analysis
- Cards: Total Students, Unique Majors, Avg GPA
- Bar: Top 15 Majors by Students
- Stacked Bar: Major by Classification
- Table: Major, Students, GPA
- Matrix: Major x Year heatmap
- Slicers: YR_CDE, TRM_CDE, Classification

### Page 11: New vs Returning Students
- Cards: Total, New, Returning, New %
- Donut: New vs Returning
- Stacked Bar: By Classification
- Line: Trend
- Table: Year, Term, New, Returning
- Slicers: YR_CDE, TRM_CDE, Classification

### Page 12: At-Risk Students
- Cards: Total, Probation, Probation %, Avg GPA
- Bar: Probation by Classification
- Bar: Probation Rate by Classification
- Line: At-Risk Trend
- Table: Classification, Probation counts
- Slicers: YR_CDE, TRM_CDE, Classification

### Page 13: Dean's List / High Achievers
- Cards: Total, High GPA, High GPA %, Avg GPA
- Bar: High GPA by Classification
- Bar: High GPA Rate by Classification
- Line: High Achiever Trend
- Table: Classification, High GPA counts
- Slicers: YR_CDE, TRM_CDE, Classification

### Page 14: First Generation Students
- Cards: Total, First Gen, First Gen %, Avg GPA
- Bar: First Gen by Classification
- Stacked Bar: First Gen by Gender
- Line: First Gen Trend
- Table: Classification, First Gen counts
- Slicers: YR_CDE, TRM_CDE, Gender Name

### Page 15: Course Registration Analysis
- Cards: Total Enrollments, Registered, Dropped, Drop Rate
- Column: Registrations by Term
- Bar: Drop Rate by Classification
- Line: Course Trend
- Table: Year, Term, Enrollments, Drops
- Slicers: YR_CDE, TRM_CDE, Classification

### Page 16: Gender Analysis
- Cards: Total, Female, Male, Ratio
- Donut: By Gender
- 100% Stacked Bar: Gender by Classification
- Line: Gender Trend
- Table: Classification, Gender breakdown
- Slicers: YR_CDE, TRM_CDE, Classification

### Page 17: Year-over-Year Deep Dive
- Cards: Total, Prior Year, YoY Change, Difference
- Column: Current vs Prior by Term
- Area: Multi-Year Trend
- Bar: YoY Change by Classification
- Table: Detailed YoY comparison
- Slicers: YR_CDE, TRM_CDE, Classification

---

## Copilot Prompts Files

| File | Contents |
|------|----------|
| Copilot_Executive_Prompts.txt | Pages 1-7 with detailed prompts |
| Copilot_Additional_Pages_Prompts.txt | Pages 8-17 with detailed prompts |
| Copilot_Short_Prompts.txt | Abbreviated prompts |

---

## Tabular Editor Scripts

| File | Purpose |
|------|---------|
| Add_Lookup_Tables_Step1.txt | Create lookup tables |
| Add_Lookup_Tables_Step2.txt | Set properties and relationships |
| Add_Classification_Lookup.txt | Classification lookup table |
| Add_New_Measures_TabularEditor.cs | Add DAX measures |
| Add_Ethnicity_Measures.txt | Add ethnicity measures |
| Set_Sort_Columns.txt | Set sort order for lookups |

---

## Relationship Notes

### Many-to-Many: ethnic_race_report
- Students can have multiple ethnicities (multi-racial)
- Set cross-filter: Single ('ethnic_race_report' filters 'stud_term_sum_div')
- Use TREATAS in measures for correct filtering

### Removed Relationships
- year_term_table relationships removed (caused ambiguous paths)
- Use YR_CDE and TRM_CDE directly from stud_term_sum_div for slicers

---

## Databricks Connection

| Setting | Value |
|---------|-------|
| Host | adb-5814127397732749.9.azuredatabricks.net |
| HTTP Path | /sql/1.0/warehouses/29078c19f03c03ca |
| Authentication | Azure AD |
| Driver | Simba Spark ODBC Driver |
| Schemas | j1, j1_snapshot, pfaids, pfaids_snapshot, wil |

---

## Troubleshooting

### Cannot convert 'TRAN' to Number
- YR_CDE contains non-numeric values like 'TRAN'
- Use IFERROR and IsNumeric checks in measures
- Fixed measures provided above

### Same numbers for all terms
- Ensure Total Students = DISTINCTCOUNT(stud_term_sum_div[ID_NUM])
- Check for ALL() removing filters
- Use direct columns from stud_term_sum_div for slicers

### Ethnicity not filtering correctly
- Set relationship to Single direction
- Use TREATAS in measures for many-to-many

### Ambiguous paths error
- Delete duplicate relationships to year_term_table
- Use stud_term_sum_div columns directly

### FTE higher than Total Students
- Expected when average credit load > 12 hours
- FTE = Total Credit Hours / 12 (full-time threshold)

---

## Contact
Generated with Claude Code - December 2025
