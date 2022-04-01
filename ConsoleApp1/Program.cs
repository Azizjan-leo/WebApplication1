using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

Console.Write("email: ");
var email = Console.ReadLine();
Console.Write("token: ");
var _myAccessToken = Console.ReadLine();

Task<string> GetToken()
{
    return Task.FromResult(_myAccessToken);
}

HubConnection hubConnection = new HubConnectionBuilder().WithUrl("https://localhost:7215/chat", options =>
{
    options.AccessTokenProvider = () => GetToken();
})
//    .ConfigureLogging(logging =>
//{
//    // Log to the Console
//    logging.AddConsole();

//    // This will set ALL logging to Debug level
//    logging.SetMinimumLevel(LogLevel.Debug);
//})
    .Build();

hubConnection.On<string, string>("Receive", Puk);

void Puk(string message, string username)
{
    Console.WriteLine($"{message}");
}

await hubConnection.StartAsync();

while (true)
{
    var message = Console.ReadLine();

    if (message == null || message == String.Empty)
        continue;
    if (message == "exit")
        break;

    await hubConnection.SendAsync("Send", message, email);
}

Console.ReadLine();
await hubConnection.StopAsync();