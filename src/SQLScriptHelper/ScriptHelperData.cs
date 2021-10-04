namespace SQLScriptHelper
{
	public class ScriptHelperData
	{
		/// <summary>
		/// All the column names which will be used in the script.
		/// </summary>
		public readonly string[] Columns;
		/// <summary>
		/// Output column names in the merge statement, can be empty if nothing needs to be outputed.
		/// </summary>
		public readonly string[] OutputColumns;
		/// <summary>
		/// Matching column names in the merge statement, which will be used to find the corresponding rows. 
		/// </summary>
		public readonly string[] MatchingColumns;
		/// <summary>
		/// Matching column names in the merge statament, but instead of finding equal values, we want to find the values in the database that is larger than the provided values.
		/// </summary>
		public readonly string[] InequalityColumns;
		/// <summary>
		/// Columns that needs to be updated if we find matching rows.
		/// </summary>
		public readonly string[] UpdateColumns;
		/// <summary>
		/// Columns that needs to be inserted if we cannot find matching rows.
		/// </summary>
		public readonly string[] InsertColumns;

		public ScriptHelperData(string[] columns,
				string[] outputColumns,
				string[] matchingColumns,
				string[] inequalityColumns,
				string[] updateColumns,
				string[] insertColumns)
		{
			Columns = columns;
			OutputColumns = outputColumns;
			MatchingColumns = matchingColumns;
			InequalityColumns = inequalityColumns;
			UpdateColumns = updateColumns;
			InsertColumns = insertColumns;
		}
	}
}
