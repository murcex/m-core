﻿namespace PlyQor.Audit.TestCases.StorageProvider
{
    using System;
    using PlyQor.Engine.Components.Storage;
    using PlyQor.Audit.Core;

    class UpdateTag
    {
        public static bool Execute()
        {
            Console.WriteLine("// Update Index Set");

            var indexes = StorageProvider.SelectTags(Configuration.Collection);

            string targetIndex = null;
            string checkIndex = null;
            string checkForStage = "Stage";

            foreach (var index in indexes)
            {
                if (index.Contains(checkForStage.ToUpper()))
                {
                    targetIndex = index;

                    break;
                }
            }

            StorageProvider.UpdateTag(Configuration.Collection, targetIndex, "ARCHIVE");

            var indexes2 = StorageProvider.SelectTags(Configuration.Collection);

            bool NoHit = false;

            foreach (var index in indexes2)
            {
                if (index.Contains("ARCHIVE"))
                {
                    checkIndex = index;
                }
                
                if (Equals(index,targetIndex))
                {
                    NoHit = true;
                }
            }

            Console.WriteLine($"Updated {targetIndex} to Archive");
            Console.WriteLine($"Index Update (True): {Equals("ARCHIVE", checkIndex)}");
            Console.WriteLine($"Old Index has been updated (True): {Equals(NoHit, false)}");
            Console.WriteLine($"");

            return true;
        }
    }
}
