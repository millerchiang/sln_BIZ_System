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

        #region 業務帳號管理
        public ActionResult CheckSales(string sales_id)
        {
            SalesInfoModel kk = salesService.getSalesInfo(sales_id);
            if (kk == null)
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(kk, JsonRequestBehavior.AllowGet);

        }


        // GET: SalesInfo
        public ActionResult SalesInfo(string where_sales_id, string where_company)
        {
            //if (Request.Cookies["SalesInfo"] == null)
                //return Redirect("Login");
            ViewBag.Title = "SalesInfo";
            //salesViewModel.groupList = salesInfoService.getAllGroup();
            salesViewModel.salesInfoList = salesService.getSalesInfoByCondition(where_sales_id, where_company);//.Pages(Request, this, 10);
            ViewBag.Where_sales_id = where_sales_id;
            ViewBag.Where_company = where_company;
            return View(salesViewModel);
        }

        [HttpPost]
        public ActionResult SalesInfoInsertUpdate(string pagetype, SalesInfoModel model)
        {
            //if (Request.Cookies["SalesInfo"] == null)
                //return Redirect("Login");
            //model.create_sales = Request.Cookies["SalesInfo"]["manager_id"];
            if ("Insert".Equals(pagetype))
            {
                salesService.SalesInfoInsertOne(model);
                //                return Redirect("SalesInfo");
            }
            else if ("Update".Equals(pagetype))
            {
                salesService.SalesInfoUpdateOne(model);
                //                bool isUpdateSuccess = salesInfoService.SalesInfoUpdateOne(model);
                //                return Json(isUpdateSuccess);
            }
            return Redirect("SalesInfo");
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
    }
}