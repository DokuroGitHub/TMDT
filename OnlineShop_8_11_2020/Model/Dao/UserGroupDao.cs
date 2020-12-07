using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class UserGroupDao
    {
        OnlineShopDbContext db = null;

        public UserGroupDao()
        {
            db = new OnlineShopDbContext();
        }

        public long Insert(UserGroup userGroup)
        {
            db.UserGroups.Add(userGroup);
            db.SaveChanges();
            return 999;
        }

        public List<UserGroup> ListAll()
        {
            return db.UserGroups.ToList();
        }
        /// <summary>
        /// Truyền id lấy UserGroup
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public UserGroup ViewDetail(string userGroupID)
        {
            return db.UserGroups.Find(userGroupID);
        }
    }
}
