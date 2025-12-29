# DAX Measures Implementation Guide

## Overview
This guide describes the 42 new DAX measures created for the Power BI Enrollment Dashboard. These measures extend the existing dashboard with comprehensive course scheduling, faculty workload, and academic performance analytics.

## Installation

### Using Tabular Editor 2
1. Open your Power BI model (.pbix file) in **Tabular Editor 2**
2. Go to **Advanced Scripting** (or press Ctrl+Shift+A)
3. Copy and paste the contents of `Add_Enrollment_Dashboard_Measures.csx`
4. Click **Run** (or press F5)
5. Review the output message showing how many measures were created/updated
6. Save the changes in Tabular Editor
7. Save and close Tabular Editor
8. Reopen your .pbix file in Power BI Desktop

## Measures Created (42 Total)

### Category 1: Course Schedule Analysis (12 measures)
**Display Folder:** Schedule

1. **Total Sections** - Total count of course sections offered
2. **Total Capacity** - Total seat capacity across all sections
3. **Total Section Enrollment** - Total students enrolled across all sections
4. **Overall Fill Rate** - Percentage of capacity filled
5. **Open Sections** - Sections with status 'O' (Open)
6. **Full Sections** - Sections that are full (status 'F' or 100%+ filled)
7. **Cancelled Sections** - Sections with status 'C' (Cancelled)
8. **Seats Available** - Available seats (Capacity - Enrollment)
9. **Average Section Size** - Average students per section
10. **Sections Near Capacity** - Sections at 80-99% capacity
11. **Low Enrollment Sections** - Sections below 25% capacity
12. **Sections by Fill Category** - Categorizes: Full/Near Full/Moderate/Low/Very Low

### Category 2: Faculty Workload (4 measures)
**Display Folder:** Faculty

13. **Total Faculty Teaching** - Count of unique instructors
14. **Average Sections per Faculty** - Average teaching load
15. **Faculty with Overload** - Faculty teaching > 4 sections
16. **Sections Without Instructor** - Unassigned sections

### Category 3: Course Performance (8 measures)
**Display Folder:** Courses

17. **Average Course Grade** - Average GPA for completed courses
18. **Course Pass Rate** - Percentage passing (excludes D/F/W/I/U)
19. **Course Withdrawal Rate** - Percentage dropped or withdrawn
20. **DFW Rate** - Percentage with D, F, or W grades
21. **Credit Hours Attempted** - Total hours attempted
22. **Credit Hours Earned** - Total hours earned
23. **Credit Completion Rate** - Earned / Attempted percentage
24. **Repeat Course Count** - Count of repeated courses

### Category 4: Academic Standing (5 measures)
**Display Folder:** GPA

25. **Dean's List Eligible** - Students with Term GPA ≥ 3.5 and ≥ 12 credits
26. **Academic Warning** - Career GPA between 1.5 and 2.0
27. **Academic Suspension Risk** - Career GPA < 1.5
28. **Average Credits to Graduate** - Average total credits for graduates
29. **On-Track Students** - Students with appropriate credits for class level

### Category 5: Time Comparisons (4 measures)
**Display Folder:** Trends

30. **Prior Term Enrollment** - Enrollment from previous term
31. **Term-over-Term Change** - Percentage change from prior term
32. **3-Year Enrollment Trend** - Change from 3 years ago (same term)
33. **Same Term Last Year** - Enrollment from same term last year

### Category 6: Section Utilization (5 measures)
**Display Folder:** Schedule

34. **Room Utilization Rate** - Percentage of sections with assigned rooms
35. **Prime Time Sections** - Sections meeting 9 AM - 3 PM
36. **Evening Sections** - Sections meeting at/after 5 PM
37. **Online/Hybrid Sections** - Sections with OL or HY in course code
38. **MWF vs TR Distribution** - Meeting pattern distribution

### Category 7: Major Analysis (4 measures)
**Display Folder:** Demographics

39. **Students by Major** - Unique students by primary major
40. **Graduates by Major** - Count of graduates by major
41. **Most Popular Majors** - Major with highest enrollment
42. **Major Completion Rate** - Percentage of students who graduated

## Format Strings Applied

- **Percentages:** `0.0%` (e.g., 85.3%)
- **GPA Values:** `0.00` (e.g., 3.45)
- **Decimals:** `#,##0.00` (e.g., 15.75)
- **Whole Numbers:** `#,##0` (e.g., 1,234)
- **Text:** Plain text (no formatting)

## Data Sources Required

The measures reference these tables in your model:

### Required Tables
- `stud_term_sum_div` - Student enrollment summary (main fact table)
- `student_crs_hist` - Course registration history
- `section_master` - Course sections offered
- `section_schedules` - Section meeting times
- `degree_history` - Graduation records

### Key Columns Used

**section_master:**
- CRS_CAPACITY, CRS_ENROLLMENT, SECTION_STS, LEAD_INSTRUCTR_ID, CRS_CDE

**student_crs_hist:**
- TRANSACTION_STS, GRADE_CDE, QUAL_PTS, HRS_GPA, HRS_ATTEMPTED, HRS_EARNED, REPEAT_FLAG

**section_schedules:**
- BEGIN_TIM, END_TIM, ROOM_CDE, MONDAY_CDE, TUESDAY_CDE, WEDNESDAY_CDE, THURSDAY_CDE, FRIDAY_CDE

**stud_term_sum_div:**
- ID_NUM, YR_CDE, TRM_CDE, TRM_GPA, CAREER_GPA, HRS_ENROLLED, HRS_EARNED, MAJOR_1, CLASS_CDE

**degree_history:**
- ID_NUM, DTE_DEGR_CONFERRED

## Key Codes Reference

### Term Codes (TRM_CDE)
- `10` = Fall
- `30` = Spring
- `50` = Summer I
- `60` = Summer II

### Section Status (SECTION_STS)
- `O` = Open
- `F` = Full
- `C` = Cancelled

### Transaction Status (TRANSACTION_STS)
- `P` = Registered/Passed
- `D` = Dropped
- `W` = Withdrawn

### Classification (CLASS_CDE)
- `01` = Freshman (0-29 credits)
- `02` = Sophomore (30-59 credits)
- `03` = Junior (60-89 credits)
- `04` = Senior (90+ credits)
- `GR` = Graduate

## DAX Best Practices Used

1. ✅ **VAR/RETURN** pattern for complex calculations
2. ✅ **DIVIDE()** function instead of `/` to handle division by zero
3. ✅ **DISTINCTCOUNT** for unique counts (students, faculty)
4. ✅ **COUNTROWS** for row counts (sections, courses)
5. ✅ **CALCULATE** with proper filter contexts
6. ✅ **ISBLANK()** checks to handle missing data
7. ✅ **Descriptive names** for all measures
8. ✅ **Comprehensive descriptions** explaining each measure
9. ✅ **Display folders** for organization

## Suggested Dashboard Usage

### Course Scheduling Dashboard
- Use measures 1-12 and 34-38 for section capacity analysis
- Filter by YR_CDE and TRM_CDE for specific terms
- Create visuals showing fill rates, available seats, meeting times

### Faculty Workload Dashboard
- Use measures 13-16 to analyze teaching loads
- Identify overloaded faculty and unassigned sections
- Track faculty assignments across terms

### Academic Performance Dashboard
- Use measures 17-24 for course success metrics
- Monitor DFW rates, pass rates, credit completion
- Identify at-risk courses needing intervention

### Student Success Dashboard
- Use measures 25-29 for academic standing
- Track Dean's List, warning, and suspension populations
- Monitor student progress toward graduation

### Enrollment Trends Dashboard
- Use measures 30-33 for time-based comparisons
- Analyze term-over-term and year-over-year changes
- Compare to 3-year historical trends

### Major/Program Dashboard
- Use measures 39-42 for program analysis
- Track popular majors and completion rates
- Analyze graduation trends by major

## Troubleshooting

### Missing Tables or Columns
If you get errors about missing tables/columns:
1. Verify all required tables are loaded in your model
2. Check column names match exactly (case-sensitive)
3. Ensure relationships are properly configured

### No Data Appearing
If measures return blank or zero:
1. Check filter context (YR_CDE, TRM_CDE slicers)
2. Verify data exists in source tables
3. Review TRANSACTION_STS codes in student_crs_hist
4. Check SECTION_STS codes in section_master

### Performance Issues
If measures are slow:
1. Ensure relationships use appropriate cardinality
2. Check that filter columns (YR_CDE, TRM_CDE) are indexed
3. Consider aggregating section_schedules if very large
4. Use "Import" mode rather than "DirectQuery" if possible

## Validation Checklist

After running the script:

- [ ] All 42 measures created successfully
- [ ] Measures appear in correct Display Folders
- [ ] Format strings applied correctly
- [ ] Sample a few measures with test data
- [ ] Verify measures respond to slicers (Year, Term)
- [ ] Check cross-filtering with dimension tables
- [ ] Test calculated measures (Fill Rate, Pass Rate, etc.)
- [ ] Confirm blank handling works correctly

## Future Enhancements

Potential additional measures to consider:

- Section wait list analysis
- Room scheduling conflicts
- Faculty qualifications vs assignments
- Course prerequisite tracking
- Cohort analysis by entry year
- Graduation rate by major and cohort
- Financial aid and enrollment correlation

## Support

For questions or issues:
1. Review the DAX expressions in `Add_Enrollment_Dashboard_Measures.csx`
2. Check the `GitHub_Agent_DAX_Instructions.md` for model structure
3. Verify table relationships in Power BI Desktop
4. Test measures individually to isolate issues

## Version History

**Version 1.0** (December 2025)
- Initial release with 42 measures
- Covers 7 major categories of analysis
- Tested with Tabular Editor 2
- Compatible with Power BI Desktop

---

**Generated for:** Spring 2026 Enrollment Dashboard  
**Data Source:** Jenzabar Data Lake (Databricks)  
**Institution:** Higher Education Institutional Research
