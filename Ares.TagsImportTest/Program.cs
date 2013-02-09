using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ares.TagsImportTest
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

        private static int Process(String dir, ref int errors)
        {
            int count = 0;
            String[] subDirs = System.IO.Directory.GetDirectories(dir);
            foreach (String subDir in subDirs)
            {
                count += Process(subDir, ref errors);
            }
            String[] files = System.IO.Directory.GetFiles(dir, "*.sqlite");
            foreach (String file in files)
            {
                String name = System.IO.Path.GetFileNameWithoutExtension(file);
                String importName = System.IO.Path.Combine(dir, name);
                importName += ".jsv";
                if (!System.IO.File.Exists(importName))
                {
                    System.Console.WriteLine("WARNING: import file " + importName + " not found.");
                    continue;
                }
                String referenceName = System.IO.Path.Combine(dir, name);
                referenceName += ".reference";
                bool referenceExists = System.IO.File.Exists(referenceName);
                String logName = System.IO.Path.Combine(dir, name);
                logName += ".result";
                String tempDBName = file + ".temp";
                ++count;
                try
                {
                    System.IO.File.Copy(file, tempDBName, true);
                    try
                    {
                        var fileInterface = Ares.Tags.TagsModule.GetTagsDB().FilesInterface;
                        fileInterface.OpenOrCreateDatabase(tempDBName);
                        System.Console.WriteLine("Opened file " + file);
                        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(referenceExists ? logName : referenceName))
                        {
                            try
                            {
                                fileInterface.ImportDatabase(importName, writer);
                            }
                            catch (Ares.Tags.TagsDbException ex)
                            {
                                writer.WriteLine("Exception occured: " + ex.GetType() + "; " + ex.Message);
                            }
                            catch (Exception ex)
                            {
                                writer.WriteLine("Exception occured: " + ex.GetType() + "; " + ex.Message);
                            }
                            finally
                            {
                                writer.Flush();
                            }
                        }
                        System.Console.WriteLine("Imported file " + importName);
                        fileInterface.CloseDatabase();
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
                            System.IO.File.Delete(tempDBName);
                        }
                        catch (System.IO.IOException)
                        {
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Exception occurred: " + e.GetType() + "; " + e.Message);
                }
                System.Console.WriteLine("---------------------------------------------------");
            }
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
