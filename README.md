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
| Term Lookup | Term Code | Term Name | stud_term_sum_div[TRM_CDE] |
| Division Lookup | Division Code | Division Name | stud_term_sum_div[DIV_CDE] |
| Enrollment Status Lookup | Status Code | Status Name | stud_term_sum_div[PT_FT_STS] |
| Gender Lookup | Gender Code | Gender Name | biograph_master[GENDER] |
| Classification Lookup | Class Code | Classification | stud_term_sum_div[CLASS_CDE] |

### Term Codes Reference
| Code | Description |
|------|-------------|
| 10 | Fall |
| 30 | Spring |
| 50 | Summer I |
| 56 | Summer II |
| 20 | Winter |

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

---

## DAX Measures Added (17 Total)

### Year-over-Year
- **YoY Student Change** - Year-over-year percentage change
- **YoY Indicator** - Arrow indicator with percentage
- **Prior Year Students** - Same term prior year count

### FTE
- **FTE** - Full-Time Equivalent (credit hours / 15)
- **FTE Display** - Formatted FTE

### Student Categories
- **New Students** - First term at institution
- **Returning Students** - Continuing students
- **Avg Course Load** - Average courses per student

### Academic Standing
- **High GPA Students** - Students with GPA >= 3.5
- **High GPA Percentage** - Percentage with high GPA
- **Probation Students** - Students with GPA < 2.0
- **Probation Percentage** - Percentage on probation

### Trends
- **Enrollment Trend** - For line charts
- **Credit Hours Trend** - For line charts

### Formatting
- **GPA Color** - Conditional formatting hex codes
- **Enrollment Change Color** - Conditional formatting hex codes
- **Selected Term** - Dynamic term/year display

---

## Fixed Measures (Text/Number Conversion)

### YoY Student Change
```dax
YoY Student Change =
VAR CurrentYear = VALUE(MAX(stud_term_sum_div[YR_CDE]))
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
        PriorStudents = 0,
        BLANK(),
        DIVIDE(CurrentStudents - PriorStudents, PriorStudents, 0)
    )
```

### Prior Year Students
```dax
Prior Year Students =
VAR CurrentYear = VALUE(SELECTEDVALUE(stud_term_sum_div[YR_CDE]))
VAR CurrentTerm = SELECTEDVALUE(stud_term_sum_div[TRM_CDE])
RETURN
    CALCULATE(
        [Total Students],
        stud_term_sum_div[YR_CDE] = FORMAT(CurrentYear - 1, "0"),
        stud_term_sum_div[TRM_CDE] = CurrentTerm,
        ALL(stud_term_sum_div[YR_CDE])
    )
```

---

## Dashboard Pages

### Page 1: Executive Summary
- KPI Cards: Total Students, FTE, Avg Term GPA, YoY Indicator
- Donut: Students by Division
- Donut: Students by Enrollment Status
- Bar: Students by Term
- Line: Enrollment Trend
- Slicers: Term, Division, Year

### Page 2: Enrollment Trends
- Line: Enrollment by Year/Term
- Line: Credit Hours Trend
- Column: Current vs Prior Year
- Card: YoY Change

### Page 3: Student Demographics
- Donut: Students by Gender
- Bar: Students by Classification
- Stacked Bar: Division by Gender
- Cards: Female, Male, First Gen counts

### Page 4: Academic Performance
- Cards: Avg Term GPA, Avg Career GPA
- Cards: High GPA Students, Probation Students
- Bar: GPA by Division
- Bar: GPA by Classification
- Line: GPA Trend

### Page 5: Enrollment Status
- Donut: FT vs PT Students
- Bar: Status by Division
- Cards: FT/PT counts and percentages
- Line: FT/PT Trend

### Page 6: Graduation Rates
- Cards: Graduated Students, Graduation Rate
- Bar: Graduates by Degree
- Bar: Graduates by Major
- Line: Graduation Trend
- Table: Major, Count, Rate

---

## Files Created

| File | Purpose |
|------|---------|
| Add_Lookup_Tables_Step1.txt | Create lookup tables (Tabular Editor) |
| Add_Lookup_Tables_Step2.txt | Set properties & relationships |
| Add_Classification_Lookup.txt | Classification lookup table |
| Add_Classification_Lookup_Step2.txt | Classification properties |
| Add_New_Measures_TabularEditor.cs | Add 17 DAX measures |
| Add_Synonyms_TabularEditor.cs | Add synonyms for Copilot |
| PowerBI_Add_Descriptions_FIXED.cs | Add descriptions |
| Set_Sort_Columns.txt | Set sort order for lookups |
| Fix_YoY_Measure.txt | Fix text/number comparison |
| Copilot_Dashboard_Prompts.txt | Prompts for building pages |

---

## Databricks Connection

- **Host**: adb-5814127397732749.9.azuredatabricks.net
- **HTTP Path**: /sql/1.0/warehouses/29078c19f03c03ca
- **Authentication**: Azure AD
- **Driver**: Simba Spark ODBC Driver
- **Schemas**: j1, j1_snapshot, pfaids, pfaids_snapshot, wil

### Query Script
```powershell
powershell -ExecutionPolicy Bypass -File "C:/Users/ruking/databricks_query.ps1" "SELECT * FROM j1.table_name LIMIT 10"
```

---

## Troubleshooting

### Issue: Same numbers showing for all terms
- Check that Total Students measure is: `DISTINCTCOUNT(stud_term_sum_div[ID_NUM])`
- Ensure no ALL() or ALLEXCEPT() is removing term filters
- Verify Term Lookup has all term codes in your data

### Issue: Text/Number comparison error
- YR_CDE is stored as text
- Use VALUE() to convert to number, FORMAT() to convert back

### Issue: Lookup table column not found
- Run Step 1, save model, then run Step 2
- Columns aren't available until model is saved

---

## Contact
Generated with Claude Code - December 2025
