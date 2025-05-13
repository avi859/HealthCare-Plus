namespace AngularAuthApi.Models
{
    public interface IAccount
    {
        int Id { get; set; }
        string Username { get; set; }
        string Email { get; set; }
        string Role { get; set; }
    }
}
