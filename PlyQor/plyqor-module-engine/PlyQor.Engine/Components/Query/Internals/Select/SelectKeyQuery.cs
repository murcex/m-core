﻿namespace PlyQor.Engine.Components.Query.Internals
{
    using System.Collections.Generic;
    using PlyQor.Models;
    using PlyQor.Resources;
    using PlyQor.Engine.Components.Storage;

    class SelectKeyQuery
    {
        public static Dictionary<string, string> Execute(RequestManager requestManager)
        {
            ResultManager resultManager = new ResultManager();

            // get values from request
            var container = requestManager.GetRequestStringValue(RequestKeys.Container);
            var key = requestManager.GetRequestStringValue(RequestKeys.Key);

            // execute internal query
            var data = StorageProvider.SelectKey(container, key);

            // build result
            resultManager.AddResultData(data);
            resultManager.AddResultSuccess();

            return resultManager.ExportDataSet();
        }
    }
}
