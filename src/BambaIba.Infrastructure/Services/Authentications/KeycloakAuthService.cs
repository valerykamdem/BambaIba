using BambaIba.Infrastructure.Persistence;
using BambaIba.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace BambaIba.Infrastructure.Services.Authentications;

public record AdminTokenResponse(string Access_Token, string Refresh_Token);

public class KeycloakAuthService : IKeycloakAuthService
{
    private readonly HttpClient _client;
    private readonly KeycloakSettings _settings;
    private readonly BambaIbaDbContext _db;

    public KeycloakAuthService(
        HttpClient client,
        IOptions<KeycloakSettings> keycloakSettings,
        BambaIbaDbContext db)
    {
        _client = client;
        _settings = keycloakSettings.Value;
        _db = db;
    }

    ///// <summary>
    ///// Échange les identifiants utilisateur pour un token JWT
    ///// </summary>
    //public async Task<TokenResponseDto?> ExchangeCredentialsForTokenAsync(string email, string password)
    //{
    //    var payload = new Dictionary<string, string>
    //    {
    //        {"grant_type", "password"},
    //        {"username", email},
    //        {"password", password},
    //        {"client_id", _settings.AuthClientId},
    //        {"client_secret", _settings.AuthClientSecret},
    //        {"scope", "openid"}
    //    };

    //    var content = new FormUrlEncodedContent(payload);
    //    HttpResponseMessage response = await _client.PostAsync(_settings.TokenUrl, content);

        
    //    if (!response.IsSuccessStatusCode)
    //    {
    //        string errorBody = await response.Content.ReadAsStringAsync();
    //        //Console.WriteLine("Erreur lors de l'échange de token : " + errorBody);
    //        return null;
    //    }

    //    string responseBody = await response.Content.ReadAsStringAsync();
    //    //Console.WriteLine("Réponse brute : " + responseBody); // 👈 Très utile

    //    TokenResponseDto? token = JsonSerializer.Deserialize<TokenResponseDto>(responseBody, new JsonSerializerOptions
    //    {
    //        PropertyNameCaseInsensitive = true
    //    });

    //    return token;

    //}

    //public async Task<bool> SetUserPasswordAsync(string userId, string password)
    //{
    //    string adminToken = await GetAdminAccessTokenAsync();

    //    string url = $"{_settings.AdminUrl}/users/{userId}/reset-password";

    //    var content = JsonContent.Create(new
    //    {
    //        type = "password",
    //        value = password,
    //        temporary = false
    //    });

    //    var request = new HttpRequestMessage(HttpMethod.Put, url)
    //    {
    //        Content = content
    //    };

    //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

    //    HttpResponseMessage response = await _client.SendAsync(request);
    //    return response.IsSuccessStatusCode;
    //}

    ///// <summary>
    ///// Rafraîchit le token avec refresh_token
    ///// </summary>
    //public async Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken)
    //{
    //    var payload = new Dictionary<string, string>
    //    {
    //        {"grant_type", "refresh_token"},
    //        {"refresh_token", refreshToken},
    //        {"client_id", _settings.AuthClientId},
    //        {"client_secret", _settings.AuthClientSecret},
    //    };

    //    var content = new FormUrlEncodedContent(payload);
    //    HttpResponseMessage response = await _client.PostAsync(_settings.TokenUrl, content);

    //    if (!response.IsSuccessStatusCode)
    //        return null;

    //    return await response.Content.ReadFromJsonAsync<TokenResponseDto>();
    //}

    ///// <summary>
    ///// Crée un utilisateur dans Keycloak via l'API admin
    ///// </summary>
    //public async Task<ApiResult<bool>> CreateUserInKeycloak(RegisterCommand request)
    //{
    //    // Obtenir le token admin
    //    string adminToken = await GetAdminAccessTokenAsync();

    //    var userPayload = new
    //    {
    //        username = request.Email,
    //        email = request.Email,
    //        enabled = true,
    //        emailVerified = true,
    //        firstName = request.FirstName ?? request.Email.Split('@')[0],
    //        lastName = request.LastName ?? "Utilisateur",
    //        credentials = new[]
    //        {
    //            new { type = "password", value = request.Password, temporary = false }
    //        }
    //    };

    //    var jsonContent = new StringContent(
    //        JsonSerializer.Serialize(userPayload),
    //        Encoding.UTF8,
    //        "application/json"
    //    );

    //    string url = $"{_settings.AdminUrl}/users";

    //    var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
    //    {
    //        Content = jsonContent
    //    };

    //    requestMessage.Headers.Authorization =
    //        new AuthenticationHeaderValue("Bearer", adminToken);

    //    HttpResponseMessage response = await _client.SendAsync(requestMessage);

    //    if (!response.IsSuccessStatusCode)
    //    {
    //        string errorBody = await response.Content.ReadAsStringAsync();
    //        return new ApiResult<bool>(false, false, new[] { $"Échec de création utilisateur : {errorBody}" });
    //    }

    //    //// Obtenir l'ID de l'utilisateur créé
    //    //string locationHeader = response.Headers.Location?.ToString()
    //    //    ?? throw new Exception("Location header introuvable");

    //    //string userId = ExtractUserIdFromLocation(locationHeader);

    //    return new ApiResult<bool>(
    //        response.IsSuccessStatusCode,
    //        response.IsSuccessStatusCode,
    //        response.IsSuccessStatusCode ? [] : ["Échec de création de l'utilisateur"]);
    //}

    ////private string ExtractUserIdFromLocation(string location) =>
    ////location.Split('/').Last(); // Extrait l'ID depuis l'URL

    ///// <summary>
    ///// Obtient un token d’administrateur (ex: admin-cli)
    ///// </summary>
    //private async Task<string> GetAdminAccessTokenAsync()
    //{
    //    var payload = new Dictionary<string, string>
    //    {
    //        {"client_id", _settings.AdminClientId},
    //        {"client_secret", _settings.AdminClientSecret},
    //        {"scope", "openid email"},
    //        {"grant_type", "client_credentials"}
    //    };

    //    var content = new FormUrlEncodedContent(payload);
    //    HttpResponseMessage response = await _client.PostAsync(_settings.TokenUrl, content);

    //    if (!response.IsSuccessStatusCode)
    //    {
    //        string error = await response.Content.ReadAsStringAsync();
    //        throw new Exception($"Échec d'obtention du token admin : {error}");
    //    }

    //    string responseBody = await response.Content.ReadAsStringAsync();
    //    //Console.WriteLine("Réponse brute de Keycloak : " + responseBody); // 👈 Très utile ici

    //    AdminTokenResponse? token = JsonSerializer.Deserialize<AdminTokenResponse>(responseBody, new JsonSerializerOptions
    //    {
    //        PropertyNameCaseInsensitive = true
    //    });

    //    return token?.Access_Token
    //        ?? throw new Exception("Impossible de parser le token");
    //}

    ///// <summary>
    ///// Récupère ou crée l’utilisateur local à partir du token Keycloak
    ///// </summary>
    //public async Task<User> GetUserFromTokenAsync(string identityId, string accessToken, string civilStatus)
    //{
    //    JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

    //    string firstName = jwtToken.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value ?? string.Empty;
    //    string lastName = jwtToken.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value ?? string.Empty;
    //    //string gender = jwtToken.Claims.FirstOrDefault(c => c.Type == "gender")?.Value ?? string.Empty;
    //    string email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty;

    //    User? user = await _db.Users
    //        .Include(u => u.Roles)
    //        .ThenInclude(ur => ur.Role)
    //        .FirstOrDefaultAsync(u => u.IdentityId == identityId);

    //    if (user == null)
    //    {
    //        user = new User
    //        {
    //            Id = Guid.CreateVersion7(),
    //            IdentityId = identityId,
    //            Email = email,
    //            FirstName = firstName,
    //            LastName = lastName,
    //            CivilStatus = civilStatus,
    //            PinHash = HashPin("1234") // Optionnel : PIN par défaut
    //        };

    //        _db.Users.Add(user);
    //        await _db.UserRoles.AddAsync(new UserRole
    //        {
    //            UserId = user.Id,
    //            RoleId = (await _db.Roles.FirstAsync(r => r.Name == "User")).Id
    //        });

    //        // Créer les comptes bancaires
    //        var checkingAccount = new Account
    //        {
    //            Id = Guid.CreateVersion7(),
    //            UserId = user.Id,
    //            Balance = 0.0m, // Balance initiale
    //            AccountNumber = await GenerateUniqueAccountNumber("CHK"), // Exemple de numéro de compte
    //            Type = AccountType.Checking
    //        };

    //        var savingsAccount = new Account
    //        {
    //            Id = Guid.CreateVersion7(),
    //            UserId = user.Id,
    //            Balance = 0.0m,
    //            Type = AccountType.Savings,
    //            AccountNumber = await GenerateUniqueAccountNumber("SAV"), // Le numéro de compte épargne sera généré plus tard 
    //            //checkingAccount.LinkedAccount = savingsAccount;
    //            // 👇 On lie le compte épargne au compte courant après sa création
    //            LinkedAccountId = checkingAccount.Id
    //        };

    //        // ✅ Sauvegarde d'abord pour obtenir checkingAccount.Id
    //        await _db.Users.AddAsync(user);

    //        await _db.Accounts.AddRangeAsync(checkingAccount, savingsAccount);

    //        await _db.SaveChangesAsync();

    //        // ✅ MAJ du compte courant après sauvegarde
    //        checkingAccount.LinkedAccount = savingsAccount;
    //        _db.Accounts.Update(checkingAccount);
    //        await _db.SaveChangesAsync();
    //    }

    //    return user;
    //}

    ///// <summary>
    ///// Inscription complète via ton Web API
    ///// </summary>
    //public async Task<ApiResult<AuthResultDto>> RegisterAsync(RegisterCommand request)
    //{
    //    // Valider les informations reçues
    //    if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
    //        return new ApiResult<AuthResultDto>(false, null, ["Email ou mot de passe manquant"]);

    //    // Étape 1 : Créer l'utilisateur dans Keycloak
    //    ApiResult<bool> creationResult = await CreateUserInKeycloak(request);

    //    if (!creationResult.Success)
    //        return new ApiResult<AuthResultDto>(false, null, creationResult.Errors);

    //    // Étape 2 : Obtenir le token utilisateur après inscription
    //    TokenResponseDto? tokenResult = await ExchangeCredentialsForTokenAsync(request.Email, request.Password);

    //    if (tokenResult == null)
    //        return new ApiResult<AuthResultDto>(false, null, ["Échec de récupération du token"]);

    //    // Étape 3 : Créer ou récupérer l'utilisateur local
    //    User user = await GetUserFromTokenAsync(tokenResult.UserId, tokenResult.Access_Token, request.CivilStatus);

    //    return new ApiResult<AuthResultDto>(true, new AuthResultDto(
    //        user.Id.ToString(),
    //        user.Email,
    //        user.Roles.Select(r => r.Role.Name).ToList(),
    //        tokenResult.Access_Token,
    //        tokenResult.Refresh_Token
    //    ), []);
    //}

    //public async Task<Guid> SetUserPin(Guid userId, string pin)
    //{
    //    User? user = await _db.Users.FindAsync(userId) ?? throw new Exception("Utilisateur introuvable");

    //    user.PinHash = HashPin(pin);
    //    await _db.SaveChangesAsync();

    //    return userId;
    //}

    //public async Task UpdateUserAsync(string keycloakUserId, string firstName, string lastName)
    //{
    //    string adminToken = await GetAdminAccessTokenAsync();
    //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

    //    var payload = new
    //    {
    //        firstName,
    //        lastName,
    //    };

    //    HttpResponseMessage response = await _client.PutAsJsonAsync(
    //        $"{_settings.AdminUrl}/users/{keycloakUserId}",
    //        payload);

    //    response.EnsureSuccessStatusCode();
    //}

    //private string HashPin(string pin) => BCrypt.Net.BCrypt.HashPassword(pin);

    //private async Task<string> GenerateUniqueAccountNumber(string prefix)
    //{
    //    string number;
    //    do
    //    {
    //        Random _random = new();
    //        string timestamp = DateTime.UtcNow.ToString("yyMMddHHmm");
    //        int random = _random.Next(100000, 999999);
    //        number = $"{prefix}-{timestamp}-{random}";
    //    } while (await _db.Accounts.AnyAsync(a => a.AccountNumber == number));

    //    return number;
    //}

}

