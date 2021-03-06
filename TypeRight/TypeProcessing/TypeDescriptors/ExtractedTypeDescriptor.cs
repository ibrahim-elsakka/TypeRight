﻿using TypeRight.CodeModel;

namespace TypeRight.TypeProcessing
{
	/// <summary>
	/// A descriptor for a type that is extracted
	/// </summary>
	public abstract class ExtractedTypeDescriptor : TypeDescriptor
	{
		/// <summary>
		/// Gets the named type
		/// </summary>
		public INamedType NamedType { get; }

		/// <summary>
		/// Gets the name of the type
		/// </summary>
		public string Name => NamedType.Name;

		/// <summary>
		/// Gets the namespace for the extracted type
		/// </summary>
		public string Namespace { get;  }

		/// <summary>
		/// Creates a new extracted type descriptor
		/// </summary>
		/// <param name="namedType">The named type</param>
		/// <param name="typeNamespace">The namespace</param>
		protected ExtractedTypeDescriptor(INamedType namedType, string typeNamespace) : base (namedType)
		{
			Namespace = typeNamespace;
			NamedType = namedType;
		}
	}
}
