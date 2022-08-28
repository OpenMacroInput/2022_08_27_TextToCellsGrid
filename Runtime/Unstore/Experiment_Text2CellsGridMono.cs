using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;

public class Experiment_Text2CellsGridMono : MonoBehaviour
{
    [TextArea(0,10)]
    public string m_text;

    public char [] m_splitEndLine = new char[] { '\n'};
    public char [] m_splitCells = new char[] { ',' };
    public CellsGrid2D m_grid = new CellsGrid2D();

    [TextArea(0, 10)]
    public string m_textOut;

    public void OnValidate()
    {
        CellsGrid2DUtility.ConvertText2Grid(in m_text, in m_splitEndLine, in m_splitCells, out m_grid);
        CellsGrid2DUtility.ConvertGrid2Text(in m_grid, '\n', ',', out m_text);
    }
}

[System.Serializable]
public struct CellsGrid2D {


    public int m_columnCount;
    public int m_rawCount;
    public int m_max1DCount;
    public string[] m_cells;
    public void SetSize(int column, int raw) {
        m_columnCount = column;
        m_rawCount = raw;
        m_max1DCount = m_columnCount * m_rawCount;
        m_cells = new string[m_max1DCount];
    }

    public void GetCellCount(in int index1D, out int count)
    {
        count = m_cells[index1D].Length;
    }
    public void GetCellCount(in int columnIndex, in int rawIndex, out int count)
    {
        count = m_cells[columnIndex + rawIndex * m_columnCount].Length;
    }
    public void SetText(in int columnIndex, in int rawIndex, string text)
    {
        m_cells[columnIndex + rawIndex * m_columnCount] = text;
    }
    public void GetText(in int columnIndex, in int rawIndex, out string text)
    {
        text = m_cells[columnIndex + rawIndex * m_columnCount];
    }
    public string GetText(in int columnIndex, in int rawIndex)
    {
        return m_cells[columnIndex + rawIndex * m_columnCount];
    }
    public void GetText(in int index1D, out string text)
    {
        text = m_cells[index1D];
    }
    public string GetText(in int index1D)
    {
        return m_cells[index1D];
    }
    public void SetText(in int index1D, out string text)
    {
        text = m_cells[index1D];
    }
    public int GetMax1DSize() { return m_max1DCount; }
    public int GetColumnSize() { return m_columnCount; }
    public int GetRawSize() { return m_rawCount; }

    public bool IsValideIndex1D(in int i)
    {
        return i > -1 && i < m_columnCount * m_rawCount;
    }

    public bool IsValideIndex2D(in int column, in int line)
    {
        return column > -1 && line > -1 && column < m_columnCount && line < m_rawCount;
    }
}

public class CellsGrid2DUtility
{
    public static char[] SplitReturnLine = new char[] { '\n'};
    public static char[] SplitComma = new char[] { ',' };
    public static bool m_ignoreCommentary=true;
    public static bool m_ignoreCaseWithSpace =false;

    public static void ConvertText2Grid(in string text, in char [] endLineChar, in char [] cellsSpliterChar, out CellsGrid2D grid) {
        string[] lines = text.Split(endLineChar);
        grid = new CellsGrid2D();
        if (lines.Length > 0)
        {
            string[] columns = lines[0].Split(cellsSpliterChar);
            grid.SetSize(columns.Length, lines.Length);
        }
        for (int y = 0; y < lines.Length; y++)
        {
            string[] columns = lines[y].Split(cellsSpliterChar);
            for (int x = 0; x < columns.Length; x++)
            {
                if (m_ignoreCommentary && (columns[x].StartsWith("\\\\") || columns[x].StartsWith("//")))
                    columns[x] = "";
                else if (m_ignoreCaseWithSpace && columns[x].IndexOf(" ")>-1)
                    columns[x] = "";
                grid.SetText(x, y, columns[x]);
            }
        }

    }

    public static void ConvertGrid2Text(in CellsGrid2D grid, in char endLine, in char cellSpliter, out string text) {
        StringBuilder sb = new StringBuilder();
        for (int y = 0; y < grid.GetRawSize(); y++)
        {
            for (int x = 0; x < grid.GetColumnSize(); x++)
            {
                grid.GetText(in x, in y, out string t);
                sb.Append(t);
                if (x < grid.GetColumnSize() - 1)
                    sb.Append(cellSpliter);
            }
            if (y < grid.GetRawSize() - 1)
                sb.Append(endLine);
        }
        text = sb.ToString();

    }

    public static void DirectDownload(in string url, out string text)
    {
        try
        {
            WebClient w = new WebClient();
            text = w.DownloadString(url);
            w.Dispose();
        }
        catch (Exception) {
            text = "";
        }
    }

    public static void FetchGridsFromCSVWebLink(in string urlLabel, in string urlValue, out CellsGrid2D labelGrid, out CellsGrid2D valueGrid)
    {

        CellsGrid2DUtility.DirectDownload(in urlLabel, out string labelGridCsv);
        CellsGrid2DUtility.DirectDownload(in urlValue, out string valueGridCsv);
        FetchGridsFromCSV(in labelGridCsv, in valueGridCsv, out labelGrid, out valueGrid);
    }
    public static void FetchGridsFromCSV(in string labelGridCsv, in string valueGridCsv, out CellsGrid2D labelGrid, out CellsGrid2D valueGrid)
    {
        CellsGrid2DUtility.ConvertText2Grid(in labelGridCsv, in CellsGrid2DUtility.SplitReturnLine, in CellsGrid2DUtility.SplitComma, out  labelGrid);
        CellsGrid2DUtility.ConvertText2Grid(in valueGridCsv, in CellsGrid2DUtility.SplitReturnLine, in CellsGrid2DUtility.SplitComma, out  valueGrid);
    }
    public static void FetchGridFromCSVWebLink(in string urlLabel,  out CellsGrid2D grid)
    {
        CellsGrid2DUtility.DirectDownload(in urlLabel, out string labelGridCsv);
        FetchGridFromCSV(in labelGridCsv, out grid);
    }
    public static void FetchGridFromCSV(in string labelGridCsv,  out CellsGrid2D grid)
    {
        CellsGrid2DUtility.ConvertText2Grid(in labelGridCsv, in CellsGrid2DUtility.SplitReturnLine, in CellsGrid2DUtility.SplitComma, out grid);
    }

    public static bool IsTrue(in string value)
    {
        if (value.Length <= 0)
            return true;
        if (value.Length == 1)
            return value[0] == '1' || value[0] == 'v' ;
        string t =  value.Trim().ToLower();
        return t == "1" || t == "v" || t == "true";
    }
}