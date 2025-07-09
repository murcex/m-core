using Crane.Internal.Test.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Crane.Internal.Test.Tests.CraneComponents
{
	[TestClass]
	public class TestComponentsAsFailures
	{
		string craneTestDir = Path.Combine(Setup.CraneTestRoot, "failure");

		[TestInitialize]
		public void Execute()
		{
			Setup.Basic();
		}

		[TestMethod]
		public void Failure_1()
		{

		}

	}
}
