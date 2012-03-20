using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Mes
{
    class TestHarness
    {
        // Variables to store filepath of test script
        string testFile;

        public string File
        {
            get 
            {
                return testFile;
            }
            set
            {
                testFile = value;
            }
        }

        // Retrieves the line by line commands in the test file.
        public List<string> getCommands()
        {
            List<string> cmds = new List<string>();

            try
            {
                StreamReader testReader = new StreamReader(testFile);
                string testLine = testReader.ReadLine();

                while (testLine != null)
                {
                    cmds.Add(testLine);
                    testLine = testReader.ReadLine();
                }
                testReader.Close();
            }
            catch
            {
                Console.WriteLine("Could not load the test file!");
                return cmds;
            }
            return cmds;
        }

        // Outputs to the console the commands in the test file.
        public void printCommands(List<string> cmdList)
        {
            int i = 0;
            Console.WriteLine("Printing out commands!");
            while (cmdList.Count > i)
            {
                Console.WriteLine(cmdList.ElementAt(i));
                i++;
            }
        }
    }
}
