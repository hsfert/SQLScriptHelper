using FluentAssertions;
using NSubstitute.ExceptionExtensions;
using SQLScriptHelper.Tests.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SQLScriptHelper.Tests
{
    public class RowExpressionBuilderTests
    {
        private readonly RowExpressionBuilder<Person> _sut;

        public RowExpressionBuilderTests()
        {
            _sut = new RowExpressionBuilder<Person>();
        }

        [Fact]
        public void Build_ShouldBreak_WhenInvalidColumns()
        {
            string[] columnNamesWithAddress = new string[]
            {
                "DateOfBirth",
                "Address",
                "Age",
                "PartnerAddress"
            };

            Func<Action<Person, StringBuilder>> action = () => _sut.Build(columnNamesWithAddress); 

            action.Should().Throws<Exception>();

        }
    }
}
