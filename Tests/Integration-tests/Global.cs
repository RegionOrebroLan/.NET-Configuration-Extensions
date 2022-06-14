using System;
using System.IO;

namespace IntegrationTests
{
	public static class Global
	{
		#region Fields

		// ReSharper disable PossibleNullReferenceException
		public static readonly string ProjectPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
		// ReSharper restore PossibleNullReferenceException

		#endregion
	}
}