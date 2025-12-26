#!/bin/bash
# Quick script to count measures in the Power BI model

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
MEASURES_DIR="$SCRIPT_DIR/Model Measures"

# Find the most recent CSV file matching the pattern
CSV_FILE=$(find "$MEASURES_DIR" -name "Measures Model Analysis *.csv" -type f -print0 2>/dev/null | xargs -0 ls -t 2>/dev/null | head -1)

if [ -z "$CSV_FILE" ]; then
    echo "Error: Measures CSV file not found"
    echo "Expected pattern: Model Measures/Measures Model Analysis *.csv"
    exit 1
fi

echo "Using measures file: $(basename "$CSV_FILE")"
echo ""

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
