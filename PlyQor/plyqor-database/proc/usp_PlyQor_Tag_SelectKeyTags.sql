CREATE PROCEDURE [dbo].[usp_PlyQor_Tag_SelectKeyTags]

@nvc_container nvarchar(20)
,@nvc_id nvarchar(50)

AS

BEGIN

SELECT [nvc_data]
FROM [dbo].[tbl_PlyQor_Tag]
WHERE [nvc_container] = @nvc_container
AND [nvc_id] = @nvc_id

END

