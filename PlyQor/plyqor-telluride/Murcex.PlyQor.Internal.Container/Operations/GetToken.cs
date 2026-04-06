namespace Murcex.PlyQor.Internal.Container.Operations
{
	public class GetToken
	{
		public static string Execute(string container)
		{
			var plyQorContainer = GetContainer.Execute(container);

			if (plyQorContainer != null)
			{
				if (!string.IsNullOrEmpty(plyQorContainer.PrimaryToken))
				{
					return plyQorContainer.PrimaryToken;
				}
				else
				{
					if (!string.IsNullOrEmpty(plyQorContainer.SecondaryToken))
					{
						return plyQorContainer.SecondaryToken;
					}
				}
			}

			return string.Empty; // if PlyQorContainer is null or no tokens are found, return an empty string
		}
	}
}
