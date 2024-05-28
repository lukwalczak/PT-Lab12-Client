using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

namespace PT_Lab12_Client;

[Serializable]
public class DataObject
{
    public int Value { get; set; }
}

public class Client
{
    private UdpClient _client;
    private IPEndPoint _serverEndPoint;

    public Client(string ip, int port)
    {
        _client = new UdpClient();
        _serverEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        Console.WriteLine("Connected to server");
    }

    public void SendData()
    {
        try
        {
            for (int i = 0; i < 5; i++)
            {
                DataObject data = new DataObject { Value = i };
                string jsonData = JsonSerializer.Serialize(data);
                byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);
                _client.Send(jsonBytes, jsonBytes.Length, _serverEndPoint);
                Console.WriteLine("Sent object with value: " + data.Value);

                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] responseBytes = _client.Receive(ref remoteEndPoint);
                string responseJson = Encoding.UTF8.GetString(responseBytes);
                DataObject modifiedData = JsonSerializer.Deserialize<DataObject>(responseJson);
                Console.WriteLine("Received modified object with value: " + modifiedData.Value);

                Thread.Sleep(1000);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            _client.Close();
        }
    }

    public static void Main(string[] args)
    {
        try
        {
            Client client = new Client("127.0.0.1", 12345);
            client.SendData();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}