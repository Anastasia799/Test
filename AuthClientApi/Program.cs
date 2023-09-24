using AuthClientApi;
using Grpc.Net.Client;

var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = new AuthService.AuthServiceClient(channel);
try {
    Console.WriteLine("Actions:\n Register - 1 \n Authorization -2");
    var action = Convert.ToInt32(Console.ReadLine());

    switch (action) {
        case 1 :
            Register(client);
            break;
        case 2:
            Authorization(client);
            break;
    }
} catch (Exception ex) {
    Console.WriteLine(ex.Message);
}

static async void Register(AuthService.AuthServiceClient client) {
    Console.WriteLine("Enter name:");
    var name = Console.ReadLine();
    Console.WriteLine("Enter Passoword");
    var password = Console.ReadLine();
    var registration = await client.RegistrationUserAsync(new RegistrationRequest {
        Username = name,
        Password = password
    });
    Console.WriteLine(registration.Message);
}

static async void Authorization(AuthService.AuthServiceClient client) {
    Console.WriteLine("Enter name:");
    var name = Console.ReadLine();
    Console.WriteLine("Enter Passoword");
    var password = Console.ReadLine();
    var authorization = await client.AuthorizationUserAsync(new AuthorizationRequest
    {
        Username = "test user",
        Password = "testUser"
    });
    if (authorization.IsAuthenticated)
    {
        Console.WriteLine(authorization.Token);
    }
}