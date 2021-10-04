using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SQLScriptHelper
{
	/// <summary>
	/// Build a function from the expression tree that parses an object to a string format according to the columns required and the data type of the attributes.
	/// </summary>
	public class RowExpressionBuilder<T> : IRowExpressionBuilder<T> where T : class
	{
		public RowExpressionBuilder()
		{

		}

		/// <summary>
		/// Build a function to parses an object to a string format and append the information to a StringBuilder.
		/// </summary>
		public Action<T, StringBuilder> Build(string[] columns)
		{
			var properties = typeof(T).GetProperties()
					.Where(prop => columns.Contains(prop.Name))
					.OrderBy(prop => Array.IndexOf(columns, prop.Name))
					.ToArray();

			if (properties.Length != columns.Length)
			{
				string[] missingColumns = columns.Where(col => properties.FirstOrDefault(prop => prop.Name == col) == null).ToArray();
				throw new Exception($"The columns for {typeof(T)} is not defined correctly! The columns {String.Join(",", missingColumns)} are missing!");
			}

			List<Expression> expressions = new List<Expression>();

			var obj = Expression.Parameter(typeof(T), "obj");
			var sql = Expression.Parameter(typeof(StringBuilder), "sql");
			var comma = Expression.Constant(',', typeof(char));
			var appendComma = typeof(StringBuilder).GetMethod("Append", new Type[] { typeof(char) });

			for (int i = 0; i < properties.Length; i++)
			{
				if (i > 0)
				{
					expressions.Add(Expression.Call(sql, appendComma));
				}
				var property = properties[i];
				var propertyType = property.PropertyType;
				var append = typeof(StringBuilder).GetMethod("Append", new Type[] { typeof(string) });
				var propertyExpression = typeof(T).GetProperty(property.Name);
				var propertyField = Expression.Property(obj, propertyExpression);

				expressions.Add(Expression.Call(sql, append, ConvertProperty(propertyType, propertyField)));
			}

			return Expression.Lambda<Action<T, StringBuilder>>(Expression.Block(expressions), obj, sql).Compile();
		}

		private Expression ConvertProperty(Type type, Expression expression)
		{
			if (type == typeof(int) ||
				type == typeof(double) ||
			    type == typeof(decimal) ||
				type == typeof(byte) ||
				type == typeof(long) ||
				type == typeof(short) ||
				type == typeof(char) )
            {
				return GetToStringExpression(type, expression);
			}

			if(type ==  typeof(int?) ||
				type == typeof(double?) ||
				type == typeof(decimal?) ||
				type == typeof(byte?) ||
				type == typeof(long?) ||
				type == typeof(short?))
            {
				return Expression.Condition(GetHasValueExpression(type, expression),
							GetToStringExpression(type, expression),
							Expression.Constant("NULL", typeof(string)));
			}

			if (type == typeof(bool))
			{
				return Expression.Condition(expression,
						Expression.Constant("1", typeof(string)),
						Expression.Constant("0", typeof(string)));
			}

			if (type == typeof(bool?))
			{
				return Expression.Condition(GetHasValueExpression(type, expression),
						ConvertProperty(typeof(bool), GetValueExpression(type, expression)),
						Expression.Constant("NULL", typeof(string)));
			}

			if (type == typeof(string))
			{
				var quote = Expression.Constant("'", typeof(string));
				var isNull = typeof(string).GetMethod(nameof(String.IsNullOrEmpty), new Type[] { typeof(string) });
				var concat = typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string), typeof(string) });
				return Expression.Condition(
						Expression.Call(isNull, expression),
						Expression.Constant("NULL", typeof(string)),
						Expression.Call(concat, quote, expression, quote));
			}

			if (type == typeof(DateTime))
			{
				var minValue = Expression.Constant(DateTime.MinValue, typeof(DateTime));
				var targetMinValue = Expression.Constant(new DateTime(1900, 1, 1), typeof(DateTime));
				expression = Expression.Condition(Expression.Equal(expression, minValue),
						targetMinValue,
						expression);
				var format = Expression.Constant("yyyy-MM-dd HH:mm:ss");
				var toString = typeof(DateTime).GetMethod("ToString", new Type[] { typeof(string) });
				return ConvertProperty(typeof(string),
						Expression.Call(expression, toString, format));
			}

			if (type == typeof(DateTime?))
			{
				return Expression.Condition(GetHasValueExpression(type, expression),
						ConvertProperty(typeof(DateTime), GetValueExpression(type, expression)),
						Expression.Constant("NULL", typeof(string)));
			}

			throw new Exception($"Do not know how to convert type {type.Name} to expression!");
		}

		private Expression GetValueExpression(Type type, Expression expression)
		{
			var value = type.GetProperty("Value");
			return Expression.Property(expression, value);
		}

		private Expression GetToStringExpression(Type type, Expression expression)
		{
			var toString = type.GetMethod("ToString", new Type[] { });
			return Expression.Call(expression, toString);
		}

		private Expression GetHasValueExpression(Type type, Expression expression)
		{
			var hasValue = type.GetProperty("HasValue");
			return Expression.Property(expression, hasValue);
		}
	}
}
