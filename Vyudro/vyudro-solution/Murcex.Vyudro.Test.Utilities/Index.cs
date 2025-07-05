using Murcex.Vyudro.Internal.Utilities.Extensions;

namespace Murcex.Vyudro.Test.Utilities
{
	[TestClass]
	public sealed class Index
	{
		[TestMethod]
		public void DictionaryExtTest()
		{
			var dict = new Dictionary<string, string>
			{
				{ "a", "1" },
				{ "b", string.Empty }
			};

			var test1 = dict.GetValue("a");
			Assert.AreEqual("1", test1);

			var test2 = dict.GetValue("x");
			Assert.AreEqual(string.Empty, test2);

			var test3 = dict.GetValue("b");
			Assert.AreEqual(string.Empty, test3);

			var exception = false;
			try
			{
				var test4 = dict.GetValue("x", keyEx: "key not found");
			}
			catch (Exception ex)
			{
				if (ex.Message.Contains("key not found"))
				{
					exception = true;
				}
			}

			Assert.AreEqual(true, exception);

			exception = false;
			try
			{
				var test5 = dict.GetValue("b", valueEx: "value is null");
			}
			catch (Exception ex)
			{
				if (ex.Message.Contains("value is null"))
				{
					exception = true;
				}
			}

			Assert.AreEqual(true, exception);
		}

		[TestMethod]
		public void DictionaryDictionaryExtTest()
		{
			var subdict = new Dictionary<string, string>();

			var dict = new Dictionary<string, Dictionary<string, string>>();
			dict.Add("a", subdict);
			dict.Add("b", null);

			var test1 = dict.GetValue("a");
			Assert.IsNotNull(test1);

			var test2 = dict.GetValue("x");
			Assert.IsNull(test2);

			var test3 = dict.GetValue("b");
			Assert.IsNull(test3);

			var exception = false;
			try
			{
				var test4 = dict.GetValue("x", keyEx: "key not found");
			}
			catch (Exception ex)
			{
				if (ex.Message.Contains("key not found"))
				{
					exception = true;
				}
			}

			Assert.AreEqual(true, exception);

			exception = false;
			try
			{
				var test5 = dict.GetValue("b", valueEx: "value is null");
			}
			catch (Exception ex)
			{
				if (ex.Message.Contains("value is null"))
				{
					exception = true;
				}
			}

			Assert.AreEqual(true, exception);
		}

		[TestMethod]
		public void ToListExtTest()
		{
			var testStr = "a,b,c,d";

			var test1 = testStr.ToList();

			Assert.IsTrue(test1 != null);
			Assert.IsTrue(test1.Count == 4);
		}
	}
}
