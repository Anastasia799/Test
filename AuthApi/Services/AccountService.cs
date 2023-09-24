using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthApi.Database;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Services;

public class AccountService : AuthService.AuthServiceBase 
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _configuration;
    
    public AccountService(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public override async Task<RegistrationResponse> RegistrationUser(RegistrationRequest request, ServerCallContext context) {
        
        if (_db.Users.Any(u => u.Username == request.Username))
        {
            return await Task.FromResult(new RegistrationResponse() {
                Message = "The user already exists"
            });
        }

        var user = new User {
            Id = GenerateUserId(),
            Username = request.Username,
            Password = request.Password 
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        
        return await Task.FromResult(new RegistrationResponse() {
            Message = "success"
        });
    }
    
    
    public override async Task<AuthorizationResponse> AuthorizationUser(AuthorizationRequest authorizationRequest, ServerCallContext context) {

        var user = _db.Users.FirstOrDefault(u => u.Username == authorizationRequest.Username);

        if (user != null && user.Password == authorizationRequest.Password)
        {
            var token = GenerateAuthorizationToken(user.Username);
            return token != null ?  await Task.FromResult( new AuthorizationResponse() { Token = token, IsAuthenticated = true}) : await Task.FromResult(new AuthorizationResponse() { Message = "invalid token" });
        }
        return await Task.FromResult(new AuthorizationResponse {Message = "Invalid credentials"});
    }

    private string? GenerateAuthorizationToken(string username) {
        var secretKey = _configuration.GetSection("JwtSettings")["SecretKey"];
        if (secretKey == null) return null;
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username)
        };

        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);

    }
    
    static int GenerateUserId()
    {
        var random = new Random();
        return random.Next(1, 1000 + 1);
    }
}