using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ant.csscript.core.domain
{
    /// <summary>
    /// 区域数据
    /// </summary>
    public class AreaData
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }
        public AreaData()
        {
            Rows = new List<AreaRow>(5);
        }
        public List<AreaRow> Rows { get; set; }
        /// <summary>
        /// 行数
        /// </summary>
        public int RowCount { get { return Rows.Count; } }

        /// <summary>
        /// 列数
        /// </summary>
        public int ColumnCount
        {
            get
            {
                if (Rows.Count <= 0) return 0;
                else return Rows[0].ColumnCount;
            }
        }

        public AreaItemValue this[int row, int column]
        {
            get
            {
                return Rows[row].Row[column];
            }
            set
            {
                if (row >= Rows.Count) Rows.Add(new AreaRow());
                Rows[row][column] = value;
            }
        }
        /// <summary>
        /// 截取一个范围内的 数据为新的区域
        /// </summary>
        /// <param name="beginRow"></param>
        /// <param name="beginColumn"></param>
        /// <param name="endRow"></param>
        /// <param name="endColumn"></param>
        /// <returns></returns>
        public AreaData this[int beginRow, int beginColumn, int endRow, int endColumn]
        {
            get
            {
                AreaData area = new AreaData();
                for (int i = beginRow; i <= endRow; i++)
                {
                    for (int j = beginColumn; j <= endColumn; j++)
                    {
                        area[i - beginRow, j - beginColumn] = this[i, j];
                    }
                }
                return area;
            }
        }

        /// <summary>
        /// 设置一个区域的数据
        /// </summary>
        /// <param name="beginRow"></param>
        /// <param name="beginColumn"></param>
        /// <param name="endRow"></param>
        /// <param name="endColumn"></param>
        /// <param name="value"></param>
        public void SetValue(int beginRow, int beginColumn, int endRow, int endColumn, AreaItemValue value)
        {
            for (int i = beginRow; i <= endRow; i++)
            {
                for (int j = beginColumn; j <= endColumn; j++)
                {
                    this[i, j] = value;
                }
            }
        }

        public void SetValue(int row, int column, AreaItemValue value)
        {
            Rows[row].Row[column] = value;
        }

        public AreaItemValue this[int row, string columnName]
        {
            get
            {
                return Rows[row].Row.Find(d =>
                {
                    return d.ColumnName.Equals(columnName);
                });
            }
        }
        /// <summary>
        /// 列名
        /// </summary>
        public List<string> ColumnNames
        {
            get
            {
                if (Rows.Count <= 0) return new List<string>(0);
                else
                {
                    List<string> columnNames = new List<string>(20);

                    //找到最大列数
                    int maxCol = 0;
                    Rows.ForEach(r => { if (maxCol < r.ColumnCount) maxCol = r.ColumnCount; });

                    for (int i = 0; i < maxCol; i++)
                    {
                        if (Rows[0].Row.Count <= i)
                        {
                            columnNames.Add("");
                            continue;
                        }
                        var cell = Rows[0].Row[i];
                        columnNames.Add(cell.ColumnName);
                    }

                    //Rows[0].Row.ForEach(cell =>
                    //{
                    //    columnNames.Add(cell == null ? "" : cell.ColumnName);
                    //});

                    //var columnNames = (from d in Rows[0].Row where 1 == 1 select d == null ? "" : d.ColumnName).ToList();
                    return columnNames;
                }
            }
        }

        /// <summary>
        /// 设置列名
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="name"></param>
        public void SetColumnName(int columnIndex, string name)
        {
            if (Rows.Count <= 0) return;
            else
            {
                if (columnIndex < 0 || columnIndex > ColumnCount) return;

                //  Rows[0][columnIndex].Value = name;
                Rows.ForEach(r => { r[columnIndex].ColumnName = name; });
            }
        }
        /// <summary>
        /// 设置列名
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="name"></param>
        public void SetColumnName(string oldName, string name)
        {
            if (Rows.Count <= 0) return;
            else
            {
                if (Rows[0][oldName] == null) return;

                // Rows[0][oldName].Value = name;
                Rows.ForEach(r => { r[oldName].ColumnName = name; });
            }
        }
    }
    /// <summary>
    /// 行
    /// </summary>
    public class AreaRow
    {
        public AreaRow()
        {
            Row = new List<AreaItemValue>(10);
        }
        public List<AreaItemValue> Row { get; set; }

        /// <summary>
        /// 列数
        /// </summary>
        public int ColumnCount { get { return Row.Count; } }

        public AreaItemValue this[int index]
        {
            get
            {
                if (index >= Row.Count)
                {
                    return null;
                }
                return Row[index];
            }
            set
            {
                if (index >= Row.Count) Row.Add(null);
                Row[index] = value;
            }
        }
        public AreaItemValue this[string columnName]
        {
            get
            {
                return Row.Find(d => d.ColumnName.Equals(columnName));
            }
            set
            {
                var r = this[columnName];
                if (r == null) return;
                this[columnName] = value;
            }
        }
    }

    /// <summary>
    /// 区域单项数据
    /// </summary>
    public class AreaItemValue
    {
        /// <summary>
        /// 列号
        /// </summary>
        public int? ColumnIndex { get; set; }
        /// <summary>
        /// 行号
        /// </summary>
        public int? RowIndex { get; set; }
        public string ColumnName { get; set; }
        private string _Value;

        public string Value
        {
            get
            {
                if (string.IsNullOrEmpty(_Value)) return "";
                return _Value;
            }
            set { _Value = value; }
        }

        /// <summary>
        /// 真实页码
        /// </summary>
        public int PageNum { get; set; }
        /// <summary>
        /// 真实行号
        /// </summary>
        public int RowNum { get; set; }
        /// <summary>
        /// 真实列号
        /// </summary>
        public int ColumnNum { get; set; }
    }
}
