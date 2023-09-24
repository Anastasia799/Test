using BlogClientApi;
using Grpc.Net.Client;

var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = new PostService.PostServiceClient(channel);

var response = await client.CreatePostAsync(new CreatePostRequest() {
    Body = "test",
    Title = "test",
});

Console.WriteLine(response.Body);
