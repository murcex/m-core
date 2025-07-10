namespace Murcex.PlyQor.Internal.Container.Model
{
	public class PlyQorContainer
	{
		public string Name { get; set; } = string.Empty;

		public int Retention { get; set; } = 0;

		public string PrimaryToken { get; set; } = string.Empty;

		public string SecondaryToken { get; set; } = string.Empty;
	}
}
