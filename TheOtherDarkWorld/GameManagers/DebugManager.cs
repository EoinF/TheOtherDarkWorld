using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TheOtherDarkWorld
{
    public class DebugManager
    {
        static StreamWriter fs;
        const bool IsDebugMode = false;
        const string DEBUG_FILENAME = "debug.txt";

        public static void E_NoUIParent()
        {
            WriteError("Error [1]: Trying to swap elements that have no parent UI");
        }

        static DebugManager()
        {
            fs = new StreamWriter(DEBUG_FILENAME);
        }

        public static void WriteError(string error)
        {
            fs.WriteLine();
            fs.WriteLine("-----------------------------------------------");
            fs.WriteLine("Error Date: " + System.DateTime.Now);
            fs.WriteLine();
            fs.WriteLine(error);
            fs.WriteLine("-----------------------------------------------");
            fs.WriteLine();
        }

        public static void WriteError(Exception ex)
        {
            if (IsDebugMode)
            {
                WriteError(ex.Message);
            }
            else
                throw ex; //Throw the exception again
        }
    }
}
