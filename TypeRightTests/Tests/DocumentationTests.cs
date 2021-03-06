﻿using System;
using TypeRightTests.HelperClasses;
using TypeRightTests.TestBuilders;
using TypeRightTests.Testers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TypeRightTests.Tests
{
	[TestClass]
	public class DocumentationTests
	{

		private PackageTester _packageTester;

		/// <summary>
		/// Sets up a parse of this solution
		/// </summary>
		[TestInitialize]
		public void SetupParse()
		{
			TestWorkspaceBuilder wkspBuilder = new TestWorkspaceBuilder();

			wkspBuilder.DefaultProject

				// Simple Generic Class with Type Param
				.CreateClassBuilder("SimpleClass")
					.SetDocumentationComments("These are comments")
					.AddProperty("SimpleProp", "int", "Property Comments Here")
					.Commit()				
				;

			wkspBuilder.ClassParseFilter = new AlwaysAcceptFilter();

			_packageTester = wkspBuilder.GetPackageTester();
		}

		[TestMethod]
		public void DocumentationTests_ClassHasComments()
		{
			_packageTester.TestReferenceTypeWithName("SimpleClass")
				.CommentsAre("These are comments");
		}

		[TestMethod]
		public void DocumentationTests_PropertyHasComments()
		{
			_packageTester.TestReferenceTypeWithName("SimpleClass")
				.TestPropertyWithName("SimpleProp")
				.CommentsAre("Property Comments Here");
		}
	}
}
