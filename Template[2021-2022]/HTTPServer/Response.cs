using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            this.code = code;
            setHeaderLines(code, contentType, content.Length, redirectoinPath);

            // TODO: Create the request string
            getResponseString(content);
        }

        private void setHeaderLines(StatusCode code, string contentType, int contentLength, string redirectoinPath)
        {
            List<string> headerLines = new List<string>();
            headerLines.Add("Date: " + DateTime.Now.ToString());
            headerLines.Add("Server: " + Configuration.ServerType);
            headerLines.Add("Content-Type: " + contentType);
            headerLines.Add("Content-Length: " + contentLength);
            if (redirectoinPath.Length > 0)
                headerLines.Add("Redirection-Path: " + redirectoinPath);

            //Assign value to Class Member headerLines
            this.headerLines = headerLines;
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine =Configuration.ServerHTTPVersion + ' ' + (int)code + ' ' + code;
            return statusLine;
        }

        private void getResponseString(string content)
        {
            //Get Status Line
            this.responseString = GetStatusLine(code) + Environment.NewLine;

            //Get Header Lines
            foreach (string line in this.headerLines)
            {
                this.responseString += line + Environment.NewLine;
            }
            this.responseString += "\r\n" + content;
        }
    }
}
