CREATE PROCEDURE [dbo].[usp_PlyQor_Data_DeleteKey]
@nvc_container nvarchar(20)
,@nvc_id nvarchar(50)

AS

BEGIN

DELETE
FROM [dbo].[tbl_PlyQor_Data]
WHERE [nvc_container] = @nvc_container
AND [nvc_id] = @nvc_id

END

