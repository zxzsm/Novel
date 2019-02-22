using Novel.Entity.Models;
using Novel.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Novel.Entity;
using Novel.Entity.ViewModels;

namespace Novel.Service
{
    public class UserService : BaseRepository<UserInfo>
    {
        public UserInfo GetUserInfo(UserInfo userInfo)
        {
            userInfo.UserName = userInfo.UserName.Trim();
            userInfo.Uesrpwd = userInfo.Uesrpwd.Trim();
            return Db.UserInfo.FirstOrDefault(m => m.UserName == userInfo.UserName && m.Uesrpwd == userInfo.Uesrpwd);
        }
        public UserInfo GetUserInfo(string userName)
        {
            userName = userName.AsTrim();
            return Db.UserInfo.FirstOrDefault(m => m.UserName == userName);
        }
        public UserInfo GetUserInfoById(int userId)
        {
            return Db.UserInfo.FirstOrDefault(m => m.UserId == userId);
        }
        public bool CheckUserInfo(UserInfo userInfo, bool needCheckEmail = false, bool needCheckMobile = false)
        {
            if (userInfo.UserName.IsEmpty())
            {
                return false;
            }
            if (userInfo.Uesrpwd.IsEmpty())
            {
                return false;
            }
            if (needCheckEmail && userInfo.UserEmail.IsEmpty())
            {
                return false;
            }
            if (needCheckMobile && userInfo.UserMoblie.IsEmpty())
            {
                return false;
            }
            return true;
        }

        public UserInfo GetUserInfo(string phone, string email)
        {
            return Db.UserInfo.FirstOrDefault(m => m.UserMoblie == phone && m.UserEmail == email);
        }
        public UserInfo AddUserInfo(UserInfo userInfo)
        {
            if (!CheckUserInfo(userInfo, true, true))
            {
                return null;
            }
            var tempUser = GetUserInfo(userInfo);
            if (tempUser != null)
            {
                return null;
            }
            userInfo = new UserInfo
            {
                UserName = userInfo.UserName.Trim(),
                UserMoblie = userInfo.UserMoblie.Trim(),
                CreateTime = DateTime.Now,
                Uesrpwd = userInfo.Uesrpwd,
                UpdateTime = DateTime.Now,
                UserEmail = userInfo.UserEmail.Trim(),
            };
            Db.UserInfo.Add(userInfo);
            return Db.SaveChanges() > 0 ? userInfo : null;
        }

    }
}
