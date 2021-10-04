using System.Collections.Generic;
using System.Text;

namespace SQLScriptHelper
{
	public class ScriptHelper
	{
		protected string sqlHeader;
		protected string sqlFooter;

		public ScriptHelper()
		{

		}

		/// <summary>
		/// Using the streams of data to generate a SQL script.
		/// </summary>
		public string ProduceSQL(IEnumerable<object> stream)
		{
			StringBuilder sql = new StringBuilder();
			sql.Append(sqlHeader);
			int count = 0;
			foreach (object obj in stream)
			{
				if (count > 0)
				{
					sql.Append(",");
				}
				sql.Append("(");
				AppendRowToStringBuilder(obj, sql);
				sql.Append(")");
				count++;
			}
			sql.Append(sqlFooter);
			return sql.ToString();
		}

		protected virtual void AppendRowToStringBuilder(object obj, StringBuilder sql)
		{
			sql.Append(obj);
		}
	}
}
