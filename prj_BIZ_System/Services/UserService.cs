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
    public class UserService
    {
        private DomSqlMapBuilder builder;
        private ISqlMapper mapper;

        public UserService()
        {
            builder = new DomSqlMapBuilder();
            mapper = builder.Configure("DBSource/Config/SqlMap.config");
        }

        /* insert方法*/
        public void UserInfoInsertOne(UserInfoModel userInfoModel)
        {
            mapper.Insert("UserInfo.InsertOne", userInfoModel);
        }

        /* select方法*/
        public IList<EnterpriseSortModel> GetSortList()
        {
            return mapper.QueryForList<EnterpriseSortModel>("UserInfo.SelectAll_sort", null);
        }


        public IList<UserInfoModel> GetUserInfoList()
        {
            return mapper.QueryForList<UserInfoModel>("UserInfo.SelectAll", null);
        }

        public UserInfoModel GeUserInfoOne(string user_id)
        {
            return (UserInfoModel)mapper.QueryForObject("UserInfo.SelectOne", user_id);
        }

        /* delect方法*/
        public int UserInfoDelectOne(string user_id)
        {
            Object obj = mapper.Delete("UserInfo.DeleteOne", user_id);
            return (int)obj;
        }

        /*update方法*/
        public int UserInfoUpdateOne(UserInfoModel userInfoModel)
        {
            Object obj = mapper.Update("UserInfo.UpdateOne", userInfoModel);
            return (int)obj;
        }


    }
}