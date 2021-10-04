using System.Text;

namespace SQLScriptHelper
{
	/// <summary>
	/// Given the column names and the table names, write the backbone of the merge statement
	/// </summary>
	internal class Merge
	{
		internal static string GetSQLHeader(string table_name,
				string[] updateColumns,
				string[] outputColumns)
		{
			bool hasTemp = (updateColumns == null || updateColumns.Length == 0) &&
					outputColumns != null && outputColumns.Length > 0;
			string header = "MERGE " + table_name + " WITH(SERIALIZABLE) AS T USING(VALUES";
			if (hasTemp)
			{
				header = "DECLARE @temp int;" + header;
			}
			return header;
		}

		internal static string GetSQLFooter(string[] columns,
				string[] matchingColumns,
				string[] inequalityColumns,
				string[] updateColumns,
				string[] insertColumns,
				string[] outputColumns)
		{
			StringBuilder sql = new StringBuilder();
			sql.Append(") AS U(");
			for (int i = 0; i < columns.Length; i++)
			{
				if (i > 0)
				{
					sql.Append(",");
				}
				sql.Append(columns[i]);
			}
			sql.Append(") ON ");
			int count = 0;
			if (matchingColumns != null && matchingColumns.Length > 0)
			{
				for (int i = 0; i < matchingColumns.Length; i++)
				{
					if (count > 0)
					{
						sql.Append(" and ");
					}
					string matchingColumn = matchingColumns[i];
					sql.Append("U.");
					sql.Append(matchingColumn);
					sql.Append("=T.");
					sql.Append(matchingColumn);
					count++;
				}
			}
			if (inequalityColumns != null && inequalityColumns.Length > 0)
			{
				for (int i = 0; i < inequalityColumns.Length; i++)
				{
					if (count > 0)
					{
						sql.Append(" and ");
					}
					string inequalityColumn = inequalityColumns[i];
					sql.Append("U.");
					sql.Append(inequalityColumn);
					sql.Append("<T.");
					sql.Append(inequalityColumn);
					count++;
				}
			}
			if (updateColumns != null && updateColumns.Length > 0)
			{
				sql.Append(" WHEN MATCHED THEN UPDATE SET ");
				for (int i = 0; i < updateColumns.Length; i++)
				{
					if (i > 0)
					{
						sql.Append(",");
					}
					string updateColumn = updateColumns[i];
					sql.Append("T.");
					sql.Append(updateColumn);
					sql.Append("=U.");
					sql.Append(updateColumn);
				}
			}
			else if (outputColumns != null && outputColumns.Length > 0)
			{
				sql.Append(" WHEN MATCHED THEN UPDATE SET @temp=1");
			}
			if (insertColumns != null && insertColumns.Length > 0)
			{
				sql.Append(" WHEN NOT MATCHED THEN INSERT(");
				for (int i = 0; i < insertColumns.Length; i++)
				{
					if (i > 0)
					{
						sql.Append(",");
					}
					sql.Append(insertColumns[i]);
				}
				sql.Append(") VALUES(");
				for (int i = 0; i < insertColumns.Length; i++)
				{
					if (i > 0)
					{
						sql.Append(",");
					}
					sql.Append("U." + insertColumns[i]);
				}
				sql.Append(")");
			}
			if (outputColumns != null && outputColumns.Length > 0)
			{
				sql.Append(" OUTPUT ");
				for (int i = 0; i < outputColumns.Length; i++)
				{
					if (i > 0)
					{
						sql.Append(",");
					}
					sql.Append("inserted." + outputColumns[i]);
				}
			}
			sql.Append(";");
			return sql.ToString();
		}
	}
}
