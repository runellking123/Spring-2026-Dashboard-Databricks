# Power BI Enrollment Dashboard - Setup Guide

## Overview
This document covers the optimization and setup of the Spring 2026 Enrollment Dashboard connected to the Jenzabar Data Lake (Databricks).

---

## Model Optimization Completed

### 1. Size Reduction
- **Before**: 908.74 MB
- **After**: 316.09 MB (65% reduction)

### 2. Changes Made
- Disabled auto date/time (removed 24 LocalDateTable tables)
- Removed address_master table (unused, 109 MB)
- Removed NULL columns from all tables
- Removed UDEF columns from all tables

---

## Lookup Tables Added

| Table | Code Column | Name Column | Relationship To |
|-------|-------------|-------------|-----------------|
| Division Lookup | Division Code | Division Name | stud_term_sum_div[DIV_CDE] |
| Enrollment Status Lookup | Status Code | Status Name | stud_term_sum_div[PT_FT_STS] |
| Gender Lookup | Gender Code | Gender Name | biograph_master[GENDER] |
| Classification Lookup | Class Code | Classification | stud_term_sum_div[CLASS_CDE] |

### Classification Codes Reference
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

### IPEDS Ethnicity Codes Reference
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

## DAX Measures Added (28 Total)

### Year-over-Year (3)
- **YoY Student Change** - Year-over-year percentage change
- **YoY Indicator** - Arrow indicator with percentage
- **Prior Year Students** - Same term prior year count

### FTE (2)
- **FTE** - Full-Time Equivalent (credit hours / 15)
- **FTE Display** - Formatted FTE

### Student Categories (3)
- **New Students** - First term at institution
- **Returning Students** - Continuing students
- **Avg Course Load** - Average courses per student

### Academic Standing (4)
- **High GPA Students** - Students with GPA >= 3.5
- **High GPA Percentage** - Percentage with high GPA
- **Probation Students** - Students with GPA < 2.0
- **Probation Percentage** - Percentage on probation

### Trends (2)
- **Enrollment Trend** - For line charts
- **Credit Hours Trend** - For line charts

### Formatting (3)
- **GPA Color** - Conditional formatting hex codes
- **Enrollment Change Color** - Conditional formatting hex codes
- **Selected Term** - Dynamic term/year display

### Ethnicity Measures (11)
- **Students by Ethnicity** - For slicer-driven filtering
- **Ethnicity Percentage** - Percentage calculation
- **Hispanic Latino Students** - IPEDS Code 3
- **Black African American Students** - IPEDS Code 6
- **White Students** - IPEDS Code 8
- **Asian Students** - IPEDS Code 5
- **Native American Students** - IPEDS Code 4
- **Pacific Islander Students** - IPEDS Code 7
- **Two or More Races Students** - IPEDS Code 9
- **Nonresident Alien Students** - IPEDS Code 1
- **Unknown Ethnicity Students** - IPEDS Code 2

---

## Fixed Measures

### YoY Student Change (handles text values)
```dax
YoY Student Change =
VAR CurrentYear =
    IFERROR(VALUE(MAX(stud_term_sum_div[YR_CDE])), BLANK())
VAR CurrentStudents =
    CALCULATE(
        [Total Students],
        stud_term_sum_div[YR_CDE] = FORMAT(CurrentYear, "0")
    )
VAR PriorStudents =
    CALCULATE(
        [Total Students],
        stud_term_sum_div[YR_CDE] = FORMAT(CurrentYear - 1, "0")
    )
RETURN
    IF(
        ISBLANK(CurrentYear) || PriorStudents = 0,
        BLANK(),
        DIVIDE(CurrentStudents - PriorStudents, PriorStudents, 0)
    )
```

### New Students
```dax
New Students =
VAR CurrentYear = MAX(stud_term_sum_div[YR_CDE])
VAR CurrentTerm = MAX(stud_term_sum_div[TRM_CDE])
RETURN
CALCULATE(
    DISTINCTCOUNT(stud_term_sum_div[ID_NUM]),
    FILTER(
        VALUES(stud_term_sum_div[ID_NUM]),
        VAR StudentID = stud_term_sum_div[ID_NUM]
        VAR FirstYear =
            CALCULATE(
                MIN(stud_term_sum_div[YR_CDE]),
                ALL(stud_term_sum_div),
                stud_term_sum_div[ID_NUM] = StudentID
            )
        VAR FirstTerm =
            CALCULATE(
                MIN(stud_term_sum_div[TRM_CDE]),
                ALL(stud_term_sum_div),
                stud_term_sum_div[ID_NUM] = StudentID,
                stud_term_sum_div[YR_CDE] = FirstYear
            )
        RETURN FirstYear = CurrentYear && FirstTerm = CurrentTerm
    )
)
```

### Ethnicity Measure Example (handles many-to-many)
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

## Dashboard Pages

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
- Bar: High GPA by Classification
- Line: GPA Trend by Classification
- Slicers: YR_CDE, TRM_CDE, Classification

### Page 5: Ethnicity Breakdown
- Cards: Total Students, Students by Ethnicity
- Bar: Students by Ethnicity (VALUE_DESCRIPTION)
- Table: Ethnicity, Count, Percentage
- Slicers: YR_CDE, TRM_CDE, Classification

### Page 6: Graduation Analysis
- Cards: Graduated Students, Graduation Rate
- Bar: Graduates by Classification
- Bar: Graduates by Degree
- Table: Graduates by Major
- Slicers: YR_CDE, DEGR_CDE

### Page 7: Enrollment Status
- Cards: FT Students, PT Students, FT%, PT%
- Donut: Students by Status
- Bar: Classification by Status
- Line: FT vs PT Trend
- Slicers: YR_CDE, TRM_CDE

---

## Copilot Prompts

### Executive Summary Prompt
```
Create a report page titled "Executive Summary" with:

ROW 1 - Four KPI cards:
- Card 1: Total Students measure
- Card 2: FTE measure
- Card 3: Avg Term GPA measure
- Card 4: YoY Student Change measure formatted as percentage

ROW 2 - Two donut charts:
- Left: Total Students by Classification from Classification Lookup
- Right: Total Students by Status Name from Enrollment Status Lookup

ROW 3 - Line chart:
- X-axis: YR_CDE from stud_term_sum_div
- Y-axis: Total Students measure
- Legend: TRM_CDE from stud_term_sum_div

SLICERS - Three dropdown slicers:
- YR_CDE from stud_term_sum_div (dropdown)
- TRM_CDE from stud_term_sum_div (dropdown)
- Classification from Classification Lookup (dropdown)
```

### Quick Fix Prompts
```
Add a dropdown slicer for Classification from Classification Lookup

Change the bar chart to horizontal sorted by value descending

Add data labels to all charts

Format cards to show no decimal places

Make YR_CDE and TRM_CDE slicers sync across all pages
```

---

## Files Created

| File | Purpose |
|------|---------|
| Add_Lookup_Tables_Step1.txt | Create lookup tables |
| Add_Lookup_Tables_Step2.txt | Set properties & relationships |
| Add_Classification_Lookup.txt | Classification lookup table |
| Add_New_Measures_TabularEditor.cs | Add DAX measures |
| Add_Ethnicity_Measures.txt | Add ethnicity measures |
| Set_Sort_Columns.txt | Set sort order for lookups |
| Copilot_Report_Prompts.txt | Prompts for building pages |

---

## Relationship Notes

### Many-to-Many: ethnic_race_report
- Students can have multiple ethnicities (multi-racial)
- Set cross-filter: **Single** ('ethnic_race_report' filters 'stud_term_sum_div')
- Use TREATAS in measures for correct filtering

### Removed Relationships
- year_term_table relationships removed (caused ambiguous paths)
- Use YR_CDE and TRM_CDE directly from stud_term_sum_div for slicers

---

## Databricks Connection

- **Host**: adb-5814127397732749.9.azuredatabricks.net
- **HTTP Path**: /sql/1.0/warehouses/29078c19f03c03ca
- **Authentication**: Azure AD
- **Driver**: Simba Spark ODBC Driver
- **Schemas**: j1, j1_snapshot, pfaids, pfaids_snapshot, wil

---

## Troubleshooting

### Issue: Cannot convert 'TRAN' to Number
- YR_CDE/TRM_CDE contain text values like 'TRAN'
- Use IFERROR(VALUE(...), BLANK()) to handle non-numeric values

### Issue: Same numbers for all terms
- Ensure Total Students = `DISTINCTCOUNT(stud_term_sum_div[ID_NUM])`
- Check for ALL() removing filters
- Use direct columns from stud_term_sum_div for slicers

### Issue: Ethnicity not filtering correctly
- Set relationship to Single direction
- Use TREATAS in measures for many-to-many

### Issue: Ambiguous paths error
- Delete duplicate relationships to year_term_table
- Use stud_term_sum_div columns directly

---

## Contact
Generated with Claude Code - December 2025
