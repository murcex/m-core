using Murcex.Implements.DataTools.Extensions;
using Murcex.PlyQor.Internal.Container;
using Murcex.PlyQor.Internal.Container.Model;
using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.PlyQor.Internal.Portal.Pages.Container
{
	public class ContainerSelectPage
	{
		// Constants for query parameter names
		private const string ContainerParam = "container";

		// Constants for element keys
		private const string TokenKey = "Token";

		// Constants for dynamic elements
		private const string TokenElement = "dym-token";
		private const string ContainerNameElement = "dym-container";
		private const string RetentionValueElement = "dym-retention";
		private const string PrimaryTokenElement = "dym-primary-token";
		private const string SecondaryTokenElement = "dym-secondary-token";

		// Default values
		private const string EmptyValue = "";

		public static Dictionary<string, string> Execute(PageFuncData pageFuncData)
		{
			pageFuncData.KLog.Trace($"Executing Container Page Func");

			var dynamicElements = new Dictionary<string, string>();

			try
			{
				// Get navigation elements
				var token = pageFuncData.Auxiliary.GetValue(TokenKey);

				// Add navigation elements to response
				dynamicElements.Add(TokenElement, token);

				// Validate container name parameter
				if (!TryGetQueryParam(pageFuncData, ContainerParam, out string containerName))
				{
					pageFuncData.KLog.Error("Container name is not provided in the request query.");
					throw new ArgumentException("Container name is required in the request query.");
				}

				// Get container data
				PlyQorContainer container = GetContainerSafely(pageFuncData, containerName);

				// Extract container properties with validation
				var containerProperties = ExtractContainerProperties(container);

				// Add container properties to response
				foreach (var property in containerProperties)
				{
					dynamicElements.Add(property.Key, property.Value);
				}
			}
			catch (ArgumentException ex)
			{
				pageFuncData.KLog.Error($"Validation error: {ex.Message}");
				throw;
			}
			catch (Exception ex)
			{
				pageFuncData.KLog.Error($"Error retrieving container details: {ex.Message}");
				throw;
			}

			return dynamicElements;
		}

		/// <summary>
		/// Tries to get a query parameter value, returns false if not found
		/// </summary>
		private static bool TryGetQueryParam(PageFuncData pageFuncData, string paramName, out string value)
		{
			value = pageFuncData.Request.Query[paramName];
			return !string.IsNullOrEmpty(value);
		}

		/// <summary>
		/// Gets a container safely with error handling
		/// </summary>
		private static PlyQorContainer GetContainerSafely(PageFuncData pageFuncData, string containerName)
		{
			var container = PlyQorContainerManager.GetContainer(containerName);

			if (container == null)
			{
				pageFuncData.KLog.Error($"Container '{containerName}' not found.");
				throw new ArgumentException($"Container '{containerName}' not found.");
			}

			return container;
		}

		/// <summary>
		/// Extracts container properties with validation
		/// </summary>
		private static Dictionary<string, string> ExtractContainerProperties(PlyQorContainer container)
		{
			var properties = new Dictionary<string, string>();

			// Validate required properties
			if (string.IsNullOrEmpty(container.Name))
			{
				throw new ArgumentException("Container name is not found in the container data.");
			}

			// Add properties to dictionary
			properties.Add(ContainerNameElement, container.Name);
			properties.Add(RetentionValueElement, container.Retention.ToString());
			properties.Add(PrimaryTokenElement, container.PrimaryToken ?? string.Empty);
			properties.Add(SecondaryTokenElement, container.SecondaryToken ?? string.Empty);

			return properties;
		}
	}
}

