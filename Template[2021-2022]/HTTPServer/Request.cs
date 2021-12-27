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

        public RequestMethod getRequestMethod()
        {
            return this.method;
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
            contentLines[0] = separatedContent[0]; //Request Line
            int i = 1;
            while (separatedContent[i] != "")//Header Lines
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

            // Validate blank line exists
            isValid &= ValidateBlankLine();

            // Load header lines into HeaderLines dictionary
            isValid &= LoadHeaderLines();

            return isValid;
        }

        private bool ParseRequestLine()
        {
            this.requestLines = contentLines[0].Split(' ');
            //Set Request Line
            setRequestMethod(requestLines[0]);
            this.relativeURI = requestLines[1].Remove(0, 1); //Remove '/' before path
            setHttpVersion(requestLines[2]);

            //Validate Header Line
            return requestLines.Length == 3 && ValidateIsURI(this.relativeURI);
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
            return this.headerLines.Count > 0;
        }

        private bool ValidateBlankLine()
        {
            return contentLines[2] == "";
        }

        private void setHttpVersion(string version)
        {
            switch (version.ToUpper())
            {
                case "HTTP/1.0":
                    this.httpVersion = HTTPVersion.HTTP10;
                    break;
                case "HTTP/1.1":
                    this.httpVersion = HTTPVersion.HTTP11;
                    break;
                default:
                    this.httpVersion = HTTPVersion.HTTP09;
                    break;
            }
        }

        private void setRequestMethod(string method)
        {
            switch (method.ToUpper())
            {
                case "GET":
                    this.method = RequestMethod.GET;
                    break;
                case "POST":
                    this.method = RequestMethod.POST;
                    break;
                default:
                    this.method = RequestMethod.HEAD;
                    break;
            }
        }

    }
}
