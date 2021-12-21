﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static StreamWriter sr = new StreamWriter("log.txt");
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            DateTime time = DateTime.Now;

            //message:
            string message = ex.Message;

            // for each exception write its details associated with datetime
            string text = message + '\t' + time.ToString();
            File.AppendAllText("log2.txt", text + Environment.NewLine);
        }
    }
}
