using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public interface IUserData
    {
        UserModel GetUserById(string Id);
    }
}