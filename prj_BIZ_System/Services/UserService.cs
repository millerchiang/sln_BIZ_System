using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;


namespace prj_BIZ_System.Services
{
    public class UserService : _BaseService
    {

        //UserInfoModel******************************************************************************//

        public IList<UserInfoModel> GetUserInfoList()
        {
            return mapper.QueryForList<UserInfoModel>("UserInfo.SelectAll", null);
        }

        public UserInfoModel GeUserInfoOne(string user_id)
        {
            return (UserInfoModel)mapper.QueryForObject("UserInfo.SelectOne", user_id);
        }

        public UserInfoModel ChkUserInfoOne(string user_id, string user_pw)
        {
            UserInfoModel tempModel = new UserInfoModel { user_id = user_id, user_pw = user_pw };
            return (UserInfoModel)mapper.QueryForObject("UserInfo.CheckOne", tempModel);
        }


        public void UserInfoInsertOne(UserInfoModel userInfoModel)
        {
            mapper.Insert("UserInfo.InsertOne", userInfoModel);
        }

        public int UserInfoUpdateOne(UserInfoModel userInfoModel)
        {
            Object obj = mapper.Update("UserInfo.UpdateOne", userInfoModel);
            return (int)obj;
        }

        public int UserInfoDelectOne(string user_id)
        {
            Object obj = mapper.Delete("UserInfo.DeleteOne", user_id);
            return (int)obj;
        }

        //EnterpriseSortModel******************************************************************************//

        public IList<EnterpriseSortModel> GetSortList()
        {
            return mapper.QueryForList<EnterpriseSortModel>("UserInfo.SelectAll_sort", null);
        }


        //UserSortModel******************************************************************************//

        public IList<UserSortModel> SelectUserSortByUserId(string user_id)
        {
            UserSortModel param = new UserSortModel() { user_id = user_id };
            return mapper.QueryForList<UserSortModel>("UserInfo.SelectUserSortByUserId", param);
        }

        public bool RefreshUserSort(string user_id , int[] sort_ids)
        {
            UserSortModel param = new UserSortModel() { user_id = user_id };
            int deleteCount = mapper.Delete("UserInfo.DeleteUserSortByUserId", param);
            UserSortModel tempModel;
            if( sort_ids != null)
            {
                foreach ( int sort_id in sort_ids)
                {
                    tempModel = new UserSortModel { user_id = user_id, sort_id = sort_id };
                    mapper.Insert("UserInfo.InsertUserSortByUserId", tempModel);
                }
            }
            return true;
        }

        //******************************************************************************//
    }
}