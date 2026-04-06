using Murcex.Vyudro.Module.Client.Interface;
using System;
using System.Collections.Generic;

namespace Murcex.Vyudro.Module.Client.StorageAdapters
{
    public class WebAdapter : IStorageAdapter
    {
        private readonly string _pageUrl;
        private readonly string _sessionUrl;
        private readonly string _replace;

        public WebAdapter(string pageUrl, string sessionUrl, string replace)
        {
            _pageUrl = pageUrl;
            _sessionUrl = sessionUrl;
            _replace = replace;
        }

        public bool GetPage(string page, out string contents, out string message)
        {
            // Minimal implementation: no remote calls. Return not found.
            contents = string.Empty;
            message = "WebAdapter.GetPage is not implemented.";
            return false;
        }

        public bool InsertSessionToken(string token, string timestamp, out string message)
        {
            message = "WebAdapter.InsertSessionToken is not implemented.";
            return false;
        }

        public bool GetSessionTokenTimestamp(string token, out string timestamp, out string message)
        {
            timestamp = string.Empty;
            message = "WebAdapter.GetSessionTokenTimestamp is not implemented.";
            return false;
        }

        public bool DeleteSessionToken(string token, out string message)
        {
            message = "WebAdapter.DeleteSessionToken is not implemented.";
            return false;
        }

        public bool GetUserAccess(string manifest, out Dictionary<string, List<string>> userAccess, out string message)
        {
            // Return empty access map so the manager can continue operating.
            userAccess = new Dictionary<string, List<string>>();
            message = "WebAdapter.GetUserAccess returned empty manifest (not implemented).";
            return true;
        }

        public bool CheckAccessToken(string manifest, string accessUser, string accessToken, out string message)
        {
            if (!GetUserAccess(manifest, out var userAccess, out message))
            {
                message = $"failed to load manifest: {message}";
                return false;
            }

            if (string.IsNullOrEmpty(accessUser))
            {
                message = "access user is empty";
                return false;
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                message = "access token is empty";
                return false;
            }

            if (userAccess.TryGetValue(accessUser, out var tokens))
            {
                if (tokens.Contains(accessToken))
                {
                    message = "access granted";
                    return true;
                }
                else
                {
                    message = "access token is incorrect";
                    return false;
                }
            }
            else
            {
                message = "access user is incorrect";
                return false;
            }
        }
    }
}
