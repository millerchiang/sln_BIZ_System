using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using System.Collections;

namespace prj_BIZ_System.Services
{
    public class PasswordService : _BaseService
    {
        public string getUserPassword(string current_id)
        {
            var param = new UserInfoModel() { user_id = current_id };
            var obj = mapper.QueryForObject<UserInfoModel>("UserInfo.SelectOne", param);
            return obj.user_pw;
        }

        public string getManagerPassword(string current_id)
        {
            var param = new ManagerInfoModel() { manager_id = current_id };
            var obj = mapper.QueryForObject<ManagerInfoModel>("Password.SelectManagerInfoOne", param);
            return obj.manager_pw;
        }

        public bool UpdateUserPassword(string current_id , string new_pw)
        {
            var param = new UserInfoModel() { user_id = current_id , user_pw = new_pw };
            return mapper.Update("Password.UpdateUserPassword", param) > 0 ;
        }

        public bool UpdateManagerPassword(string current_id, string new_pw)
        {
            var param = new ManagerInfoModel() { manager_id = current_id , manager_pw = new_pw };
            return mapper.Update("Password.UpdateManagerPassword", param) > 0;
        }

        public UserInfoModel SelectOneByIdEmail(string user_id , string email)
        {
            var param = new UserInfoModel() { user_id = user_id, email = email };
            return mapper.QueryForObject<UserInfoModel>("Password.SelectOneByIdEmail", param);
        }
    }
}