﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeRightTests.TestBuilders
{
	partial class TestClassBuilder
	{
		private TestProjectBuilder _parentBuilder;

		private string _className;

		private string _comments = "";

		private string _baseClass = "";

		private List<string> _genericParameters = new List<string>();

		private List<SymbolInfo> _properties = new List<SymbolInfo>();

		public List<MethodInfo> Methods { get; set; } = new List<MethodInfo>();

		public TestClassBuilder(TestProjectBuilder projBuilder, string className)
		{
			_parentBuilder = projBuilder;
			_className = className;
		}

		public TestClassBuilder SetDocumentationComments(string comments)
		{
			_comments = comments;
			return this;
		}

		public TestClassBuilder AddGenericParameter(string name)
		{
			_genericParameters.Add(name);
			return this;
		}

		public TestClassBuilder AddBaseClass(string baseClassName)
		{
			_baseClass = baseClassName;
			return this;
		}

		public TestClassBuilder AddProperty(string name, string type, string comments = "")
		{
			_properties.Add(new SymbolInfo() { Name = name, Type = type, Comments = comments });
			return this;
		}

		public TestMethodBuilder AddConstructor()
		{
			return AddMethod(_className, "");
		}

		public TestMethodBuilder AddMethod(string name, string type)
		{
			return new TestMethodBuilder(this, name, type);
		}

		public TestProjectBuilder Commit()
		{
			string text = TransformText();
			SourceText sourceText = SourceText.From(text);
			_parentBuilder.Workspace.AddDocument(_parentBuilder.ProjectID, _className + ".cs", sourceText);
			return _parentBuilder;
		}

		private	string GetBaseClass()
		{
			return string.IsNullOrEmpty(_baseClass) ? "" : $": {_baseClass} ";
		}

		private string GetGenericParams()
		{
			return _genericParameters.Count == 0 ? "" : ($"<{string.Join(", ", _genericParameters)}>");
		}

	}
}
