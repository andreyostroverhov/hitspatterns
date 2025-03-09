namespace Common.DataTransferObjects;

public class UserShortDto
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string Email { get; set; }
    public string? Role { get; set; }
    public bool IsBanned { get; set; }
}
