#!/usr/bin/env python3
"""
Power BI Model - Measure Counter
Reads the measures CSV export and provides detailed statistics
"""

import csv
import os
from collections import defaultdict
from datetime import datetime

def count_measures(csv_path):
    """Count measures from the exported CSV file"""
    
    if not os.path.exists(csv_path):
        print(f"Error: CSV file not found at {csv_path}")
        return
    
    total_measures = 0
    measures_by_table = defaultdict(int)
    measures_by_folder = defaultdict(int)
    measures_by_state = defaultdict(int)
    measures_list = []
    
    with open(csv_path, 'r', encoding='utf-8') as f:
        reader = csv.DictReader(f)
        
        for row in reader:
            name = (row.get('Name') or '').strip()
            state = (row.get('State') or '').strip()
            table = (row.get('Table') or '').strip()
            folder = (row.get('Display Folder') or '').strip()
            
            # Filter for valid measures: has a real name and is in Ready state
            # The CSV export has multiline DAX expressions that create invalid rows
            if name and state == 'Ready' and len(name) > 2 and not name in [')"]', ')"', ')']:
                total_measures += 1
                
                if not folder:
                    folder = '(No Folder)'
                
                measures_by_table[table if table else 'Unknown'] += 1
                measures_by_folder[folder] += 1
                measures_by_state[state] += 1
                measures_list.append({
                    'name': name,
                    'table': table,
                    'folder': folder,
                    'state': state
                })
    
    # Print results
    print("=" * 80)
    print("MEASURE COUNT REPORT")
    print("=" * 80)
    print()
    print(f"Total Measures in Model: {total_measures}")
    print()
    
    if measures_by_table:
        print("Measures by Table:")
        print("-" * 80)
        for table, count in sorted(measures_by_table.items(), key=lambda x: x[1], reverse=True):
            print(f"  {table}: {count} measures")
        print()
    
    if measures_by_folder:
        print("Measures by Display Folder:")
        print("-" * 80)
        for folder, count in sorted(measures_by_folder.items(), key=lambda x: x[1], reverse=True):
            print(f"  {folder}: {count} measures")
        print()
    
    if measures_by_state:
        print("Measures by State:")
        print("-" * 80)
        for state, count in sorted(measures_by_state.items(), key=lambda x: x[1], reverse=True):
            print(f"  {state}: {count} measures")
        print()
    
    print("=" * 80)
    print(f"Analysis completed at {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
    print("=" * 80)
    
    return {
        'total': total_measures,
        'by_table': dict(measures_by_table),
        'by_folder': dict(measures_by_folder),
        'by_state': dict(measures_by_state),
        'measures': measures_list
    }

if __name__ == "__main__":
    # Path to the measures CSV file
    csv_path = "Model Measures/Measures Model Analysis 20251226 162605.csv"
    
    # If running from different directory, try to find the file
    if not os.path.exists(csv_path):
        csv_path = os.path.join(os.path.dirname(__file__), csv_path)
    
    if not os.path.exists(csv_path):
        print("Error: Could not find measures CSV file.")
        print("Please run this script from the repository root directory.")
        exit(1)
    
    result = count_measures(csv_path)
    
    if result:
        print()
        print(f"Summary: Your model contains {result['total']} measures.")
