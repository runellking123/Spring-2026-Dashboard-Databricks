# GitHub Agent Instructions: Create DAX Measures for Power BI Model

## Overview
Create meaningful DAX measures for a higher education enrollment dashboard connected to the Jenzabar Data Lake (Databricks). Focus on measures that provide actionable insights for institutional research, enrollment management, and academic planning.

---

## Model Structure

### Fact Tables (Primary Data Sources)
| Table | Purpose | Key Columns |
|-------|---------|-------------|
| `stud_term_sum_div` | **Main fact table** - Student enrollment by term | ID_NUM, YR_CDE, TRM_CDE, DIV_CDE, PT_FT_STS, HRS_ENROLLED, TRM_GPA, CAREER_GPA, CLASS_CDE, NUM_OF_CRS |
| `student_crs_hist` | Course registration history | ID_NUM, YR_CDE, TRM_CDE, CRS_CDE, TRANSACTION_STS, GRADE_CDE, CREDIT_HRS, HRS_EARNED, QUAL_PTS |
| `section_master` | Course sections offered | YR_CDE, TRM_CDE, CRS_CDE, CRS_TITLE, CRS_CAPACITY, CRS_ENROLLMENT, SECTION_STS, LEAD_INSTRUCTR_ID, DIVISION_CDE |
| `section_schedules` | Section meeting times | YR_CDE, TRM_CDE, CRS_CDE, BLDG_CDE, ROOM_CDE, BEGIN_TIM, END_TIM, MONDAY_CDE-FRIDAY_CDE |
| `faculty_load_table` | Faculty teaching assignments | YR_CDE, TRM_CDE, INSTRCTR_ID_NUM, CRS_CDE |

### Dimension Tables (Lookups & Attributes)
| Table | Purpose | Key Columns |
|-------|---------|-------------|
| `biograph_master` | Demographics | ID_NUM, GENDER, BIRTH_DTE, CITIZEN_OF |
| `student_master` | Student attributes | ID_NUM, FIRST_GENERATION, ENTRY_DTE |
| `name_master` | Contact information | ID_NUM, FIRST_NAME, LAST_NAME |
| `degree_history` | Graduation records | ID_NUM, DTE_DEGR_CONFERRED, DEGR_CDE, MAJOR_CDE |
| `ethnic_race_report` | Ethnicity for IPEDS | ID_NUM, IPEDS_REPORT_VALUE |
| `catalog_master` | Course catalog | CRS_CDE, DFLT_CREDIT_HRS, CRS_TITLE |
| `major_minor_def` | Major/minor definitions | MAJOR_CDE, MAJOR_DESC |
| `degree_definition` | Degree type definitions | DEGR_CDE, DEGR_DESC |
| `term_def` | Term definitions | TRM_CDE, TRM_DESC, BEGIN_DTE, END_DTE |
| `year_term_table` | Academic calendar | YR_CDE, TRM_CDE |
| `ipeds_ethnic_race_val_def` | IPEDS ethnicity categories | IPEDS_REPORT_VALUE, IPEDS_DESC |

### Lookup Tables (DAX-created)
| Table | Purpose |
|-------|---------|
| `Division Lookup` | UG/GR division names |
| `Enrollment Status Lookup` | FT/PT status names |
| `Gender Lookup` | M/F gender names |
| `Classification Lookup` | Class standing (Freshman, Sophomore, etc.) |
| `CohortYears` | Year pairs for retention calculations |

---

## Key Codes Reference

### Term Codes (TRM_CDE)
- `10` = Fall
- `30` = Spring
- `50` = Summer I
- `60` = Summer II

### Division Codes (DIV_CDE)
- `UG` = Undergraduate
- `GR` = Graduate

### Enrollment Status (PT_FT_STS)
- `F` = Full-Time
- `P` = Part-Time

### Transaction Status (TRANSACTION_STS)
- `P` = Registered/Passed
- `D` = Dropped
- `W` = Withdrawn

### Classification Codes (CLASS_CDE)
- `00` = First-Time Freshman
- `01` = Freshman
- `02` = Sophomore
- `03` = Junior
- `04` = Senior
- `GR` = Graduate

### IPEDS Ethnicity Codes (IPEDS_REPORT_VALUE)
- `1` = Nonresident Alien
- `2` = Unknown
- `3` = Hispanic/Latino
- `4` = American Indian/Alaska Native
- `5` = Asian
- `6` = Black/African American
- `7` = Native Hawaiian/Pacific Islander
- `8` = White
- `9` = Two or More Races

---

## Existing Measures (DO NOT DUPLICATE)

The model already contains these measures:
- Total Students, FT Students, PT Students, UG Students, GR Students
- Total Credit Hours, Avg Credit Hours, FTE
- Total Courses, Avg Course Load
- Avg Term GPA, Avg Career GPA
- Female/Male Students and Percentages
- First Gen Students and Percentage
- All ethnicity-specific measures (Hispanic, Black, White, Asian, etc.)
- Persistence Rate, Fall/Spring Enrollment
- Graduated Students, Graduation Rate
- Fall-to-Fall Retention Rate/Count, Attrition Count
- IPEDS FT Retention Rate, Cohort Counts
- YoY Student Change, Prior Year Students
- New Students, Returning Students
- High GPA Students, Probation Students
- Various formatting measures (colors, indicators)

---

## NEW MEASURES TO CREATE

### Category 1: Course Schedule Analysis (section_master, section_schedules)

Create measures for Spring 2026 course scheduling dashboard:

```
1. Total Sections
   - Count of course sections offered
   - Filter by YR_CDE and TRM_CDE

2. Total Capacity
   - SUM of CRS_CAPACITY from section_master

3. Total Section Enrollment
   - SUM of CRS_ENROLLMENT from section_master

4. Overall Fill Rate
   - DIVIDE(Total Section Enrollment, Total Capacity)
   - Format as percentage

5. Open Sections
   - Count where SECTION_STS = "O"

6. Full Sections
   - Count where SECTION_STS = "F" or fill rate >= 100%

7. Cancelled Sections
   - Count where SECTION_STS = "C"

8. Seats Available
   - Total Capacity - Total Section Enrollment

9. Average Section Size
   - AVERAGE of CRS_ENROLLMENT

10. Sections Near Capacity
    - Count where fill rate >= 80% and < 100%

11. Low Enrollment Sections
    - Count where fill rate < 25%

12. Sections by Fill Category
    - Create categories: Full, Near Full (80-99%), Moderate (50-79%), Low (25-49%), Very Low (<25%)
```

### Category 2: Faculty Workload (faculty_load_table, section_master)

```
13. Total Faculty Teaching
    - DISTINCTCOUNT of instructors with sections

14. Average Sections per Faculty
    - Total Sections / Total Faculty Teaching

15. Faculty with Overload
    - Count of faculty teaching > 4 sections (adjust threshold as needed)

16. Sections Without Instructor
    - Count where LEAD_INSTRUCTR_ID is blank
```

### Category 3: Course Performance (student_crs_hist)

```
17. Average Course Grade
    - Use QUAL_PTS / HRS_GPA for GPA calculation
    - Only for completed courses (TRANSACTION_STS = "P")

18. Course Pass Rate
    - Courses with passing grade / Total completed courses

19. Course Withdrawal Rate
    - Withdrawn courses / Total registrations

20. DFW Rate (D, F, Withdraw)
    - Count of D, F, W grades / Total graded courses

21. Credit Hours Attempted
    - SUM of HRS_ATTEMPTED

22. Credit Hours Earned
    - SUM of HRS_EARNED

23. Credit Completion Rate
    - Credit Hours Earned / Credit Hours Attempted

24. Repeat Course Count
    - Count where REPEAT_FLAG is set
```

### Category 4: Academic Standing & Progress

```
25. Dean's List Eligible
    - Students with TRM_GPA >= 3.5 AND HRS_ENROLLED >= 12

26. Academic Warning
    - Students with CAREER_GPA between 1.5 and 2.0

27. Academic Suspension Risk
    - Students with CAREER_GPA < 1.5

28. Average Credits to Graduate
    - For graduated students, calculate average total credits

29. On-Track Students
    - Students with appropriate credits for their class standing
```

### Category 5: Time-Based Comparisons

```
30. Prior Term Enrollment
    - Enrollment from previous term (Fall to Spring, Spring to Fall)

31. Term-over-Term Change
    - Current term vs prior term percentage change

32. 3-Year Enrollment Trend
    - Compare current to 3 years ago

33. Same Term Last Year
    - Compare to same term in prior year
```

### Category 6: Course/Section Utilization

```
34. Room Utilization Rate
    - Sections using rooms / Available room slots

35. Prime Time Sections
    - Count of sections meeting 9am-3pm weekdays

36. Evening Sections
    - Count of sections meeting after 5pm

37. Online/Hybrid Sections
    - Count by modality (if CRS_CDE contains OL or HY)

38. MWF vs TR Distribution
    - Count of sections by meeting pattern
```

### Category 7: Major/Program Analysis

```
39. Students by Major
    - Using degree_history or student enrollment major

40. Graduates by Major
    - Count of degrees conferred by MAJOR_CDE

41. Most Popular Majors
    - Top N majors by enrollment

42. Major Completion Rate
    - Graduates in major / Students who declared major
```

---

## DAX Best Practices

1. **Use Variables (VAR/RETURN)** for complex calculations
2. **Use CALCULATE with proper filters** for context modification
3. **Use DIVIDE() instead of /** to handle division by zero
4. **Use DISTINCTCOUNT for unique counts** (students, faculty)
5. **Use COUNTROWS for row counts** (sections, courses)
6. **Format percentages** with "0.0%" or "0.00%"
7. **Format numbers** with "#,##0" for thousands separator
8. **Add descriptions** to every measure explaining what it calculates
9. **Organize into Display Folders**: Enrollment, Courses, Schedule, Faculty, GPA, Demographics, Retention, Trends

---

## Output Format

For each measure, provide:

```dax
// Measure Name
// Description: [What this measure calculates]
// Display Folder: [Folder name]

Measure Name =
[DAX Expression]
```

---

## Important Notes

1. **DO NOT create measures that duplicate existing functionality**
2. **Focus on Spring 2026 scheduling and course analysis** - this is the primary new use case
3. **All measures must use only tables listed in the Model Structure section**
4. **Test edge cases**: empty selections, blank values, division by zero
5. **Consider filter context**: measures should work with slicers for Year, Term, Division, Department
6. **Use TREATAS or INTERSECT for tables without relationships** (like ethnic_race_report)

---

## File References

Check these files in the `Model Measures/` folder for current model state:
- `Tables Model Analysis 20251228 235145.csv` - All tables and their sources
- `Columns Model Analysis 20251228 235145.csv` - All columns with data types
- `Measures Model Analysis 20251228 235145.csv` - All existing measures with DAX

---

## Deliverable

Create a Tabular Editor 2 C# script (.csx file) that adds all new measures to the model with:
- Proper measure names
- DAX expressions
- Descriptions
- Display folders
- Format strings

The script should be executable in Tabular Editor 2 without modification.
