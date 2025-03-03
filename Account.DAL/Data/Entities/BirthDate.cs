namespace Account.DAL.Data.Entities;

public class BirthDate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required User User { get; set; }
    public DateTime? Value { get; set; }
}

