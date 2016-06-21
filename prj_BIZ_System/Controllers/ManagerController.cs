using Newtonsoft.Json;
using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using prj_BIZ_System.App_Start;
using System.Collections;


namespace prj_BIZ_System.Controllers
{
    public class ManagerController : Controller
    {
        public ActivityService activityService;
        public ManagerService managerService;
        public UserService userService;
        public Manager_ViewModel managerViewModel;
        public Manager_Activity_ViewModel activityModel;

        public ManagerController()
        {
            userService = new UserService();
            managerService = new ManagerService();
            managerViewModel = new Manager_ViewModel();
            activityService = new ActivityService();
            activityModel = new Manager_Activity_ViewModel();
            ViewBag.Form = "Manager";

        }

        public ActionResult Logout()
        {

            Session.Clear();
            HttpCookie aCookie;
            string cookieName;
//            int limit = Request.Cookies.Count;
//            for (int i = 0; i < limit; i++)
            {
                cookieName = "ManagerInfo";// Request.Cookies[i].Name;
                aCookie = new HttpCookie(cookieName);
                aCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(aCookie);
            }
            return Redirect("Login");
        }


        public ActionResult Login()
        {
            return View();
        }

        public ActionResult IdentifyManager()
        {
            ManagerInfoModel model = managerService.ManagerInfoCheckOne(Request["manager_id"], Request["manager_pw"]);

            HttpCookie cookie = null;

            if (model == null)
            {
                return Redirect("Login");
            }
            else
            {
                cookie = new HttpCookie("ManagerInfo");
                cookie.Values.Add("manager_id", model.manager_id);
                cookie.Values.Add("name", model.name);
                cookie.Values.Add("phone", model.phone);
                cookie.Values.Add("email", model.email);

                Dictionary<string, string> limitsDict = new Dictionary<string, string>();
                limitsDict = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(model.limit);

                cookie.Values.Add("user", limitsDict["user"]);
                cookie.Values.Add("activity", limitsDict["activity"]);
                cookie.Values.Add("push", limitsDict["push"]);
                cookie.Values.Add("news", limitsDict["news"]);
                cookie.Values.Add("manager", limitsDict["manager"]);
                cookie.Values.Add("statistic", limitsDict["statistic"]);

                Response.AppendCookie(cookie);
            }
//            return Redirect("ManagerInfo");
           return Redirect("Index");
        }


        public ActionResult Index()
        {
            return View();
        }


        #region ManagerInfo 帳號管理
        // GET: ManagerInfo
        public ActionResult ManagerInfo(int? where_grp_id , string where_manager_id)
        {
            ViewBag.Title = "ManagerInfo";
            managerViewModel.groupList = managerService.getAllGroup();
            managerViewModel.managerInfoList = managerService.getManagerInfoByCondition(where_grp_id, where_manager_id).Pages(Request, this);
            ViewBag.Where_GroupId = where_grp_id;
            ViewBag.Where_ManagerId = where_manager_id;
            return View(managerViewModel);
        }

        [HttpPost]
        public ActionResult ManagerInfoInsertUpdate(string pagetype, ManagerInfoModel model)
        {
            model.create_manager = "admin";  // Request.Cookies["UserInfo"]["manager_id"]
            if ("Insert".Equals(pagetype))
            {
                managerService.ManagerInfoInsertOne(model);
                return Redirect("ManagerInfo");
            }
            else if ("Update".Equals(pagetype))
            {
                bool isUpdateSuccess = managerService.ManagerInfoUpdateOne(model);
                return Json(isUpdateSuccess);
            }
            return Redirect("ManagerInfo");
        }

        public ActionResult DeleteManagerInfoJson(string manager_id)
        {
            //非真的刪 , 只是停用
            bool isDelSuccess = managerService.ManagerInfoDisableOne(manager_id);
            return Json(isDelSuccess, JsonRequestBehavior.AllowGet);
        }



        #endregion

        #region Group 群組管理

        // GET: Group
        public ActionResult Group()
        {
            managerViewModel.groupList = managerService.getAllGroup();

            return View(managerViewModel);
        }

        public ActionResult GetGroupDetail(int grp_id)
        {
            managerViewModel.groupList = managerService.getAllGroup();
            GroupModel gp_model = managerService.GroupSelectOne(grp_id);
            Dictionary<string, object> result = new Dictionary<string, object>();
            Dictionary<string, string> limitsDict = new Dictionary<string, string>();

            if (gp_model != null)
            {
                limitsDict = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(gp_model.limit);
            }

            IList<ManagerInfoModel> manager_model_list = managerService.getManagerInfoByGrpId(grp_id);
            result.Add("memangers", manager_model_list);
            result.Add("limits", limitsDict);

            return Json( result , JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GroupInsertUpdate(
              int? grp_id   , string grp_name   , string user    , string activity 
            , string push   , string news       , string manager , string statistic
        )
        {
            Dictionary<string, string> limits = new Dictionary<string, string>();
            limits.Add("user", user);
            limits.Add("activity", activity);
            limits.Add("push", push);
            limits.Add("news", news);
            limits.Add("manager", manager);
            limits.Add("statistic", statistic);

            if(grp_id == null)
            {
                managerService.GroupInsertOne(grp_name , limits);
                return Redirect("Group");
            }
            else
            {
                bool isUpdateSuccess = managerService.GroupUpdateOne( grp_id , grp_name , limits);
                return Json(isUpdateSuccess);
            }
        }

        public ActionResult DeleteGroupJson(int grp_id)
        {
            bool isDelSuccess = managerService.GroupDeleteOne(grp_id);
            return Json(isDelSuccess, JsonRequestBehavior.AllowGet);
        }
        #endregion


        ////News
        #region 新聞列表*/
        [HttpGet]
        public ActionResult B_NewsList()
        {
            string manager_id = null;
            int? grp_id = null;
            if (Request.Cookies["ManagerInfo"]["news"] == "2")
            {
                manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
                grp_id = managerService.getManagerGroup(Request.Cookies["ManagerInfo"]["manager_id"]);
            }
            activityModel.newsList = activityService.GetNewsType(Request["news_type"], grp_id);
            return View(activityModel);
        }
        #endregion

        #region 新增修改刪除活動訊息
        [HttpGet]
        public ActionResult EditNewsActivity()
        {
            string manager_id = null;
            int? grp_id = null;
            if (Request.Cookies["ManagerInfo"]["news"] == "2")
            {
                manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
                grp_id = managerService.getManagerGroup(Request.Cookies["ManagerInfo"]["manager_id"]);
            }
            activityModel.activityinfoList = activityService.GetActivityInfoList(grp_id);
            ViewBag.Action = "EditNewsActivityInsertUpdate";
            if (Request["Id"] == null)
            {
                activityModel.news = new NewsModel();
                ViewBag.PageType = "Create";
                ViewBag.SubmitName = "新增";
            }
            else {

                activityModel.news = activityService.GetNewsOne(int.Parse(Request["Id"]));
                ViewBag.PageType = "Edit";
                ViewBag.SubmitName = "修改";
            }
            return View(activityModel);
        }

        [HttpPost]
        public ActionResult EditNewsActivityInsertUpdate(NewsModel model)
        {
            model.manager_id = Request.Cookies["ManagerInfo"]["manager_id"];

            if (model.news_no == 0)
                activityService.NewsInsertOne(model);
            else
                activityService.NewsUpdateOne(model);

            return Redirect("B_NewsList?news_type=0");
        }

        [HttpGet]
        public ActionResult EditNewsActivityDelete()
        {
            activityService.NewsDeleteOne(int.Parse(Request["Id"]));
            return Redirect("B_NewsList?news_type=0");
        }
        #endregion

        #region 新增修改刪除新聞訊息
        [HttpGet]
        public ActionResult EditNewsInfo()
        {
            ViewBag.Action = "EditNewsInfoInsertUpdate";
            if (Request["Id"] == null)
            {
                activityModel.news = new NewsModel();
                ViewBag.PageType = "Create";
                ViewBag.SubmitName = "新增";
            }
            else {
                //                activityModel.news.news_no = int.Parse(Request["Id"]);
                activityModel.news = activityService.GetNewsOne(int.Parse(Request["Id"]));
                //                activityModel.news.content = HttpUtility.HtmlDecode(activityModel.news.content);
                ViewBag.PageType = "Edit";
                ViewBag.SubmitName = "修改";
            }
            return View(activityModel.news);
        }

        [HttpPost]
        public ActionResult EditNewsInfoInsertUpdate(NewsModel news)
        {
            //            activityModel.news.news_no = int.Parse(Request["news_no"]);
            /*
                        news.news_title = Request["news_title"];
                        news.news_date = DateTime.Parse(Request["news_date"]);
                        news.website = Request["website"];
                        news.news_type = Request["news_type"];
                        news.content = Request["content"];
            */
            news.manager_id = Request.Cookies["ManagerInfo"]["manager_id"];

            if (news.news_no == 0)
                activityService.NewsInsertOne(news);
            else
                activityService.NewsUpdateOne(news);

            return Redirect("B_NewsList?news_type=1");
        }

        [HttpGet]
        public ActionResult EditNewsInfoDelete()
        {
            activityService.NewsDeleteOne(int.Parse(Request["Id"]));
            return Redirect("B_NewsList?news_type=1");
        }
        #endregion

        #region 新增修改刪除活動
        [HttpGet]
        public ActionResult EditActivity()
        {

            ViewBag.Action = "ActivityInsertUpdate";
            if (Request["Id"] == null)
            {
                activityModel.activityinfo = new ActivityInfoModel();
                ViewBag.PageType = "Create";
                ViewBag.SubmitName = "新增";
            }
            else {
                activityModel.activityinfo = activityService.GetActivityInfoOne(int.Parse(Request["Id"]));
                ViewBag.PageType = "Edit";
                ViewBag.SubmitName = "修改";
            }

            return View(activityModel.activityinfo);
        }

        public ActionResult DeleteActivity()
        {
            activityService.ActivityInfoDelectOne(int.Parse(Request["Id"]));
            return Redirect("ActivityList");
        }

        [HttpPost]
        public ActionResult ActivityInsertUpdate(ActivityInfoModel model)
        {
            model.manager_id = Request.Cookies["ManagerInfo"]["manager_id"];

            if (model.activity_id == 0)
                activityService.ActivityInfoInsertOne(model);
            else
            {
                model.update_time = DateTime.Now;
                activityService.ActivityInfoUpdateOne(model);

            }
            return Redirect("ActivityList");
        }
        #endregion

        ////Activityinfo

        #region 活動資訊
        public ActionResult ActivityInfo()
        {
            activityModel.activityinfo = activityService.GetActivityInfoOne(int.Parse(Request["Id"]));
            return View(activityModel);
        }
        #endregion

        #region 活動列表
        public ActionResult ActivityList()
        {
            string manager_id = null;
            int? grp_id = null;
            if (Request.Cookies["ManagerInfo"]["activity"] == "2")
            {
                manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
                grp_id = managerService.getManagerGroup(Request.Cookies["ManagerInfo"]["manager_id"]);
            }


            activityModel.activityinfoList = activityService.GetActivityInfoList(grp_id);
            return View(activityModel);
        }
        #endregion

        #region 活動報名審核

        [HttpGet]
        public ActionResult ActivityRegisterCheck(string selectActivityName, string selectCompany,string startDate,string endDate)
        {
            string manager_id = null;
            int? grp_id = null;
            if (Request.Cookies["ManagerInfo"]["activity"] == "2")
            {
                manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
                grp_id = managerService.getManagerGroup(Request.Cookies["ManagerInfo"]["manager_id"]);
            }
            activityModel.activityregisterList = activityService.GetActivityCheckAllByCondition(selectActivityName, selectCompany, startDate, endDate, grp_id);
            ViewBag.Where_ActivityName = selectActivityName;
            ViewBag.Where_Company = selectCompany;
            return View(activityModel);
        }

        [HttpPost]
        public ActionResult EditActivityRegisterUpdateChk(ActivityRegisterModel model, int register_id, string manager_check)
        {
            model.register_id = register_id;
            model.manager_check = manager_check;
            activityService.ActivityRegisterUpdateOneChk(model);
            return Redirect("ActivityRegisterCheck");
        }
        #endregion

        #region 活動報名查詢
        public ActionResult GetRegisterSearchByActivityName(string term, string selectActivityName, string selectCompany, string startDate, string endDate)
        {
            string manager_id = null;
            int? grp_id = null;
            if (Request.Cookies["ManagerInfo"]["activity"] == "2")
            {
                manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
                grp_id = managerService.getManagerGroup(Request.Cookies["ManagerInfo"]["manager_id"]);
            }
            activityModel.activityregisterList = activityService.GetActivityCheckAllByCondition(selectActivityName, selectCompany, startDate, endDate,grp_id);
            ArrayList activityNameList = new ArrayList();


            foreach (ActivityRegisterModel model in activityModel.activityregisterList)
            {
                activityNameList.Add(model.activity_name);

            }
            string[] activityNameItems = (string[])activityNameList.ToArray(typeof(string));

            var fiilteredItems = activityNameItems.Where(
                item => item.IndexOf(term,
                StringComparison.InvariantCultureIgnoreCase) >= 0
                );

            return Json(fiilteredItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegisterSearchByCompany(string term, string selectActivityName, string selectCompany, string startDate, string endDate)
        {
            string manager_id = null;
            int? grp_id = null;
            if (Request.Cookies["ManagerInfo"]["activity"] == "2")
            {
                manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
                grp_id = managerService.getManagerGroup(Request.Cookies["ManagerInfo"]["manager_id"]);
            }
            activityModel.activityregisterList = activityService.GetActivityCheckAllByCondition(selectActivityName, selectCompany, startDate, endDate,grp_id);
            ArrayList companyList = new ArrayList();
            foreach (ActivityRegisterModel model in activityModel.activityregisterList)
            {

                companyList.Add(model.company);
            }

            string[] companyItems = (string[])companyList.ToArray(typeof(string));

            var fiilteredItems = companyItems.Where(
                item => item.IndexOf(term,
                StringComparison.InvariantCultureIgnoreCase) >= 0
                );

            return Json(fiilteredItems, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 使用者資料管理

        public ActionResult UserList(string user_id, string company)
        {
            activityModel.userinfoList = userService.GetUserInfoListkw(user_id,company);
            return View(activityModel);
        }
        public ActionResult DeleteUser()
        {
            userService.UserInfoDelectOne(Request["user_id"]);
            return Redirect("UserList");
        }

        [HttpGet]
        public ActionResult UserEdit()
        {
            activityModel.enterprisesortList = userService.GetSortList();
            ViewBag.Action = "UserInsertUpdate";
            string userid = Request["user_id"];

            HttpCookie cookie = new HttpCookie("Action");

            if (userid == null) //新增
            {
                ViewBag.tname = "會員註冊";
                activityModel.userinfo = new UserInfoModel();
                ViewBag.PageType = "Create";
                ViewBag.SubmitName = "確定送出";
                cookie.Values.Add("edit", "Add");
                ViewBag.userSortList = "[]";
            }
            else //修改
            {
                ViewBag.tname = "會員資料";
                activityModel.userinfo = userService.GeUserInfoOne(userid);
                ViewBag.user = activityModel.userinfo;
                activityModel.usersortList = userService.SelectUserSortByUserId(activityModel.userinfo.user_id);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ViewBag.userSortList = serializer.Serialize(activityModel.usersortList);
                ViewBag.logoDir = UploadHelper.getPictureDirPath(activityModel.userinfo.user_id, "logo");
                if (ViewBag.userSortList == null)
                {
                    ViewBag.userSortList = "[]";
                }
                ViewBag.PageType = "Edit";
                ViewBag.SubmitName = "修改";
                cookie.Values.Add("edit", "Update");
                cookie.Values.Add("user_id", userid);
            }

            Response.AppendCookie(cookie);
            return View(activityModel);
        }


        [HttpPost]
        public ActionResult UserInsertUpdate(UserInfoModel model, int[] sort_id, HttpPostedFileBase logo_img)
        {
            if (Request.Cookies["Action"]["edit"] == "Add")//新增
            {
                if (logo_img != null && logo_img.ContentLength > 0 && !string.IsNullOrEmpty(model.user_id))
                {
                    UploadHelper.doUploadFile(logo_img, UploadConfig.subDirForLogo, model.user_id);
                    model.logo_img = logo_img.FileName;
                }
                var id = userService.UserInfoInsertOne(model);

            }
            else //修改
            {
                string current_user_id = model.user_id;
                var old_model = userService.GeUserInfoOne(current_user_id);
                model.update_time = DateTime.Now;
//                model.user_id = current_user_id;
                if (logo_img != null && logo_img.ContentLength > 0 && !string.IsNullOrEmpty(current_user_id))
                {
                    if (old_model.logo_img != null)
                        UploadHelper.deleteUploadFile(old_model.logo_img, "logo", current_user_id);
                    UploadHelper.doUploadFile(logo_img, UploadConfig.subDirForLogo, model.user_id);
                    model.logo_img = logo_img.FileName;
                }
                userService.UserInfoUpdateOne(model);

            }

            bool refreshResult = userService.RefreshUserSort(model.user_id, sort_id);
            return Redirect("UserList");
        }

        #endregion


        #region 買主資訊列表
        [HttpGet]
        public ActionResult BuyerInfoList()
        {
            string manager_id = null;
            int? grp_id = null;
            if (Request.Cookies["ManagerInfo"]["activity"] == "2")
            {
                manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
                grp_id = managerService.getManagerGroup(Request.Cookies["ManagerInfo"]["manager_id"]);
            }
            activityModel.buyerinfoList = activityService.GetBuyerInfoAll(grp_id);
            return View(activityModel);

        }
        #endregion

        #region 新增修改刪除買主資訊*/
        [HttpGet]
        public ActionResult EditBuyerInfo()
        {
            string manager_id = null;
            int? grp_id = null;
            if (Request.Cookies["ManagerInfo"]["activity"] == "2")
            {
                manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
                grp_id = managerService.getManagerGroup(Request.Cookies["ManagerInfo"]["manager_id"]);
            }
            activityModel.userinfotoidandcpList = activityService.GetUserInfoToIdandCp();
            activityModel.activityinfoList = activityService.GetActivityInfoList(grp_id);
            ViewBag.Action = "EditBuyerInfoInsertUpdate";
            if (Request["Id"] == null)
            {
                activityModel.buyerinfo = new BuyerInfoModel();
                ViewBag.PageType = "Create";
                ViewBag.SubmitName = "新增";
            }
            else {
                activityModel.buyerinfo = activityService.GetBuyerInfoOne(int.Parse(Request["Id"]));
                ViewBag.PageType = "Edit";
                ViewBag.SubmitName = "修改";
            }
            return View(activityModel);
        }

        [HttpPost]
        public ActionResult EditBuyerInfoInsertUpdate(BuyerInfoModel model)
        {
            if (model.serial_no == 0)
            {
                activityService.BuyerInfoInsertOne(model);
            }
            else {
                activityService.BuyerInfoUpdateOne(model);
            }
            return Redirect("BuyerInfoList");
        }

        public ActionResult GetUserInfoToIdCp(string term)
        {
            activityModel.userinfotoidandcpList = activityService.GetUserInfoToIdandCp();
            ArrayList arrayList = new ArrayList();

            foreach (UserInfoToIdAndCpModel model in activityModel.userinfotoidandcpList)
            {
                arrayList.Add(model.user_id + "," + model.company);
            }

            string[] items = (string[])arrayList.ToArray(typeof(string));

            var filteredItems = items.Where(
                item => item.IndexOf(term,
                StringComparison.InvariantCultureIgnoreCase) >= 0
            );
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditBuyerInfoDelete()
        {
            activityService.BuyerInfoDeleteOne(int.Parse(Request["Id"]));
            return Redirect("BuyerInfoList");
        }
        #endregion



        #region 新聞文字編輯器圖片上傳
        public ActionResult NewsInfoUpload(HttpPostedFileBase upload, string CKEditorFuncNum)
        {
            string result = "";
            string manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
            UploadHelper.doUploadFile(upload, UploadConfig.subDirForNews, manager_id);

            var imageUrl = Url.Content(UploadConfig.CatalogRootPath + manager_id + "/" + UploadConfig.subDirForNews + upload.FileName);

            var vMessage = string.Empty;

            result = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + imageUrl + "\", \"" + vMessage + "\");</script></body></html>";
            return Content(result);
        }
        #endregion


    }
}