﻿namespace Implements
{
    using System;
    using System.Diagnostics;

    public class Utility
    {
        /// <summary>
        /// Utility to convert string to int.
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static int ConvertValueToInt(string inputValue)
        {
            int outputValue;

            try
            {
                outputValue = Int32.Parse(inputValue);
            }
            catch
            {
                outputValue = 0;
            }

            return outputValue;
        }

        /// <summary>
        /// Utility to convert string to bool.
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static bool ConvertValueToBool(string inputValue)
        {
            bool outputValue;

            if (inputValue == "1")
            {
                outputValue = true;
            }
            else
            {
                outputValue = false;
            }

            return outputValue;
        }

        public static string GetAssemblyVersion(string dllName)
        {
            string version;

            try
            {
                version = FileVersionInfo.GetVersionInfo(dllName + ".dll").FileVersion;
            }
            catch
            {
                version = null;
            }

            return version;
        }
    }
}