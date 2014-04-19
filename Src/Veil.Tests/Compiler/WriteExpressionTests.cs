﻿using NUnit.Framework;

namespace Veil.Compiler
{
    [TestFixture]
    internal class WriteExpressionTests : CompilerTestBase
    {
        [SetCulture("en-US")]
        [TestCaseSource("TestCases")]
        public void Should_be_able_to_write_model_property<T>(T model, string expectedResult)
        {
            var template = SyntaxTreeNode.Block(SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.ModelProperty(model.GetType(), "Data")));
            var result = ExecuteTemplate(template, model);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [SetCulture("en-US")]
        [TestCaseSource("TestCases")]
        public void Should_be_able_to_write_model_field<T>(T model, string expectedResult)
        {
            var template = SyntaxTreeNode.Block(SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.ModelField(model.GetType(), "DataField")));
            var result = ExecuteTemplate(template, model);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [SetCulture("en-US")]
        [TestCaseSource("TestCases")]
        public void Should_be_able_to_write_from_sub_model<T>(T model, string expectedResult)
        {
            var template = SyntaxTreeNode.Block(SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.ModelSubModel(SyntaxTreeNode.ExpressionNode.ModelProperty(model.GetType(), "Sub"), SyntaxTreeNode.ExpressionNode.ModelProperty(model.GetType().GetProperty("Sub").PropertyType, "SubData"))));
            var result = ExecuteTemplate(template, model);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Should_be_able_to_write_from_root_model()
        {
            var model = new { Text = "Hello!" };
            var template = SyntaxTreeNode.Block(
                SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.ModelProperty(model.GetType(), "Text", SyntaxTreeNode.ExpressionScope.RootModel))
            );
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Hello!"));
        }

        public object[] TestCases()
        {
            return new object[] {
                new object[] { new Model<string> { DataField = "World" }, "World" },
                new object[] { new Model<int> { DataField = 123 }, "123" },
                new object[] { new Model<double> { DataField = 1.54 }, "1.54" },
                new object[] { new Model<float> { DataField = 1.1F }, "1.1" },
                new object[] { new Model<long> { DataField = 1234L }, "1234" },
                new object[] { new Model<uint> { DataField = 12U }, "12" },
                new object[] { new Model<ulong> { DataField = 12345UL }, "12345" }
            };
        }

        internal class Model<T>
        {
            public T Data { get { return DataField; } }

            public T DataField;

            public SubModel<T> Sub { get { return new SubModel<T> { SubData = DataField }; } }
        }

        internal class SubModel<T>
        {
            public T SubData { get; set; }
        }
    }
}