using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;

namespace UDPListener
{
    class Program
    {
        //The URL of the Rest service
        //Improvement: this should be configurable
        private static string URL = "http://localhost:23864/api/speed";
        static void Main(string[] args)
        {
            Console.WriteLine("UDP Listener");

            //Declaring the UdpClient outside of the while loop to reuse the same UdpClient object
            //surrounding UdpClient with using, to automatically clean up after use
            //In this case as the socket is reused and the while loop is inside here, it will only stop if the console is closed.
            using (UdpClient socket = new UdpClient())
            {
                //"Binds" the UdpClient to the port 10100 on any network adapter on the PC
                //Works like TcpListener where we listen for packages on the given port
                socket.Client.Bind(new IPEndPoint(IPAddress.Any, 10100));

                //Declaring the HttpClient outside of the while loop to reuse the same HttpClient object
                //surrounding UdpClient with using, to automatically clean up after use
                //In this case as the HttpClient is reused and the while loop is inside here, it will only stop if the console is closed.
                using (HttpClient client = new HttpClient())
                {
                    //Keeps listening as long as the program is running
                    while (true)
                    {
                        //An endpoint that stores the client that sends data
                        IPEndPoint from = null;
                        //Waits here until a client sends some data to the bound port on the PC
                        //Reads the data received into a byte array
                        byte[] data = socket.Receive(ref from);
                        //converts the bytes recieved into a string, using the UTF8 encoding, same encoding should be used in the client/sender
                        string recieved = Encoding.UTF8.GetString(data);
                        Console.WriteLine("Server receieved: " + recieved + " From " + from.Address);

                        //as the string we recieve is already JSON we don't need to deserialize it, only to serialize it again
                        //Instead we just put the JSON string in the body (using the HttpContent class) and set the header content-type to application/json
                        //UTF8 is again specified here as the enconding of the characters
                        HttpContent content = new StringContent(recieved, Encoding.UTF8, "application/json");
                        //Sends our string with our defined header to the Rest service
                        client.PostAsync(URL, content);
                    }
                }
            }
        }
    }
}
