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
using prj_BIZ_System.Extensions;

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
        private const long notFoundIndex = 99999999999999999;
        private string IsBothOrBuyer; //判斷是雙方媒合意願或買方媒合意願的顏色
        private string excelTemplatePath = "~/Content/Template/Import/";

        public PasswordService passwordService;
        public Password_ViewModel passwordViewModel;
        public Sales_ViewModel salesViewModel;

        public ClusterService clusterService;
        public Cluster_ViewModel clusterViewModel;

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

            clusterService = new ClusterService();
            clusterViewModel = new Cluster_ViewModel();

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

        public ActionResult ClusterSize(string cluster_name,string company)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");
            string cluster_name1 = cluster_name;
            string company1 = company;

            if (cluster_name == "") cluster_name1 = null;
            if (company == "") company1 = null;
            if (cluster_name1 != null) cluster_name1 = cluster_name1.ToUpper();
            if (company1 != null) company1 = company1.ToUpper();


            clusterViewModel.clusterInfoList = clusterService.GetClusterInfoListkw(company1, cluster_name1).Pages(Request, this, 5);
            ViewBag.cluster_name = cluster_name;
            ViewBag.company = company;
            return View(clusterViewModel);
        }

        public ActionResult Cluster_UpdateSize(string cluster_no, string file_limit)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");
            ClusterInfoModel clusterInfoModel = new ClusterInfoModel();
            clusterInfoModel.cluster_no = int.Parse(cluster_no);
            clusterInfoModel.file_limit = Double.Parse(file_limit) *1024;

            int kk = clusterService.ClusterInfoUpdateSize(clusterInfoModel);

            if (kk == 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult Questionnaire()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            int activity_id = int.Parse(Request["activity_id"]);
            activityModel.activityregisterList = activityService.GetSellerInfoActivity(activity_id);
            ViewBag.activity_id = activity_id;
            ViewBag.buyer_id = Request["buyer_id"];
            ViewBag.company = Request["company"];
            ViewBag.activity_name = Request["activity_name"];

            return View(activityModel);
        }

        public ActionResult QuestionnaireList()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            int activity_id = int.Parse(Request["activity_id"]);
            string buyer_id = Request["buyer_id"];
            activityModel.questionnaireList = activityService.GetQuestionnaireList(activity_id, buyer_id).Pages(Request, this, 10);
            ViewBag.activity_id = activity_id;
            ViewBag.activity_name = Request["activity_name"];
            ViewBag.buyer_id = buyer_id;
            ViewBag.company = Request["company"];
            return View(activityModel);
        }

        public ActionResult QuestionnaireDelete()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            int activity_id = int.Parse(Request["activity_id"]);
            string buyer_id = Request["buyer_id"];
            string seller_id = Request["seller_id"];
            activityService.QuestionnaireDeleteOne(activity_id, buyer_id, seller_id);
            //ViewBag.activity_id = activity_id;
            //ViewBag.buyer_id = buyer_id;
            //ViewBag.company = Request["company"];
            return Redirect("QuestionnaireList?activity_id=" + activity_id + "&buyer_id=" + buyer_id + "&company=" + Request["company"]);
        }



        public ActionResult QuestionnaireEdit(int activity_id,string buyer_id, string seller_id,
            string question_1, string question_1_1, string set02, string question_1_2_other,
            string question_1_4, string question_2, string qedit,string company)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            //ViewBag.activity_id = activity_id;
            //ViewBag.buyer_id = buyer_id;
            //ViewBag.company = company;

            QuestionnaireModel qmodel = new QuestionnaireModel();

            qmodel.activity_id = activity_id;
            qmodel.buyer_id = buyer_id;
            qmodel.seller_id = seller_id;
            qmodel.question_1 = question_1;
            //qmodel.question_1_1 = "";
            //qmodel.question_1_2 = "";
            //qmodel.question_1_2_other = "";
            //qmodel.question_1_4 = "";
            //qmodel.question_2 = "";

            if (qmodel.question_1=="0")
            {
                qmodel.question_1_1 = question_1_1;
            }
            else if (qmodel.question_1 == "1")
            {
                qmodel.question_1_2 = set02;
                if (qmodel.question_1_2=="4")
                {
                    qmodel.question_1_2_other = question_1_2_other;
                }
            }
            else if (qmodel.question_1 == "3")
            {
                qmodel.question_1_4 = question_1_4;
            }
            qmodel.question_2 = question_2;

            if (qedit=="New")
            {
                activityService.QuestionnaireInsertOne(qmodel);
            }
            else
            {
                activityService.QuestionnaireUpdateOne(qmodel);
            }
            return Redirect("QuestionnaireList?activity_id=" + activity_id + "&buyer_id=" + buyer_id + "&company=" + company);
        }

        [HttpGet]
        public ActionResult CheckQuestionnaire(int activity_id, string buyer_id, string seller_id)
        {
            //            bool Huser = true;
            activityModel.questionnaire = activityService.GetQuestionnaireOne(activity_id, buyer_id,seller_id);
            if (activityModel.questionnaire != null)
            {
                return Json(activityModel.questionnaire, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        #region 匯出問卷結果
        [HttpGet]
        public ActionResult ExportQuestionnaireFormExcel(int activity_id, string activity_name,string buyer_id)
        {
            string questionnaireFormTemplateFileName = "questionnaire.xls";
            string questionnaireFormFileName = activity_id.ToString() + "_" + activity_name + "_" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + ".xls";

            var questionnairePOIDatas = activityService.GetQuestionnaireList(activity_id, buyer_id)
                                            .Select(ar =>
                                               new string[]
                                               {
                                                    ar.buyer_id,
                                                    ar.buyer_name,
                                                    ar.buyer_name_en,
                                                    ar.seller_id,
                                                    ar.seller_name,
                                                    ar.seller_name_en,
                                                    ar.question_1+","+ar.question_1_1+","+ar.question_1_2+","+ar.question_1_2_other+","+ar.question_1_4,
                                                    ar.question_2
                                               }
                                            ).ToList();

            IWorkbook workbook = loadExcelTemplate(excelTemplatePath + questionnaireFormTemplateFileName);
            setupQuestionnaireFormData(workbook, questionnairePOIDatas, 0);

            exportExcelFileByMemorySpace(workbook, questionnaireFormFileName);
            return null;
            //return exportExcelFile(workbook, activityFormFileName);
        }

        private void setupQuestionnaireFormData(IWorkbook workbook, IList<string[]> datas, int sheetNum)
        {
            ISheet _sheet = workbook.GetSheetAt(sheetNum);//因第一頁有編輯 所有不用CreateSheet
            for (int i = 0; i < datas.Count; i++)
            {
                IRow row = _sheet.GetRow(i + 1);
                if (row == null)
                {
                    row = _sheet.CreateRow(i + 1);
                }
                string[] columns = datas[i];
                for (int j = 0; j < columns.Length; j++)
                {
                    ICell _cell = row.CreateCell(j);
                    if (j==6)
                    {
                        string[] result = columns[j].Split(',');
                        string r = "";
                        if (result[0] == "0")
                        {
                            r = "1：訂單已成立";
                            if (result[1] != "")
                            {
                                r = r + "(訂單成交金額:US$ " + result[1] + ")";
                            }
                        }
                        else if (result[0] == "1")
                        {
                            r = "2：訂單成立可能性高";
                            if (result[2] != "")
                            {
                                string m = "";
                                if (result[2] == "0"){
                                    m = "Under USD 500,000";
                                }else if (result[2] == "1")
                                {
                                    m = "USD 510,000 ~ USD 1,000,000";
                                }
                                else if (result[2] == "2")
                                {
                                    m = "USD 1,010,000 ~ USD 1,500,000";
                                }
                                else if (result[2] == "3")
                                {
                                    m = "USD 1,510,000 ~ USD 2,000,000";
                                }
                                else if (result[2] == "4")
                                {
                                    m = "Other";
                                    if (result[3] != "")
                                    {
                                        m=m+" [USD "+ result[3] + "]";
                                    }
                                }

                                r = r + "(訂單預估成交金額:" + m + ")";
                            }
                        }
                        else if (result[0] == "2")
                        {
                            r = "3：不考慮立即下單";
                        }
                        else if (result[0] == "3")
                        {
                            r = "4：其他";
                            if (result[4] != "")
                            {
                                r = r + "  說明:" + result[4];
                            }
                        }
                        columns[j] = r;
                    }
                    _cell.SetCellValue(columns[j]);
                }
            }
        }

        #endregion


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
        public ActionResult ManagerInfo(int? where_grp_id, string where_manager_id)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");
            ViewBag.Title = "ManagerInfo";
            managerViewModel.groupList = managerService.getAllGroup();
            managerViewModel.managerInfoList = managerService.getManagerInfoByCondition(where_grp_id, where_manager_id).Pages(Request, this, 10);
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

        public ActionResult DeleteManagerInfoJson(string manager_id, string enable)
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

            managerViewModel.groupList = managerService.getAllGroup().Pages(Request, this, 10);

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

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GroupInsertUpdate(
              int? grp_id, string grp_name, string user, string activity
            , string push, string news, string manager, string statistic
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

            if (grp_id == null)
            {
                managerService.GroupInsertOne(grp_name, limits);
                return Redirect("Group");
            }
            else
            {
                bool isUpdateSuccess = managerService.GroupUpdateOne(grp_id, grp_name, limits);
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
        public ActionResult B_NewsList(string news_style,string news_type)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");
            string manager_id = null;
            int? grp_id = null;
            ViewBag.news_style = news_style;
            ViewBag.news_type = news_type;
            if (Request.Cookies["ManagerInfo"]["news"] == "2")
            {
                manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
                grp_id = managerService.getManagerGroup(Request.Cookies["ManagerInfo"]["manager_id"]);
            }
            if (news_style == "3") news_style = null;
            activityModel.newsList = activityService.GetNewsTypeView(news_type, grp_id, news_style).Pages(Request, this, 10);
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
            activityModel.activityinfoList = activityService.GetActivityInfoList(grp_id,null);
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


            activityModel.activityinfoList = activityService.GetActivityInfoList(grp_id,null).Pages(Request, this, 10);
            return View(activityModel);
        }
        #endregion

        #region 活動報名審核

        [HttpGet]
        public ActionResult ActivityRegisterCheck(int? selectActivityId, string selectCompany, string startDate, string endDate,string selectYesNoId)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            string manager_id = null;
            int? grp_id = null;

            if (selectYesNoId == "")
                selectYesNoId = null;

            if (Request.Cookies["ManagerInfo"]["activity"] == "2")
            {
                manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
                grp_id = managerService.getManagerGroup(Request.Cookies["ManagerInfo"]["manager_id"]);
            }
            activityModel.activityregisterList = activityService.GetActivityCheckAllByConditionWithId(selectActivityId, selectCompany, startDate, endDate, grp_id, selectYesNoId).Pages(Request, this, 10);
            activityModel.activityinfoList = activityService.GetActivityInfoList(grp_id,null);
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
                      registInfo.manager_check, registInfo.activity_id, registInfo.activity_name, registInfo.starttime.ToString("yyyy-MM-dd HH:mm"), registInfo.endtime.ToString("yyyy-MM-dd HH:mm")
                    , registInfo.addr, registInfo.quantity, registInfo.name_b, registInfo.phone, registInfo.email);
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
            activityModel.activityregisterList = activityService.GetActivityCheckAllByCondition(selectActivityName, selectCompany, startDate, endDate, grp_id);
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
            activityModel.activityregisterList = activityService.GetActivityCheckAllByCondition(selectActivityName, selectCompany, startDate, endDate, grp_id);
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

            activityModel.userinfoList = userService.GetUserInfoListkw(user_id, company).Pages(Request, this, 10);
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

        public static string GetNewSortId(string QldSortId)
        {
            string NewSortId = "";
            //New sort_id------------
            if (QldSortId == "C1" || QldSortId == "C2")
                NewSortId = "A";
            else if (QldSortId == "C3"
                || QldSortId == "CI" || QldSortId == "CJ"
                || QldSortId == "CK" || QldSortId == "CL"
                || QldSortId == "C4")
                NewSortId = "B";
            else if (QldSortId == "C8" || QldSortId == "C9"
                || QldSortId == "CA" || QldSortId == "CB"
                || QldSortId == "CC" || QldSortId == "CD"
                || QldSortId == "CE" || QldSortId == "CF"
                || QldSortId == "CG")
                NewSortId = "C";
            else if (QldSortId == "C5" || QldSortId == "C6"
                || QldSortId == "CH" || QldSortId == "CM"
                || QldSortId == "CN" || QldSortId == "CO"
                || QldSortId == "C7")
                NewSortId = "D";
            else if (QldSortId == "CP" || QldSortId == "CQ"
                || QldSortId == "CR"
                || QldSortId == "CZ")
                NewSortId = "E";
            else if (QldSortId == "G1" || QldSortId == "G2"
                || QldSortId == "G3" || QldSortId == "G4"
                || QldSortId == "G5" || QldSortId == "G6"
                || QldSortId == "G7")
                NewSortId = "F";
            else if (QldSortId == "G8" || QldSortId == "G9"
                || QldSortId == "IE"
                || QldSortId == "GA")
                NewSortId = "G";
            else if (QldSortId == "I1" || QldSortId == "I3"
                || QldSortId == "I4" || QldSortId == "IC"
                || QldSortId == "ID" || QldSortId == "IF"
                || QldSortId == "IG" || QldSortId == "IZ"
                || QldSortId == "I5")
                NewSortId = "H";
            else if (QldSortId == "I7" || QldSortId == "I8"
                || QldSortId == "I9"
                || QldSortId == "IB")
                NewSortId = "I";
            else
                NewSortId = "J";
            //-------------------------------
            return NewSortId;
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
                        companydata.Business_Item.Add(GetNewSortId(jarray[0].Cmp_Business[i].Business_Item.Substring(0, 2)));
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
        public ActionResult BuyerInfoList(string activity_name, string company)
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

            if (activity_name == "") activity_name = null;
            if (company == "") company = null;
            //            activityModel.buyerinfoList = activityService.GetBuyerInfoAll(grp_id, DateTime.Now).Pages(Request, this, 10);
            activityModel.buyerinfoList = activityService.GetBuyerInfoAll(grp_id, null, activity_name, company).Pages(Request, this, 10);
            ViewBag.activity_name = activity_name;
            ViewBag.company = company;
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
            activityModel.activityinfoList = activityService.GetActivityInfoList(grp_id,DateTime.Now);
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
                if (model.activity_id == 0 || string.IsNullOrEmpty(model.buyer_id))
                {
                    TempData["buyer_errMsg"] = "此買主或此活動不存在 ";
                }
                else
                {
                    var buyModel = activityService.GetBuyerDataByActivityWithIdOne(model.activity_id, model.buyer_id);
                    if (buyModel == null)
                    {
                        var serial_no = activityService.BuyerInfoInsertOne(model);
                        if (serial_no != null)
                        {
                            UserInfoModel buyer = userService.GeUserInfoOne(model.buyer_id);
                            ActivityInfoModel activity = activityService.GetActivityInfoOne(model.activity_id);
                            MailHelper.sendActivityAddBuyerNotify(
                                buyer.email, model.activity_id, activity.activity_name, activity.starttime
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
                    //var buyModel = activityService.GetBuyerDataByActivityWithIdOne(model.activity_id, model.buyer_id);
                    //if (buyModel == null)
                    //{
                    bool isUpdateSuccess = activityService.BuyerInfoUpdateOne(model);
                    //}
                    //else
                    //{
                    //    TempData["buyer_errMsg"] = "更新失敗...此企業原本就是該活動買主";
                    //}
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

            logger.Info("上傳檔案名稱:" + iupexl.FileName);
            if (iupexl != null && iupexl.FileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase) && iupexl.ContentLength > 0)
            {
                string targetDir = "_temp";
                Dictionary<string, string> uploadResultDic = null;
                uploadResultDic = UploadHelper.doUploadFile(iupexl, targetDir, UploadConfig.AdminManagerDirName);
                logger.Info("上傳結果:" + uploadResultDic["result"]);

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
            foreach (KeyValuePair<int, object> kvp in successUserInfos)
            {
                string user_id = ((Dictionary<string, string>)kvp.Value)["user_id"];
                try
                {
                    CompanyData compdata = GetDataFromWeb(user_id);
                    logger.Info(user_id + "取回的值是否成功:");
                    if (compdata == null)
                    {
                        logger.Info("不存在");
                    }
                    else
                    {
                        logger.Info("存在" + "=>資本額為" + (compdata.Capital_Stock_Amount == null ? "0" : compdata.Capital_Stock_Amount.ToString()));
                    }
                    var sort_id = result.Where(item => compdata != null && compdata.Business_Item != null && compdata.Business_Item.Contains(item.enterprise_sort_id)).Select(item => item.sort_id).Distinct().ToArray();
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
                catch (Exception ex)
                {
                    logger.Error("-----------------------------------");
                    logger.Error("user_id=" + user_id + "抓取opendata失敗..");
                    logger.Error(ex.Message);
                    continue;
                }
            }
        }

        #endregion

        #region 匯出活動買家和賣家表單
        [HttpGet]
        public ActionResult ExportActivityFormExcel(int activity_id, string activity_name)
        {
            string activityFormTemplateFileName = "activity_form.xls";
            string activityFormFileName = activity_id.ToString() + "_" + activity_name + "_" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + ".xls";
            var sellerNeedList = matchService.GetSellerNeedWithCompany(activity_id);
            var sellerNeedIds = sellerNeedList.GetSelectList(sn => sn["seller_id"]);
            var sellerNeedPOIDatas = sellerNeedList.Select( sn => 
                                                            new string[]
                                                            {
                                                                (string)sn["seller_id"],
                                                                (string)sn["seller_company"],
                                                                (string)sn["seller_company_en"],
                                                                (string)sn["buyer_id"],
                                                                (string)sn["buyer_company"],
                                                                (string)sn["buyer_company_en"]
                                                            }
                                                         ).ToList();

            var buyerNeedList = matchService.GetBuyerNeedWithCompany(activity_id);
            var buyerNeedIds = buyerNeedList.GetSelectList(bn => bn["buyer_id"]);
            var buyerNeedPOIDatas = buyerNeedList.Select(sn =>
                                                           new string[]
                                                           {
                                                                (string)sn["buyer_id"],
                                                                (string)sn["buyer_company"],
                                                                (string)sn["buyer_company_en"],
                                                                (string)sn["seller_id"],
                                                                (string)sn["seller_company"],
                                                                (string)sn["seller_company_en"]

                                                            }
                                                         ).ToList();
            var activityRegisterPOIDatas = activityService
                                            .GetARCheckPassList(activity_id)
                                            .Select( ar =>
                                                new string[]
                                                {
                                                    ar.register_id.ToString(),
                                                    ar.create_time.ToString("yyyy-MM-dd HH:mm"),
                                                    sellerNeedIds.IndexOf(ar.user_id) != -1  ? "是" : "否",
                                                    ar.user_id,
                                                    ar.company,
                                                    ar.company_en,
                                                    ar.quantity.ToString(),
                                                    ar.name_a,
                                                    ar.title_a,
                                                    ar.name_b,
                                                    ar.title_b,
                                                    ar.telephone,
                                                    ar.phone,
                                                    ar.email,
                                                    ar.addr,
                                                    ar.addr_en,
                                                    string.Join(",", activityService.GetActivityCatalogSelectList(ar.user_id, ar.activity_id)
                                                                        .Select( cl => cl.catalog_name )
                                                                        .ToArray()),
                                                    string.Join(",", activityService.GetActivityProductSelectList(ar.user_id, ar.activity_id)
                                                                        .Select(cl => cl.product_name)
                                                                        .ToArray()),
                                                    string.Join(",", userService.SelectUserSortByUserId(ar.user_id)
                                                                        .Select(us => us.enterprise_sort_name)
                                                                        .ToArray()),
                                                    ar.user_info,
                                                    ar.user_info_en
                                                }
                                            ).ToList();
            
            var buyerListPOIDatas = activityService.GetBuyerInfoActivity(activity_id)
                                                   .Select(ba => 
                                                    new string[] 
                                                    {
                                                        ba.serial_no.ToString(),
                                                        ba.buyer_id,
                                                        ba.company,
                                                        ba.company_en,
                                                        ba.buyer_need,
                                                        buyerNeedIds.IndexOf(ba.buyer_id) != -1 ? "是" : "否"
                                                    }
                                                   ).ToList();

            IWorkbook workbook = loadExcelTemplate(excelTemplatePath + activityFormTemplateFileName);
            setupActivityFormData(workbook, activityRegisterPOIDatas, 0);
            setupActivityFormData(workbook, buyerListPOIDatas, 1);
            setupActivityFormData(workbook, sellerNeedPOIDatas, 2);
            setupActivityFormData(workbook, buyerNeedPOIDatas, 3);

            exportExcelFileByMemorySpace(workbook, activityFormFileName);
            return null;
            //return exportExcelFile(workbook, activityFormFileName);
        }

        private void setupActivityFormData(IWorkbook workbook, IList<string[]> datas, int sheetNum)
        {
            ISheet _sheet = workbook.GetSheetAt(sheetNum);//因第一頁有編輯 所有不用CreateSheet
            for(int i=0; i<datas.Count; i++)
            {
                IRow row = _sheet.GetRow(i+1);
                if (row == null)
                {
                    row = _sheet.CreateRow(i+1);
                }
                string[] columns = datas[i];
                for (int j = 0; j<columns.Length; j++)
                {
                    ICell _cell = row.CreateCell(j);
                    _cell.SetCellValue(columns[j]);
                }
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

        #region 媒合時程大表列表與匯出Excel共用的方法
        public Match_ViewModel MakeSchedule()
        {
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
            matchModel.matchmakingBuyerList = matchService.GetCertainActivityBuyerCheckSellerList(int.Parse(Request["activity_id"]), "");
            /*列出某活動的賣家有意願*/
            matchModel.matchmakingSellerList = matchService.GetMSneedBySellerCompanyList(int.Parse(Request["activity_id"]));

            /*列出某活動的雙方媒合意願與買方媒合意願的合併資料*/
            if (matchModel.schedulePeriodSetList.Count != 0)
            {
                int allCount = (matchModel.schedulePeriodSetList.Count / matchModel.schedulePeriodSetList.Count)
                                * matchModel.buyerinfoList.Count;
                matchModel.matchSellerCompanyDatamergeList = new List<List<Tuple<string, string, string>>>();
                matchModel.matchBothForbuyer_idList = new List<List<Tuple<string, string, string>>>();

                for (int temp = 0; temp < allCount; temp++)
                {
                    List<Tuple<string, string, string>> mergeData = new List<Tuple<string, string, string>>();
                    List<Tuple<string, string, string>> bothData = new List<Tuple<string, string, string>>();
                    matchModel.matchSellerCompanyDatamergeList.Add(mergeData);
                    matchModel.matchBothForbuyer_idList.Add(bothData);
                }

                for (int i = 0; i < matchModel.buyerinfoList.Count; i++)
                {
                    var bothAllBuyer_id = matchModel.matchmakingBothList.Select(both => both.buyer_id).ToArray();
                    if (bothAllBuyer_id.Contains(matchModel.buyerinfoList[i].buyer_id))
                    {
                        var bothList =
                            matchModel.matchmakingBothList
                            .Where(both => both.buyer_id == matchModel.buyerinfoList[i].buyer_id)
                            .Select(both => new Tuple<string, string, string>(IsBothOrBuyer = "both", both.seller_id, both.company)).ToList();

                        matchModel.matchSellerCompanyDatamergeList[i].AddRange(bothList);
                        matchModel.matchBothForbuyer_idList[i].AddRange(bothList);
                    }

                    var buyerAllBuyer_id = matchModel.matchmakingBuyerList.Select(buyer => buyer.buyer_id).ToArray();
                    if (buyerAllBuyer_id.Contains(matchModel.buyerinfoList[i].buyer_id))
                    {
                        var bothList =
                            matchModel.matchmakingBothList
                            .Where(both => both.buyer_id == matchModel.buyerinfoList[i].buyer_id)
                            .Select(both => new Tuple<string, string, string>(IsBothOrBuyer = "buyer", both.seller_id, both.company)).ToList();

                        var buyerList =
                            matchModel.matchmakingBuyerList
                            .Where(buyer => buyer.buyer_id == matchModel.buyerinfoList[i].buyer_id)
                            .Select(buyer => new Tuple<string, string, string>(IsBothOrBuyer = "buyer", buyer.seller_id, buyer.company)).ToList();

                        var exceptList = buyerList.Except(bothList);
                        matchModel.matchSellerCompanyDatamergeList[i].AddRange(exceptList);
                    }
                }

                /*當只設定時間時,需複製資料給下拉選單,不然會出錯*/
                for (int i = 0; i < matchModel.schedulePeriodSetList.Count - 1; i++) //原先就加過一次
                {
                    var copyMergeData = matchModel.matchSellerCompanyDatamergeList.Select(copy => copy.ToList()).ToList();
                    var copyMergeDataRange = copyMergeData.GetRange(0, matchModel.buyerinfoList.Count);
                    matchModel.matchSellerCompanyDatamergeList.AddRange(copyMergeDataRange);
                    copyMergeData.Clear();
                    copyMergeDataRange.Clear();
                }

                /*matchSellerCompanyDatamergeList 取company存到陣列中*/
                matchModel.bothWithbuyerMergeSellerCompany = Enumerable.Repeat(String.Empty, matchModel.matchSellerCompanyDatamergeList.Count).ToArray();
                var companys = matchModel.matchSellerCompanyDatamergeList
                                          .SelectMany(data =>
                                                      data.Select(comapny => comapny.Item3)).ToList();
                var companysArrays = companys.Distinct().ToArray();

                //matchModel.bothWithbuyerMergeSellerCompany = companys;
                matchModel.bothWithbuyerMergeSellerCompany = companysArrays;
            }

            /*列出某活動的媒合大表資料*/
            matchModel.matchmakingScheduleList = matchService.GetCertainActivityMatchMakingDataList(int.Parse(Request["activity_id"]));

            /*列出某活動的時間區段輸入媒合的賣家*/
            long x = notFoundIndex, y = notFoundIndex; //x是時段, y是買主
            matchModel.matchMakingScheduleSellerCompany = Enumerable.Repeat(String.Empty, matchModel.buyerinfoList.Count * matchModel.schedulePeriodSetList.Count).ToArray();
            matchModel.matchMakingScheduleSellerId = Enumerable.Repeat(String.Empty, matchModel.buyerinfoList.Count * matchModel.schedulePeriodSetList.Count).ToArray();

            foreach (MatchmakingScheduleModel matchmakingScheduleModel in matchModel.matchmakingScheduleList)
            {
                foreach (SchedulePeriodSetModel schedulePeriodSetModel in matchModel.schedulePeriodSetList)
                {
                    if (schedulePeriodSetModel.period_sn == matchmakingScheduleModel.period_sn)
                    {
                        x = matchModel.schedulePeriodSetList.IndexOf(schedulePeriodSetModel);
                    }
                }

                foreach (BuyerInfoModel buyerInfoModel in matchModel.buyerinfoList)
                {
                    if (buyerInfoModel.buyer_id.Equals(matchmakingScheduleModel.buyer_id))
                    {
                        y = matchModel.buyerinfoList.IndexOf(buyerInfoModel);
                    }
                }

                if ((x != notFoundIndex) && (y != notFoundIndex))
                {
                    matchModel.matchMakingScheduleSellerCompany[x * matchModel.buyerinfoList.Count + y] = matchmakingScheduleModel.company;
                    matchModel.matchMakingScheduleSellerId[x * matchModel.buyerinfoList.Count + y] = matchmakingScheduleModel.seller_id;
                }
            }

            return matchModel;
        }
        #endregion

        #region 媒合時程大表列表
        [HttpGet]
        public ActionResult MatchScheduleList()
        {
            if (Request.Cookies["ManagerInfo"] == null)
            {
                return Redirect("Login");
            }

            MakeSchedule();

            /*刪除某時段，媒合大表中的相同時段資料刪除*/
            var schedulePeriodSn = matchModel.schedulePeriodSetList
                   .Select(time => time.period_sn);
            var matchmakingPeriodSn = matchModel.matchmakingScheduleList
                   .Select(data => data.period_sn);

            var periodSns = from periodSn in matchmakingPeriodSn
                            where schedulePeriodSn.Contains(periodSn) == false
                            select periodSn;

            if (periodSns.Count() != 0)
            {
                foreach (int periodSn in periodSns)
                {
                    matchService.MatchkingDataByActivityWithPeriodDelete(int.Parse(Request["activity_id"]), periodSn);
                }
            }

            /*加這段是為了判斷刪除媒合大表資料後,資料要新的狀態*/
            matchModel.matchmakingScheduleList = matchService.GetCertainActivityMatchMakingDataList(int.Parse(Request["activity_id"]));

            if (matchModel.matchmakingScheduleList.Count != 0)
            {
                List<int> indexAll = new List<int>();
                for (int i = 0; i < matchModel.matchMakingScheduleSellerId.Length; i++)
                {
                    if (!matchModel.matchMakingScheduleSellerId[i].IsNullOrEmpty())
                    {
                        indexAll.Add(i);
                    }
                }

                /*刪除列的資料*/
                foreach (int index in indexAll)
                {
                    double row = Math.Floor((double)index / matchModel.buyerinfoList.Count);
                    for (int indexRow = 0; indexRow < matchModel.buyerinfoList.Count; indexRow++)
                    {
                        matchModel.matchSellerCompanyDatamergeList
                            [(int)row * matchModel.buyerinfoList.Count + indexRow]
                            .RemoveAll(item => item.Item2.Equals(matchModel.matchMakingScheduleSellerId[index]));
                    }
                }

                /*刪除行的資料*/
                foreach (int index in indexAll)
                {
                    int column = index % matchModel.buyerinfoList.Count;
                    for (int indexColumn = column; indexColumn < matchModel.matchSellerCompanyDatamergeList.Count; indexColumn += matchModel.buyerinfoList.Count)
                    {
                        matchModel.matchSellerCompanyDatamergeList
                            [indexColumn]
                            .RemoveAll(item => item.Item2.Equals(matchModel.matchMakingScheduleSellerId[index]));
                    }
                }

                /*回填資料*/
                for (int i = 0; i < matchModel.matchMakingScheduleSellerId.Length; i++)
                {
                    if (!matchModel.matchMakingScheduleSellerId[i].IsNullOrEmpty())
                    {
                        var seller_idArray = matchModel.matchBothForbuyer_idList[i % matchModel.buyerinfoList.Count]
                                  .Select(item => item.Item2).ToArray();

                        if (seller_idArray.Contains(matchModel.matchMakingScheduleSellerId[i]))
                        {
                            List<Tuple<string, string, string>> backFillDataList = new List<Tuple<string, string, string>>();
                            Tuple<string, string, string> backFillData = new Tuple<string, string, string>(IsBothOrBuyer = "both", matchModel.matchMakingScheduleSellerId[i], matchModel.matchMakingScheduleSellerCompany[i]);
                            backFillDataList.Add(backFillData);
                            matchModel.matchSellerCompanyDatamergeList[i].AddRange(backFillDataList);
                        }
                        else
                        {
                            List<Tuple<string, string, string>> backFillDataList = new List<Tuple<string, string, string>>();
                            Tuple<string, string, string> backFillData = new Tuple<string, string, string>(IsBothOrBuyer = "buyer", matchModel.matchMakingScheduleSellerId[i], matchModel.matchMakingScheduleSellerCompany[i]);
                            backFillDataList.Add(backFillData);
                            matchModel.matchSellerCompanyDatamergeList[i].AddRange(backFillDataList);
                        }
                    }
                }

            }

            return View(matchModel);
        }
        #endregion

        #region 媒合時程大表條件限制
        [HttpPost]
        public ActionResult MatchConstrains(string[] allSellerId, int activity_id)
        {
            string IsBothOrBuyer;
            /*列出某活動的所有買主*/
            matchModel.buyerinfoList = matchService.GetSellerMatchToBuyerNameAndNeedList(activity_id);
            /*列出某活動的所有媒合時段*/
            matchModel.schedulePeriodSetList = matchService.GetActivityMatchTimeIntervalList(activity_id);
            /*列出某活動的雙方有意願*/
            matchModel.matchmakingBothList = matchService.GetMatchmakingbothneedList(activity_id);
            /*列出某活動的買家有意願*/
            matchModel.matchmakingBuyerList = matchService.GetCertainActivityBuyerCheckSellerList(activity_id, "");

            int allCount = (matchModel.schedulePeriodSetList.Count / matchModel.schedulePeriodSetList.Count)
                                * matchModel.buyerinfoList.Count;

            matchModel.matchSellerCompanyDatamergeList = new List<List<Tuple<string, string, string>>>();


            for (int temp = 0; temp < allCount; temp++)
            {
                List<Tuple<string, string, string>> data = new List<Tuple<string, string, string>>();
                matchModel.matchSellerCompanyDatamergeList.Add(data);
            }

            for (int i = 0; i < matchModel.buyerinfoList.Count; i++)
            {
                var bothAllBuyer_id = matchModel.matchmakingBothList.Select(both => both.buyer_id).ToArray();
                if (bothAllBuyer_id.Contains(matchModel.buyerinfoList[i].buyer_id))
                {
                    var bothList =
                        matchModel.matchmakingBothList
                        .Where(both => both.buyer_id == matchModel.buyerinfoList[i].buyer_id)
                        .Select(both => new Tuple<string, string, string>(IsBothOrBuyer = "both", both.seller_id, both.company)).ToList();

                    matchModel.matchSellerCompanyDatamergeList[i].AddRange(bothList);
                }

                var buyerAllBuyer_id = matchModel.matchmakingBuyerList.Select(buyer => buyer.buyer_id).ToArray();
                if (buyerAllBuyer_id.Contains(matchModel.buyerinfoList[i].buyer_id))
                {
                    var bothList =
                        matchModel.matchmakingBothList
                        .Where(both => both.buyer_id == matchModel.buyerinfoList[i].buyer_id)
                        .Select(both => new Tuple<string, string, string>(IsBothOrBuyer = "buyer", both.seller_id, both.company)).ToList();

                    var buyerList =
                        matchModel.matchmakingBuyerList
                        .Where(buyer => buyer.buyer_id == matchModel.buyerinfoList[i].buyer_id)
                        .Select(buyer => new Tuple<string, string, string>(IsBothOrBuyer = "buyer", buyer.seller_id, buyer.company)).ToList();

                    var exceptList = buyerList.Except(bothList);
                    matchModel.matchSellerCompanyDatamergeList[i].AddRange(exceptList);
                }
            }

            /*複製資料*/
            for (int i = 0; i < matchModel.schedulePeriodSetList.Count - 1; i++) //原先就加過一次
            {
                var copyMergeData = matchModel.matchSellerCompanyDatamergeList.Select(copy => copy.ToList()).ToList();
                var copyMergeDataRange = copyMergeData.GetRange(0, matchModel.buyerinfoList.Count);
                matchModel.matchSellerCompanyDatamergeList.AddRange(copyMergeDataRange);
                copyMergeData.Clear();
                copyMergeDataRange.Clear();
            }

            List<int> indexAll = new List<int>();

            for (int i = 0; i < allSellerId.Length; i++)
            {
                if (!allSellerId[i].IsNullOrEmpty())
                {
                    indexAll.Add(i);
                }
            }

            /*刪除列的資料*/
            foreach (int index in indexAll)
            {
                double x = Math.Floor((double)index / matchModel.buyerinfoList.Count);
                for (int indexRow = 0; indexRow < matchModel.buyerinfoList.Count; indexRow++)
                {
                    matchModel.matchSellerCompanyDatamergeList
                              [(int)x * matchModel.buyerinfoList.Count + indexRow].RemoveAll(item => item.Item2.Equals(allSellerId[index]));
                }
            }

            /*刪除行的資料*/
            foreach (int index in indexAll)
            {
                int y = index % matchModel.buyerinfoList.Count;
                for (int indexColumn = y; indexColumn < matchModel.matchSellerCompanyDatamergeList.Count; indexColumn += matchModel.buyerinfoList.Count)
                {
                    matchModel.matchSellerCompanyDatamergeList
                              [indexColumn].RemoveAll(item => item.Item2.Equals(allSellerId[index]));
                }
            }

            //var result = new { filteredSellerData = matchModel.matchSellerCompanyDatamergeList, selectedSellerData = allSellerCompany };
            return Json(matchModel.matchSellerCompanyDatamergeList);
        }
        #endregion

        #region 媒合時程大表新增修改刪除新版
        [HttpPost]
        public ActionResult StoreMatchData(int[] period_sn, int activity_id, string[] buyer_id, string[] seller_id)
        {
            if (Request.Cookies["ManagerInfo"] == null)
            {
                return Redirect("Login");
            }

            /*列出某活動的媒合大表資料*/
            matchModel.matchmakingScheduleList = matchService.GetCertainActivityMatchMakingDataList(activity_id);
            /*列出某活動的所有買主*/
            matchModel.buyerinfoList = matchService.GetSellerMatchToBuyerNameAndNeedList(activity_id);
            /*列出某活動的所有媒合時段*/
            matchModel.schedulePeriodSetList = matchService.GetActivityMatchTimeIntervalList(activity_id);
            /*列出某活動的時間區段輸入媒合的賣家*/
            long i = notFoundIndex, j = notFoundIndex; //i是時段, j是買主
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
                    if (buyerInfoModel.buyer_id.Equals(model.buyer_id))
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
                if (matchModel.matchMakingScheduleSellerCompany[x].Equals("") && seller_id[x].Length != 0)
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
                    matchmakingScheduleModel.update_time = DateTime.Now;
                    matchService.CertainActivityMatchkingDataUpdate(matchmakingScheduleModel);
                }
                else if (matchModel.matchMakingScheduleSellerCompany[x].Length != 0 && seller_id[x].Equals(""))
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

        #region 媒合大表匯出Excel新版
        [HttpGet]
        public ActionResult ExportExcelByNPOI()
        {
            if (Request.Cookies["ManagerInfo"] == null)
            {
                return Redirect("Login");
            }

            MakeSchedule();

            /*讀取樣板*/
            //string excelPath = Path.Combine(Server.MapPath("~/Content/Template/Import"), "tmpmatchmaking.xls");
            IWorkbook workbook = loadExcelTemplate(excelTemplatePath + "tmpmatchmaking.xls");

            ISheet _sheet = workbook.GetSheetAt(0);//因第一頁有編輯 所有不用CreateSheet
            ICellStyle cellStyleheader = _sheet.GetRow(0).Cells[0].CellStyle;//取第一行的第一個的style;
            ICellStyle cellStyleTime = _sheet.GetRow(0).Cells[1].CellStyle;
            ICellStyle cellStyleBoth = _sheet.GetRow(0).Cells[2].CellStyle;
            ICellStyle cellStyleBuyer = _sheet.GetRow(0).Cells[3].CellStyle;
            ICellStyle cellStyleSeller = _sheet.GetRow(0).Cells[4].CellStyle;
            int column = 1; //填資料都從第二格開始
            int colorColumn = 0;//填顏色資料從第一個開始

            #region header部分
            /* header部分*/
            IRow headerRow = _sheet.CreateRow(0);//第一行
            CreateCell("時段\\買方名稱", headerRow, 0, cellStyleheader);
            foreach (BuyerInfoModel buyerInfoModel in matchModel.buyerinfoList)
            {
                CreateCell(buyerInfoModel.company, headerRow, column, cellStyleheader);
                column++;
            }
            #endregion

            #region 時間與有輸入媒合的部分
            /* 時間與有輸入媒合的部分*/
            int rowCount = 1; //資料從二行開始
            if (matchModel.schedulePeriodSetList.Count != 0)
            {
                column = 1;
                for (int i = 0; i < matchModel.schedulePeriodSetList.Count; i++)
                {
                    IRow timeRow = _sheet.GetRow(rowCount);
                    if (timeRow == null)
                    {
                        timeRow = _sheet.CreateRow(rowCount);
                    }
                    CreateCell(matchModel.schedulePeriodSetList[i].time_start.ToString("yyyy/MM/dd HH:mm") + "~" +
                               matchModel.schedulePeriodSetList[i].time_end.ToString("yyyy/MM/dd HH:mm"),
                               timeRow, 0, cellStyleTime);

                    for (int j = 0; j < matchModel.buyerinfoList.Count; j++)
                    {
                        IRow matchRow = _sheet.GetRow(rowCount);
                        CreateCell(matchModel.matchMakingScheduleSellerCompany[i * matchModel.buyerinfoList.Count + j],
                            matchRow, column, cellStyleTime);
                        column++;
                    }
                    column = 1;
                    rowCount++;
                }
            }
            #endregion

            #region 雙方有媒合意願部分
            /*雙方有媒合意願部分*/
            rowCount = 1;
            column = 1;
            if (matchModel.schedulePeriodSetList.Count != 0)
            {
                rowCount += matchModel.schedulePeriodSetList.Count;
            }

            IRow bothRow = _sheet.GetRow(rowCount);
            if (bothRow == null)
            {
                bothRow = _sheet.CreateRow(rowCount);
            }

            var bothmaximum = matchModel.matchBothForbuyer_idList.Select(data => data)
                        .Aggregate((a, x) => (x.Count > a.Count) ? x : a);

            for (int i = 0; i < bothmaximum.Count; i++)
            {
                for (int j = 0; j < matchModel.buyerinfoList.Count + 1; j++)
                {
                    bothRow = _sheet.GetRow(rowCount);
                    if (bothRow == null)
                    {
                        bothRow = _sheet.CreateRow(rowCount);
                    }
                    CreateCell("", bothRow, colorColumn, cellStyleBoth);
                    colorColumn++;
                }
                colorColumn = 0;
                rowCount++;
            }

            if (matchModel.schedulePeriodSetList.Count != 0)
            {
                rowCount = 1;
                rowCount += matchModel.schedulePeriodSetList.Count;
            }
            else
            {
                rowCount = 1;
            }

            bothRow = _sheet.GetRow(rowCount);
            CreateCell("雙方有媒合意願", bothRow, 0, cellStyleBoth);

            for (int i = 0; i < matchModel.matchBothForbuyer_idList.Count; i++)
            {
                bothRow = _sheet.GetRow(rowCount);
                if (bothRow == null)
                {
                    bothRow = _sheet.CreateRow(rowCount);
                }

                for (int j = 0; j < bothmaximum.Count; j++)
                {
                    try
                    {
                        bothRow = _sheet.GetRow(rowCount);
                        if (bothRow == null)
                        {
                            bothRow = _sheet.CreateRow(rowCount);
                        }
                        CreateCell(matchModel.matchBothForbuyer_idList[i][j].Item3,
                                    bothRow, column, cellStyleBoth);
                        rowCount++;
                    }
                    catch
                    {

                    }
                }
                rowCount = matchModel.schedulePeriodSetList.Count + 1;
                column++;
            }

            #endregion

            #region 買家有媒合意願部分
            /*買家有媒合意願部分*/
            matchModel.matchBuyerForbuyer_idList = new List<string[]>();
            foreach (BuyerInfoModel buyerInfoModel in matchModel.buyerinfoList)
            {
                var buyerNeedAllArray = matchModel.matchmakingBuyerList.Select(buyer => buyer.buyer_id).ToArray();

                if (buyerNeedAllArray.Contains(buyerInfoModel.buyer_id))
                {
                    var bothallstr = matchModel.matchmakingBothList
                               .Where(both => both.buyer_id == buyerInfoModel.buyer_id)
                               .Select(both => both.company);


                    var buyerallstr = matchModel.matchmakingBuyerList
                                .Where(buyer => buyer.buyer_id == buyerInfoModel.buyer_id)
                                .Select(buyer => buyer.company);

                    var exceptStr = buyerallstr.Except(bothallstr).ToArray();//取差集後的公司名稱
                    matchModel.matchBuyerForbuyer_idList.Add(exceptStr);
                }
            }

            var buyermaximun = matchModel.matchBuyerForbuyer_idList.Select(data => data)
                        .Aggregate((a, x) => (x.Length > a.Length) ? x : a);

            int buyerRowCount;
            column = 1;
            if (matchModel.schedulePeriodSetList.Count != 0)
            {
                buyerRowCount = matchModel.schedulePeriodSetList.Count + bothmaximum.Count + 1;//時間+雙方+自己
            }
            else
            {
                buyerRowCount = bothmaximum.Count + 1; //雙方+自己
            }

            IRow buyerRow = _sheet.GetRow(buyerRowCount);
            if (buyerRow == null)
            {
                buyerRow = _sheet.CreateRow(buyerRowCount);
            }

            for (int i = 0; i < buyermaximun.Length; i++)
            {
                for (int j = 0; j < matchModel.buyerinfoList.Count + 1; j++)
                {
                    buyerRow = _sheet.GetRow(buyerRowCount);
                    if (buyerRow == null)
                    {
                        buyerRow = _sheet.CreateRow(buyerRowCount);
                    }
                    CreateCell("", buyerRow, colorColumn, cellStyleBuyer);
                    colorColumn++;
                }
                colorColumn = 0;
                buyerRowCount++;
            }

            if (matchModel.schedulePeriodSetList.Count != 0)
            {
                buyerRowCount = matchModel.schedulePeriodSetList.Count + bothmaximum.Count + 1;//時間+雙方+自己
            }
            else
            {
                buyerRowCount = bothmaximum.Count + 1; //雙方+自己
            }

            buyerRow = _sheet.GetRow(buyerRowCount);
            CreateCell("買家有媒合意願", buyerRow, 0, cellStyleBuyer);

            for (int i = 0; i < matchModel.matchBuyerForbuyer_idList.Count; i++)
            {
                buyerRow = _sheet.GetRow(buyerRowCount);

                for (int j = 0; j < buyermaximun.Length; j++)
                {
                    try
                    {
                        buyerRow = _sheet.GetRow(buyerRowCount);
                        CreateCell(matchModel.matchBuyerForbuyer_idList[i][j],
                                    buyerRow, column, cellStyleBuyer);
                        buyerRowCount++;
                    }
                    catch
                    {

                    }
                }

                if (matchModel.schedulePeriodSetList.Count != 0)
                {
                    buyerRowCount = matchModel.schedulePeriodSetList.Count +
                                     bothmaximum.Count + 1;
                }
                else {
                    buyerRowCount = bothmaximum.Count + 1;
                }
                column++;
            }

            #endregion

            #region 賣家有媒合意願部分
            /*賣家有媒合意願部分*/
            matchModel.matchSellerForbuyer_idList = new List<string[]>();
            foreach (BuyerInfoModel buyerInfoModel in matchModel.buyerinfoList)
            {
                var sellerNeedAllArray = matchModel.matchmakingSellerList.Select(seller => seller.buyer_id).ToArray();

                if (sellerNeedAllArray.Contains(buyerInfoModel.buyer_id))
                {
                    var bothAllStr = matchModel.matchmakingBothList
                               .Where(both => both.buyer_id == buyerInfoModel.buyer_id)
                               .Select(both => both.company);

                    var sellerAllstr = matchModel.matchmakingSellerList
                                .Where(seller => seller.buyer_id == buyerInfoModel.buyer_id)
                                .Select(seller => seller.company);

                    var exceptStr = sellerAllstr.Except(bothAllStr).ToArray();//取差集後的公司名稱
                    matchModel.matchSellerForbuyer_idList.Add(exceptStr);
                }
            }

            var sellermaximun = matchModel.matchSellerForbuyer_idList.Select(data => data)
                        .Aggregate((a, x) => (x.Length > a.Length) ? x : a);

            int sellerRowCount;
            column = 1;

            if (matchModel.schedulePeriodSetList.Count != 0)
            {
                sellerRowCount = matchModel.schedulePeriodSetList.Count + 
                     bothmaximum.Count + buyermaximun.Length + 1;//時間+雙方+買家+自己
            }
            else
            {
                sellerRowCount = bothmaximum.Count + buyermaximun.Length + 1; //雙方+買家+自己
            }

            IRow sellerRow = _sheet.GetRow(sellerRowCount);
            if (sellerRow == null)
            {
                sellerRow = _sheet.CreateRow(sellerRowCount);
            }

            for (int i = 0; i < sellermaximun.Length; i++)
            {
                for (int j = 0; j < matchModel.buyerinfoList.Count + 1; j++)
                {
                    sellerRow = _sheet.GetRow(sellerRowCount);
                    if (sellerRow == null)
                    {
                        sellerRow = _sheet.CreateRow(sellerRowCount);
                    }
                    CreateCell("", sellerRow, colorColumn, cellStyleSeller);
                    colorColumn++;
                }
                colorColumn = 0;
                sellerRowCount++;
            }

            if (matchModel.schedulePeriodSetList.Count != 0)
            {
                sellerRowCount = matchModel.schedulePeriodSetList.Count +
                     bothmaximum.Count + buyermaximun.Length + 1;//時間+雙方+買家+自己
            }
            else
            {
                sellerRowCount = bothmaximum.Count + buyermaximun.Length + 1; //雙方+買家+自己
            }

            sellerRow = _sheet.GetRow(sellerRowCount);
            CreateCell("賣家有媒合意願", sellerRow, 0, cellStyleSeller);

            for (int i = 0; i < matchModel.matchSellerForbuyer_idList.Count; i++)
            {
                sellerRow = _sheet.GetRow(sellerRowCount);

                for (int j = 0; j < sellermaximun.Length; j++)
                {
                    try
                    {
                        sellerRow = _sheet.GetRow(sellerRowCount);
                        CreateCell(matchModel.matchSellerForbuyer_idList[i][j],
                                    sellerRow, column, cellStyleSeller);
                        sellerRowCount++;
                    }
                    catch
                    {

                    }
                }

                if (matchModel.schedulePeriodSetList.Count != 0)
                {
                    sellerRowCount = matchModel.schedulePeriodSetList.Count +
                                     bothmaximum.Count + buyermaximun.Length + 1;
                }
                else {
                    sellerRowCount = bothmaximum.Count + buyermaximun.Length + 1;
                }
                column++;
            }
            #endregion

            #region 直接利用Memory匯出Excel
            //var ms = new MemoryStream();
            //workbook.Write(ms);
            //Response.AddHeader("Content-Disposition", string.Format("attachment; filename=matchmaking.xls"));
            //Response.BinaryWrite(ms.ToArray());
            //ms.Close();
            //ms.Dispose();
            //Response.End();
            #endregion 
            exportExcelFileByMemorySpace(workbook, "matchmaking.xls");
            return null;
            //return exportExcelFile(workbook, "matchmaking.xls");
        }

        #endregion

        #region 讀取Excel樣板
        //refactoring by. Rong 2016/10/13
        private IWorkbook loadExcelTemplate(string path)
        {
            /*讀取樣板*/
            string excelPath = Server.MapPath(path);
            FileStream template = new FileStream(excelPath, FileMode.Open, FileAccess.Read);//樣板
            IWorkbook workbook = new HSSFWorkbook(template);//建立excel版本,放入指定樣板
            template.Close();
            return workbook;
        }
        #endregion

        #region 產生Excel檔
        //refactoring by. Rong 2016/10/13
        private FileResult exportExcelFile(IWorkbook workbook, string filename)
        {
            string savePath = @"D:/Download/" + filename;
            FileStream file = new FileStream(savePath, FileMode.Create);
            workbook.Write(file);
            file.Close();
            return File(savePath, "application/ms-excel", filename);
        }


        private void exportExcelFileByMemorySpace(IWorkbook workbook, string filename)
        {
            var ms = new MemoryStream();
            workbook.Write(ms);
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + filename));
            Response.BinaryWrite(ms.ToArray());
            ms.Close();
            ms.Dispose();
            Response.End();
        }
        #endregion

        #region 媒合大表匯出Excel
        [HttpGet]
        public ActionResult ExportExcelByNPOI_old()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("Login");

            ViewBag.Action = "StoreMatchData";

            int i, j;

            MakeSchedule();

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
            int datecount = matchModel.schedulePeriodSetList.Count + 2;
            int CurrRowMax = datecount; //起始列(跳過標題列)
            int tok = 0;

            CreateCell("時段\\買方名稱", MyRow, 0, CellStyle);

            foreach (BuyerInfoModel buyerInfoModel in matchModel.buyerinfoList)
            {
                CreateCell(buyerInfoModel.company, MyRow, CurrCol, CellStyle); //買家公司名稱
                try
                {
                    for (i = 0; i < matchModel.matchBothForbuyer_idList[CurrCol - 1].Count; i++)
                    {
                        if (CurrRowMax < datecount + i)
                            CurrRowMax = datecount + i;
                        IRow MyRow1 = _sheet.GetRow(datecount + i);
                        if (MyRow1 == null)
                            MyRow1 = _sheet.CreateRow(datecount + i);
                        if (i == 0 && tok == 0)
                        {
                            tok = 1;
                            CreateCell("雙方有媒合意願", MyRow1, 0, CellStyle); //
                        }
                        CreateCell(matchModel.matchBothForbuyer_idList[CurrCol - 1][i].Item3, MyRow1, CurrCol, CellStyle1); //買家公司名稱

                    }
                }
                catch
                {
                }
                CurrCol++;
            }

            CurrCol = 1;
            tok = 0;
            int k = 0;
            int CurrRowMax1 = 0;
            foreach (BuyerInfoModel buyerInfoModel in matchModel.buyerinfoList)
            {
                var buyerNeedAllArray = matchModel.matchmakingBuyerList.Select(buyer => buyer.buyer_id).ToArray();

                if (buyerNeedAllArray.Contains(buyerInfoModel.buyer_id))
                {
                    var bothallstr = matchModel.matchmakingBothList
                               .Where(both => both.buyer_id == buyerInfoModel.buyer_id)
                               .Select(both => both.company);


                    var buyerallstr = matchModel.matchmakingBuyerList
                                .Where(buyer => buyer.buyer_id == buyerInfoModel.buyer_id)
                                .Select(buyer => buyer.company);

                    var exceptStr = buyerallstr.Except(bothallstr);//取差集後的公司名稱

                    i = 0;
                    foreach (var str in exceptStr)
                    {
                        k = CurrRowMax + 2 + i;
                        IRow MyRow1 = _sheet.GetRow(k);
                        if (MyRow1 == null)
                            MyRow1 = _sheet.CreateRow(k);
                        if (i == 0 && tok == 0)
                        {
                            tok = 1;
                            CreateCell("買家有媒合意願", MyRow1, 0, CellStyle); //
                        }

                        CreateCell(str, MyRow1, CurrCol, CellStyle1); //買家公司名稱
                        i++;
                    }
                }
                if (CurrRowMax1 < k)
                    CurrRowMax1 = k;
                CurrCol++;
            }
            CurrRowMax = CurrRowMax1;

            CurrCol = 1;
            tok = 0;
            foreach (BuyerInfoModel buyerInfoModel in matchModel.buyerinfoList)
            {
                var sellerNeedAllArray = matchModel.matchmakingSellerList.Select(seller => seller.buyer_id).ToArray();
                if (sellerNeedAllArray.Contains(buyerInfoModel.buyer_id))
                {

                    var bothAllStr = matchModel.matchmakingBothList
                               .Where(both => both.buyer_id == buyerInfoModel.buyer_id)
                               .Select(both => both.company);

                    var sellerAllstr = matchModel.matchmakingSellerList
                                .Where(seller => seller.buyer_id == buyerInfoModel.buyer_id)
                                .Select(seller => seller.company);

                    var exceptStr = sellerAllstr.Except(bothAllStr);//取差集後的公司名稱

                    i = 0;
                    foreach (var str in exceptStr)
                    {
                        k = CurrRowMax + 2 + i;
                        IRow MyRow1 = _sheet.GetRow(k);
                        if (MyRow1 == null)
                            MyRow1 = _sheet.CreateRow(k);
                        if (i == 0 && tok == 0)
                        {
                            tok = 1;
                            CreateCell("賣家有媒合意願", MyRow1, 0, CellStyle); //
                        }

                        CreateCell(str, MyRow1, CurrCol, CellStyle1); //買家公司名稱
                        i++;
                    }
                }
                if (CurrRowMax1 < k)
                    CurrRowMax1 = k;
                CurrCol++;
            }
            CurrRowMax = CurrRowMax1;

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


        #region 活動照片管理
        [HttpPost]
        public ActionResult doPhotoInsertOrUpdate(ActivityPhotoModel model, HttpPostedFileBase photo_img)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("~/Manager/Login");

            model.manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
            if (model.photo_id == null)
            {

                if (photo_img != null && photo_img.ContentLength > 0)
                {
                    model.photo_pic_site = photo_img.FileName.Replace(".", "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".");
                    UploadHelper.doUploadFilePlus(photo_img, UploadConfig.subDirForActivity, model.manager_id, model.photo_pic_site);
                }
                int photo_id = (int)activityService.insertPhotoList(model);

            }
            else
            {
                if (photo_img != null && photo_img.ContentLength > 0 && !string.IsNullOrEmpty(model.manager_id))
                {
                    var old_photo_model = activityService.getPhotoOne(model.photo_id);
                    UploadHelper.deleteUploadFile(old_photo_model.photo_pic_site, "activity", model.manager_id);
                    model.photo_pic_site = photo_img.FileName.Replace(".", "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".");
                    UploadHelper.doUploadFilePlus(photo_img, UploadConfig.subDirForActivity, model.manager_id, model.photo_pic_site);
                }
            }


            int updateCount = (int)activityService.updatePhotoList(model);

            return Redirect("PhotoListEdit");
        }

        [HttpPost]
        public ActionResult PhotoDelete(int[] del_photos)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("~/Manager/Login");

            string manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
            bool isDelSuccess = activityService.PhotoListDeleteFake(del_photos); //假刪
            return Redirect("PhotoListEdit");

        }

        public ActionResult PhotoListEdit()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("~/Manager/Login");

            string manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
            IList<ActivityPhotoModel> photoLists = activityService.getAllPhoto(manager_id).Pages<ActivityPhotoModel>(Request, this, 10);
//            UserInfoModel userInfoModel = userService.GeUserInfoOne(user_id);
//            ViewBag.company = userInfoModel == null ? "" : userInfoModel.company;
            ViewBag.photoDir = UploadHelper.getPictureDirPath(manager_id, "activity");
//            docookie("_mainmenu", "ProductListEdit");
            return View(photoLists);
        }

        public ActionResult PhotoDetail(int? photo_id)
        {
            ActivityPhotoModel result = activityService.getPhotoOne(photo_id);
            ViewBag.photoDir = UploadHelper.getPictureDirPath(result.manager_id, "activity");
//            docookie("_mainmenu", "ProductDetail");
            return View(result);
        }

        public ActionResult PhotoDetailEdit(int? photo_id)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("~/Manager/Login");
            ActivityPhotoModel result = activityService.getPhotoOne(photo_id);
            ViewBag.photoDir = result != null ? UploadHelper.getPictureDirPath(result.manager_id, "activity") : "";

//            docookie("_mainmenu", "ProductDetailEdit");
            return result == null ? View(new ActivityPhotoModel() {photo_time= DateTime.Now}) : View(result);
        }
        #endregion

        #region Banner照片管理
        [HttpPost]
        public ActionResult doBannerInsertOrUpdate(BannerPhotoModel model, HttpPostedFileBase photo_img)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("~/Manager/Login");

            model.manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
            if (model.photo_id == null)
            {

                if (photo_img != null && photo_img.ContentLength > 0)
                {
                    model.photo_pic_site = photo_img.FileName.Replace(".", "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".");
                    UploadHelper.doUploadFilePlus(photo_img, UploadConfig.subDirForBanner, model.manager_id, model.photo_pic_site);
                }
                int photo_id = (int)activityService.insertBannerList(model);

            }
            else
            {
                if (photo_img != null && photo_img.ContentLength > 0 && !string.IsNullOrEmpty(model.manager_id))
                {
                    var old_photo_model = activityService.getBannerOne(model.photo_id);
                    UploadHelper.deleteUploadFile(old_photo_model.photo_pic_site, "banner", model.manager_id);
                    model.photo_pic_site = photo_img.FileName.Replace(".", "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".");
                    UploadHelper.doUploadFilePlus(photo_img, UploadConfig.subDirForBanner, model.manager_id, model.photo_pic_site);
                }
            }


            int updateCount = (int)activityService.updateBannerList(model);

            return Redirect("BannerListEdit");
        }

        [HttpPost]
        public ActionResult BannerViewDelete(int[] del_photos, int[] view_photos)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("~/Manager/Login");

            string manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
            if (del_photos!=null && del_photos.Count()>0)
            {
                bool isDelSuccess = activityService.BannerListDeleteFake(del_photos); //假刪
            }
            if (view_photos!=null && view_photos.Count() > 0)
            {
                bool isDelSuccess = activityService.BannerListUpdateActive(view_photos); 
            }
            return Redirect("BannerListEdit");

        }

        public ActionResult BannerListEdit()
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("~/Manager/Login");

            string manager_id = Request.Cookies["ManagerInfo"]["manager_id"];
            IList<BannerPhotoModel> photoLists = activityService.getAllBanner(manager_id).Pages<BannerPhotoModel>(Request, this, 10);
            //            UserInfoModel userInfoModel = userService.GeUserInfoOne(user_id);
            //            ViewBag.company = userInfoModel == null ? "" : userInfoModel.company;
            ViewBag.photoDir = UploadHelper.getPictureDirPath(manager_id, "banner");
            //            docookie("_mainmenu", "ProductListEdit");
            return View(photoLists);
        }

        public ActionResult BannerDetail(int? photo_id)
        {
            BannerPhotoModel result = activityService.getBannerOne(photo_id);
            ViewBag.photoDir = UploadHelper.getPictureDirPath(result.manager_id, "banner");
            //            docookie("_mainmenu", "ProductDetail");
            return View(result);
        }

        public ActionResult BannerDetailEdit(int? photo_id)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("~/Manager/Login");
            BannerPhotoModel result = activityService.getBannerOne(photo_id);
            ViewBag.photoDir = result != null ? UploadHelper.getPictureDirPath(result.manager_id, "banner") : "";

            //            docookie("_mainmenu", "ProductDetailEdit");
            return result == null ? View(new BannerPhotoModel{}) : View(result);
        }
        #endregion



        public ActionResult VideoListEdit()
        {

            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("~/Manager/Login");
            IList<VideoListModel> videoLists = userService.getVideoListAll().Pages(Request, this, 10);
            IList<ActiveVideoModel> activevideoLists = userService.SelectActiveVideo();
            ViewBag.active = "";
            foreach (ActiveVideoModel model in activevideoLists)
            {
                ViewBag.active = ViewBag.active + model.video_no.ToString() + ",";
            }
            ViewBag.active = "," + ViewBag.active;

            return View(videoLists);
        }

        [HttpPost]
        public ActionResult VideoActive(int video_no)
        {
            if (Request.Cookies["ManagerInfo"] == null)
                return Redirect("~/Manager/Login");
            userService.ActiveVideo(video_no);
            return Redirect("VideoListEdit");
        }


    }
}