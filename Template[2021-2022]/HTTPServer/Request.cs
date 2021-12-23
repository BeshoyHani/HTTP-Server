using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //TODO: parse the receivedRequest using the \r\n delimeter
            bool isValid = true;
            contentLines = new string[3];

            string[] stringSeparators = new string[] { "\r\n" }; // Separate by blank line
            string[] separatedContent = requestString.Split(stringSeparators, StringSplitOptions.None);
            contentLines[0] = separatedContent[0];
            int i = 1;
            while (separatedContent[i] != "")
            {
                contentLines[1] += separatedContent[i];
                i++;
                if (separatedContent[i] != "")
                    contentLines[1] += "\r\n";
            }            
            contentLines[2] = separatedContent[i];


            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if(contentLines.Length < 3)
            {
                return false;
            }

            // Parse Request line
            isValid = ParseRequestLine();
            Console.WriteLine("1) " + isValid);

            // Validate blank line exists
            isValid &= ValidateBlankLine();
            Console.WriteLine("2)" + isValid);
            // Load header lines into HeaderLines dictionary
            isValid &= LoadHeaderLines();
            Console.WriteLine("3)" + isValid);
            return isValid;
        }

        private bool ParseRequestLine()
        {
            this.requestLines = contentLines[0].Split(' ');
            this.relativeURI = requestLines[1];
            Console.WriteLine(requestLines[2]);
            return requestLines.Length == 3 && ValidateIsURI(this.requestLines[1]);
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            string[] stringSeparators = new string[] { "\r\n" };
            string []headerDate = contentLines[1].Split(stringSeparators, StringSplitOptions.None);

            string[] headerSeparator = new string[] { ": " };
            this.headerLines = new Dictionary<string, string>();
            foreach (string header in headerDate)
            {
                string []line = header.Split(headerSeparator, StringSplitOptions.None);
                this.headerLines.Add(line[0], line[1]);
            }
            return headerLines.Count > 0;
        }

        private bool ValidateBlankLine()
        {
            return contentLines[2] == "";
        }

    }
}
