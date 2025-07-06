using Crane.Internal.Engine.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Crane.Internal.Test.Tests.CraneComponents
{
	[TestClass]
	public class TestRedactor
	{
		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void TestRedactor_Hit()
		{
			Dictionary<string, string> taskCfg = new()
			{
				{ "crane_redact", "testa1,testb2" }
			};

			Dictionary<string, string> testa = new()
			{
				{ "testa1", "a1key" },
				{ "testa2", "a2" }
			};

			Dictionary<string, string> testb = new()
			{
				{ "testb1", "b1" },
				{ "testb2", "b2key" }
			};

			Dictionary<string, Dictionary<string, string>> taskParameters = new()
			{
				{ "testa", testa },
				{ "testb", testb }
			};

			CraneRedactor redactor = new();

			var consoleCollection = redactor.Execute(taskCfg, taskParameters);

			consoleCollection.TryGetValue("testa", out var testaColsone);
			Assert.IsNotNull(testaColsone);
			Assert.AreEqual(2, testaColsone.Count);
			var a1 = testaColsone["testa1"];
			var a2 = testaColsone["testa2"];
			Assert.AreEqual("*redacted*", a1);
			Assert.AreEqual("a2", a2);

			consoleCollection.TryGetValue("testb", out var testbColsone);
			Assert.IsNotNull(testbColsone);
			Assert.AreEqual(2, testbColsone.Count);
			var b1 = testbColsone["testb1"];
			var b2 = testbColsone["testb2"];
			Assert.AreEqual("b1", b1);
			Assert.AreEqual("*redacted*", b2);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void TestRedactor_Miss_EmptyKey()
		{
			Dictionary<string, string> taskCfg = new();

			Dictionary<string, string> testa = new()
			{
				{ "testa1", "a1key" },
				{ "testa2", "a2" }
			};

			Dictionary<string, string> testb = new()
			{
				{ "testb1", "b1" },
				{ "testb2", "b2key" }
			};

			Dictionary<string, Dictionary<string, string>> taskParameters = new()
			{
				{ "testa", testa },
				{ "testb", testb }
			};

			CraneRedactor redactor = new();

			var consoleCollection = redactor.Execute(taskCfg, taskParameters);

			consoleCollection.TryGetValue("testa", out var testaColsone);
			Assert.IsNotNull(testaColsone);
			Assert.AreEqual(2, testaColsone.Count);
			var a1 = testaColsone["testa1"];
			var a2 = testaColsone["testa2"];
			Assert.AreEqual("a1key", a1);
			Assert.AreEqual("a2", a2);

			consoleCollection.TryGetValue("testb", out var testbColsone);
			Assert.IsNotNull(testbColsone);
			Assert.AreEqual(2, testbColsone.Count);
			var b1 = testbColsone["testb1"];
			var b2 = testbColsone["testb2"];
			Assert.AreEqual("b1", b1);
			Assert.AreEqual("b2key", b2);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void TestRedactor_Miss_EmptyValue()
		{
			Dictionary<string, string> taskCfg = new()
			{
				{ "crane_redact", string.Empty }
			};

			Dictionary<string, string> testa = new()
			{
				{ "testa1", "a1key" },
				{ "testa2", "a2" }
			};

			Dictionary<string, string> testb = new()
			{
				{ "testb1", "b1" },
				{ "testb2", "b2key" }
			};

			Dictionary<string, Dictionary<string, string>> taskParameters = new()
			{
				{ "testa", testa },
				{ "testb", testb }
			};

			CraneRedactor redactor = new();

			var consoleCollection = redactor.Execute(taskCfg, taskParameters);

			consoleCollection.TryGetValue("testa", out var testaColsone);
			Assert.IsNotNull(testaColsone);
			Assert.AreEqual(2, testaColsone.Count);
			var a1 = testaColsone["testa1"];
			var a2 = testaColsone["testa2"];
			Assert.AreEqual("a1key", a1);
			Assert.AreEqual("a2", a2);

			consoleCollection.TryGetValue("testb", out var testbColsone);
			Assert.IsNotNull(testbColsone);
			Assert.AreEqual(2, testbColsone.Count);
			var b1 = testbColsone["testb1"];
			var b2 = testbColsone["testb2"];
			Assert.AreEqual("b1", b1);
			Assert.AreEqual("b2key", b2);
		}
	}
}
