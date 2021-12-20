using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;
        int backlog = 100;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            this.serverSocket.Bind(hostEndPoint);

        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            this.serverSocket.Listen(backlog);

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                Console.WriteLine("New client accepted: {0}", clientSocket.RemoteEndPoint);
                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newthread.Start(clientSocket);

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            Socket clientSock = (Socket)obj;
            clientSock.ReceiveTimeout = 0;

            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte []requestBytes = new byte[1024 * 1024];
                    int receivedLen = clientSock.Receive(requestBytes);
                    string request = Encoding.ASCII.GetString(requestBytes, 0, receivedLen);

                    // TODO: break the while loop if receivedLen==0
                    if (receivedLen == 0)
                        break;

                    // TODO: Create a Request object using received request string
                    Request clientReq = new Request(request);

                    // TODO: Call HandleRequest Method that returns the response
                    Response response =  HandleRequest(clientReq);

                    // TODO: Send Response back to client
                    clientSock.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSock.Close();
        }

        Response HandleRequest(Request request)
        {
            Response response;
            string content;
            try
            {
                //TODO: check for bad request 
                if (request.ParseRequest() == false)
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    response = new Response(HTTPServer.StatusCode.BadRequest, "text/html", content, "");

                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                string physPath = Path.Combine(Configuration.RootPath, request.relativeURI);

                //TODO: check for redirect
                if(Configuration.RedirectionRules.ContainsKey(physPath) == true)
                {
                    content = LoadDefaultPage(Configuration.RedirectionDefaultPageName);
                    string redirectPath = GetRedirectionPagePathIFExist(request.relativeURI);
                    response = new Response(HTTPServer.StatusCode.Redirect, "text/html", content, redirectPath);
                }

                //TODO: check file exists
                if (File.Exists(physPath) == false)
                {
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    response = new Response(HTTPServer.StatusCode.NotFound, "text/html", content, "");
                }

                //TODO: read the physical file
                content = LoadDefaultPage(request.relativeURI);

                // Create OK response
                response = new Response(HTTPServer.StatusCode.OK, "text/html", content, "");
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);

                // TODO: in case of exception, return Internal Server Error.
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                response = new Response(HTTPServer.StatusCode.NotFound, "text/html", content, "");
            }
            return response;
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if(Configuration.RedirectionRules.ContainsKey(relativePath))
                return Configuration.RedirectionRules[relativePath];
            
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            string content = "";
            try
            {
                content = File.ReadAllText(filePath);

            }catch(Exception ex)
            {
                Logger.LogException(ex);
            }

            // else read file and return its content
            return content;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Environment.Exit(1);
            }
        }
    }
}
