-- ============================================================================
-- POWER BI ENROLLMENT DASHBOARD QUERIES
-- Jenzabar Data Lake (Databricks)
-- Generated: 2025-12-20
-- ============================================================================

-- ============================================================================
-- OPTION 1: STUDENT-LEVEL SUMMARY (One Row Per Student Per Term)
-- Best for: Headcount, retention, persistence, and demographic dashboards
-- ============================================================================

SELECT DISTINCT
    -- STUDENT IDENTIFIERS
    sch.ID_NUM,
    nm.LAST_NAME,
    nm.FIRST_NAME,
    nm.PREFERRED_NAME,

    -- TERM INFO
    sch.YR_CDE,
    sch.TRM_CDE,
    ytt.YR_TRM_DESC,
    ytt.TRM_BEGIN_DTE,
    ytt.TRM_END_DTE,

    -- ENROLLMENT STATUS
    std.PT_FT_STS,
    std.DIV_CDE,
    div.DIV_DESC,
    std.CLASS_CDE,

    -- ACADEMIC PROGRAM
    std.DEGREE_CDE,
    std.MAJOR_1,
    std.MAJOR_2,
    std.MINOR_1,
    mmd.MAJOR_MINOR_DESC AS MAJOR_1_DESC,

    -- CREDIT HOURS & GPA
    std.HRS_ENROLLED,
    std.NUM_OF_CRS,
    std.TRM_GPA,
    std.CAREER_GPA,
    std.TRM_HRS_ATTEMPT,
    std.TRM_HRS_EARNED,
    std.CAREER_HRS_ATTEMPT,
    std.CAREER_HRS_EARNED,

    -- DEMOGRAPHICS
    bio.GENDER,
    bio.BIRTH_DTE,
    bio.MARITAL_STS,
    bio.CITIZEN_OF,
    bio.CITIZENSHIP_STS,
    bio.VISA_TYPE,
    bio.DISABILITY_STS,

    -- IPEDS RACE/ETHNICITY (using subquery to avoid duplicates)
    eth.IPEDS_ETHNICITY,

    -- ADDRESS / STATE
    addr.CITY,
    addr.STATE,
    addr.ZIP5,
    addr.COUNTY,
    addr.COUNTRY,

    -- STUDENT ATTRIBUTES & RETENTION TRACKING
    sm.ENTRANCE_YR,
    sm.ENTRANCE_TRM,
    CONCAT(sm.ENTRANCE_YR, '-', sm.ENTRANCE_TRM) AS COHORT,
    sm.FIRST_GENERATION,
    sm.RESID_COMMUTER_STS,
    sm.LOC_CDE,
    sm.MOST_RECNT_YR_ENR,
    sm.MOST_RECNT_TRM_ENR,

    -- YEARS/TERMS SINCE ENTRY (for retention/persistence)
    CAST(sch.YR_CDE AS INT) - CAST(sm.ENTRANCE_YR AS INT) AS YEARS_SINCE_ENTRY,

    -- DEGREE / GRADUATION INFO
    dh.DEGR_CDE AS DEGREE_EARNED,
    degd.DEGREE_DESC,
    dh.DTE_DEGR_CONFERRED,
    dh.DEGREE_YR,
    dh.DEGREE_TRM,
    dh.EXPECT_GRAD_YR,
    dh.EXPECT_GRAD_TRM,
    dh.ENTRY_DTE AS PROGRAM_ENTRY_DTE,
    dh.EXIT_DTE,
    dh.EXIT_REASON,
    dh.WITHDRAWAL_DTE,
    dh.DEGR_HONORS_1,
    CASE WHEN dh.DTE_DEGR_CONFERRED IS NOT NULL THEN 'Y' ELSE 'N' END AS GRADUATED_FLAG,

    -- ADMISSION INFO
    cand.CANDIDACY_TYPE,
    cand.READMIT

FROM j1.student_crs_hist sch

-- Student term summary
LEFT JOIN j1.stud_term_sum_div std
    ON sch.ID_NUM = std.ID_NUM
    AND sch.YR_CDE = std.YR_CDE
    AND sch.TRM_CDE = std.TRM_CDE

-- Name
LEFT JOIN j1.name_master nm
    ON sch.ID_NUM = nm.ID_NUM

-- Demographics
LEFT JOIN j1.biograph_master bio
    ON sch.ID_NUM = bio.ID_NUM

-- Student master
LEFT JOIN j1.student_master sm
    ON sch.ID_NUM = sm.ID_NUM

-- Address (current)
LEFT JOIN j1.address_master addr
    ON sch.ID_NUM = addr.ID_NUM
    AND addr.ADDR_CDE = '*CUR'

-- Term definitions
LEFT JOIN j1.year_term_table ytt
    ON sch.YR_CDE = ytt.YR_CDE
    AND sch.TRM_CDE = ytt.TRM_CDE

-- Major/Minor definitions
LEFT JOIN j1.major_minor_def mmd
    ON std.MAJOR_1 = mmd.MAJOR_CDE

-- Division definitions
LEFT JOIN j1.division_def div
    ON std.DIV_CDE = div.DIV_CDE

-- IPEDS Ethnicity (deduplicated)
LEFT JOIN (
    SELECT DISTINCT err.ID_NUM, ipe.VALUE_DESCRIPTION AS IPEDS_ETHNICITY
    FROM j1.ethnic_race_report err
    LEFT JOIN j1.ipeds_ethnic_race_val_def ipe
        ON err.IPEDS_REPORT_VALUE = ipe.IPEDS_REPORT_VALUE
) eth ON sch.ID_NUM = eth.ID_NUM

-- Degree history (current degree)
LEFT JOIN j1.degree_history dh
    ON sch.ID_NUM = dh.ID_NUM
    AND dh.CUR_DEGREE = 'Y'

-- Degree definitions
LEFT JOIN j1.degree_definition degd
    ON dh.DEGR_CDE = degd.DEGREE

-- Candidacy (current)
LEFT JOIN j1.candidacy cand
    ON sch.ID_NUM = cand.ID_NUM
    AND cand.CUR_CANDIDACY = 'Y'

WHERE sch.TRANSACTION_STS = 'P'
;


-- ============================================================================
-- OPTION 2: COURSE-LEVEL DETAIL (One Row Per Course Enrollment)
-- Best for: Credit hour analysis and course-level reporting
-- ============================================================================

SELECT
    -- STUDENT IDENTIFIERS
    sch.ID_NUM,
    nm.LAST_NAME,
    nm.FIRST_NAME,
    nm.PREFERRED_NAME,

    -- TERM INFO
    sch.YR_CDE,
    sch.TRM_CDE,
    ytt.YR_TRM_DESC,

    -- COURSE DETAILS
    sch.CRS_CDE,
    sch.CRS_TITLE,
    sch.CREDIT_HRS,
    sch.GRADE_CDE,
    sch.ADD_DTE,
    sch.DROP_DTE,
    sch.TRANSACTION_STS,

    -- ENROLLMENT STATUS
    std.PT_FT_STS,
    std.DIV_CDE,
    div.DIV_DESC,
    std.CLASS_CDE,

    -- DEMOGRAPHICS
    bio.GENDER,
    bio.BIRTH_DTE,
    eth.IPEDS_ETHNICITY,

    -- ADDRESS
    addr.STATE,
    addr.CITY,
    addr.ZIP5,

    -- COHORT / RETENTION
    sm.ENTRANCE_YR,
    sm.ENTRANCE_TRM,
    CONCAT(sm.ENTRANCE_YR, '-', sm.ENTRANCE_TRM) AS COHORT,
    sm.FIRST_GENERATION,
    sm.LOC_CDE

FROM j1.student_crs_hist sch

LEFT JOIN j1.stud_term_sum_div std
    ON sch.ID_NUM = std.ID_NUM
    AND sch.YR_CDE = std.YR_CDE
    AND sch.TRM_CDE = std.TRM_CDE

LEFT JOIN j1.name_master nm
    ON sch.ID_NUM = nm.ID_NUM

LEFT JOIN j1.biograph_master bio
    ON sch.ID_NUM = bio.ID_NUM

LEFT JOIN j1.student_master sm
    ON sch.ID_NUM = sm.ID_NUM

LEFT JOIN j1.address_master addr
    ON sch.ID_NUM = addr.ID_NUM
    AND addr.ADDR_CDE = '*CUR'

LEFT JOIN j1.year_term_table ytt
    ON sch.YR_CDE = ytt.YR_CDE
    AND sch.TRM_CDE = ytt.TRM_CDE

LEFT JOIN j1.division_def div
    ON std.DIV_CDE = div.DIV_CDE

LEFT JOIN (
    SELECT DISTINCT err.ID_NUM, ipe.VALUE_DESCRIPTION AS IPEDS_ETHNICITY
    FROM j1.ethnic_race_report err
    LEFT JOIN j1.ipeds_ethnic_race_val_def ipe
        ON err.IPEDS_REPORT_VALUE = ipe.IPEDS_REPORT_VALUE
) eth ON sch.ID_NUM = eth.ID_NUM

WHERE sch.TRANSACTION_STS = 'P'
;


-- ============================================================================
-- OPTION 3: RETENTION/PERSISTENCE ANALYSIS
-- Best for: Year-over-year retention and term-to-term persistence tracking
-- ============================================================================

SELECT
    sm.ID_NUM,
    nm.LAST_NAME,
    nm.FIRST_NAME,

    -- COHORT INFO
    sm.ENTRANCE_YR AS COHORT_YEAR,
    sm.ENTRANCE_TRM AS COHORT_TERM,
    CONCAT(sm.ENTRANCE_YR, '-', sm.ENTRANCE_TRM) AS COHORT,

    -- DEMOGRAPHICS
    bio.GENDER,
    eth.IPEDS_ETHNICITY,
    addr.STATE,
    sm.FIRST_GENERATION,

    -- ENROLLMENT BY TERM (pivot in Power BI)
    std.YR_CDE,
    std.TRM_CDE,
    ytt.YR_TRM_DESC,
    std.DIV_CDE,
    std.PT_FT_STS,
    std.HRS_ENROLLED,

    -- RETENTION METRICS
    CAST(std.YR_CDE AS INT) - CAST(sm.ENTRANCE_YR AS INT) AS YEARS_SINCE_ENTRY,

    -- DEGREE INFO
    dh.DTE_DEGR_CONFERRED,
    dh.DEGREE_YR,
    dh.DEGREE_TRM,
    dh.EXIT_DTE,
    dh.EXIT_REASON,
    CASE WHEN dh.DTE_DEGR_CONFERRED IS NOT NULL THEN 'Graduated'
         WHEN dh.EXIT_DTE IS NOT NULL THEN 'Exited'
         WHEN std.YR_CDE IS NOT NULL THEN 'Enrolled'
         ELSE 'Not Enrolled'
    END AS ENROLLMENT_STATUS

FROM j1.student_master sm

LEFT JOIN j1.name_master nm
    ON sm.ID_NUM = nm.ID_NUM

LEFT JOIN j1.biograph_master bio
    ON sm.ID_NUM = bio.ID_NUM

LEFT JOIN j1.address_master addr
    ON sm.ID_NUM = addr.ID_NUM
    AND addr.ADDR_CDE = '*CUR'

LEFT JOIN (
    SELECT DISTINCT err.ID_NUM, ipe.VALUE_DESCRIPTION AS IPEDS_ETHNICITY
    FROM j1.ethnic_race_report err
    LEFT JOIN j1.ipeds_ethnic_race_val_def ipe
        ON err.IPEDS_REPORT_VALUE = ipe.IPEDS_REPORT_VALUE
) eth ON sm.ID_NUM = eth.ID_NUM

LEFT JOIN j1.stud_term_sum_div std
    ON sm.ID_NUM = std.ID_NUM

LEFT JOIN j1.year_term_table ytt
    ON std.YR_CDE = ytt.YR_CDE
    AND std.TRM_CDE = ytt.TRM_CDE

LEFT JOIN j1.degree_history dh
    ON sm.ID_NUM = dh.ID_NUM
    AND dh.CUR_DEGREE = 'Y'

WHERE sm.ENTRANCE_YR IS NOT NULL
;


-- ============================================================================
-- REFERENCE: TERM CODES
-- ============================================================================
-- YR_CDE = Academic Year (e.g., '2025' = 2025-2026 Academic Year)
-- TRM_CDE:
--   10 = Fall
--   30 = Spring
--   50 = Summer I
--   56 = Summer Bridge
--
-- Example: Spring 2026 = YR_CDE '2025', TRM_CDE '30'
-- ============================================================================


-- ============================================================================
-- REFERENCE: KEY TABLES
-- ============================================================================
-- j1.student_crs_hist      - Course enrollments (one row per course)
-- j1.stud_term_sum_div     - Student term summary (one row per student/term)
-- j1.name_master           - Student names
-- j1.biograph_master       - Demographics (gender, DOB, citizenship)
-- j1.student_master        - Student attributes (cohort, location)
-- j1.address_master        - Addresses (use ADDR_CDE = '*CUR' for current)
-- j1.degree_history        - Degree/graduation info
-- j1.ethnic_race_report    - IPEDS race/ethnicity
-- j1.year_term_table       - Term definitions
-- j1.major_minor_def       - Major/minor descriptions
-- j1.division_def          - Division descriptions (UG/GR)
-- j1.candidacy             - Admission info
-- ============================================================================
