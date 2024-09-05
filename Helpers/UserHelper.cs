using CDO.Data;
using CDO.Models;
using Microsoft.EntityFrameworkCore;
namespace CDO.Helpers
{
    public static class UserHelper
    {
        public static User GetCurrentUser(this HttpContext context) 
        {
            var username = context.User.Identity.Name;
            using var dbcontext = new PostgresContext();
            return dbcontext.Users.Include(a => a.RolecdNavigation).FirstOrDefault(x => x.Login == username);
        }
    }
}
