using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

using ApplicationSharedKernel.Data.Format.Json;

public static class Program
{
    public static HubConnection connection;
    public static void Main(string[] args)
    {
        Init();

        while(true)
        {
            var key = Console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.A:
                    SendTest();
                    break;
            }
        }
    }
    public static async void Init()
    {
        HttpClient httpClient = new HttpClient();
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/apiv1/authentification/login");
        string json = null;
        using (JsonHandler jsonHandler = new JsonHandler())
        {
            json = jsonHandler.JsonSerialize(new { user = "root", password = "root" });
        }
        httpRequestMessage.Headers.Add("User-Agent", "PostmanRuntime/7.29.2");
        httpRequestMessage.Content = new StringContent(json, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
        var response = await httpClient.SendAsync(httpRequestMessage);

        string content = await response.Content.ReadAsStringAsync();
        AuthModel authModel = null;
        using (JsonHandler jsonHandler = new JsonHandler())
        {
            authModel = jsonHandler.JsonDeserialize<AuthModel>(content);
        }
        connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/testhub", options =>
                {
                    options.Headers.Add("User-Agent", "PostmanRuntime/7.29.2");
                    options.AccessTokenProvider = () => Task.FromResult(authModel.Token);
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets | Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
                    options.DefaultTransferFormat = Microsoft.AspNetCore.Connections.TransferFormat.Text;
                }).AddJsonProtocol(options =>
                {
                   
                    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
                })
                .Build();
        connection.On<string, string>("ReceiveMessage", (user, message) =>
        {

            Console.WriteLine("receive msg: " + user + ", msg: " + message + "");
        });
    }
    public static async void SendTest()
    {
        try
        {
            if(connection.State != HubConnectionState.Connected)
            {
                await connection.StartAsync();
                Console.WriteLine("start connection");
            }
            await connection.InvokeAsync("SendMessage",
                "arg1", "arg2");

            Console.WriteLine("send msg");
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.Message);
        }
    }
}
