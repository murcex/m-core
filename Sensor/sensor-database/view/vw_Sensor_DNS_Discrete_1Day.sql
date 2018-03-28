CREATE VIEW [dbo].[vw_Sensor_DNS_Discrete_1Day]

AS

SELECT
[dt_session] AS [Session]
,[nvc_dns] AS [DNS]
,[nvc_datacentertag] AS [Data Center Tag]
,COUNT(*) AS [Count]
FROM [dbo].[tbl_Sensor_DNS_Stage]
WHERE [dt_session] > DATEADD(day,-1,GETDATE())
GROUP BY [dt_session], [nvc_dns], [nvc_datacentertag]