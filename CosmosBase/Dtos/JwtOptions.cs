namespace CosmosBase.Dtos
{
    internal class JwtOptions
    {
        public List<string> Audience { get; set; }
        public string Issuer { get; set; }
        public int AccessTokenExpiration { get; set; }
        public string SecurityKey { get; set; }
        public int RefreshTokenTTL { get; set; }

    }
}
