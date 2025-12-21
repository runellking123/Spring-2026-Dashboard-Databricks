// Remove all 100% empty columns from Power BI model
// Run in Tabular Editor: C# Script tab -> Paste -> Run

var columnsToRemove = new Dictionary<string, List<string>>
{
    {"student_crs_hist", new List<string> {
        "WAITLIST_INVITATION_DATE", "ADV_APPROVAL_REQ_DTE", "PESC_COURSE_CREDIT_BASIS_CDE",
        "BILLING_PERIOD_ID", "EDUC_SUCCESS_LEVEL_DEF_APPID", "FACULTY_PTY", "STUDENT_PTY",
        "RANDOM_PTY", "MUST_PAY_DTE", "FEE_SEQUENCE_NUMBER", "UDEF_1A_4", "UDEF_1A_5", "UDEF_2A_1"
    }},
    {"student_master", new List<string> {
        "LICENSE_PLATE", "PARKING_PERMIT_NUM", "PURGED_DTE", "TAPE_VOLUME_IDENT",
        "TAPE_SEQ_NUM", "RESERVED_SB", "UDEF_1A_4", "UDEF_1A_5"
    }},
    {"name_master", new List<string> {
        "THIRD_PTY_BILL_TYPE_DEF_APPID", "UDEF_1A_5", "UDEF_2A_3", "UDEF_3A_2",
        "UDEF_3A_3", "UDEF_5A_1", "UDEF_ID_1", "UDEF_ID_2", "UDEF_DTE_2"
    }},
    {"biograph_master", new List<string> {
        "UDEF_1A_2", "UDEF_1A_3", "UDEF_1A_4", "UDEF_1A_5", "UDEF_1A_6", "UDEF_1A_7",
        "UDEF_2A_3", "UDEF_2A_4", "UDEF_3A_1", "UDEF_3A_2", "UDEF_3A_3", "UDEF_5A_1",
        "UDEF_5A_2", "UDEF_10A_1", "UDEF_ID_1", "UDEF_ID_2", "UDEF_DTE_1", "UDEF_DTE_2"
    }},
    {"address_master", new List<string> {
        "UDEF_1A_2", "UDEF_1A_3", "UDEF_2A_1", "UDEF_2A_2", "UDEF_3A_1",
        "UDEF_3A_2", "UDEF_5A_1", "UDEF_ID_1"
    }},
    {"stud_term_sum_div", new List<string> {
        "CLASS_RANK_HI_RANG", "UDEF_1A_1", "UDEF_1A_2", "UDEF_1A_3", "UDEF_1A_4",
        "UDEF_1A_5", "UDEF_2A_1", "UDEF_2A_2", "UDEF_2A_3", "UDEF_3A_1", "UDEF_3A_3", "UDEF_5A_1"
    }},
    {"degree_history", new List<string> {
        "MAJOR_3", "MAJOR_4", "MINOR_3", "MINOR_4", "CONCENTRATION_5", "CERTIFICATION_3",
        "CERTIFICATION_4", "LAST_LEAVE_OF_ABSE_END_DTE", "PROGRAM_AREA_1", "PROGRAM_AREA_2",
        "DIPLOMA_ORDER_DTE", "UDEF_1A_2", "UDEF_1A_3", "UDEF_1A_4", "UDEF_1A_5",
        "UDEF_2A_1", "UDEF_2A_2", "UDEF_2A_3", "UDEF_5A_1", "UDEF_5A_2", "UDEF_DTE_1", "UDEF_DTE_2"
    }},
    {"year_term_table", new List<string> {
        "PESC_SESSION_TYPE", "BILLING_PERIOD_ID", "PAYMENT_DUE_DATE", "STUDENT_WITHDRAWAL_END_DATE"
    }},
    {"major_minor_def", new List<string> {
        "CIP_CDE_EXT", "ALT_CIP_CODE", "UDEF_1A_1", "UDEF_DTE_3"
    }},
    {"degree_definition", new List<string> {
        "GE_CREDENTIAL_LEVEL"
    }}
};

int totalRemoved = 0;
int totalSkipped = 0;
var summary = new System.Text.StringBuilder();
summary.AppendLine("=== COLUMN REMOVAL SUMMARY ===");
summary.AppendLine("");

foreach (var tableEntry in columnsToRemove)
{
    string tableName = tableEntry.Key;
    var columns = tableEntry.Value;
    int tableRemoved = 0;
    int tableSkipped = 0;

    var table = Model.Tables.FirstOrDefault(t => t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));

    if (table == null)
    {
        summary.AppendLine("[SKIP] Table '" + tableName + "' not found in model");
        tableSkipped = columns.Count;
        totalSkipped += tableSkipped;
        continue;
    }

    foreach (var colName in columns)
    {
        var column = table.Columns.FirstOrDefault(c => c.Name.Equals(colName, StringComparison.OrdinalIgnoreCase));

        if (column != null)
        {
            column.Delete();
            tableRemoved++;
            totalRemoved++;
        }
        else
        {
            tableSkipped++;
            totalSkipped++;
        }
    }

    summary.AppendLine("[" + tableName + "] Removed: " + tableRemoved + ", Skipped: " + tableSkipped);
}

summary.AppendLine("");
summary.AppendLine("=== TOTAL ===");
summary.AppendLine("Columns Removed: " + totalRemoved);
summary.AppendLine("Columns Skipped (not found): " + totalSkipped);
summary.AppendLine("");
summary.AppendLine("Save the model to apply changes.");

Info(summary.ToString());
