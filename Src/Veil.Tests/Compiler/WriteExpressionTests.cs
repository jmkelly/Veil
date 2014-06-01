﻿using System.Collections.Generic;
using NUnit.Framework;
using Veil.Parser;

namespace Veil.Compiler
{
    [TestFixture]
    internal class WriteExpressionTests : CompilerTestBase
    {
        [SetCulture("en-US")]
        [TestCaseSource("TestCases")]
        public void Should_be_able_to_write_model_property<T>(T model, string expectedResult)
        {
            var template = SyntaxTreeNode.Block(SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.Property(model.GetType(), "Data")));
            var result = ExecuteTemplate(template, model);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [SetCulture("en-US")]
        [TestCaseSource("TestCases")]
        public void Should_be_able_to_write_model_field<T>(T model, string expectedResult)
        {
            var template = SyntaxTreeNode.Block(SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.Field(model.GetType(), "DataField")));
            var result = ExecuteTemplate(template, model);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [SetCulture("en-US")]
        [TestCaseSource("TestCases")]
        public void Should_be_able_to_write_from_sub_model<T>(T model, string expectedResult)
        {
            var template = SyntaxTreeNode.Block(SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.SubModel(SyntaxTreeNode.ExpressionNode.Property(model.GetType(), "Sub"), SyntaxTreeNode.ExpressionNode.Property(model.GetType().GetProperty("Sub").PropertyType, "SubData"))));
            var result = ExecuteTemplate(template, model);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Should_be_able_to_write_from_root_model()
        {
            var model = new { Text = "Hello!" };
            var template = SyntaxTreeNode.Block(
                SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.Property(model.GetType(), "Text", SyntaxTreeNode.ExpressionScope.RootModel))
            );
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Hello!"));
        }

        [Test]
        public void Should_be_able_to_write_from_parent_scope_model()
        {
            var model = new Model<string>
            {
                Name = "Root",
                SubModels = new[] { 
                    new SubModel<string> { Name = "1", Strings = new [] { "A", "B" } },
                    new SubModel<string> { Name = "2", Strings = new [] { "C", "D" } }
                }
            };
            var template = SyntaxTreeNode.Block(
                SyntaxTreeNode.Iterate(
                    SyntaxTreeNode.ExpressionNode.Property(model.GetType(), "SubModels", SyntaxTreeNode.ExpressionScope.RootModel),
                    SyntaxTreeNode.Block(
                        SyntaxTreeNode.Iterate(
                            SyntaxTreeNode.ExpressionNode.Property(model.Sub.GetType(), "Strings", SyntaxTreeNode.ExpressionScope.CurrentModelOnStack),
                            SyntaxTreeNode.Block(
                                SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.Self(typeof(string), SyntaxTreeNode.ExpressionScope.CurrentModelOnStack)),
                                SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.Property(model.Sub.GetType(), "Name", SyntaxTreeNode.ExpressionScope.ModelOfParentScope)),
                                SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.Property(model.GetType(), "Name", SyntaxTreeNode.ExpressionScope.RootModel))
                            )
                        )
                    )
                )
            );
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("A1RootB1RootC2RootD2Root"));
        }

        [Test]
        public void Should_apply_html_encoding_when_requested()
        {
            var model = new { Text = "<h1>Hello</h1>" };
            var template = SyntaxTreeNode.Block(
                SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.Property(model.GetType(), "Text", SyntaxTreeNode.ExpressionScope.RootModel), true)
            );
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("&lt;h1&gt;Hello&lt;/h1&gt;"));
        }

        [Test]
        public void Should_be_able_to_write_item_from_dictionary()
        {
            var model = new Dictionary<string, object>();
            model.Add("Name", "Joe");
            var template = SyntaxTreeNode.Block(
                SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.LateBound("Name"))
            );
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Joe"));
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

            public SubModel<T>[] SubModels { get; set; }

            public string Name { get; set; }
        }

        internal class SubModel<T>
        {
            public T SubData { get; set; }

            public string[] Strings { get; set; }

            public string Name { get; set; }
        }
    }
}