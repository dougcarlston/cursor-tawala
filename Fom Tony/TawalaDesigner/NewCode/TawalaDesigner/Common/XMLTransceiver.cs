// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Tawala.Common
{
    /// <summary>
    /// Transmits text to and receives text from a specified URL
    /// </summary>
    public class XMLTransceiver
    {
        protected HttpWebRequest webRequest;

        public XMLTransceiver(string URL)
        {
            try
            {
                // create and configure web request
                webRequest = (HttpWebRequest)WebRequest.Create(URL);

                webRequest.Method = "POST";
                webRequest.ContentType = "text/xml";

                webRequest.Timeout = 30000;
            }
            catch
            {
            }
        }

        public void Transmit(string text)
        {
            try
            {
                // encode text as Unicode UTF8
                var encoding = new UTF8Encoding();
                byte[] data = encoding.GetBytes(text);

                webRequest.Timeout = CalcRequestTimeout(data.Length);

                // establish length of web request data
                webRequest.ContentLength = data.Length;

                // get stream for writing data
                Stream requestStream = webRequest.GetRequestStream();

                // send the data
                requestStream.Write(data, 0, data.Length);

                // release stream resources
                requestStream.Close();
            }
            catch
            {
            }
        }

        public string Receive()
        {
            var receivedText = new StringBuilder();
            try
            {
                WebResponse webResponse = webRequest.GetResponse();

                // set up stream reader
                var streamReader = new StreamReader(webResponse.GetResponseStream(), true);
                var characters = new Char[4096];

                // Read up to 4096 characters and place in return string
                int count = streamReader.Read(characters, 0, 4096);

                while (count > 0)
                {
                    // move the characters to return string
                    receivedText.Append(characters, 0, count);

                    // Read up to 4096 characters
                    count = streamReader.Read(characters, 0, 4096);
                }

                // fix up end-of-line characters
                receivedText.Replace("\n", "\r\n");

                // release resources
                streamReader.Close();
                webResponse.Close();
            }
            catch
            {
            }

            return receivedText.ToString();
        }

        public static int CalcRequestTimeout(int contentLength)
        {
            const int halfMB = 1048576/2;
            return 30000 + (contentLength/halfMB)*15000;
        }
    }
}