using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Model.EF;
using PagedList.Mvc;
using PagedList;
using System.Configuration;

namespace Model.Dao
{
    public class UserDao
    {
        OnlineShopDbContext db = null;

        public object Encryptor { get; private set; }

        public UserDao()
        {
            db = new OnlineShopDbContext();
        }

        public long Insert(User entity)
        {
            db.Users.Add(entity);
            db.SaveChanges();
            return entity.ID;
        }

        public long InsertForFacebook(User entity)
        {
            var user = db.Users.SingleOrDefault(x => x.UserName == entity.UserName);
            if (user == null) {
                db.Users.Add(entity);
                db.SaveChanges();
                return entity.ID;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// Trả về id khi update thành công
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public long Update(User entity)
        {
            //còn cần thêm trường language
            try
            {
                //lấy user ra để update
                var user = db.Users.Find(entity.ID);
                user.UserName = entity.UserName;
                if (entity.Password != null)
                {
                    user.Password = entity.Password;
                }
                else
                {
                    user.Password = user.Password;
                }
                user.GroupID = entity.GroupID;
                user.Name = entity.Name;
                user.Address = entity.Address;
                user.Email = entity.Email;
                user.Phone = entity.Phone;
                //ko đổi CreatedDate+By
                user.CreatedDate = user.CreatedDate;
                user.CreatedBy = user.CreatedBy;
                user.ModifiedDate = entity.ModifiedDate;
                user.ModifiedBy = entity.ModifiedBy;
                user.Status = entity.Status;
                user.ProvinceID = entity.ProvinceID != null ? entity.ProvinceID : null;
                user.DistrictID = entity.DistrictID != null ? entity.DistrictID : null;
                db.SaveChanges();
                return user.ID;
            }
            catch (Exception ex)
            {
                //logging
                Common.Logger.Log("Error" + ex.Message);
                return 0;
            }
        }

        public IEnumerable<User> ListAllPaging(string searchString, int page)
        {

            var pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"].ToString());
            IQueryable<User> model = db.Users;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.UserName.Contains(searchString) || x.Name.Contains(searchString));
            }
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }

        public User GetByUserName(string userName)
        {
            return db.Users.SingleOrDefault(x => x.UserName == userName);
        }

        public User GetByID(long? id)
        {
            return db.Users.Find(id);
        }

        public List<string> GetListCredential(string userName)
        {
            //cần fix lấy (List<Role>)listRole_From_UserName

            //lấy User từ userName trong database
            var user = db.Users.SingleOrDefault(x=>x.UserName==userName);
            //bảng GroupID_RoleID với cột GroupID == GroupID của user
            var userGroupName = db.UserGroups.Find(user.GroupID).Name;
            var UserGroupID_RoleID = db.Credentials.Where(x => x.UserGroupID == user.GroupID);
            //Lấy hết cột RoleID làm listRoleID
            return UserGroupID_RoleID.Select(x => x.RoleID).ToList();
        } 

        public int Login(string userName, string passWord, int loginAs = 3) //loginAs Admin(1) Shipper(2) User(3) //mặc định 3
        {
            var result = db.Users.SingleOrDefault(x => x.UserName == userName);
            //Khong ton tai username
            if (result == null)
                return 0;
            //tồn tại username
            else
            {
                //kiểm tra login quyền admin/mod
                if (loginAs == 1)
                {
                    //là admin/mod?
                    if ((result.GroupID == Common.CommonConstants.ADMIN_GROUP || result.GroupID == Common.CommonConstants.MOD_GROUP))
                    {
                        if (result.Status == false)
                        {
                            //tài khoản admin/mod bị khóa
                            return -1;
                        }
                        else
                        {
                            //đăng nhập admin/mod thành công
                            if (result.Password == passWord)
                                return 1;
                            else
                                //đăng nhập admin/mod sai mật khẩu
                                return -2;
                        }
                    }
                    else
                        //tài khoản ko có quyền đăng nhập admin/mod
                        return -3;
                }
                //kiểm tra login quyền shipper
                if (loginAs == 2)
                {
                    //là admin/mod?
                    if (result.GroupID == Common.CommonConstants.ADMIN_GROUP || 
                        result.GroupID == Common.CommonConstants.MOD_GROUP ||
                        result.GroupID == Common.CommonConstants.SHIPPER_GROUP)
                    {
                        if (result.Status == false)
                        {
                            //tài khoản bị khóa. Không thể login với vai trò shipper.
                            return -1;
                        }
                        else
                        {
                            //đăng nhập shipper thành công
                            if (result.Password == passWord)
                                return 1;
                            else
                                //đăng nhập shipper sai mật khẩu
                                return -2;
                        }
                    }
                    else
                        //tài khoản ko có quyền đăng nhập shipper
                        return -3;
                }
                //đăng nhập quyền user
                else //if(loginAs==3)
                {
                    if (result.Status == false)
                    {
                        //tài khoản user bị khóa
                        return -1;
                    }
                    else
                    {
                        if (result.Password == passWord)
                            //đăng nhập quyền user thành công
                            return 1;
                        else
                            //đăng nhập quyền user sai mật khẩu
                            return -2;
                    }
                }
            }
        }

        public bool Delete(int id)
        {

            try
            {
                var user = db.Users.Find(id);
                db.Users.Remove(user);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool ChangeStatus(long id)
        {
            var user = db.Users.Find(id);
            user.Status = !user.Status;
            db.SaveChanges();
            return user.Status;
        }
        /// <summary>
        /// Trả về Count
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool CheckUserName(string userName)
        {
            return db.Users.Count(x=>x.UserName==userName)>0;
        }

        public bool CheckEmail(string email)
        {
            return db.Users.Count(x => x.Email == email) > 0;
        }
    }
}
