#!/bin/bash
# Quick script to count measures in the Power BI model

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CSV_FILE="$SCRIPT_DIR/Model Measures/Measures Model Analysis 20251226 162605.csv"

if [ ! -f "$CSV_FILE" ]; then
    echo "Error: Measures CSV file not found at $CSV_FILE"
    exit 1
fi

# Count measures that are in 'Ready' state (actual valid measures)
# The CSV export contains multiline DAX expressions that create additional rows,
# so we filter for rows with State="Ready" to get the actual count
MEASURE_COUNT=$(tail -n +2 "$CSV_FILE" | grep -c ',"Ready",')

echo "=================================="
echo "Power BI Model Measure Count"
echo "=================================="
echo ""
echo "Total Measures: $MEASURE_COUNT"
echo ""
echo "For detailed breakdown, run: python3 count_measures.py"
echo ""
