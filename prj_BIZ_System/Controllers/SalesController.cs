using prj_BIZ_System.App_Start;
using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace prj_BIZ_System.Controllers
{

    public class SalesController : _BaseController
    {
        public SalesService salesService;
        public Sales_ViewModel salesModel;

        public PasswordService passwordService;
        public Password_ViewModel passwordViewModel;

        public Sales_ViewModel salesViewModel;

        public SalesController()
        {
            salesService = new SalesService();
            salesModel = new Sales_ViewModel();

            passwordService = new PasswordService();
            passwordViewModel = new Password_ViewModel();

            salesViewModel = new Sales_ViewModel();
        }

        #region 業務帳號ByCompany管理
        public ActionResult CheckSales(string sales_id)
        {
            SalesInfoModel kk = salesService.getSalesInfo(sales_id);
            if (kk == null)
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(kk, JsonRequestBehavior.AllowGet);

        }
        // GET: SalesInfo

        /* 

        */

        // GET: SalesInfo
        public ActionResult SalesInfoByCompany(string where_sales_id, string where_sales_name)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            string user_id = Request.Cookies["UserInfo"]["user_id"];
            ViewBag.Title = "SalesInfoByCompany";
            //salesViewModel.groupList = salesInfoService.getAllGroup();
            salesViewModel.salesInfoList = salesService.getSalesInfoByConditionForACompany(where_sales_id, where_sales_name, user_id).Pages(Request, this, 10);
            ViewBag.Where_sales_id = where_sales_id;
            ViewBag.Where_sales_name = where_sales_name;
            return View(salesViewModel);
        }

        [HttpPost]
        public ActionResult SalesInfoInsertUpdateByCompany(string pagetype, SalesInfoModel model)
        {
            //if (Request.Cookies["SalesInfo"] == null)
            //return Redirect("Login");
            //model.create_sales = Request.Cookies["SalesInfo"]["manager_id"];
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            if (Request.Cookies["UserInfo"] != null)
                model.user_id = Request.Cookies["UserInfo"]["user_id"];

            if ("Insert".Equals(pagetype))
            {
                salesService.SalesInfoInsertOne(model);
                //                return Redirect("SalesInfo");
            }
            else if ("Update".Equals(pagetype))
            {
                salesService.UpdateSalesInfoOneByCompany(model);
                //                bool isUpdateSuccess = salesInfoService.SalesInfoUpdateOne(model);
                //                return Json(isUpdateSuccess);
            }
            return Redirect("SalesInfoByCompany");
        }

        public ActionResult DeleteSalesInfoJson(string sales_id, string enable)
        {
           // if (Request.Cookies["SalesInfo"] == null)
                //return Redirect("Login");
            //非真的刪 , 只是停用
            bool isDelSuccess = salesService.SalesInfoDisableOne(sales_id, enable);
            return Json(isDelSuccess, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 業務自己管理

        public ActionResult SalesInfoBySales()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");
            return View();
        }

        [HttpPost]
        public ActionResult SalesInfoUpdateBySales(string old_sales_pw , SalesInfoModel model)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            if (salesService.isPasswdValidByCheck(old_sales_pw, model.sales_id))
            {
                bool isUpdateSuccess = salesService.UpdateSalesInfoOneBySales(model);
                TempData["salesUpdateResult"] = isUpdateSuccess? "修改成功":"修改失敗";
            }
            else
            {
                TempData["salesUpdateResult"] = "舊密碼錯誤";
            }
            return Redirect("SalesInfo");
        }
        #endregion


    }
}