﻿namespace TypeRight.CodeModel
{
	/// <summary>
	/// Represents a parameter for a method
	/// </summary>
	public interface IMethodParameter
	{
		/// <summary>
		/// Gets the name of the parameter
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the comments for this parameter
		/// </summary>
		string Comments { get; }

		/// <summary>
		/// Gets the type of the parameter
		/// </summary>
		IType ParameterType { get; }
	}
}
