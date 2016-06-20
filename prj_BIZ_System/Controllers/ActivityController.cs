using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using prj_BIZ_System.App_Start;
using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace prj_BIZ_System.Controllers
{
    public class ActivityController : Controller
    {
        public ActivityService activityService;
        public UserService userService;
        public Activity_ViewModel activityModel;

        public ActivityController()
        {
            activityService = new ActivityService();
            userService = new UserService();
            activityModel = new Activity_ViewModel();
        }

        #region 活動資訊
        public ActionResult ActivityInfo()
        {
            activityModel.activityinfo = activityService.GetActivityInfoOne(int.Parse(Request["Id"]));
            if (Request.Cookies["UserInfo"] != null && Request.Cookies["UserInfo"]["user_id"] != null)
            {
                activityModel.activityregister = activityService.GetActivityRegisterSelectOne
                    (int.Parse(Request["Id"]), Request.Cookies["UserInfo"]["user_id"]);
            }
            return View(activityModel);
        }
        #endregion

        ////BuyerInfo

        #region 某活動買主資訊列表
        [HttpGet]
        public ActionResult BuyerInfoActivity()
        {

            activityModel.buyerinfoList = activityService.GetBuyerInfoActivity(int.Parse(Request["Id"]));
            return View(activityModel);

        }
        #endregion



        ////ActivityRegister
        #region 活動報名
        [HttpGet]
        public ActionResult EditActivityRegister()
        {
            activityModel.activityinfoList = activityService.GetActivityInfoList(null);


            activityModel.userinfo = new UserInfoModel();
            activityModel.userinfo.company = Request.Cookies["UserInfo"]["company"];
            activityModel.userinfo.website = Request.Cookies["UserInfo"]["website"];
            activityModel.userinfo.addr = Request.Cookies["UserInfo"]["addr"];
            activityModel.userinfo.info = Request.Cookies["UserInfo"]["info"];

            activityModel.enterprisesortandlistList = activityService.GetEnterpriseSortAndListOne(Request.Cookies["UserInfo"]["user_id"]);
            activityModel.productsortList = userService.getAllProduct(Request.Cookies["UserInfo"]["user_id"]);
            activityModel.cataloglistList = userService.getAllCatalog(Request.Cookies["UserInfo"]["user_id"]);

            activityModel.activityinfo = activityService.GetActivityInfoOne(int.Parse(Request["activity_id"]));

            if (Request["register_id"] == null)
            {
                ViewBag.Action = "EditActivityRegisterInsert";

                activityModel.activityregister = new ActivityRegisterModel();
//                activityModel.activityinfo = new ActivityInfoModel();
                activityModel.activityregister.user_id = Request.Cookies["UserInfo"]["user_id"];

            ViewBag.coverDir = UploadConfig.CatalogRootPath + activityModel.activityregister.user_id + "/" +
                UploadConfig.subDirForCover;
                ViewBag.PageType = "CreateRegister";
                ViewBag.SubmitName = "送出報名";

            }else {
                ViewBag.Action = "EditActivityRegisterUpdate";
                activityModel.activityregister = activityService.GetActivityRegisterOne(int.Parse(Request["register_id"]));
                activityModel.activityregister.user_id = Request.Cookies["UserInfo"]["user_id"];

//                activityModel.activityinfo = activityService.GetActivityInfoOne(int.Parse(Request["activity_id"]));

                activityModel.activityproductselectList = activityService.GetActivityProductSelectList(Request.Cookies["UserInfo"]["user_id"], int.Parse(Request["activity_id"]));
                activityModel.activitycatalogselectList = activityService.GetActivityCatalogSelectList(Request.Cookies["UserInfo"]["user_id"], int.Parse(Request["activity_id"]));
                ViewBag.productselectList = new JavaScriptSerializer().Serialize(activityModel.activityproductselectList);
                if (ViewBag.productselectList == null) {
                    ViewBag.productselectList = "[]";
                }
                ViewBag.catalogselectList = new JavaScriptSerializer().Serialize(activityModel.activitycatalogselectList);
                if (ViewBag.catalogselectList == null)
                {
                    ViewBag.catalogselectList = "[]";
                }

                ViewBag.coverDir = UploadConfig.CatalogRootPath + activityModel.activityregister.user_id + "/" +
                UploadConfig.subDirForCover;
                ViewBag.PageType = "UpdateRegister";
                ViewBag.SubmitName = "確認修改";
            }
            return View(activityModel);
        }

        public ActionResult ActivityRegister()
        {
            activityModel.activityinfoList = activityService.GetActivityInfoList(null);


            activityModel.userinfo = new UserInfoModel();
            activityModel.userinfo.company = Request.Cookies["UserInfo"]["company"];
            activityModel.userinfo.website = Request.Cookies["UserInfo"]["website"];
            activityModel.userinfo.addr = Request.Cookies["UserInfo"]["addr"];
            activityModel.userinfo.info = Request.Cookies["UserInfo"]["info"];

            activityModel.enterprisesortandlistList = activityService.GetEnterpriseSortAndListOne(Request.Cookies["UserInfo"]["user_id"]);
            activityModel.productsortList = userService.getAllProduct(Request.Cookies["UserInfo"]["user_id"]);
            activityModel.cataloglistList = userService.getAllCatalog(Request.Cookies["UserInfo"]["user_id"]);

            activityModel.activityinfo = activityService.GetActivityInfoOne(int.Parse(Request["activity_id"]));

            activityModel.activityregister = activityService.GetActivityRegisterOne(int.Parse(Request["register_id"]));
            activityModel.activityregister.user_id = Request.Cookies["UserInfo"]["user_id"];

            //                activityModel.activityinfo = activityService.GetActivityInfoOne(int.Parse(Request["activity_id"]));

            activityModel.activityproductselectList = activityService.GetActivityProductSelectList(Request.Cookies["UserInfo"]["user_id"], int.Parse(Request["activity_id"]));
            activityModel.activitycatalogselectList = activityService.GetActivityCatalogSelectList(Request.Cookies["UserInfo"]["user_id"], int.Parse(Request["activity_id"]));
            ViewBag.productselectList = new JavaScriptSerializer().Serialize(activityModel.activityproductselectList);
            if (ViewBag.productselectList == null)
            {
                ViewBag.productselectList = "[]";
            }
            ViewBag.catalogselectList = new JavaScriptSerializer().Serialize(activityModel.activitycatalogselectList);
            if (ViewBag.catalogselectList == null)
            {
                ViewBag.catalogselectList = "[]";
            }

            ViewBag.coverDir = UploadConfig.CatalogRootPath + activityModel.activityregister.user_id + "/" +
            UploadConfig.subDirForCover;
            ViewBag.PageType = "DispalyRegister";
            return View(activityModel);
        }



        [HttpPost]
        public ActionResult EditActivityRegisterInsert(ActivityRegisterModel activityRegisterModel, int[] product_id, int[] catalog_no)
        {
            activityRegisterModel.user_id = Request.Cookies["UserInfo"]["user_id"];
//            activityRegisterModel.user_info = Request.Cookies["UserInfo"]["info"];
            activityRegisterModel.manager_check = "0";
            activityRegisterModel.create_time = DateTime.Now;
            activityService.ActivityRegisterInserOne(activityRegisterModel);

            if (product_id != null)
            {
            ActivityProductSelectModel activityProductSelectModel = new ActivityProductSelectModel();
            activityProductSelectModel.user_id = activityRegisterModel.user_id;
            activityProductSelectModel.activity_id = activityRegisterModel.activity_id;
            
                foreach (int id in product_id)
                {
                    activityProductSelectModel.product_id = id;
                    activityService.ActivityProductInsertOne(activityProductSelectModel);
                }
            }

            if (catalog_no != null)
            {
                ActivityCatalogSelectModel activityCatalogSelectModel = new ActivityCatalogSelectModel();
                activityCatalogSelectModel.user_id = activityRegisterModel.user_id;
                activityCatalogSelectModel.activity_id = activityRegisterModel.activity_id;

                foreach (int no in catalog_no)
                {
                    activityCatalogSelectModel.catalog_no = no;
                    activityService.ActivityCatalogInsertOne(activityCatalogSelectModel);
                }
            }
            //            return Content("新增成功");
            return Redirect("ActivityInfo?Id=" + activityRegisterModel.activity_id);
        }

        public ActionResult EditActivityRegisterUpdate(ActivityRegisterModel activityRegisterModel, int[] product_id, int[] catalog_no)
        {
            activityRegisterModel.user_id = Request.Cookies["UserInfo"]["user_id"];
//            activityRegisterModel.user_info = Request.Cookies["UserInfo"]["info"];
//            activityRegisterModel.manager_check = "0";
//            activityRegisterModel.update_time = DateTime.Now;
            activityService.ActivityRegisterUpdateOne(activityRegisterModel);

            activityService.ActivityProductDeleteOne(activityRegisterModel.activity_id, activityRegisterModel.user_id);
            activityService.ActivityCatalogDeleteOne(activityRegisterModel.activity_id, activityRegisterModel.user_id);
            if (product_id != null)
            {
                ActivityProductSelectModel activityProductSelectModel = new ActivityProductSelectModel();
                activityProductSelectModel.user_id = activityRegisterModel.user_id;
                activityProductSelectModel.activity_id = activityRegisterModel.activity_id;
            foreach (int id in product_id)
            {
                activityProductSelectModel.product_id = id;
                activityService.ActivityProductInsertOne(activityProductSelectModel);
            }
            }

            if (catalog_no != null)
            {
            ActivityCatalogSelectModel activityCatalogSelectModel = new ActivityCatalogSelectModel();
            activityCatalogSelectModel.user_id = activityRegisterModel.user_id;
            activityCatalogSelectModel.activity_id = activityRegisterModel.activity_id;

            foreach (int no in catalog_no)
            {
                activityCatalogSelectModel.catalog_no = no;
                activityService.ActivityCatalogInsertOne(activityCatalogSelectModel);
            }
            }
            //            return Content("修改成功");
            return Redirect("ActivityInfo?Id=" + activityRegisterModel.activity_id);
        }

        public ActionResult EditActivityRegisterDelete()
        {
            int activity_id = int.Parse(Request["activity_id"]);
            string user_id = Request.Cookies["UserInfo"]["user_id"];
            activityService.ActivityRegisterDeleteOne(activity_id, user_id);

            activityService.ActivityProductDeleteOne(activity_id, user_id);
            activityService.ActivityCatalogDeleteOne(activity_id, user_id);
            //            return Content("刪除成功");
            return Redirect("ActivityInfo?Id="+ activity_id);
        }


        public ActionResult selectedActivityId(int activity_id)
        {
            activityModel.activityinfo = activityService.GetActivityInfoOne(activity_id);
            return Json(activityModel.activityinfo, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}