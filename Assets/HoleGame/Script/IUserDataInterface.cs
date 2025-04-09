using System.Threading.Tasks;

public interface IUserDataInterface
{
    Task<UserData> LoadPlayerDataAsync(string userId);
    Task SavePlayerDataAsync(UserData data);
}