using System;
using System.Text;

namespace SQLScriptHelper
{
    public interface IRowExpressionBuilder<T> where T : class
    {
        Action<T, StringBuilder> Build(string[] columns);
    }
}
