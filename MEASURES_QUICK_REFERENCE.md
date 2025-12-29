# Quick Reference: 42 DAX Measures

## How to Use

1. Open your .pbix file in **Tabular Editor 2**
2. Go to **Advanced Scripting** (Ctrl+Shift+A)
3. Load `Add_Enrollment_Dashboard_Measures.csx`
4. Click **Run** (F5)
5. Save changes

---

## Measures Quick List

### 📊 Course Schedule Analysis (12) - Folder: "Schedule"
1. Total Sections
2. Total Capacity
3. Total Section Enrollment
4. Overall Capacity Fill Rate
5. Open Sections
6. Full Sections
7. Cancelled Sections
8. Seats Available
9. Average Section Size
10. Sections Near Capacity
11. Low Enrollment Sections
12. Sections by Fill Category

### 👨‍🏫 Faculty Workload (4) - Folder: "Faculty"
13. Total Faculty Teaching
14. Average Sections per Faculty
15. Faculty with Overload
16. Sections Without Instructor

### 📚 Course Performance (8) - Folder: "Courses"
17. Average Course Grade
18. Course Pass Rate
19. Course Withdrawal Rate
20. DFW Rate
21. Credit Hours Attempted
22. Credit Hours Earned
23. Credit Completion Rate
24. Repeat Course Count

### 🎓 Academic Standing (5) - Folder: "GPA"
25. Dean's List Eligible
26. Academic Warning
27. Academic Suspension Risk
28. Average Credits to Graduate
29. On-Track Students

### 📈 Time Comparisons (4) - Folder: "Trends"
30. Prior Term Enrollment
31. Term-over-Term Change
32. 3-Year Enrollment Trend
33. Same Term Last Year

### 🏢 Section Utilization (5) - Folder: "Schedule"
34. Room Utilization Rate
35. Prime Time Sections
36. Evening Sections
37. Online/Hybrid Sections
38. MWF vs TR Distribution

### 🎯 Major Analysis (4) - Folder: "Demographics"
39. Students by Major
40. Graduates by Major
41. Most Popular Majors
42. Major Completion Rate

---

## Files Created

- `Add_Enrollment_Dashboard_Measures.csx` - Tabular Editor 2 script (598 lines)
- `DAX_MEASURES_GUIDE.md` - Complete documentation with examples
- `MEASURES_QUICK_REFERENCE.md` - This quick reference

---

## Key Tables Used

- **section_master** - Course sections and capacity
- **student_crs_hist** - Course registration and grades
- **section_schedules** - Meeting times and rooms
- **stud_term_sum_div** - Student enrollment summary
- **degree_history** - Graduation records

---

## Sample Visualizations

### Course Scheduling Dashboard
```
KPIs: Total Sections | Total Capacity | Overall Capacity Fill Rate | Seats Available
Chart: Fill Rate by Department (Bar)
Table: Section Details with Fill Category
Slicer: YR_CDE, TRM_CDE
```

### Faculty Workload Dashboard
```
KPIs: Total Faculty | Avg Sections per Faculty | Faculty with Overload
Chart: Sections per Faculty (Histogram)
Table: Faculty with Section Counts
Filter: Department, Division
```

### Course Performance Dashboard
```
KPIs: Pass Rate | DFW Rate | Completion Rate | Avg Grade
Chart: Pass Rate by Course (Bar)
Chart: DFW Rate Trend (Line)
Slicer: Department, Course Level
```

### Academic Success Dashboard
```
KPIs: Dean's List | Academic Warning | Suspension Risk
Chart: GPA Distribution (Histogram)
Table: At-Risk Students by Classification
Slicer: Term, Division, Class
```

---

## Format Examples

- **Percentages:** 85.3%
- **GPA:** 3.45
- **Numbers:** 1,234
- **Decimals:** 15.75

---

## Support

See `DAX_MEASURES_GUIDE.md` for:
- Detailed measure descriptions
- Troubleshooting guide
- Usage examples
- Validation checklist

---

**Version:** 1.0  
**Date:** December 2025  
**Compatible with:** Tabular Editor 2, Power BI Desktop
