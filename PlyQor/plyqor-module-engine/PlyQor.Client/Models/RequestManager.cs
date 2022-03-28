﻿namespace PlyQor.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using PlyQor.Client.Resources;

    public class RequestManager : IDisposable
    {
        private bool dispose = false;

        Dictionary<string, string> _request;

        public RequestManager(Dictionary<string, string> request)
        {
            if (request == null)
            {
                throw new JavelinException(StatusCode.ERR001);
            }

            if (request.Count == 0)
            {
                throw new JavelinException(StatusCode.ERR002);
            }

            _request = request;
        }

        public string GetRequestStringValue(string key)
        {
            if (_request.TryGetValue(key.ToString(), out string result))
            {
                if (string.IsNullOrEmpty(result) || string.IsNullOrWhiteSpace(result))
                {
                    throw new JavelinException($"{StatusCode.ERR003},KEY={key}");
                }

                return result;
            }
            else
            {
                throw new JavelinException($"{StatusCode.ERR004},KEY={key}");
            }
        }

        public int GetRequestIntValue(string key, bool positive = true)
        {
            if (_request.TryGetValue(key.ToString(), out string result))
            {
                if (int.TryParse(result, out int testValue))
                {
                    if (positive)
                    {
                        if (testValue > 0)
                        {
                            return testValue;
                        }
                        else
                        {
                            throw new JavelinException($"{StatusCode.ERR005},KEY={key}");
                        }
                    }

                    return testValue;
                }
                else
                {
                    throw new JavelinException($"{StatusCode.ERR006},KEY={key}");
                }
            }
            else
            {
                throw new JavelinException($"{StatusCode.ERR004},KEY={key}");
            }
        }

        public List<string> GetRequestTags()
        {
            if (_request.TryGetValue(RequestKeys.Tags, out string result))
            {
                List<string> tags = new List<string>();

                try
                {
                    tags = JsonConvert.DeserializeObject<List<string>>(result);
                }
                catch
                {
                    throw new JavelinException(StatusCode.ERR008);
                }

                foreach (var tag in tags)
                {
                    if (string.IsNullOrEmpty(result) || string.IsNullOrWhiteSpace(result))
                    {
                        throw new JavelinException(StatusCode.ERR003);
                    }
                }

                return tags;
            }
            else
            {
                throw new JavelinException(StatusCode.ERR004);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!dispose)
            {
                if (disposing)
                {
                    //dispose managed resources
                }
            }

            //dispose unmanaged resources
            dispose = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
