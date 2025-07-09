using Crane.Internal.Engine.Components;
using Crane.Internal.Engine.Interface;
using Crane.Internal.Test.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Crane.Internal.Test.Tests.CraneComponents
{
	[TestClass]
	public class TestConfirm
	{
		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void Confirm_EmptyKey()
		{
			Dictionary<string, string> craneCfg = new();
			Dictionary<string, Dictionary<string, string>> cfg = new()
			{
				{ "crane", craneCfg }
			};

			List<string> logs = new();
			ICraneLogger logger = new TestLogger(logs);

			CraneFileManager fileManager = new();

			var confirm = fileManager.CheckForConformation(logger, cfg);

			Assert.IsTrue(confirm);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void Confirm_EmptyValue()
		{
			Dictionary<string, string> craneCfg = new()
			{
				{ "confirm", string.Empty }
			};
			Dictionary<string, Dictionary<string, string>> cfg = new()
			{
				{ "crane", craneCfg }
			};

			List<string> logs = new();
			ICraneLogger logger = new TestLogger(logs);

			CraneFileManager fileManager = new();

			var confirm = fileManager.CheckForConformation(logger, cfg);

			Assert.IsTrue(confirm);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void Confirm_Yes()
		{
			Dictionary<string, string> craneCfg = new()
			{
				{ "confirm", "true" }
			};
			Dictionary<string, Dictionary<string, string>> cfg = new()
			{
				{ "crane", craneCfg }
			};

			List<string> logs = new();
			ICraneLogger logger = new TestLogger(logs);

			CraneFileManager fileManager = new();

			var confirm = fileManager.CheckForConformation(logger, cfg);

			Assert.IsTrue(confirm);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void Confirm_No()
		{
			Dictionary<string, string> craneCfg = new()
			{
				{ "confirm", "false" }
			};
			Dictionary<string, Dictionary<string, string>> cfg = new()
			{
				{ "crane", craneCfg }
			};

			List<string> logs = new();
			ICraneLogger logger = new TestLogger(logs);

			CraneFileManager fileManager = new();

			var confirm = fileManager.CheckForConformation(logger, cfg);

			Assert.IsFalse(confirm);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void Confirm_Unknown()
		{
			Dictionary<string, string> craneCfg = new()
			{
				{ "confirm", Guid.NewGuid().ToString() }
			};
			Dictionary<string, Dictionary<string, string>> cfg = new()
			{
				{ "crane", craneCfg }
			};

			List<string> logs = new();
			ICraneLogger logger = new TestLogger(logs);

			CraneFileManager fileManager = new();

			var confirm = fileManager.CheckForConformation(logger, cfg);

			Assert.IsTrue(confirm);
		}
	}
}
