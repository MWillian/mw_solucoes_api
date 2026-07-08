namespace MwSolucoes.Domain.Entities
{
    public class UserToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        private UserToken() { }
        public UserToken(Guid userId, int expirationMinutes)
        {
            Token = Guid.NewGuid().ToString();
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
            UserId = userId;
        }
        public void MarkAsUsed()
        {
            UsedAt = DateTime.UtcNow;
        }
        public bool IsValid() => UsedAt == null && DateTime.UtcNow <= ExpiresAt;
    }
}
