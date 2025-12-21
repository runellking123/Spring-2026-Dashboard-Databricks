# Power BI Enrollment Dashboard - README

## Overview
This Power BI dashboard connects to the Jenzabar Data Lake (Databricks) and provides enrollment analytics for higher education institutional research.

## Connection Details
| Setting | Value |
|---------|-------|
| Host | adb-5814127397732749.9.azuredatabricks.net |
| HTTP Path | /sql/1.0/warehouses/29078c19f03c03ca |
| Driver | Simba Spark ODBC Driver |
| Authentication | Azure AD (browser-based) |
| DSN Name | JenzabarDataLake |

## Data Model

### Tables Loaded
| Table | Description | Type |
|-------|-------------|------|
| stud_term_sum_div | Student term enrollment summary | Fact |
| student_crs_hist | Course-level enrollment history | Fact |
| biograph_master | Demographics (gender, DOB, citizenship) | Dimension |
| student_master | Student attributes (cohort, first-gen) | Dimension |
| name_master | Student names | Dimension |
| address_master | Geographic data (state, city, zip) | Dimension |
| degree_history | Graduation and degree information | Dimension |
| ethnic_race_report | Student ethnicity links | Bridge |
| ipeds_ethnic_race_val_def | Ethnicity descriptions | Dimension |
| year_term_table | Term definitions and dates | Dimension |
| major_minor_def | Major/minor descriptions | Dimension |
| division_def | Division descriptions (UG/GR) | Dimension |

### Key Columns
| Column | Table | Description |
|--------|-------|-------------|
| ID_NUM | All tables | Student ID (primary key) |
| YR_CDE | stud_term_sum_div | Academic year (2025 = 2025-2026) |
| TRM_CDE | stud_term_sum_div | Term code (10=Fall, 30=Spring, 50=Summer) |
| DIV_CDE | stud_term_sum_div | Division (UG=Undergraduate, GR=Graduate) |
| PT_FT_STS | stud_term_sum_div | Status (F=Full-time, P=Part-time) |

### Term Code Reference
| Term | YR_CDE | TRM_CDE | Example |
|------|--------|---------|---------|
| Fall 2025 | 2025 | 10 | YR_CDE='2025' AND TRM_CDE='10' |
| Spring 2026 | 2025 | 30 | YR_CDE='2025' AND TRM_CDE='30' |
| Summer 2026 | 2025 | 50 | YR_CDE='2025' AND TRM_CDE='50' |

---

## Measures Reference

### Enrollment Measures
| Measure | Description | Expected (Spring 2026) |
|---------|-------------|------------------------|
| Total Students | Distinct student headcount | 969 |
| Total Credit Hours | Sum of enrolled credit hours | 13,538 |
| Avg Credit Hours | Average credit load per student | 14 |
| Total Courses | Sum of courses enrolled | 5,624 |
| FT Students | Full-time student count | 879 |
| PT Students | Part-time student count | 60 |
| FT Percentage | Full-time percentage | 90.7% |
| PT Percentage | Part-time percentage | 6.2% |
| UG Students | Undergraduate count | 840 |
| GR Students | Graduate count | 131 |
| UG Percentage | Undergraduate percentage | 86.7% |
| GR Percentage | Graduate percentage | 13.5% |

### Demographics Measures
| Measure | Description | Expected (Spring 2026) |
|---------|-------------|------------------------|
| Female Students | Female student count | 536 |
| Male Students | Male student count | 426 |
| Female Percentage | Female percentage | 55.3% |
| Male Percentage | Male percentage | 44.0% |
| First Gen Students | First-generation count | 0* |
| First Gen Percentage | First-generation percentage | 0.0%* |

*Note: All students have FIRST_GENERATION='U' (Unknown) in current data

### GPA Measures
| Measure | Description | Expected (Spring 2026) |
|---------|-------------|------------------------|
| Avg Term GPA | Average term GPA | 0.00* |
| Avg Career GPA | Average cumulative GPA | 2.86 |

*Note: Term GPA is 0 because Spring 2026 grades not yet posted

### Retention Measures
| Measure | Description | Expected (2025) |
|---------|-------------|-----------------|
| Fall Enrollment | Fall term headcount | 1,125 |
| Spring Enrollment | Spring term headcount | 969 |
| Persistence Rate | Fall-to-Spring persistence | 79.2% |

### Graduation Measures
| Measure | Description | Expected (Spring 2026) |
|---------|-------------|------------------------|
| Graduated Students | Students with degree conferred | 83 |
| Graduation Rate | Graduation percentage | 8.6% |

### Course Measures
| Measure | Description | Expected (Spring 2026) |
|---------|-------------|------------------------|
| Total Course Enrollments | Total course registrations | 7,393 |
| Registered Enrollments | Active registrations | 5,624 |
| Dropped Enrollments | Dropped courses | 1,769 |
| Drop Rate | Course drop percentage | 23.9% |

---

## Setup Instructions

### Step 1: Install ODBC Driver
1. Download Simba Spark ODBC Driver from Databricks
2. Install with default settings

### Step 2: Configure DSN
1. Run `JenzabarDataLake_DSN.reg` to add the DSN
2. Or manually create via ODBC Data Source Administrator (64-bit)

### Step 3: Connect Power BI
1. Open Power BI Desktop
2. Get Data > ODBC
3. Select "JenzabarDataLake" DSN
4. Authenticate via Azure AD browser popup
5. Select tables from j1 schema

### Step 4: Apply Measures Script
1. Download and install Tabular Editor 2
2. In Power BI: External Tools > Tabular Editor
3. Press Ctrl+Shift+C to open C# Script editor
4. Paste contents of `PowerBI_Complete_Fix.cs`
5. Press F5 to run
6. Press Ctrl+S to save back to Power BI

---

## Tabular Editor Scripts

| Script | Purpose |
|--------|---------|
| PowerBI_Complete_Fix.cs | Creates all measures, hides columns |
| PowerBI_Fix_Measures.cs | Fixes graduation/retention measures only |
| PowerBI_Hide_Columns_For_Copilot.cs | Hides raw columns for Copilot |

---

## Copilot Usage

### Prep for AI Summary
Copy the contents of `PowerBI_Prep_For_AI_Summary.txt` into the "Prep data for AI" feature in Power BI to help Copilot understand how to use your measures.

### Sample Prompts
- "What is the total enrollment for Spring 2026?"
- "Show me female vs male student percentages"
- "What is the FT/PT breakdown?"
- "Show UG vs GR enrollment by term"
- "What is the persistence rate?"
- "Show graduation rate"
- "What is the course drop rate?"

### Important Rules for Copilot
- Always use MEASURES, not raw columns
- "Headcount" = Total Students measure
- "Enrollment" = Total Students measure
- "Credit hours" = Total Credit Hours measure
- "Retention" = Persistence Rate measure
- "Graduation rate" = Graduation Rate measure

---

## Troubleshooting

### Measures showing wrong values (e.g., 100% for both genders)
- Run `PowerBI_Complete_Fix.cs` in Tabular Editor
- This script uses ID-based filtering that doesn't depend on relationships

### Copilot using columns instead of measures
- Run the script to hide all raw columns
- Only measures and essential filter columns should be visible

### Connection timeout
- Check VPN connection
- Verify Databricks warehouse is running
- Try refreshing Azure AD token

### Missing data
- Verify YR_CDE and TRM_CDE filters
- Spring 2026 = YR_CDE '2025', TRM_CDE '30'

---

## Files Reference

| File | Description |
|------|-------------|
| PowerBI_Enrollment_Dashboard_README.md | This documentation |
| PowerBI_Complete_Fix.cs | Main Tabular Editor script |
| PowerBI_Prep_For_AI_Summary.txt | Copilot instructions |
| PowerBI_Enrollment_Dashboard_Queries.sql | Source SQL queries |
| JenzabarDataLake_DSN.reg | Windows DSN registry file |

---

## Contact
For issues with the Jenzabar Data Lake connection, contact your IT department or Jenzabar support.

---

*Last Updated: December 2025*
