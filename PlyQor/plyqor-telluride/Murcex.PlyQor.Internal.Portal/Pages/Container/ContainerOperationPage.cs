using Murcex.Implements.DataTools.Extensions;
using Murcex.PlyQor.Internal.Container;
using Murcex.PlyQor.Internal.Container.Model;
using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.PlyQor.Internal.Portal.Pages.Container;

/// <summary>
/// ExecuteOperationPage handles add, update, and delete operations for PlyQor containers
/// based on HTTP query parameters. It validates input, performs the requested operation,
/// and returns a dictionary of dynamic elements for UI feedback and navigation.
/// Supported operations: add, update, delete.
/// </summary>
public class ContainerOperationPage
{
	// Constants for query parameter names
	private const string OperationParam = "operation";
	private const string ContainerNameParam = "container";
	private const string RetentionValueParam = "retention";
	private const string PrimaryTokenParam = "primary-token";
	private const string SecondaryTokenParam = "secondary-token";
	private const string ConfirmParam = "confirm";

	// Constants for operation types
	private const string CreateOperation = "create";
	private const string UpdateOperation = "update";
	private const string DeleteOperation = "delete";
	private const string ConfirmValue = "delete";

	// Constants for dynamic elements returned to the UI
	private const string TokenElement = "dym-token";
	private const string MessageElement = "dym-message";
	private const string ResultElement = "dym-result";

	// Constants for element keys used in auxiliary data
	private const string TokenKey = "Token";

	/// <summary>
	/// Executes the requested container operation (add, update, delete) based on query parameters.
	/// Validates input, performs the operation, and returns dynamic elements for UI feedback.
	/// </summary>
	/// <param name="pageFuncData">Context data for the page function, including request and logging.</param>
	/// <returns>Dictionary of dynamic elements for UI rendering.</returns>
	public static Dictionary<string, string> Execute(PageFuncData pageFuncData)
	{
		pageFuncData.KLog.Trace($"Executing ExecuteOperation Page Func");

		var dynamicElements = new Dictionary<string, string>();

		// Get and validate operation type from query
		if (!TryGetQueryParam(pageFuncData, OperationParam, out string operation))
		{
			pageFuncData.KLog.Error("Operation type is not provided in the request query.");
			throw new ArgumentException("Operation type is required in the request query.");
		}

		// Validate that the operation type is supported
		if (!IsValidOperationType(operation))
		{
			pageFuncData.KLog.Error($"Unsupported operation type: {operation}");
			throw new NotSupportedException($"Operation type '{operation}' is not supported.");
		}

		var message = string.Empty;
		var result = false;

		try
		{
			// Handle add operation
			if (string.Equals(operation, CreateOperation, StringComparison.OrdinalIgnoreCase))
			{
				// Validate required parameters for add (excluding token parameters which can be empty)
				ValidateRequiredParams(pageFuncData, ContainerNameParam, RetentionValueParam);

				// Create container object from query parameters
				var container = CreateContainer(pageFuncData);

				// Add container using manager
				PlyQorContainerManager.AddContainer(container);

				message = $"Container '{container.Name}' added successfully.";
				result = true;
			}
			// Handle update operation
			else if (string.Equals(operation, UpdateOperation, StringComparison.OrdinalIgnoreCase))
			{
				// Validate required parameters for update (excluding token parameters which can be empty)
				ValidateRequiredParams(pageFuncData, ContainerNameParam, RetentionValueParam);

				// Create container object from query parameters
				var container = CreateContainer(pageFuncData);

				// Update container using manager
				PlyQorContainerManager.UpdateContainer(container);

				message = $"Container '{container.Name}' updated successfully.";
				result = true;
			}
			// Handle delete operation
			else if (string.Equals(operation, DeleteOperation, StringComparison.OrdinalIgnoreCase))
			{
				// Validate required parameters for delete
				ValidateRequiredParams(pageFuncData, ContainerNameParam, ConfirmParam);

				var confirmation = pageFuncData.Request.Query[ConfirmParam];

				// Check if deletion is confirmed
				if (string.Equals(confirmation, ConfirmValue, StringComparison.OrdinalIgnoreCase))
				{
					// Create minimal container object for deletion
					var container = new PlyQorContainer
					{
						Name = pageFuncData.Request.Query[ContainerNameParam]
					};

					// Delete container using manager
					PlyQorContainerManager.DeleteContainer(container);

					message = $"Container '{container.Name}' deleted successfully.";
					result = true;
				}
				else
				{
					message = "Container deletion was not confirmed.";
					result = false;
				}
			}
		}
		catch (FormatException ex)
		{
			// Handle invalid format for parameters
			pageFuncData.KLog.Error($"Invalid format for parameter: {ex.Message}");
			message = $"Error: Invalid format for parameter. Please check your input values.";
			result = false;
		}
		catch (Exception ex)
		{
			// Handle general errors
			pageFuncData.KLog.Error($"Error executing operation: {ex.Message}");
			message = $"Error: {ex.Message}";
			result = false;
		}

		// Prepare navigation and feedback elements for UI
		var token = pageFuncData.Auxiliary.GetValue(TokenKey);

		// Add all dynamic elements to the result dictionary
		dynamicElements.Add(TokenElement, token);
		dynamicElements.Add(MessageElement, message);
		dynamicElements.Add(ResultElement, result.ToString().ToLower());

		return dynamicElements;
	}

	/// <summary>
	/// Checks if the provided operation type is supported (add, update, delete).
	/// </summary>
	private static bool IsValidOperationType(string type)
	{
		return string.Equals(type, CreateOperation, StringComparison.OrdinalIgnoreCase) ||
			   string.Equals(type, UpdateOperation, StringComparison.OrdinalIgnoreCase) ||
			   string.Equals(type, DeleteOperation, StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// Attempts to retrieve a query parameter value from the request.
	/// Returns false if the parameter is missing or empty.
	/// </summary>
	private static bool TryGetQueryParam(PageFuncData pageFuncData, string paramName, out string value)
	{
		value = pageFuncData.Request.Query[paramName];
		return !string.IsNullOrEmpty(value);
	}

	/// <summary>
	/// Validates that all required query parameters are present in the request.
	/// Throws ArgumentException if any are missing.
	/// </summary>
	private static void ValidateRequiredParams(PageFuncData pageFuncData, params string[] paramNames)
	{
		foreach (var paramName in paramNames)
		{
			if (!TryGetQueryParam(pageFuncData, paramName, out _))
			{
				throw new ArgumentException($"Required parameter '{paramName}' is missing.");
			}
		}
	}

	/// <summary>
	/// Creates a PlyQorContainer object from query parameters.
	/// Validates and parses the retention value.
	/// Throws FormatException if retention value is invalid.
	/// Replaces string value "null" with empty string for token parameters.
	/// </summary>
	private static PlyQorContainer CreateContainer(PageFuncData pageFuncData)
	{
		// Try to parse retention value from query
		if (!int.TryParse(pageFuncData.Request.Query[RetentionValueParam], out int retention))
		{
			throw new FormatException($"Invalid retention value: {pageFuncData.Request.Query[RetentionValueParam]}");
		}

		// Get token values and replace "null" string with empty string
		string primaryToken = pageFuncData.Request.Query[PrimaryTokenParam];
		string secondaryToken = pageFuncData.Request.Query[SecondaryTokenParam];

		// Replace "null" string with empty string
		if (string.Equals(primaryToken, "null", StringComparison.OrdinalIgnoreCase))
		{
			primaryToken = string.Empty;
		}

		if (string.Equals(secondaryToken, "null", StringComparison.OrdinalIgnoreCase))
		{
			secondaryToken = string.Empty;
		}

		return new PlyQorContainer
		{
			Name = pageFuncData.Request.Query[ContainerNameParam],
			Retention = retention,
			PrimaryToken = primaryToken,
			SecondaryToken = secondaryToken
		};
	}
}
