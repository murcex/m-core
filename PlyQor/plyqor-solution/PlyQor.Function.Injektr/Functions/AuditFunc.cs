namespace PlyQor.Injektr.Functions
{
    using KirokuG2;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using PlyQor.Client;
    using PlyQor.Injektr.Core;
    using System;
    using System.Collections.Generic;

    public class AuditFunc
    {
        [Function("PlyQor-Audit")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, FunctionContext functionContext)
        {
            using (var klog = KManager.NewInstance(functionContext.FunctionDefinition.Name))
            {
                try
                {
                    // build client
                    PlyClient plyClient = new PlyClient(
                        Configuration.Url,
                        Configuration.Container,
                        Configuration.Token);

                    // setup
                    var key_1 = Guid.NewGuid().ToString();
                    var key_2 = Guid.NewGuid().ToString();
                    var key_3 = Guid.NewGuid().ToString();

                    var data_2 = Guid.NewGuid().ToString();

                    List<string> tags = new List<string>
                    {
                        "Update",
                        "V2Operation"
                    };

                    List<string> tags_2 = new List<string>
                    {
                        "UpdateThisTag1",
                        "DeleteThisTag2",
                        "DeleteThisTag3"
                    };

                    // insert
                    plyClient.Insert(key_1, Guid.NewGuid().ToString(), tags);

                    plyClient.Insert(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "TagOne");

                    plyClient.Insert(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "TagOne", "TagTwo");

                    plyClient.Insert(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "TagOne", "TagTwo", "TagThree");

                    plyClient.InsertTag(key_1, "TestTag");

                    plyClient.Insert(key_3, Guid.NewGuid().ToString(), tags_2);

                    // select
                    plyClient.Select(key_1);

                    plyClient.SelectTags();

                    plyClient.SelectCount("TagTwo");

                    plyClient.Select("TagOne", 3);

                    plyClient.SelectTags(key_1);

                    // update
                    plyClient.Update(key_1, key_2);

                    plyClient.UpdateData(key_2, data_2);

                    plyClient.UpdateTag(key_2, "TestTag", "TestTagV2");

                    plyClient.UpdateTag("UpdateThisTag1", "UpdateThisTag2");

                    // delete
                    plyClient.DeleteTag(key_2, "TestTagV2");

                    plyClient.Delete(key_2);

                    plyClient.DeleteTag("DeleteThisTag2");

                    plyClient.DeleteTags(key_3);
                }
                catch (Exception ex)
                {
                    klog.Error(ex.ToString());
                }
            }
        }
    }
}
