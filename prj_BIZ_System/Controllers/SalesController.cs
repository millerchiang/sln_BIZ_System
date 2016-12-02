using prj_BIZ_System.App_Start;
using prj_BIZ_System.Extensions;
using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace prj_BIZ_System.Controllers
{

    public class SalesController : _BaseController
    {
        public SalesService salesService;

        public PasswordService passwordService;
        public Password_ViewModel passwordViewModel;

        public Sales_ViewModel salesViewModel;
        public SalesPermission_ViewModel salesPermissionViewModel;
        public SalesController()
        {
            salesService = new SalesService();
            salesPermissionViewModel = new SalesPermission_ViewModel();
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
                return Redirect("~/Home/Index");

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
                return Redirect("~/Home/Index");

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
        public ActionResult EditPasswd()
        {
            if (Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");

            return View();
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
            if (Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");

            string sales_id = Request.Cookies["SalesInfo"]["sales_id"];
            salesViewModel.salesInfo = salesService.getSalesInfo(sales_id);
            return View(salesViewModel);
        }

        //修改密碼
        public ActionResult PasswordInsertUpdate(string old_pw, string new_pw)
        {
            if (Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");

            string sales_id = Request.Cookies["SalesInfo"]["sales_id"];

            string errMsg = "修改成功";
            if (!string.IsNullOrEmpty(sales_id))
            {
                SalesInfoModel salesInfo = salesService.getSalesInfo(sales_id);
                if (salesInfo != null)
                {
                    if (salesInfo.sales_pw.Equals(old_pw)) //Todo 之後要用加密後的值比較
                    {
                        if (salesService.UpdateSalesPassword(sales_id, new_pw))
                        {
                            errMsg = "修改成功";
                        }else
                        {
                            errMsg = "修改失敗";
                        }
                    }
                    else
                    {
                        errMsg = "輸入的舊密碼不正確";
                    }
                }
            }
            TempData["pw_errMsg"] = errMsg;

            return Redirect("EditPasswd");
        }

        [HttpPost]
        public ActionResult SalesInfoUpdateBySales(SalesInfoModel model)
        {
            if (Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");

            //if (salesService.isPasswdValidByCheck(old_sales_pw, model.sales_id))
            //{
                model.sales_id = Request.Cookies["SalesInfo"]["sales_id"];
                bool isUpdateSuccess = salesService.UpdateSalesInfoOneBySales(model);
                TempData["salesUpdateResult"] = isUpdateSuccess? "修改成功":"修改失敗";
            //}
            //else
            //{
            //TempData["salesUpdateResult"] = "舊密碼錯誤";
            //}
            if (isUpdateSuccess) {
                Request.Cookies["SalesInfo"]["sales_name"] = model.sales_name;
                Request.Cookies["SalesInfo"].Expires = DateTime.Now.AddMinutes(-1);

                var cookie = new HttpCookie("SalesInfo");
                cookie.Values.Add("id_enable", Request.Cookies["SalesInfo"]["id_enable"]);
                cookie.Values.Add("sales_id", Request.Cookies["SalesInfo"]["sales_id"]);
                cookie.Values.Add("sales_name", HttpUtility.UrlEncode(model.sales_name));
                cookie.Values.Add("limit_of_company", Request.Cookies["SalesInfo"]["limit_of_company"]);
                cookie.Values.Add("limit_of_video", Request.Cookies["SalesInfo"]["limit_of_video"]);
                cookie.Values.Add("limit_of_sales", Request.Cookies["SalesInfo"]["limit_of_sales"]);
                cookie.Values.Add("limit_of_message", Request.Cookies["SalesInfo"]["limit_of_message"]);
                cookie.Values.Add("phone", model.phone);
                cookie.Values.Add("email", model.email);
                cookie.Values.Add("user_id", Request.Cookies["SalesInfo"]["user_id"]);
                cookie.Values.Add("company", Request.Cookies["SalesInfo"]["company"]);
                cookie.Values.Add("company_en", Request.Cookies["SalesInfo"]["company_en"]);
                Response.AppendCookie(cookie);
            }

            return Redirect("SalesInfoBySales");
        }
        #endregion


        [HttpGet]
        public ActionResult Permissions()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            string user_id = Request.Cookies["UserInfo"]["user_id"];


            salesPermissionViewModel.companySalesList = new List<SalesInfoModel>();
            salesPermissionViewModel.videoSalesList = new List<SalesInfoModel>();
            salesPermissionViewModel.salesSalesList = new List<SalesInfoModel>();
            salesPermissionViewModel.messageSalesList = new List<SalesInfoModel>();

            salesPermissionViewModel.unCompanySalesList = new List<SalesInfoModel>();
            salesPermissionViewModel.unVideoSalesList = new List<SalesInfoModel>();
            salesPermissionViewModel.unSalesSalesList = new List<SalesInfoModel>();
            salesPermissionViewModel.unMessageSalesList = new List<SalesInfoModel>();

            IList<SalesInfoModel> sales = salesService.SelectSalesInfos(user_id);
            JavaScriptSerializer jsonParser = new JavaScriptSerializer();
            Dictionary<string, string> limitDict = new Dictionary<string, string>();
            
            ((List<SalesInfoModel>)sales).ForEach(sm => {
                limitDict = jsonParser.Deserialize<Dictionary<string, string>>(sm.limit);
                switch (limitDict["company"])
                {
                    case "1": salesPermissionViewModel.companySalesList.Add(sm); break;
                    default : salesPermissionViewModel.unCompanySalesList.Add(sm); break;
                }
                switch (limitDict["video"])
                {
                    case "1": salesPermissionViewModel.videoSalesList.Add(sm); break;
                    default: salesPermissionViewModel.unVideoSalesList.Add(sm); break;
                }
                switch (limitDict["sales"])
                {
                    case "1": salesPermissionViewModel.salesSalesList.Add(sm); break;
                    default: salesPermissionViewModel.unSalesSalesList.Add(sm); break;
                }
                switch (limitDict["message"])
                {
                    case "1": salesPermissionViewModel.messageSalesList.Add(sm); break;
                    default: salesPermissionViewModel.unMessageSalesList.Add(sm); break;
                }
            });

            //轉換狀態值
            int total_sales = sales.Count;

            if (salesPermissionViewModel.companySalesList.Count == 0)
            {
                salesPermissionViewModel.company = "0";
            }
            else if (salesPermissionViewModel.companySalesList.Count == total_sales)
            {
                salesPermissionViewModel.company = "1";
            }
            else
            {
                salesPermissionViewModel.company = "2";
            }

            if (salesPermissionViewModel.videoSalesList.Count == 0)
            {
                salesPermissionViewModel.video = "0";
            }
            else if (salesPermissionViewModel.videoSalesList.Count == total_sales)
            {
                salesPermissionViewModel.video = "1";
            }
            else
            {
                salesPermissionViewModel.video = "2";
            }

            if (salesPermissionViewModel.salesSalesList.Count == 0)
            {
                salesPermissionViewModel.sales = "0";
            }
            else if (salesPermissionViewModel.salesSalesList.Count == total_sales)
            {
                salesPermissionViewModel.sales = "1";
            }
            else
            {
                salesPermissionViewModel.sales = "2";
            }

            if (salesPermissionViewModel.messageSalesList.Count == 0)
            {
                salesPermissionViewModel.message = "0";
            }
            else if (salesPermissionViewModel.messageSalesList.Count == total_sales)
            {
                salesPermissionViewModel.message = "1";
            }
            else
            {
                salesPermissionViewModel.message = "2";
            }

            return View(salesPermissionViewModel);
        }

        [HttpPost]
        public ActionResult doUpdatePermissions(SalesPermission_ViewModel salesPermissionViewModel)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            string user_id = Request.Cookies["UserInfo"]["user_id"];
            JavaScriptSerializer jsonParser = new JavaScriptSerializer();

            IList<SalesInfoModel> sales = salesService.SelectSalesInfos(user_id);
            Dictionary<string, Dictionary<string, string>> limitDicts = new Dictionary<string, Dictionary<string, string>>();

            foreach (var sm in sales)
            {
                limitDicts.Add(sm.sales_id ,  jsonParser.Deserialize<Dictionary<string, string>>(sm.limit));
            }
            
            switch (salesPermissionViewModel.company)
            {
                case "0":
                    sales.ForEach(sm => limitDicts[sm.sales_id]["company"] = "0");
                    break;
                case "1":
                    sales.ForEach(sm => limitDicts[sm.sales_id]["company"] = "1");
                    break;
                case "2":
                    salesPermissionViewModel.companySalesIds.ForEach(sm => limitDicts[sm]["company"] = "1");
                    salesPermissionViewModel.unCompanySalesIds.ForEach(sm =>limitDicts[sm]["company"] = "0" );
                    break;
            }

            switch (salesPermissionViewModel.video)
            {
                case "0":
                    sales.ForEach(sm =>limitDicts[sm.sales_id]["video"] = "0");
                    break;
                case "1":
                    sales.ForEach(sm =>limitDicts[sm.sales_id]["video"] = "1");
                    break;
                case "2":
                    salesPermissionViewModel.videoSalesIds.ForEach(sm => limitDicts[sm]["video"] = "1");
                    salesPermissionViewModel.unVideoSalesIds.ForEach(sm => limitDicts[sm]["video"] = "0");
                    break;
            }

            switch (salesPermissionViewModel.sales)
            {
                case "0":
                    sales.ForEach(sm =>limitDicts[sm.sales_id]["sales"] = "0");
                    break;
                case "1":
                    sales.ForEach(sm =>limitDicts[sm.sales_id]["sales"] = "1");
                    break;
                case "2":
                    salesPermissionViewModel.salesSalesIds.ForEach(sm =>limitDicts[sm]["sales"] = "1");
                    salesPermissionViewModel.unSalesSalesIds.ForEach(sm =>limitDicts[sm]["sales"] = "0");
                    break;
            }

            switch (salesPermissionViewModel.message)
            {
                case "0":
                    sales.ForEach(sm => limitDicts[sm.sales_id]["message"] = "0" );
                    break;
                case "1":
                    sales.ForEach(sm => limitDicts[sm.sales_id]["message"] = "1" );
                    break;
                case "2":
                    salesPermissionViewModel.messageSalesIds.ForEach(sm => limitDicts[sm]["message"] = "1");
                    salesPermissionViewModel.unMessageSalesIds.ForEach(sm => limitDicts[sm]["message"] = "0");
                    break;
            }

            foreach (KeyValuePair<string,Dictionary<string,string>> kvp in limitDicts) {
                salesService.UpdateSalesPermissions(kvp.Key, kvp.Value);
            }
            
            TempData["salesUpdatePermissionResult"] =  "修改成功" ;

            return Redirect("Permissions");
        }

    }
}