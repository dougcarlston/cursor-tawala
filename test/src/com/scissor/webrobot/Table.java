package com.scissor.webrobot;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import com.meterware.httpunit.WebTable;

public class Table {
    WebTable rawTable;
    String[][] cachedCellText;

    Table(WebTable rawTable) {
        this.rawTable = rawTable;
    }

    public int getRowCount() {
        return rawTable.getRowCount();
    }

    public int getColumnCount() {
        return rawTable.getColumnCount();
    }

    public String getCellAsText(int i, int i1) {
        return rawTable.getCellAsText(i, i1);
    }

    public String getContents() {
        return join(rawTable.asText());
    }

    public String getID() {
        return rawTable.getID();
    }

    public String getName() {
        return rawTable.getName();
    }

    public String getTitle() {
        return rawTable.getTitle();
    }

    public String getSummary() {
        return rawTable.getSummary();
    }

    private String join(String[][] strings) {
        StringBuffer sb = new StringBuffer();
        for (int i = 0; i < strings.length; i++) {
            String[] row = strings[i];
            for (int j = 0; j < row.length; j++) {
                String cell = row[j];
                sb.append(clean(cell));
                if (j < row.length-1) {
                    sb.append(" | ");
                }
            }
            if (i < strings.length-1) {
                sb.append(" /// ");
            }
        }
        return sb.toString();
    }


    private String clean(String cell) {
        StringBuffer sb = new StringBuffer();
        char[] chars = cell.toCharArray();
        for (int i = 0; i < chars.length; i++) {
            char c = chars[i];
            if (!isPrintable(c)) c = ' ';
            if (!twoSpacesInARow(sb, c)) sb.append(c);
        }


        return sb.toString();
    }

    private boolean isPrintable(char c) {
        if (c=='\n' || c == '\t' || c == '\r') return false;
        return true;
    }

    private boolean twoSpacesInARow(StringBuffer sb, char c) {
        if (sb.length()==0) return false;
        return ((sb.charAt(sb.length()-1) == ' ') && c == ' ');
    }

    public Iterator rowIterator() {
        List<Row> rows = new ArrayList<Row>();
        String[][] rowText = getCellTextArray();
        for (int i = 0; i < rowText.length; i++) {
            rows.add(getRow(i));
        }

        return rows.iterator();
    }

    private Row getRow(int i) {
        return new Row(this, i, getCellTextArray()[i]);
    }

    private String[][] getCellTextArray() {
        if (cachedCellText==null) {
            cachedCellText = rawTable.asText();
        }
        return cachedCellText;
    }

    public int columnNumberforName(String columnName) {
        Row header = getRow(0);
        return header.indexOfCellWithContents(columnName);
    }


}
