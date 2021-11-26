using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using UDPProxyLibrary;

namespace UDPBroadcast
{
    class Program
    {
        //Unique name for each sensor
        //This is used to differentiate between who is sending the data 
        //Improvement: this should be read from a configuration, instead of having it hardcoded
        private static string MySensorName = "MoveSpeedtrap";
        static void Main(string[] args)
        {
            Console.WriteLine("UDP Broadcaster started");

            //Using a randomizer, we can reuse the same.
            //this could be initialized with a seed for testing purposes, if you want it to generate the same values each time
            Random rand = new Random();

            //Declaring the UdpClient outside of the while loop to reuse the same UdpClient object
            //surrounding UdpClient with using, to automatically clean up after use
            //In this case as the socket is reused and the while loop is inside here, it will only stop if the console is closed.
            using (UdpClient socket = new UdpClient())
            {
                //The sensor sends data as long as it is on
                while (true)
                {
                    //Create a new SensorData object with a random speed, and the unique name of the sensor
                    SensorData randomData = new SensorData() { Speed = rand.Next(0, 160), SensorName = MySensorName };
                    //Serializes the new sensorData object, the values we didn't set: timestamp and Id are set to the default values
                    string serializedData = JsonSerializer.Serialize(randomData);
                    //Because we send Bytes in our UdpClient, here we convert the string to a byte array, using the encoding UTF8
                    //The receiver should use the same encoding when converting to a string
                    byte[] data = Encoding.UTF8.GetBytes(serializedData);

                    //Sends the packet to the broadcast address on port 10100, this way we don't need to know any server addresses, and multiple servers can listen
                    socket.Send(data, data.Length, "255.255.255.255", 10100);
                    Console.WriteLine("Send JSON: " + serializedData);
                    //Pauses the loop, so we don't spam the network with packets
                    //sleeps between 1 and 5 seconds (could be longer, but for testing it faster to test with a low interval)
                    Thread.Sleep(rand.Next(1000, 5000));
                }
            }
        }
    }
}
