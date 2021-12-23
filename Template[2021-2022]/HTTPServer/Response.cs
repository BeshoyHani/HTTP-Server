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
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            this.code = code;
            this.headerLines.Add("Date: " + DateTime.Now.ToString());
            this.headerLines.Add("Content-Type: " + contentType);
            this.headerLines.Add("Content-Length: " + content.Length);
            if (redirectoinPath.Length > 0)
                this.headerLines.Add("Redirection-Path: " + redirectoinPath);

            // TODO: Create the request string
            responseString = GetStatusLine(code) + Environment.NewLine;
            foreach(string line in headerLines)
            {
                responseString += line + Environment.NewLine;
            }
            responseString += "\r\n" + content;
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            
            string statusLine =Configuration.ServerHTTPVersion + ' ' + (int)code + ' ' + code;
            return statusLine;
        }
    }
}
