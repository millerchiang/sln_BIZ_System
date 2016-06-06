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
            return mapper.QueryForList<ManagerInfoModel>("Manager.getManagerInfoByCondition", param);
        }

        public void ManagerInfoInsertOne(ManagerInfoModel model)
        {
            model.enable = 1;
            mapper.Insert("Manager.InsertManagerInfo", model);
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
        #endregion

        #region 群組管理

        #endregion
    }
}