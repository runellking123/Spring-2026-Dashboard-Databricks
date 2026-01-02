# Spring 2026 Dashboard - Databricks
## Power BI Data Model Documentation

**Model Name:** Spring 2026 Dashboard-Databricks
**Compatibility Level:** 1600
**Last Updated:** January 1, 2026
**Data Source:** Jenzabar Data Lake (Databricks)

---

## Table of Contents
1. [Overview](#overview)
2. [Data Tables](#data-tables)
3. [DAX Measures](#dax-measures)
4. [Data Model Relationships](#data-model-relationships)
5. [Measure Categories](#measure-categories)

---

## Overview

This Power BI dashboard connects to the Jenzabar Data Lake via Databricks to provide comprehensive analytics for Wiley University including:

- Student Enrollment Analysis
- Course & Section Management
- Faculty Workload Tracking
- Financial Aid Reporting
- Revenue & Accounts Receivable
- Retention & Graduation Metrics

---

## Data Tables

### Core Academic Tables (10)

| Table | Description |
|-------|-------------|
| `student_crs_hist` | Student course history with grades and credits |
| `stud_term_sum_div` | Student term summary (GPA, credit hours, enrollment status) |
| `section_master` | Course sections with capacity and enrollment |
| `section_schedules` | Section scheduling (rooms, times, days) |
| `catalog_master` | Course catalog information |
| `faculty_load_table` | Faculty teaching assignments and workload |
| `degree_history` | Student graduation and degree records |
| `term_def` | Term/semester definitions |
| `Year_Dim` | Academic year dimension table |
| `year_term_table` | Year-term combinations |

### Student Information Tables (5)

| Table | Description |
|-------|-------------|
| `name_master` | Student names and identifiers |
| `student_master` | Core student demographic data |
| `biograph_master` | Biographical information (gender, DOB) |
| `ethnic_race_report` | Ethnicity and race reporting |
| `ipeds_ethnic_race_val_def` | IPEDS ethnicity code definitions |

### Reference/Lookup Tables (9)

| Table | Description |
|-------|-------------|
| `major_minor_def` | Major and minor definitions |
| `degree_definition` | Degree type definitions |
| `Division Lookup` | Academic division codes |
| `Enrollment Status Lookup` | Full-time/Part-time status |
| `Gender Lookup` | Gender code definitions |
| `Classification Lookup` | Class standing (FR, SO, JR, SR) |
| `faculty_master` | Faculty information |
| `Faculty_Summary` | Faculty summary data |
| `Instructor_Type_Lookup` | Instructor type codes |

### Financial Tables (10)

| Table | Description |
|-------|-------------|
| `fees` | Student charges and fees |
| `trans_hist` | Transaction history (payments, credits, debits) |
| `chg_fee_def` | Charge/fee code definitions |
| `subsid_def` | Subsidy definitions |
| `stu_award` | Student financial aid awards |
| `stu_award_year` | Award year assignments |
| `funds` | Financial aid fund definitions |
| `award_year_defn` | Award year definitions |
| `Fee_Type_Lookup` | Fee type classifications |
| `Source_Code_Lookup` | Transaction source codes |
| `Fund_Type_Lookup` | Fund type classifications |
| `Fund_Source_Lookup` | Fund source classifications |

### Special Tables (1)

| Table | Description |
|-------|-------------|
| `CohortYears` | Retention cohort tracking for IPEDS |

---

## DAX Measures

### Total Measures: 175+

### By Display Folder:

#### Courses (13 measures)
| Measure | Description |
|---------|-------------|
| Total Course Registrations | Count of all course registrations |
| Total Course Enrollments | Count of all course enrollments |
| Average Course Grade | Average GPA across courses |
| Course Pass Rate | Percentage of passing grades |
| Course Withdrawal Rate | Percentage of withdrawals |
| DFW Rate | D, F, and W grade rate |
| Credit Hours Attempted | Total credit hours attempted |
| Credit Hours Earned | Total credit hours earned |
| Credit Completion Rate | Earned / Attempted ratio |
| Repeat Course Count | Count of repeated courses |
| Registered Enrollments | Active registrations |
| Dropped Enrollments | Dropped registrations |
| Drop Rate | Percentage of drops |

#### Enrollment (14 measures)
| Measure | Description |
|---------|-------------|
| Total Students | Distinct student count |
| Total Credit Hours | Sum of credit hours |
| Avg Credit Hours | Average credits per student |
| Total Courses | Count of course enrollments |
| FT Students | Full-time student count |
| PT Students | Part-time student count |
| FT Percentage | Full-time percentage |
| PT Percentage | Part-time percentage |
| UG Students | Undergraduate count |
| GR Students | Graduate count |
| FTE | Full-time equivalent |
| FTE Display | Formatted FTE display |
| Avg Course Load | Average courses per student |
| New Students / Returning Students | Student category counts |

#### Demographics (8 measures)
| Measure | Description |
|---------|-------------|
| First Gen Students | First generation count |
| First Gen Percentage | First generation percentage |
| Female Students | Female student count |
| Female Percentage | Female percentage |
| Male Students | Male student count |
| Male Percentage | Male percentage |
| Most Popular Majors | Top majors analysis |
| Major Completion Rate | Degree completion by major |

#### GPA (5 measures)
| Measure | Description |
|---------|-------------|
| Avg Term GPA | Average term GPA |
| Avg Career GPA | Average cumulative GPA |
| Dean's List Eligible | Students with 3.5+ GPA |
| On-Track Students | Students in good standing |
| Average Credits to Graduate | Credits needed to graduate |

#### Academic Standing (8 measures)
| Measure | Description |
|---------|-------------|
| High GPA Students | Count with GPA >= 3.5 |
| High GPA Percentage | High GPA percentage |
| Probation Students | Academic probation count |
| Probation Percentage | Probation percentage |
| Academic Warning | Warning status count |
| Academic Suspension Risk | At-risk students |
| Deans List Eligible | Dean's list qualifiers |
| High Achievers | Top performing students |

#### Ethnicity (10 measures)
| Measure | Description |
|---------|-------------|
| Students by Ethnicity | Ethnicity breakdown |
| Ethnicity Percentage | Percentage by ethnicity |
| Hispanic Latino Students | Hispanic/Latino count |
| Black African American Students | Black/African American count |
| White Students | White student count |
| Asian Students | Asian student count |
| Native American Students | Native American count |
| Pacific Islander Students | Pacific Islander count |
| Two or More Races Students | Multi-racial count |
| Nonresident Alien Students | International count |
| Unknown Ethnicity Students | Unknown ethnicity count |

#### Retention (4 measures)
| Measure | Description |
|---------|-------------|
| Fall Enrollment | Fall term enrollment |
| Spring Enrollment | Spring term enrollment |
| Persistence Rate | Term-to-term persistence |
| Fall-to-Fall Retention Rate | Annual retention rate |

#### Sections (12 measures)
| Measure | Description |
|---------|-------------|
| Total Sections | Count of sections |
| Total Capacity | Total seat capacity |
| Total Section Enrollment | Enrolled students |
| Fill Rate | Enrollment / Capacity |
| Open Sections | Sections with availability |
| Full Sections | Sections at capacity |
| Cancelled Sections | Cancelled section count |
| Seats Available | Open seats remaining |
| Avg Section Size | Average enrollment |
| Sections Near Capacity | 80-99% full sections |
| Low Enrollment Sections | Under 25% capacity |
| Sections by Fill Category | Fill rate distribution |

#### Schedule (9 measures)
| Measure | Description |
|---------|-------------|
| Total Schedules | Schedule record count |
| Unique Rooms Used | Distinct rooms |
| Unique Buildings Used | Distinct buildings |
| Avg Sections per Room | Room utilization |
| Online Sections | Online section count |
| Room Utilization Rate | Room usage percentage |
| Prime Time Sections | Peak hour sections |
| Evening Sections | Evening section count |
| MWF vs TR Distribution | Day pattern analysis |

#### Faculty / Faculty Metrics (22 measures)
| Measure | Description |
|---------|-------------|
| Total Faculty | Faculty count |
| Active Faculty | Active teaching faculty |
| Total Sections Taught | Sections by faculty |
| Lead Sections | Lead instructor sections |
| Avg Load Percentage | Average workload |
| Full-Time Faculty | FT faculty count |
| Part-Time Faculty | PT faculty count |
| FT/PT Faculty Percentage | FT/PT distribution |
| Courses per Faculty | Teaching load |
| Students Taught | Students per faculty |
| Faculty Teaching Online | Online instructors |
| Faculty with Overload | Overload assignments |
| Sections Without Instructor | Unstaffed sections |

#### Workload (7 measures)
| Measure | Description |
|---------|-------------|
| Faculty FTE | Faculty full-time equivalent |
| Avg Students per Faculty | Student-faculty ratio |
| Full Load Faculty | At capacity faculty |
| Part Load Faculty | Under capacity faculty |
| Adjunct vs Full Load | Faculty type comparison |
| Avg Load per Faculty | Average teaching load |
| Total Faculty Load | Sum of all loads |

#### Charges & Fees (6 measures)
| Measure | Description |
|---------|-------------|
| Total Charges | Sum of all charges |
| Students with Charges | Students billed |
| Avg Charge per Student | Average billing |
| Course-Based Charges | Per-course fees |
| Miscellaneous Charges | Other fees |
| Students by Major 1 | Revenue by program |

#### Revenue Categories (5 measures)
| Measure | Description |
|---------|-------------|
| Tuition Revenue | Tuition income |
| Room Board Revenue | Housing income |
| Technology Fees | Tech fee income |
| General Fees | General fee income |
| Online Course Fees | Online fee income |

#### Financial Performance (8 measures)
| Measure | Description |
|---------|-------------|
| Gross Tuition | Total tuition billed |
| Net Tuition Revenue | Tuition after discounts |
| Discount Rate | Institutional discount % |
| Revenue YoY Growth | Year-over-year change |
| Prior Year Revenue | Previous year comparison |
| Revenue Change | Revenue delta |
| Collection Rate | Payment collection % |
| Institutional Aid | Institutional grants |

#### Transactions (8 measures)
| Measure | Description |
|---------|-------------|
| Total Payments | Sum of payments |
| Total Debits | Sum of debits |
| Total Credits | Sum of credits |
| Net Transaction Balance | Net balance |
| Transaction Count | Number of transactions |
| Charges_1098T | 1098-T reportable charges |
| Financial Aid Applied | Aid applied to accounts |
| Accounts with Balance | Accounts with AR |

#### Accounts Receivable (8 measures)
| Measure | Description |
|---------|-------------|
| AR 0-90 Days | Current AR |
| AR 91-180 Days | 3-6 month AR |
| AR 181-365 Days | 6-12 month AR |
| AR 1-2 Years | 1-2 year AR |
| AR 2-3 Years | 2-3 year AR |
| AR 3-5 Years | 3-5 year AR |
| AR Over 5 Years | 5+ year AR |
| Total AR Balance | Total accounts receivable |

#### Financial Aid (6 measures)
| Measure | Description |
|---------|-------------|
| Total Aid Awarded | Sum of awards |
| Total Aid Disbursed | Disbursed aid |
| Pending Disbursement | Pending aid |
| Students Receiving Aid | Aid recipients |
| Avg Award per Student | Average award |
| Pell Recipients | Pell grant recipients |

#### Graduation (4 measures)
| Measure | Description |
|---------|-------------|
| Graduated Students | Graduate count |
| Graduation Rate | Graduation percentage |
| Graduates by Major | Degrees by program |
| Average Credits to Graduate | Credits to completion |

#### Trends (9 measures)
| Measure | Description |
|---------|-------------|
| Enrollment Trend | Enrollment over time |
| Credit Hours Trend | Credit hours over time |
| Prior Term Enrollment | Previous term count |
| Term-over-Term Change | ToT change |
| 3-Year Enrollment Trend | 3-year analysis |
| Same Term Last Year | STLY comparison |
| YoY Enrollment Change | Year-over-year change |

#### Year over Year (4 measures)
| Measure | Description |
|---------|-------------|
| YoY Student Change | Student count change |
| YoY Indicator | Change indicator |
| Prior Year Students | Previous year count |

---

## Data Model Relationships

### Primary Join Keys
- **ID_NUM** - Student/Person identifier
- **APPID** - Application/Record identifier
- **YR_CDE** - Academic year code
- **TRM_CDE** - Term code

### Key Relationship Chains

```
stud_term_sum_div (Central Fact Table)
    ├── name_master (ID_NUM)
    ├── student_master (ID_NUM)
    ├── biograph_master (ID_NUM)
    ├── degree_history (ID_NUM)
    ├── ethnic_race_report (ID_NUM)
    ├── major_minor_def (MAJOR_1 → MAJOR_CDE)
    ├── Year_Dim (YR_CDE)
    └── term_def (TRM_CDE)

section_master
    ├── Year_Dim (YR_CDE)
    └── term_def (TRM_CDE)

faculty_load_table
    ├── faculty_master (INSTRCTR_ID_NUM → ID_NUM)
    ├── Year_Dim (YR_CDE)
    └── term_def (TRM_CDE)

fees / trans_hist
    ├── name_master (ID_NUM)
    ├── biograph_master (ID_NUM)
    ├── student_master (ID_NUM)
    ├── Year_Dim (YR / CHG_YR_TRAN_HIST)
    └── term_def (TRM / CHG_TRM_TRAN_HIST)

stu_award
    └── stu_award_year (stu_award_year_token)
        └── award_year_defn (award_year_token)
```

---

## Measure Categories

| Category | Count | Primary Table |
|----------|-------|---------------|
| Courses | 13 | student_crs_hist |
| Enrollment | 14 | stud_term_sum_div |
| Demographics | 8 | stud_term_sum_div, biograph_master |
| GPA | 5 | stud_term_sum_div |
| Academic Standing | 8 | stud_term_sum_div |
| Ethnicity | 10 | stud_term_sum_div |
| Retention | 4 | stud_term_sum_div, CohortYears |
| Sections | 12 | section_master |
| Schedule | 9 | section_schedules |
| Faculty | 22 | faculty_load_table |
| Workload | 7 | faculty_load_table |
| Charges & Fees | 6 | fees |
| Revenue | 5 | fees |
| Financial Performance | 8 | fees, trans_hist |
| Transactions | 8 | trans_hist |
| Accounts Receivable | 8 | trans_hist |
| Financial Aid | 6 | stu_award |
| Graduation | 4 | degree_history |
| Trends | 9 | stud_term_sum_div |
| Year over Year | 4 | stud_term_sum_div |
| **TOTAL** | **175+** | |

---

## Data Source Connection

```
Host: adb-5814127397732749.9.azuredatabricks.net
HTTP Path: /sql/1.0/warehouses/29078c19f03c03ca
DSN: JenzabarDataLake
Authentication: Azure AD
Driver: Simba Spark ODBC Driver
```

---

## Notes

1. **Data Refresh:** Model connects live to Databricks - refresh as needed
2. **Salary Data:** HR salary data not available (stops at 2019)
3. **Academic Data:** Full coverage 2020-2025
4. **Financial Data:** Full coverage including AR aging
5. **Hidden Columns:** Many technical columns are hidden for Copilot optimization

---

*Documentation generated January 1, 2026*
