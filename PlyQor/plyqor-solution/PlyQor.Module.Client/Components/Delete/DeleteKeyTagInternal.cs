﻿namespace PlyQor.Client
{
    using PlyQor.Client.Resources;

    class DeleteKeyTagInternal
    {
        public static Dictionary<string, string> Execute(
            string uri,
            string container,
            string token,
            string key,
            string tag)
        {
            Dictionary<string, string> request = new Dictionary<string, string>
            {
                { RequestKeys.Token, token },
                { RequestKeys.Container, container },
                { RequestKeys.Operation, QueryOperation.DeleteKeyTag },
                { RequestKeys.Key, key },
                { RequestKeys.Tag, tag }
            };

            return Transmitter.Execute(uri, request);
        }
    }
}
