using Murcex.Implements.DataTools.Extensions;

namespace Murcex.Implements.DataTools.Extensions
{
	public static class StringExtension
	{
		public static List<string> ToList(this string input, string delimiter = ",", string exMsg = null)
		{
			try
			{
				return input.Split(delimiter).ToList();
			}
			catch (Exception ex)
			{
				if (string.IsNullOrEmpty(exMsg))
				{
					return null;
				}
				else
				{
					throw new Exception(exMsg);
				}
			}
		}
	}
}
