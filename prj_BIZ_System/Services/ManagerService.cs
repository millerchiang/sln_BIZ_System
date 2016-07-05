using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace prj_BIZ_System.Services
{
    public class ManagerService : _BaseService
    {

        #region 帳號管理
        public IList<GroupModel> getAllGroup()
        {
            return mapper.QueryForList<GroupModel>("Manager.SelectAllGroup", null);
        }

        public IList<ManagerInfoModel> getManagerInfoByCondition(int? grp_id , string manager_id)
        {
            var param = new ManagerInfoModel() { grp_id = grp_id, manager_id = manager_id };
            return mapper.QueryForList<ManagerInfoModel>("Manager.SelectManagerInfoByCondition", param);
        }

        public void ManagerInfoInsertOne(ManagerInfoModel model)
        {
            model.enable = "1";
            model.manager_pw = "11111"; //預設密碼
            mapper.Insert("Manager.InsertManagerInfo", model);
        }

        public int? getManagerGroup(string current_id)
        {
            var param = new ManagerInfoModel() { manager_id = current_id };
            var obj = mapper.QueryForObject<ManagerInfoModel>("Manager.SelectManagerInfoOne", param);
            return obj.grp_id;
        }

        public ManagerInfoModel getManagerInfo(string current_id)
        {
            var param = new ManagerInfoModel() { manager_id = current_id };
            var obj = mapper.QueryForObject<ManagerInfoModel>("Manager.SelectManagerInfoOne", param);
            return obj;
        }


        public bool ManagerInfoUpdateOne(ManagerInfoModel model)
        {
            return mapper.Update("Manager.UpdateManagerInfo", model) > 0;
        }

        public bool ManagerInfoDeleteOne(string manager_id)
        {
            var param = new ManagerInfoModel() { manager_id = manager_id };
            return mapper.Delete("Manager.DeleteManagerInfo", param) > 0;
        }

        public bool ManagerInfoDisableOne(string manager_id)
        {
            var param = new ManagerInfoModel() { manager_id = manager_id , enable = "0" , grp_id = 0 };
            return mapper.Update("Manager.ManagerInfoDisableOne", param) > 0;
        }

        public ManagerInfoModel ManagerInfoCheckOne(string manager_id,string manager_pw)
        {
            var param = new ManagerInfoModel() { manager_id = manager_id, manager_pw = manager_pw };
            return mapper.QueryForObject<ManagerInfoModel>("Manager.ManagerInfoCheckOne", param);
        }


        #endregion

        #region 群組管理

        public GroupModel GroupSelectOne(int grp_id)
        {
            var param = new GroupModel() { grp_id = grp_id };
            return mapper.QueryForObject<GroupModel>("Manager.SelectGroupOne", param);
        }


        public IList<ManagerInfoModel> getManagerInfoByGrpId(int grp_id)
        {
            var param = new GroupModel() { grp_id = grp_id };
            return mapper.QueryForList<ManagerInfoModel>("Manager.GetManagerInfoByGrpId", param);
        }


        public void GroupInsertOne(string grp_name, Dictionary<string, string> limits)
        {
            string limits_str = new JavaScriptSerializer().Serialize(limits);
            var param = new GroupModel() { grp_name = grp_name, limit = limits_str };
            mapper.Insert("Manager.InsertGroup", param);
        }

        public bool GroupUpdateOne(int? grp_id , string grp_name, Dictionary<string, string> limits)
        {
            string limits_str = new JavaScriptSerializer().Serialize(limits);
            var param = new GroupModel() { grp_id = grp_id , grp_name = grp_name, limit = limits_str };
            return mapper.Update("Manager.UpdateGroup", param) > 0 ;
        }

        internal bool GroupDeleteOne(int grp_id)
        {
            var param = new GroupModel { grp_id = grp_id };
            return mapper.Delete("Manager.DeleteGroup", param) > 0;
        }

        #endregion
    }
}