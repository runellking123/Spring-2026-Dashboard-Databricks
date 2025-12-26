# Measure Count Analysis

## Summary

**Total Measures in Model: 57**

This count is based on the Power BI model export dated 2025-12-26.

## How to Count Measures

### Method 1: Quick Count (Shell Script)
```bash
./count_measures.sh
```

### Method 2: Detailed Analysis (Python)
```bash
python3 count_measures.py
```

### Method 3: From Tabular Editor
Open Tabular Editor 2 and run:
```csharp
// File: Count_Measures.cs
```

## Measure Breakdown by Display Folder

| Display Folder | Count |
|---------------|-------|
| Enrollment | 15 |
| Ethnicity | 11 |
| Demographics | 6 |
| Courses | 4 |
| Academic Standing | 4 |
| Retention | 3 |
| Year over Year | 3 |
| GPA | 2 |
| Graduation | 2 |
| Student Categories | 2 |
| Trends | 2 |
| Formatting | 2 |
| Page Elements | 1 |
| **Total** | **57** |

## Sample Measures

### Core Enrollment (4)
- Total Students
- Total Credit Hours
- Avg Credit Hours
- Total Courses

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

### Demographics (6)
- Female Students
- Male Students
- Female Percentage
- Male Percentage
- First Gen Students
- First Gen Percentage

### GPA (2)
- Avg Term GPA
- Avg Career GPA

### Academic Standing (4)
- High GPA Students
- High GPA Percentage
- Probation Students
- Probation Percentage

### Retention (3)
- Fall Enrollment
- Spring Enrollment
- Persistence Rate

### Graduation (2)
- Graduated Students
- Graduation Rate

### Year-over-Year (3)
- YoY Student Change
- YoY Indicator
- Prior Year Students

### Student Categories (2)
- New Students
- Returning Students

### Trends (2)
- Enrollment Trend
- Credit Hours Trend

### Courses (4)
- Total Course Enrollments
- Course Credit Hours
- Registered Enrollments
- Dropped Enrollments

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

### Formatting (2)
- GPA Color
- Enrollment Change Color

### Page Elements (1)
- Selected Term

## Notes

- The CSV export includes multiline DAX expressions which create additional rows that are not actual measures
- All counting tools filter for measures with `State = "Ready"` to get the accurate count
- Some measures may not have a table assignment in the export, showing as "Unknown"
- The measure count may differ from earlier documentation (README listed 39) as the model has evolved

## Source Data

The measure count is based on the exported CSV file:
`Model Measures/Measures Model Analysis 20251226 162605.csv`
