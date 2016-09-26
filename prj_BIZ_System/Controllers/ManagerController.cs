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
using System.Net;
using Newtonsoft.Json.Linq;
using System.Threading;
using IBatisNet.DataMapper.SessionStore;

namespace prj_BIZ_System.Controllers
{
    public class ManagerController : _BaseController
    {
        public ActivityService activityService;
        public ManagerService managerService;
        public UserService userService;
        public SalesService salesService;
        public Manager_ViewModel managerViewModel;
        public Manager_Activity_ViewModel activityModel;
        public Match_ViewModel matchModel;
        public MatchService matchService;
        private const int notFoundIndex = 999;

        public PasswordService passwordService;
        public Password_ViewModel passwordViewModel;
        public Sales_ViewModel salesViewModel;

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

            salesService = new SalesService();
            salesViewModel = new Sales_ViewModel();

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

                cookie.Values.Add("name", HttpUtility.UrlEncode(model.name));

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

        public ActionResult CheckManager(string manager_id)
        {
            ManagerInfoModel kk = managerService.getManagerInfo(manager_id);
            if (kk == null)
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(kk, JsonRequestBehavior.AllowGet);

        }


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
            model.create_manager = Request.Cookies["ManagerInfo"]["manager_id"];
            if ("Insert".Equals(pagetype))
            {
                managerService.ManagerInfoInsertOne(model);
//                return Redirect("ManagerInfo");
            }
            else if ("Update".Equals(pagetype))
            {
                managerService.ManagerInfoUpdateOne(model);
//                bool isUpdateSuccess = managerService.ManagerInfoUpdateOne(model);
//                return Json(isUpdateSuccess);
            }
            return Redirect("ManagerInfo");
        }

        public ActionResult DeleteManagerInfoJson(string manager_id , string enable)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");
            //非真的刪 , 只是停用
            bool isDelSuccess = managerService.ManagerInfoDisableOne(manager_id, enable);
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
        public ActionResult ActivityRegisterCheck(int? selectActivityId , string selectCompany,string startDate,string endDate)
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
            activityModel.activityregisterList = activityService.GetActivityCheckAllByConditionWithId(selectActivityId, selectCompany, startDate, endDate, grp_id).Pages(Request, this, 10);
            activityModel.activityinfoList = activityService.GetActivityInfoList(grp_id);
            ViewBag.Where_ActivityId = selectActivityId;
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

            // send mail 
            ActivityRegisterModel registInfo = activityService.ActivityRegisterChkMailInfo(register_id);
            if (registInfo != null)
            {
                MailHelper.sendActivityCheckNotify(
                      registInfo.manager_check  , registInfo.activity_id    , registInfo.activity_name  , registInfo.starttime.ToString("yyyy-MM-dd HH:mm") , registInfo.endtime.ToString("yyyy-MM-dd HH:mm")
                    , registInfo.addr           , registInfo.quantity       , registInfo.name_b         , registInfo.phone , registInfo.email );
            }
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


        public class CompanyData
        {
            public string Business_Accounting_NO;
            public string Company_Name;
            public string Capital_Stock_Amount;
            public string Company_Location;
            public int Business_Item_Count;
            public List<string> Business_Item = new List<string>();
        }

        public class Subcata
        {
            [JsonProperty(PropertyName = "Business_Seq_NO")]
            public string Business_Seq_NO { get; set; }
            [JsonProperty(PropertyName = "Business_Item")]
            public string Business_Item { get; set; }
            [JsonProperty(PropertyName = "Business_Item_Desc")]
            public string Business_Item_Desc { get; set; }
        }


        public class Cata
        {
            [JsonProperty(PropertyName = "Business_Accounting_NO")]
            public string Business_Accounting_NO { get; set; }
            [JsonProperty(PropertyName = "Company_Name")]
            public string Company_Name { get; set; }
            [JsonProperty(PropertyName = "Company_Status")]
            public string Company_Status { get; set; }
            [JsonProperty(PropertyName = "Company_Status_Desc")]
            public string Company_Status_Desc { get; set; }
            [JsonProperty(PropertyName = "Company_Setup_Date")]
            public string Company_Setup_Date { get; set; }
            public Subcata[] Cmp_Business { get; set; }
        }

        public class Basedata
        {
            [JsonProperty(PropertyName = "Business_Accounting_NO")]
            public string Business_Accounting_NO { get; set; }
            [JsonProperty(PropertyName = "Company_Status_Desc")]
            public string Company_Status_Desc { get; set; }
            [JsonProperty(PropertyName = "Company_Name")]
            public string Company_Name { get; set; }
            [JsonProperty(PropertyName = "Capital_Stock_Amount")]
            public string Capital_Stock_Amount { get; set; }
            [JsonProperty(PropertyName = "Paid_In_Capital_Amount")]
            public string Paid_In_Capital_Amount { get; set; }
            [JsonProperty(PropertyName = "Responsible_Name")]
            public string Responsible_Name { get; set; }
            [JsonProperty(PropertyName = "Company_Location")]
            public string Company_Location { get; set; }
            [JsonProperty(PropertyName = "Register_Organization_Desc")]
            public string Register_Organization_Desc { get; set; }
            [JsonProperty(PropertyName = "Company_Setup_Date")]
            public string Company_Setup_Date { get; set; }
            [JsonProperty(PropertyName = "Change_Of_Approval_Data")]
            public string Change_Of_Approval_Data { get; set; }
            [JsonProperty(PropertyName = "Revoke_App_Date")]
            public string Revoke_App_Date { get; set; }
            [JsonProperty(PropertyName = "Sus_App_Date")]
            public string Sus_App_Date { get; set; }
            [JsonProperty(PropertyName = "Sus_Beg_Date")]
            public string Sus_Beg_Date { get; set; }
            [JsonProperty(PropertyName = "Sus_End_Date")]
            public string Sus_End_Date { get; set; }
        }


        public static CompanyData GetDataFromWeb(string user_id)
        {
            try
            {
                CompanyData companydata = new CompanyData();
                string url1 = "http://data.gcis.nat.gov.tw/od/data/api/5F64D864-61CB-4D0D-8AD9-492047CC1EA6?$format=json&$filter=Business_Accounting_NO eq " + user_id;
                string url2 = "http://data.gcis.nat.gov.tw/od/data/api/236EE382-4942-41A9-BD03-CA0709025E7C?$format=json&$filter=Business_Accounting_NO eq " + user_id;

///////////////////URL2///////////////
                HttpWebRequest request1 = WebRequest.Create(url1) as HttpWebRequest;
                using (HttpWebResponse response1 = request1.GetResponse() as HttpWebResponse)
                {
                    if (response1.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response1.StatusCode,
                        response1.StatusDescription));
                    var rawJson = new StreamReader(response1.GetResponseStream()).ReadToEnd();
                    if (rawJson == null || rawJson == "")
                        return null;
                    var jarray = JsonConvert.DeserializeObject<List<Basedata>>(rawJson);
                    companydata.Business_Accounting_NO = jarray[0].Business_Accounting_NO;
                    companydata.Company_Name = jarray[0].Company_Name;
                    companydata.Company_Location = jarray[0].Company_Location;
                    companydata.Capital_Stock_Amount = jarray[0].Capital_Stock_Amount;
                }
                ///////////////////URL2///////////////
                HttpWebRequest request = WebRequest.Create(url2) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    var rawJson = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    var jarray = JsonConvert.DeserializeObject<List<Cata>>(rawJson);
                    //                    string[] kk = new string[53];
                    companydata.Business_Item_Count = jarray[0].Cmp_Business.Count();
                    for (int i = 0; i < companydata.Business_Item_Count; i++)
                    {
                        companydata.Business_Item.Add(jarray[0].Cmp_Business[i].Business_Item.Substring(0,2));
                    }
                }
                ////////////////////////////////////
                return companydata;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }


        }


        [HttpGet]
        public ActionResult CheckUser(string user_id)
        {
//            bool Huser = true;
            activityModel.userinfo = userService.GeUserInfoOne(user_id);
            if (activityModel.userinfo == null || activityModel.userinfo.user_id == null)
            {
                CompanyData compdata = GetDataFromWeb(user_id);
                return Json(compdata, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckUserExit(string user_id)
        {
            //            bool Huser = true;
//            activityModel.userinfo = userService.GeUserInfoOne(user_id);
//            if (activityModel.userinfo == null || activityModel.userinfo.user_id == null)
//            {
                CompanyData compdata = GetDataFromWeb(user_id);
                return Json(compdata, JsonRequestBehavior.AllowGet);
//            }
//            else
//            {
//                return Json(true, JsonRequestBehavior.AllowGet);
//            }
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
                ViewBag.tname = LanguageResource.User.lb_signup;
                activityModel.userinfo = new UserInfoModel();
                ViewBag.PageType = "Create";
                ViewBag.SubmitName = LanguageResource.User.lb_submit_sure;
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

            activityModel.buyerinfoList = activityService.GetBuyerInfoAll(grp_id,DateTime.Now).Pages(Request, this, 10);
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
                if (model.activity_id == 0 || string.IsNullOrEmpty(model.buyer_id)) {
                    TempData["buyer_errMsg"] = "此買主或此活動不存在 ";
                }
                else
                {
                    var buyModel = activityService.GetBuyerDataByActivityWithIdOne(model.activity_id, model.buyer_id);
                    if(buyModel == null)
                    {
                        var serial_no = activityService.BuyerInfoInsertOne(model);
                        if (serial_no !=null )
                        {
                            UserInfoModel buyer = userService.GeUserInfoOne(model.buyer_id);
                            ActivityInfoModel activity = activityService.GetActivityInfoOne(model.activity_id);
                            MailHelper.sendActivityAddBuyerNotify(
                                buyer.email , model.activity_id , activity.activity_name , activity.starttime
                                , activity.endtime, activity.addr, activity.organizer);
                        }
                    }
                    else
                    {
                        TempData["buyer_errMsg"] = "新增失敗...此企業原本就是該活動買主";
                    }
                }
            }
            else
            {
                if (model.activity_id == 0 || string.IsNullOrEmpty(model.buyer_id))
                {
                    TempData["buyer_errMsg"] = "此買主或此活動不存在 ";
                }
                else
                {
                    var buyModel = activityService.GetBuyerDataByActivityWithIdOne(model.activity_id, model.buyer_id);
                    if (buyModel == null)
                    {
                        bool isUpdateSuccess = activityService.BuyerInfoUpdateOne(model);
                    }
                    else
                    {
                        TempData["buyer_errMsg"] = "更新失敗...此企業原本就是該活動買主";
                    }
                }
            }
            return Redirect("BuyerInfoList");
        }

        public ActionResult GetUserInfoToIdCp(string term)
        {
//            if (Request.Cookies["ManagerInfo"] == null)
//                return Redirect("Login");

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

            var imageUrl = Url.Content(UploadConfig.UploadRootPath + manager_id + "/" + UploadConfig.subDirForNews + upload.FileName);

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
            logger.Info("已登入");

            logger.Info("上傳檔案名稱:"+ iupexl.FileName);
            if (iupexl != null && iupexl.FileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase) && iupexl.ContentLength > 0)
            {
                string targetDir = "_temp";
                Dictionary<string, string> uploadResultDic = null;
                uploadResultDic = UploadHelper.doUploadFile(iupexl, targetDir, UploadConfig.AdminManagerDirName);
                logger.Info("上傳結果:"+uploadResultDic["result"]);

                if ("success".Equals(uploadResultDic["result"]))
                {
                    Dictionary<string, object> result = userService.UserInfoMultiInsert(uploadResultDic["filepath"]);
                    TempData["import_msg"] = "匯入完成";
                    TempData["allStatusUserInfos"] = ((List<List<object>>)result["allStatusUserInfos"]);
                    UploadHelper.deleteUploadFile(iupexl.FileName, "_temp", UploadConfig.AdminManagerDirName);
                    Dictionary<int, object> success = (Dictionary<int, object>)result["success"];
                    if (success.Count > 0) new Thread(new ParameterizedThreadStart(doImportWebData)).Start(success);
                }
                else
                {
                    TempData["import_msg"] = "匯入失敗";
                }
            }
            else
            {
                TempData["import_msg"] = "檔案格式不正確";
            }
            return Redirect("UserInfoImport");
        }

        void doImportWebData(object param)
        {
            IList<EnterpriseSortListModel> result;
            _BaseService.mapper.SessionStore = new HybridWebThreadSessionStore(_BaseService.mapper.Id);
            var isCacheON = CacheConfig._NavSearchPartial_load_cache_isOn;
            if (isCacheON)
            {
                if (CacheDataStore.EnterpriseSortListModelCache == null)
                {
                    CacheDataStore.EnterpriseSortListModelCache = userService.GetSortList();
                }
                result = CacheDataStore.EnterpriseSortListModelCache;
            }
            else
            {
                result = userService.GetSortList();
            }



            Dictionary<int, object> successUserInfos = new Dictionary<int, object>();
            successUserInfos = (Dictionary<int, object>)param;
            foreach (KeyValuePair<int,object> kvp in successUserInfos)
            {
                string user_id = ((Dictionary<string, string>)kvp.Value)["user_id"];
                CompanyData compdata = GetDataFromWeb(user_id);
                logger.Info(user_id + "取回的值是否成功:");
                if (compdata == null)
                {
                    logger.Info("不存在");
                }
                else
                {
                    logger.Info("存在" + "=>資本額為" + (compdata.Capital_Stock_Amount==null?"0": compdata.Capital_Stock_Amount.ToString()));
                }
                var sort_id = result.Where(item => compdata!=null && compdata.Business_Item!=null && compdata.Business_Item.Contains(item.enterprise_sort_id)).Select(item => item.sort_id).Distinct().ToArray();
                _BaseService.mapper.SessionStore = new HybridWebThreadSessionStore(_BaseService.mapper.Id);
                long intCaptial = 0;
                if (Int64.TryParse(compdata.Capital_Stock_Amount, out intCaptial) && intCaptial > 0)
                {
                    bool isUpdateCapSuccess = userService.UserInfoUpdateCapital(user_id, intCaptial);
                }
                bool isUpdateAddSuccess = userService.UserInfoUpdateAddr(user_id, compdata.Company_Location);
                logger.Info("user_id=" + user_id + " 匯入後更新地址結果為:" + isUpdateAddSuccess);
                bool refreshResult = userService.RefreshUserSort(user_id, sort_id);
                logger.Info("user_id=" + user_id + " 匯入後新增產業別結果為:" + refreshResult);
            }
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
            matchModel.schedulePeriodSet = new SchedulePeriodSetModel();
            matchModel.schedulePeriodSet.activity_id = int.Parse(Request["activity_id"]);
            matchModel.schedulePeriodSet.time_start = DateTime.Now;
            matchModel.schedulePeriodSet.time_end = DateTime.Now.AddMinutes(1);
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

        #region 媒合時程大表列表舊版
        [HttpGet]
        public ActionResult MatchScheduleList123()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            ViewBag.Action = "StoreMatchData";
            int sellercount = 0;
            //IList<MatchmakingNeedModel> CheckIs1List = matchService.GetCertainActivityWithBuyerReplyAllList
            //    (int.Parse(Request["activity_id"]), "1");
            //IList<MatchmakingNeedModel> CheckIs0List = matchService.GetCertainActivityWithBuyerReplyAllList
            //    (int.Parse(Request["activity_id"]), "");
            ISet<string> buyerReply1Set = new HashSet<string>();
            ISet<string> buyerReply0Set = new HashSet<string>();

            /*列出某活動所有買主*/
            matchModel.buyerinfoList = matchService.GetSellerMatchToBuyerNameAndNeedList(int.Parse(Request["activity_id"]));
            /*列出某活動媒合時段*/
            matchModel.schedulePeriodSetList = matchService.GetActivityMatchTimeIntervalList(int.Parse(Request["activity_id"]));
            /*取得設定時間的活動編號*/
            matchModel.schedulePeriodSet = new SchedulePeriodSetModel();
            matchModel.schedulePeriodSet.activity_id = int.Parse(Request["activity_id"]);

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
            //foreach (MatchmakingNeedModel model in CheckIs1List)
            //{
            //    buyerReply1Set.Add(model.buyer_id);
            //}

            foreach (string buyer in buyerReply1Set)
            {
                matchModel.sellerCompanyNamereply1Dic[buyer] = new List<string>();
            }

            //foreach (MatchmakingNeedModel model in CheckIs1List)
            //{
            //    matchModel.sellerCompanyNamereply1Dic[model.buyer_id].Add(model.company);
            //}

            /*列出有媒合意願的賣家*/
            //foreach (MatchmakingNeedModel model in CheckIs0List)
            //{
            //    buyerReply0Set.Add(model.buyer_id);
            //}

            foreach (string buyer in buyerReply0Set)
            {
                matchModel.sellerCompanyNamereply0Dic[buyer] = new List<string>();
            }

            //foreach (MatchmakingNeedModel model in CheckIs0List)
            //{
            //    matchModel.sellerCompanyNamereply0Dic[model.buyer_id].Add(model.company);
            //}

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

        #region 媒合大表列表新版

        [HttpGet]
        public ActionResult MatchScheduleList()
        {
            if(Request.Cookies["ManagerInfo"] == null){
                return Redirect("Login");
            }

            /*頁面端取得資訊*/
            ViewBag.Action = "StoreMatchData";
            /*取得設定時間的活動編號*/
            matchModel.schedulePeriodSet = new SchedulePeriodSetModel();
            matchModel.schedulePeriodSet.activity_id = int.Parse(Request["activity_id"]);
            /*列出某活動的所有買主*/
            matchModel.buyerinfoList = matchService.GetSellerMatchToBuyerNameAndNeedList(int.Parse(Request["activity_id"]));
            /*列出某活動的所有媒合時段*/
            matchModel.schedulePeriodSetList = matchService.GetActivityMatchTimeIntervalList(int.Parse(Request["activity_id"]));

            /*列出某活動的雙方有意願*/
            matchModel.matchmakingBothList = matchService.GetMatchmakingbothneedList(int.Parse(Request["activity_id"]));
            /*列出某活動的買家有意願*/
            matchModel.matchmakingBuyerList = matchService.GetCertainActivityBuyerCheckSellerList(int.Parse(Request["activity_id"]),"");
            /*列出某活動的賣家有意願*/
            matchModel.matchmakingSellerList = matchService.GetMSneedBySellerCompanyList(int.Parse(Request["activity_id"]));

            /*列出某活動的雙方媒合意願與買方媒合意願的合併資料*/
            //matchModel.matchSellerCompanyDatamergeList = Enumerable.Repeat(new List<object>(), matchModel.schedulePeriodSetList.Count * matchModel.buyerinfoList.Count).ToList();

            int allCount = matchModel.schedulePeriodSetList.Count * matchModel.buyerinfoList.Count;
            matchModel.matchSellerCompanyDatamergeList = new List<List<object>>();
            for (int temp = 0; temp < allCount; temp++)
            {
                List<object> data = new List<object>();
                matchModel.matchSellerCompanyDatamergeList.Add(data);
            }

            for (int i = 0; i < matchModel.buyerinfoList.Count; i++)
            {
                var bothAllBuyer_id = matchModel.matchmakingBothList.Select(both => both.buyer_id).ToArray();
                if (bothAllBuyer_id.Contains(matchModel.buyerinfoList[i].buyer_id))
                {
                    /*雙方媒合意願 某買主有那些賣家*/
                    var bothList =
                        matchModel.matchmakingBothList
                        .Where(both => both.buyer_id == matchModel.buyerinfoList[i].buyer_id)
                        .Select(both => new  { IsBothOrBuyer = "both",  both.seller_id, both.company }).ToList();
                    matchModel.matchSellerCompanyDatamergeList[i].AddRange(bothList);
                   
                }

                var buyerAllBuyer_id = matchModel.matchmakingBuyerList.Select(buyer => buyer.buyer_id).ToArray();
                if (buyerAllBuyer_id.Contains(matchModel.buyerinfoList[i].buyer_id))
                {
                    /*買方媒合意願 某買主有那些賣家*/
                    var bothList =
                        matchModel.matchmakingBothList
                        .Where(both => both.buyer_id == matchModel.buyerinfoList[i].buyer_id)
                        .Select(both => new  { IsBothOrBuyer = "buyer", both.seller_id, both.company }).ToList();

                    var buyerList =
                        matchModel.matchmakingBuyerList
                        .Where(buyer => buyer.buyer_id == matchModel.buyerinfoList[i].buyer_id)
                        .Select(buyer => new { IsBothOrBuyer = "buyer", buyer.seller_id, buyer.company }).ToList();

                    var exceptList = buyerList.Except(bothList);
                    matchModel.matchSellerCompanyDatamergeList[i].AddRange(exceptList);
                }
            }

            return View(matchModel);
        }

        #endregion

        #region 媒合大表匯出Excel
        [HttpGet]
        public ActionResult ExportExcelByNPOI()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            ViewBag.Action = "StoreMatchData";
            //IList<MatchmakingNeedModel> CheckIs1List = matchService.GetCertainActivityWithBuyerReplyAllList
            //    (int.Parse(Request["activity_id"]), "1");
            //IList<MatchmakingNeedModel> CheckIs0List = matchService.GetCertainActivityWithBuyerReplyAllList
            //    (int.Parse(Request["activity_id"]), "");
            ISet<string> buyerReply1Set = new HashSet<string>();
            ISet<string> buyerReply0Set = new HashSet<string>();

            /*列出某活動所有買主*/
            matchModel.buyerinfoList = matchService.GetSellerMatchToBuyerNameAndNeedList(int.Parse(Request["activity_id"]));
            /*列出某活動媒合時段*/
            matchModel.schedulePeriodSetList = matchService.GetActivityMatchTimeIntervalList(int.Parse(Request["activity_id"]));
            /*取得設定時間的活動編號*/
            matchModel.schedulePeriodSet = new SchedulePeriodSetModel();
            matchModel.schedulePeriodSet.activity_id = int.Parse(Request["activity_id"]);

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
            //foreach (MatchmakingNeedModel model in CheckIs1List)
            //{
            //    buyerReply1Set.Add(model.buyer_id);
            //}

            foreach (string buyer in buyerReply1Set)
            {
                matchModel.sellerCompanyNamereply1Dic[buyer] = new List<string>();
            }

            //foreach (MatchmakingNeedModel model in CheckIs1List)
            //{
            //    matchModel.sellerCompanyNamereply1Dic[model.buyer_id].Add(model.company);
            //}

            /*列出有媒合意願的賣家*/
            //foreach (MatchmakingNeedModel model in CheckIs0List)
            //{
            //    buyerReply0Set.Add(model.buyer_id);
            //}

            foreach (string buyer in buyerReply0Set)
            {
                matchModel.sellerCompanyNamereply0Dic[buyer] = new List<string>();
            }

            //foreach (MatchmakingNeedModel model in CheckIs0List)
            //{
            //    matchModel.sellerCompanyNamereply0Dic[model.buyer_id].Add(model.company);
            //}


            /*讀取樣板*/
            string ExcelPath = Server.MapPath("~/Content/Template/Import/tmpmatchmaking.xls");
            FileStream Template = new FileStream(ExcelPath, FileMode.Open, FileAccess.Read);
            IWorkbook workbook = new HSSFWorkbook(Template);
            Template.Close();

            ISheet _sheet = workbook.GetSheetAt(0);
            // 取得剛剛在Excel設定的字型 (第二列首欄)
            ICellStyle CellStyle = _sheet.GetRow(0).Cells[0].CellStyle;
            ICellStyle CellStyle1 = _sheet.GetRow(1).Cells[0].CellStyle;
            int CurrRow = 0; //起始列(跳過標題列)
            int CurrCol = 1; //起始列(跳過標題列)
            IRow MyRow = _sheet.CreateRow(CurrRow);
            int CurrRowMax = 4; //起始列(跳過標題列)
            int tok = 0;

            CreateCell("時段\\買方名稱", MyRow, 0, CellStyle);

            foreach (BuyerInfoModel buyerInfoModel in matchModel.buyerinfoList)
            {
                CreateCell(buyerInfoModel.company, MyRow, CurrCol, CellStyle); //買家公司名稱
                try
                {
                    for (i = 0; i < matchModel.sellerCompanyNamereply1Dic[buyerInfoModel.buyer_id].Count; i++)
                    {
                        if (CurrRowMax < 4 + i)
                            CurrRowMax = 4 + i;
                        IRow MyRow1 = _sheet.GetRow(4 + i);
                        if (MyRow1 == null)
                            MyRow1 = _sheet.CreateRow(4 + i);
                        if (i == 0 && tok == 0)
                        {
                            tok = 1;
                            CreateCell("雙方有媒合意願", MyRow1, 0, CellStyle); //
                        }
                        CreateCell(matchModel.sellerCompanyNamereply1Dic[buyerInfoModel.buyer_id][i], MyRow1, CurrCol, CellStyle1); //買家公司名稱

                    }
                }
                catch
                {
                }

                CurrCol++;
            }

            CurrCol = 1;
            foreach (BuyerInfoModel buyerInfoModel in matchModel.buyerinfoList)
            {
                tok = 0;
                try
                {
                    for (i = 0; i < matchModel.sellerCompanyNamereply0Dic[buyerInfoModel.buyer_id].Count; i++)
                    {
                        IRow MyRow1 = _sheet.GetRow(CurrRowMax + 2 + i);
                        if (MyRow1 == null)
                            MyRow1 = _sheet.CreateRow(CurrRowMax + 2 + i);
                        if (i == 0 && tok == 0)
                        {
                            tok = 1;
                            CreateCell("賣方有媒合意願", MyRow1, 0, CellStyle); //
                        }
                        CreateCell(matchModel.sellerCompanyNamereply0Dic[buyerInfoModel.buyer_id][i], MyRow1, CurrCol, CellStyle1); //買家公司名稱

                    }
                }
                catch
                {
                }

                CurrCol++;
            }

            CurrRow = 1;
            foreach (SchedulePeriodSetModel schedulePeriodSetModel in matchModel.schedulePeriodSetList)
            {
                IRow MyRow1 = _sheet.GetRow(CurrRow);
                if (MyRow1 == null)
                    MyRow1 = _sheet.CreateRow(CurrRow);
                CreateCell(schedulePeriodSetModel.time_start.ToString("yyyy/MM/dd HH:mm") + "~" + schedulePeriodSetModel.time_end.ToString("yyyy/MM/dd HH:mm")
                    , MyRow1, 0, CellStyle); //
                CurrRow++;
            }


            for (i = 1; i < CurrRow; i++)
            {
                for (j = 1; j < CurrCol; j++)
                {
                    IRow MyRow1 = _sheet.GetRow(i);
                    CreateCell(matchModel.matchMakingScheduleSellerCompany[(i - 1) * (CurrCol - 1) + (j - 1)],
                        MyRow1, j, CellStyle1); //
                }
            }

            string SavePath = @"D:/Download/matchmaking.xls";
            FileStream file = new FileStream(SavePath, FileMode.Create);
            workbook.Write(file);
            file.Close();

            return File(SavePath, "application/ms-excel", "matchmaking.xls");
        }

        /// <summary>NPOI新增儲存格資料</summary>
        /// <param name="Word">顯示文字</param>
        /// <param name="ContentRow">NPOI IROW</param>
        /// <param name="CellIndex">儲存格列數</param>
        /// <param name="cellStyleBoder">ICellStyle樣式</param>
        private static void CreateCell(string Word, IRow ContentRow, int CellIndex, ICellStyle cellStyleBoder)
        {
            ICell _cell = ContentRow.CreateCell(CellIndex);
            _cell.SetCellValue(Word);
            _cell.CellStyle = cellStyleBoder;
        }
        #endregion

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


        #region  業務資料管理

        public ActionResult SalesInfo(string where_sales_id, string where_company)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("~/Manager/Login");
            ViewBag.Title = "SalesInfo";
            salesViewModel.salesInfoList = salesService.getSalesInfoByConditionForManager(where_sales_id, where_company).Pages(Request, this, 10);
            ViewBag.Where_sales_id = where_sales_id;
            ViewBag.Where_company = where_company;
            return View(salesViewModel);
        }

        #endregion

    }
}