namespace Account.DAL.Data.Entities;

public class UserSettings
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool DarkMode { get; set; } = false;
    public Guid UserId { get; set; }
    public required User User { get; set; }
}
