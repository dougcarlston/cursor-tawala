package com.scissor.webrobot;


public class Row {
    private Table table;
    private String[] cells;

    Row(Table table, int row, String[] cells) {
        this.table = table;
        this.cells = cells;
    }

    public String getCell(String columnName) {
        return cells[table.columnNumberforName(columnName)].trim();
    }

    public int indexOfCellWithContents(String columnName) {
        for (int i = 0; i < cells.length; i++) {
            String cell = cells[i];
            if (columnName.equalsIgnoreCase(cell)) return i;
        }
        return -1;
    }
}
