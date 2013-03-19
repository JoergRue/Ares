using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ares.GlobalDbUploadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("Call with base directory as parameter.");
                return;
            }
            int count = 0; int errors = 0;
            count += Process(args[0], ref errors);
            System.Console.WriteLine("Processed " + count + " files with " + errors + " errors.");
        }

        private static String LOCAL_DB = "Local.sqlite";
        private static String GLOBAL_DB = "Global.sqlite";
        private static String TEMP_DB = "Temp.sqlite";
        private static String REFERENCE = "Reference.txt";
        private static String RESULTS = "Results.txt";

        private static String USER_ID = "UploadTester";

        private static int Process(String dir, ref int errors)
        {
            int count = 0;
            String[] subDirs = System.IO.Directory.GetDirectories(dir);
            foreach (String subDir in subDirs)
            {
                count += Process(subDir, ref errors);
            }

            String localDbName = System.IO.Path.Combine(dir, LOCAL_DB);
            if (!System.IO.File.Exists(localDbName))
                return count;

            String globalDbName = System.IO.Path.Combine(dir, GLOBAL_DB);
            if (!System.IO.File.Exists(globalDbName))
            {
                System.Console.WriteLine("WARNING: global db file " + globalDbName + " not found.");
                return count;
            }

            String referenceName = System.IO.Path.Combine(dir, REFERENCE);
            bool referenceExists = System.IO.File.Exists(referenceName);
            String logName = System.IO.Path.Combine(dir, RESULTS);
            String tempName = System.IO.Path.Combine(dir, TEMP_DB);

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(referenceExists ? logName : referenceName))
            {
                try
                {
                    ++count;
                    System.IO.File.Copy(globalDbName, tempName, true);
                    try
                    {
                        var fileInterface = Ares.Tags.TagsModule.GetTagsDB().FilesInterface;
                        fileInterface.OpenOrCreateDatabase(localDbName);
                        System.Console.WriteLine("Start test " + dir);
                        try
                        {
                            var browseInterface = Ares.Tags.TagsModule.GetTagsDB().BrowseInterface;
                            var files = browseInterface.GetAllFiles();
                            var readInterface = Ares.Tags.TagsModule.GetTagsDB().ReadInterface;
                            List<int> fileIds = new List<int>();
                            foreach (var file in files)
                            {
                                fileIds.Add(file.Id);
                            }

                            var globalDb = Ares.Tags.TagsModule.GetNewTagsDB();
                            globalDb.FilesInterface.OpenOrCreateDatabase(tempName);
                            try
                            {
                                var exportedData = fileInterface.ExportDatabaseForGlobalDB(fileIds);

                                int newFiles = 0; int newTags = 0;
                                globalDb.GlobalDBInterface.ImportDataFromClient(exportedData, USER_ID, writer, out newFiles, out newTags);
                                globalDb.FilesInterface.CloseDatabase();
                                writer.WriteLine("New files: " + newFiles + "; new tags: " + newTags);
                                writer.Flush();
                                writer.Close();
                                System.Console.WriteLine("Upload to global DB finished.");
                                if (referenceExists)
                                {
                                    if (!CompareFiles(logName, referenceName))
                                    {
                                        ++errors;
                                    }
                                }
                                else
                                {
                                    System.Console.WriteLine("CREATED reference " + referenceName);
                                }
                            }
                            finally
                            {
                                try
                                {
                                    globalDb.FilesInterface.CloseDatabase();
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                        finally
                        {
                            try
                            {
                                fileInterface.CloseDatabase();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    finally
                    {
                        try
                        {
                            System.IO.File.Delete(tempName);
                        }
                        catch (System.IO.IOException)
                        {
                            System.Console.WriteLine("WARNING: Could not delete temp DB, Resource leak?");
                        }
                    }
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    writer.WriteLine("Exception occured: " + ex.GetType() + "; " + ex.Message);
                    ++errors;
                }
                catch (Exception ex)
                {
                    writer.WriteLine("Exception occured: " + ex.GetType() + "; " + ex.Message);
                    ++errors;
                }
            }


            System.Console.WriteLine("---------------------------------------------------");
            return count;
        }

        private static bool CompareFiles(String file1, String file2)
        {
            using (System.IO.StreamReader reader1 = new System.IO.StreamReader(file1))
            {
                using (System.IO.StreamReader reader2 = new System.IO.StreamReader(file2))
                {
                    int lineNr = 0;
                    while (true)
                    {
                        String line1 = reader1.ReadLine();
                        String line2 = reader2.ReadLine();
                        ++lineNr;
                        if (line1 == null)
                        {
                            if (line2 != null)
                            {
                                System.Console.WriteLine("ERROR: Result file shorter then Reference file.");
                                return false;
                            }
                            else
                            {
                                System.Console.WriteLine("Result file is equal to Reference file.");
                                return true;
                            }
                        }
                        else if (line2 == null)
                        {
                            System.Console.WriteLine("ERROR: Reference file shorter then Result file.");
                            return false;
                        }
                        else if (line1 != line2)
                        {
                            System.Console.WriteLine("ERROR: Reference file different to Result file in line " + lineNr);
                            return false;
                        }
                    }
                }
            }
        }
    }
}
