﻿using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using System.Collections;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;
using prj_BIZ_System.App_Start;

namespace prj_BIZ_System.Services
{
    public class SalesService : _BaseService
    {

        //UserInfoModel******************************************************************************//


        

        public SalesInfoModel getSalesInfo(string sales_id)
        {
            var param = new SalesInfoModel() { sales_id = sales_id };
            return mapper.QueryForObject<SalesInfoModel>("SalesInfo.SelectSalesInfoById", param);
        }

        #region 公司修改

        public IList<SalesInfoModel> getSalesInfoByConditionForManager(string where_sales_id, string where_company)
        {
            var param = new SalesInfoModel() { sales_id = where_sales_id , company = where_company };
            return mapper.QueryForList<SalesInfoModel>("SalesInfo.SelectSalesInfoByCondition", param);
        }

        public IList<SalesInfoModel> getSalesInfoByConditionForACompany(string where_sales_id, string where_sales_name, string user_id)
        {
            var param = new SalesInfoModel() { sales_id = where_sales_id, sales_name = where_sales_name, user_id = user_id};
            return mapper.QueryForList<SalesInfoModel>("SalesInfo.getSalesInfoByConditionForACompany", param);
        }

        public void SalesInfoInsertOne(SalesInfoModel model)
        {
            //底下資料先寫死
            model.sales_pw = "1111"; //臨時給的密碼,之後改
            model.id_enable = "1"; //有效性
            model.limit = "0"; //業務權限關閉
            var param = model;
            mapper.Insert("SalesInfo.InsertSalesInfo", param);
        }

        public int UpdateSalesInfoOneByCompany(SalesInfoModel model)
        {
            var param = model;
            return mapper.Update("SalesInfo.UpdateSalesInfoOneByCompany", param);
        }

        #endregion

        public bool SalesInfoDisableOne(string sales_id, string enable)
        {
            throw new NotImplementedException();
        }


        #region 業務自己修改

        public bool UpdateSalesInfoOneBySales(SalesInfoModel model)
        {
            var param = model;
            return mapper.Update("SalesInfo.UpdateSalesInfoOneBySales", param) > 0;
        }

        public bool isPasswdValidByCheck(string old_sales_pw , string sales_id)
        {
            var md = getSalesInfo(sales_id);
            if (md != null)
            {
                return old_sales_pw.Equals(md.sales_pw);
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}