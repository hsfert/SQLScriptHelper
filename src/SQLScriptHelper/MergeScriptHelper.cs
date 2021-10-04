using System;
using System.Text;

namespace SQLScriptHelper
{
	/// <summary>
	/// ScriptHelper which will generate a Merge statement given a stream of data.
	/// </summary>
	public class MergeScriptHelper<T> : ScriptHelper where T : class
	{
		private Action<T, StringBuilder> Append;
		public MergeScriptHelper(string table_name, ScriptHelperData columnData) : base()
		{
			sqlHeader = Merge.GetSQLHeader(table_name,
					columnData.UpdateColumns,
					columnData.OutputColumns);
			sqlFooter = Merge.GetSQLFooter(columnData.Columns,
					columnData.MatchingColumns,
					columnData.InequalityColumns,
					columnData.UpdateColumns,
					columnData.InsertColumns,
					columnData.OutputColumns);
			Append = (new RowExpressionBuilder<T>()).Build(columnData.Columns);
		}

		protected override void AppendRowToStringBuilder(object obj, StringBuilder sql)
		{
			if (obj is T)
			{
				Append((T)obj, sql);
			}
			else
			{
				throw new Exception($"GenericScriptHelper only accept object of type {typeof(T)} but received type{obj.GetType()} instead!");
			}
		}
	}
}
