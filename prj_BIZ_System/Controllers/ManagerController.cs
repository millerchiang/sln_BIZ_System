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
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;

namespace prj_BIZ_System.Controllers
{
    public class ManagerController : Controller
    {
        public ActivityService activityService;
        public ManagerService managerService;
        public UserService userService;
        public Manager_ViewModel managerViewModel;
        public Manager_Activity_ViewModel activityModel;
        public Match_ViewModel matchModel;
        public MatchService matchService;
        private const int notFoundIndex = 999;

        public PasswordService passwordService;
        public Password_ViewModel passwordViewModel;

        public ManagerController()
        {
            userService = new UserService();
            managerService = new ManagerService();
            managerViewModel = new Manager_ViewModel();
            activityService = new ActivityService();
            activityModel = new Manager_Activity_ViewModel();

            matchService = new MatchService();
            matchModel = new Match_ViewModel();

            passwordService = new PasswordService();
            passwordViewModel = new Password_ViewModel();

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

                TempData["pw_errMsg"] = "帳號或密碼錯誤!!";

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
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");
            return View();
        }


        #region ManagerInfo 帳號管理
        // GET: ManagerInfo
        public ActionResult ManagerInfo(int? where_grp_id , string where_manager_id)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");
            ViewBag.Title = "ManagerInfo";
            managerViewModel.groupList = managerService.getAllGroup();
            managerViewModel.managerInfoList = managerService.getManagerInfoByCondition(where_grp_id, where_manager_id).Pages(Request, this,10);
            ViewBag.Where_GroupId = where_grp_id;
            ViewBag.Where_ManagerId = where_manager_id;
            return View(managerViewModel);
        }

        [HttpPost]
        public ActionResult ManagerInfoInsertUpdate(string pagetype, ManagerInfoModel model)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");
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
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");
            //非真的刪 , 只是停用
            bool isDelSuccess = managerService.ManagerInfoDisableOne(manager_id);
            return Json(isDelSuccess, JsonRequestBehavior.AllowGet);
        }



        #endregion

        #region Group 群組管理

        // GET: Group
        public ActionResult Group()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            managerViewModel.groupList = managerService.getAllGroup().Pages(Request, this,10);

            return View(managerViewModel);
        }

        public ActionResult GetGroupDetail(int grp_id)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");
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
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");
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
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");
            bool isDelSuccess = managerService.GroupDeleteOne(grp_id);
            return Json(isDelSuccess, JsonRequestBehavior.AllowGet);
        }
        #endregion


        ////News
        #region 新聞列表*/
        [HttpGet]
        public ActionResult B_NewsList()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");
            string manager_id = null;
            int? grp_id = null;
            if (Request.Cookies["ManagerInfo"]["news"] == "2")
            {
                manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
                grp_id = managerService.getManagerGroup(Request.Cookies["ManagerInfo"]["manager_id"]);
            }
            activityModel.newsList = activityService.GetNewsType(Request["news_type"], grp_id).Pages(Request, this, 10);
            return View(activityModel);
        }
        #endregion

        #region 新增修改刪除活動訊息
        [HttpGet]
        public ActionResult EditNewsActivity()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");
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
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");
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
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            activityService.NewsDeleteOne(int.Parse(Request["Id"]));
            return Redirect("B_NewsList?news_type=0");
        }
        #endregion

        #region 新增修改刪除新聞訊息
        [HttpGet]
        public ActionResult EditNewsInfo()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");
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
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

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
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            activityService.NewsDeleteOne(int.Parse(Request["Id"]));
            return Redirect("B_NewsList?news_type=1");
        }
        #endregion

        #region 新增修改刪除活動
        [HttpGet]
        public ActionResult EditActivity()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

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
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            activityService.ActivityInfoDelectOne(int.Parse(Request["Id"]));
            return Redirect("ActivityList");
        }

        [HttpPost]
        public ActionResult ActivityInsertUpdate(ActivityInfoModel model)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

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
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            activityModel.activityinfo = activityService.GetActivityInfoOne(int.Parse(Request["Id"]));
            return View(activityModel);
        }
        #endregion

        #region 活動列表
        public ActionResult ActivityList()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            string manager_id = null;
            int? grp_id = null;
            if (Request.Cookies["ManagerInfo"]["activity"] == "2")
            {
                manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
                grp_id = managerService.getManagerGroup(Request.Cookies["ManagerInfo"]["manager_id"]);
            }


            activityModel.activityinfoList = activityService.GetActivityInfoList(grp_id).Pages(Request, this, 10); 
            return View(activityModel);
        }
        #endregion

        #region 活動報名審核

        [HttpGet]
        public ActionResult ActivityRegisterCheck(string selectActivityName, string selectCompany,string startDate,string endDate)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            string manager_id = null;
            int? grp_id = null;
            if (Request.Cookies["ManagerInfo"]["activity"] == "2")
            {
                manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
                grp_id = managerService.getManagerGroup(Request.Cookies["ManagerInfo"]["manager_id"]);
            }
            activityModel.activityregisterList = activityService.GetActivityCheckAllByCondition(selectActivityName, selectCompany, startDate, endDate, grp_id).Pages(Request, this, 10);
            ViewBag.Where_ActivityName = selectActivityName;
            ViewBag.Where_Company = selectCompany;
            return View(activityModel);
        }

        [HttpPost]
        public ActionResult EditActivityRegisterUpdateChk(ActivityRegisterModel model, int register_id, string manager_check)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            model.register_id = register_id;
            model.manager_check = manager_check;
            activityService.ActivityRegisterUpdateOneChk(model);
            return Redirect("ActivityRegisterCheck");
        }
        #endregion

        #region 活動報名查詢
        public ActionResult GetRegisterSearchByActivityName(string term, string selectActivityName, string selectCompany, string startDate, string endDate)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

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
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

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
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            activityModel.userinfoList = userService.GetUserInfoListkw(user_id,company).Pages(Request, this, 10);
            return View(activityModel);
        }
        public ActionResult DeleteUser()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            userService.UserInfoDelectOne(Request["user_id"]);
            return Redirect("UserList");
        }

        [HttpGet]
        public ActionResult CheckUser(string user_id)
        {
            bool Huser = true;
            activityModel.userinfo = userService.GeUserInfoOne(user_id);
            if (activityModel.userinfo==null || activityModel.userinfo.user_id == null)
                Huser = false;
            return Json(Huser, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UserEdit()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

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
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

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
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            string manager_id = null;
            int? grp_id = null;
            if (Request.Cookies["ManagerInfo"]["activity"] == "2")
            {
                manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
                grp_id = managerService.getManagerGroup(Request.Cookies["ManagerInfo"]["manager_id"]);
            }
            activityModel.buyerinfoList = activityService.GetBuyerInfoAll(grp_id).Pages(Request, this, 10);
            return View(activityModel);

        }
        #endregion

        #region 新增修改刪除買主資訊*/
        [HttpGet]
        public ActionResult EditBuyerInfo()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

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
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

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
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

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
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            activityService.BuyerInfoDeleteOne(int.Parse(Request["Id"]));
            return Redirect("BuyerInfoList");
        }
        #endregion



        #region 新聞文字編輯器圖片上傳
        public ActionResult NewsInfoUpload(HttpPostedFileBase upload, string CKEditorFuncNum)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            string result = "";
            string manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
            UploadHelper.doUploadFile(upload, UploadConfig.subDirForNews, manager_id);

            var imageUrl = Url.Content(UploadConfig.CatalogRootPath + manager_id + "/" + UploadConfig.subDirForNews + upload.FileName);

            var vMessage = string.Empty;

            result = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + imageUrl + "\", \"" + vMessage + "\");</script></body></html>";
            return Content(result);
        }
        #endregion

        #region 會員資料匯入
        public ActionResult UserInfoImport()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            return View();
        }

        [HttpPost]
        public ActionResult UserInfoMultiInsert(HttpPostedFileBase iupexl, string upexl_name)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            if (iupexl != null && iupexl.FileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase) && iupexl.ContentLength > 0)
            {
                string targetDir = "_temp";
                Dictionary<string, string> uploadResultDic = null;
                uploadResultDic = UploadHelper.doUploadFile(iupexl, targetDir, "admin");

                if ("success".Equals(uploadResultDic["result"]))
                {
                    Dictionary<string, object> result = userService.UserInfoMultiInsert(uploadResultDic["filepath"]);
                    TempData["import_msg"] = "匯入完成";
                    TempData["allStatusUserInfos"] = result["allStatusUserInfos"];
                    UploadHelper.deleteUploadFile(iupexl.FileName, "_temp", "admin");
                }
                else
                {
                    TempData["import_msg"] = "匯入失敗";
                }
            }
            return Redirect("UserInfoImport");
        }
        #endregion

        ////媒合大表
        #region 媒合時程表時間設定新增與刪除
        [HttpGet]
        public ActionResult MatchScheduleTime()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            ViewBag.Action = "StoreMatchTimeInterval";
            matchModel.schedulePeriodSetList = matchService.GetActivityMatchTimeIntervalList(int.Parse(Request["activity_id"]));
            matchModel.SchedulePeriodSet = new SchedulePeriodSetModel();
            matchModel.SchedulePeriodSet.activity_id = int.Parse(Request["activity_id"]);
            return View(matchModel);
        }

        [HttpPost]
        public ActionResult StoreMatchTimeInterval(SchedulePeriodSetModel schedulePeriodSetModel, int[] old_period_sn, DateTime[] old_time_start, DateTime[] old_time_end)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            SchedulePeriodSetModel model = new SchedulePeriodSetModel();
            if (old_period_sn != null)
            {
                for (int i = 0; i < old_period_sn.Length; i++)
                {
                    model.period_sn = old_period_sn[i];
                    model.time_start = old_time_start[i];
                    model.time_end = old_time_end[i];
                    matchService.MatchTimeIntervalUpdateOne(model);
                }
            }

            if ((schedulePeriodSetModel.time_start.ToString() != "0001/1/1 上午 12:00:00")
                || (schedulePeriodSetModel.time_end.ToString() != "0001/1/1 上午 12:00:00"))
            {
                matchService.MatchTimeIntervalInsert(schedulePeriodSetModel);
            }

            return Redirect("MatchScheduleTime?activity_id=" + schedulePeriodSetModel.activity_id);
        }

        [HttpGet]
        public ActionResult MatchTimeIntervalDelect()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            int activity_id = int.Parse(Request["activity_id"]);
            matchService.MatchTimeIntervalDeleteOne(int.Parse(Request["period_sn"]));
            return Redirect("MatchScheduleTime?activity_id=" + activity_id);
        }
        #endregion

        #region 媒合時程大表列表
        [HttpGet]
        public ActionResult MatchScheduleList()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            ViewBag.Action = "StoreMatchData";
            int sellercount = 0;
            IList<MatchmakingNeedModel> CheckIs1List = matchService.GetCertainActivityWithBuyerReplyAllList
                (int.Parse(Request["activity_id"]), "1");
            IList<MatchmakingNeedModel> CheckIs0List = matchService.GetCertainActivityWithBuyerReplyAllList
                (int.Parse(Request["activity_id"]), "");
            ISet<string> buyerReply1Set = new HashSet<string>();
            ISet<string> buyerReply0Set = new HashSet<string>();

            /*列出某活動所有買主*/
            matchModel.buyerinfoList = matchService.GetSellerMatchToBuyerNameAndNeedList(int.Parse(Request["activity_id"]));
            /*列出某活動媒合時段*/
            matchModel.schedulePeriodSetList = matchService.GetActivityMatchTimeIntervalList(int.Parse(Request["activity_id"]));
            /*取得設定時間的活動編號*/
            matchModel.SchedulePeriodSet = new SchedulePeriodSetModel();
            matchModel.SchedulePeriodSet.activity_id = int.Parse(Request["activity_id"]);

            /*列出某活動有審核的賣家*/
            matchModel.activityregisterList = matchService.GetCertainActivityHaveCheckSellerNameList(int.Parse(Request["activity_id"]));

            /*列出某活動的媒合大表資料*/
            matchModel.matchmakingScheduleList = matchService.GetCertainActivityMatchMakingDataList(int.Parse(Request["activity_id"]));

            /*列出某活動的時間區段輸入媒合的賣家*/
            int i = notFoundIndex, j = notFoundIndex;//i是時段, j是買主

            matchModel.matchMakingScheduleSellerCompany = Enumerable.Repeat(String.Empty, matchModel.buyerinfoList.Count * matchModel.schedulePeriodSetList.Count).ToArray();
            matchModel.matchMakingScheduleSellerId = Enumerable.Repeat(String.Empty, matchModel.buyerinfoList.Count * matchModel.schedulePeriodSetList.Count).ToArray();

            foreach (MatchmakingScheduleModel matchmakingScheduleModel in matchModel.matchmakingScheduleList)
            {
                foreach (SchedulePeriodSetModel schedulePeriodSetModel in matchModel.schedulePeriodSetList)
                {
                    if (schedulePeriodSetModel.period_sn == matchmakingScheduleModel.period_sn)
                    {
                        i = matchModel.schedulePeriodSetList.IndexOf(schedulePeriodSetModel);
                    }
                }

                foreach (BuyerInfoModel buyerInfoModel in matchModel.buyerinfoList)
                {
                    if (buyerInfoModel.buyer_id == matchmakingScheduleModel.buyer_id)
                    {
                        j = matchModel.buyerinfoList.IndexOf(buyerInfoModel);
                    }
                }

                if ((i != notFoundIndex) && (j != notFoundIndex))
                {
                    matchModel.matchMakingScheduleSellerCompany[i * matchModel.buyerinfoList.Count + j] = matchmakingScheduleModel.company;
                    matchModel.matchMakingScheduleSellerId[i * matchModel.buyerinfoList.Count + j] = matchmakingScheduleModel.seller_id;
                }
            }

            /*列出雙方有媒合意願的賣家*/
            foreach (MatchmakingNeedModel model in CheckIs1List)
            {
                buyerReply1Set.Add(model.buyer_id);
            }

            foreach (string buyer in buyerReply1Set)
            {
                matchModel.sellerCompanyNamereply1Dic[buyer] = new List<string>();
            }

            foreach (MatchmakingNeedModel model in CheckIs1List)
            {
                matchModel.sellerCompanyNamereply1Dic[model.buyer_id].Add(model.company);
            }

            /*列出有媒合意願的賣家*/
            foreach (MatchmakingNeedModel model in CheckIs0List)
            {
                buyerReply0Set.Add(model.buyer_id);
            }

            foreach (string buyer in buyerReply0Set)
            {
                matchModel.sellerCompanyNamereply0Dic[buyer] = new List<string>();
            }

            foreach (MatchmakingNeedModel model in CheckIs0List)
            {
                matchModel.sellerCompanyNamereply0Dic[model.buyer_id].Add(model.company);
            }

            /*刪除某時段,媒合大表中的相同時段資料刪除*/
            ISet<int> schedulePeriodSn = new HashSet<int>();
            ISet<int> matchmakingPeriodSn = new HashSet<int>();
            foreach (SchedulePeriodSetModel schedulePeriodSetModel in matchModel.schedulePeriodSetList)
            {
                schedulePeriodSn.Add(schedulePeriodSetModel.period_sn);
            }

            foreach (MatchmakingScheduleModel matchmakingScheduleModel in matchModel.matchmakingScheduleList)
            {
                matchmakingPeriodSn.Add(matchmakingScheduleModel.period_sn);
            }

            var periodSns = from n1 in matchmakingPeriodSn
                            where schedulePeriodSn.Contains(n1) == false
                            select n1;
            if (periodSns.Count() != 0)
            {
                foreach (int periodSn in periodSns)
                {
                    matchService.MatchkingDataByActivityWithPeriodDelete(int.Parse(Request["activity_id"]), periodSn);
                }
            }

            /*activityregisterList取出user_id(賣家)存到陣列中*/
            matchModel.activityRegisterSellerCompany = Enumerable.Repeat(String.Empty, matchModel.activityregisterList.Count).ToArray();
            foreach (ActivityRegisterModel model in matchModel.activityregisterList)
            {
                matchModel.activityRegisterSellerCompany[sellercount] = model.company;
                sellercount++;
            }

            return View(matchModel);
        }
        #endregion

        //#region 媒合大表匯出Excel
        ////[HttpGet]
        ////public ActionResult ExportExcelByNPOI()
        ////{
        ////    /*列出某活動所有買主*/
        ////    matchModel.buyerinfoList = matchService.GetSellerMatchToBuyerNameAndNeedList(int.Parse(Request["activity_id"]));
            
        ////    /*讀取樣板*/
        ////    string ExcelPath = Server.MapPath("~/Content/Template/Import/manager_matchmaking_sample.xls");
        ////    FileStream Template = new FileStream(ExcelPath, FileMode.Open, FileAccess.Read);
        ////    IWorkbook workbook = new HSSFWorkbook(Template);
        ////    Template.Close();

        ////    ISheet _sheet = workbook.GetSheetAt(0);
        ////    // 取得剛剛在Excel設定的字型 (第二列首欄)
        ////    ICellStyle CellStyle = _sheet.GetRow(1).Cells[0].CellStyle;
        ////    int CurrRow = 1; //起始列(跳過標題列)
        ////    foreach (BuyerInfoModel buyerInfoModel in matchModel.buyerinfoList)
        ////    {
        ////        IRow MyRow = _sheet.CreateRow(CurrRow);
        ////        CreateCell(buyerInfoModel.company, MyRow, 0, CellStyle); //訂單編號
        ////        CurrRow++;
        ////    }

        ////    string SavePath = @"D:/matchmaking.xls";
        ////    FileStream file = new FileStream(SavePath, FileMode.Create);
        ////    workbook.Write(file);
        ////    file.Close();

        ////    return File(SavePath, "application/ms-excel", "matchmaking1.xls");
        ////}

        ///// <summary>NPOI新增儲存格資料</summary>
        ///// <param name="Word">顯示文字</param>
        ///// <param name="ContentRow">NPOI IROW</param>
        ///// <param name="CellIndex">儲存格列數</param>
        ///// <param name="cellStyleBoder">ICellStyle樣式</param>
        //private static void CreateCell(string Word, IRow ContentRow, int CellIndex, ICellStyle cellStyleBoder)
        //{
        //    ICell _cell = ContentRow.CreateCell(CellIndex);
        //    _cell.SetCellValue(Word);
        //    _cell.CellStyle = cellStyleBoder;
        //}
        //#endregion

        #region 媒合時程大表新增修改刪除
        [HttpPost]
        public ActionResult StoreMatchData(int[] period_sn, int activity_id, string[] buyer_id, string[] seller_id)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            /*列出某活動的媒合大表資料*/
            matchModel.matchmakingScheduleList = matchService.GetCertainActivityMatchMakingDataList(activity_id);
            /*列出某活動所有買主*/
            matchModel.buyerinfoList = matchService.GetSellerMatchToBuyerNameAndNeedList(activity_id);
            /*列出某活動媒合時段*/
            matchModel.schedulePeriodSetList = matchService.GetActivityMatchTimeIntervalList(activity_id);
            /*列出某活動的時間區段輸入媒合的賣家*/
            int i = notFoundIndex, j = notFoundIndex;//i是時段, j是買主

            matchModel.matchMakingScheduleSellerCompany = Enumerable.Repeat(String.Empty, buyer_id.Length * period_sn.Length).ToArray();

            foreach (MatchmakingScheduleModel model in matchModel.matchmakingScheduleList)
            {
                foreach (SchedulePeriodSetModel schedulePeriodSetModel in matchModel.schedulePeriodSetList)
                {
                    if (schedulePeriodSetModel.period_sn == model.period_sn)
                    {
                        i = matchModel.schedulePeriodSetList.IndexOf(schedulePeriodSetModel);
                    }
                }

                foreach (BuyerInfoModel buyerInfoModel in matchModel.buyerinfoList)
                {
                    if (buyerInfoModel.buyer_id == model.buyer_id)
                    {
                        j = matchModel.buyerinfoList.IndexOf(buyerInfoModel);
                    }
                }

                if ((i != notFoundIndex) && (j != notFoundIndex))
                {
                    matchModel.matchMakingScheduleSellerCompany[i * matchModel.buyerinfoList.Count + j] = model.company;
                }

            }
            MatchmakingScheduleModel matchmakingScheduleModel = new MatchmakingScheduleModel();
            matchmakingScheduleModel.activity_id = activity_id;
            matchmakingScheduleModel.create_time = DateTime.Now;

            for (int x = 0; x < period_sn.Length * buyer_id.Length; x++)
            {

                if (matchModel.matchMakingScheduleSellerCompany[x] == "" && seller_id[x].Length != 0)
                {
                    matchmakingScheduleModel.period_sn = period_sn[x / buyer_id.Length];
                    matchmakingScheduleModel.buyer_id = buyer_id[x % buyer_id.Length];
                    matchmakingScheduleModel.seller_id = seller_id[x];
                    matchService.CertainTimeMatchSellerInsert(matchmakingScheduleModel);
                }
                else if (matchModel.matchMakingScheduleSellerCompany[x] != "" && seller_id[x] != "")
                {
                    matchmakingScheduleModel.period_sn = period_sn[x / buyer_id.Length];
                    matchmakingScheduleModel.buyer_id = buyer_id[x % buyer_id.Length];
                    matchmakingScheduleModel.seller_id = seller_id[x];
                    matchService.CertainActivityMatchkingDataUpdate(matchmakingScheduleModel);
                }
                else if (matchModel.matchMakingScheduleSellerCompany[x].Length != 0 && seller_id[x] == "")
                {
                    matchmakingScheduleModel.period_sn = period_sn[x / buyer_id.Length];
                    matchmakingScheduleModel.buyer_id = buyer_id[x % buyer_id.Length];
                    matchmakingScheduleModel.seller_id = seller_id[x];
                    matchService.CertainActivityMatchkingDataDelete(matchmakingScheduleModel);

                }
            }

            return Redirect("MatchScheduleList?activity_id=" + activity_id);
        }
        #endregion


        #region 密碼編輯
        // GET: Password
        public ActionResult EditPasswd()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("~/Manager/Login");

            return View();
        }

        //修改密碼
        public ActionResult PasswordInsertUpdate(string old_pw, string new_pw)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("~/Manager/Login");

            string current_id = "";
            string current_manager_id = Request.Cookies["ManagerInfo"]["manager_id"]; // 取 manager_id 的 cookie
//            string current_user_id = ""; // "12345678"; // 取 user_id 的 cookie
            string errMsg = "修改成功";
            if (!string.IsNullOrEmpty(current_manager_id))
            {
                current_id = current_manager_id;
                if (passwordService.getManagerPassword(current_id).Equals(old_pw))
                {
                    if (!passwordService.UpdateManagerPassword(current_id, new_pw))
                    {
                        errMsg = "修改失敗";
                    }
                }
                else
                {
                    errMsg = "輸入的舊密碼不正確";
                }
            }

/*
            if (!string.IsNullOrEmpty(current_user_id))
            {
                current_id = current_user_id;
                if (passwordService.getUserPassword(current_id).Equals(old_pw))
                {
                    if (!passwordService.UpdateUserPassword(current_id, new_pw))
                    {
                        errMsg = "修改失敗";
                    }
                }
                else
                {
                    errMsg = "輸入的舊密碼不正確";
                }
            }
*/
            TempData["pw_errMsg"] = errMsg;

            return Redirect("EditPasswd");
        }
        #endregion

    }
}