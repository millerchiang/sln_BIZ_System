using prj_BIZ_System.App_Start;
using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prj_BIZ_System.Controllers
{
    enum MessageType
    {
        Person,
        CompanyPublic , CompanyPrivate,
        ClusterPublic , ClusterPrivate
    }

    public enum MessageCatalog
    {
        Private,
        Company,
        Cluster
    }

    public class MessageController : _BaseController
    {
        public MessageService messageService;
        public ClusterService clusterService;
        public Message_ViewModel messageViewModel;

        public MessageController()
        {
            messageService = new MessageService();
            clusterService = new ClusterService();
            messageViewModel = new Message_ViewModel();
        }

        #region --私人訊息--
        public ActionResult MessagePrivateList(string keyword)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            var user_id = Request.Cookies["UserInfo"]["user_id"];
            var totalMsg = messageService.SelectMsgPrivate(keyword, user_id);
            IList<MsgModel> result = totalMsg.Where( msg => "0".Equals(msg.is_read)).ToList().Pages(Request, this, 10); ;
            IList<MsgModel> result2 = totalMsg.Where(msg => "1".Equals(msg.is_read)).ToList().Pages(Request, this, 10); ;

            ViewBag.keyword = keyword;
            ViewBag.contentTitle    = getLabelString(MessageCatalog.Private, "contentTitle");
            ViewBag.searchUrl       = getLabelString(MessageCatalog.Private, "searchUrl");
            ViewBag.addUrl          = getLabelString(MessageCatalog.Private, "addUrl");
            ViewBag.detailUrl       = getLabelString(MessageCatalog.Private, "detailUrl");
            messageViewModel.msgPrivateList = result;
            messageViewModel.msgPrivateList2 = result2;
            return View(messageViewModel);
        }
        
        public ActionResult PrivateAdd()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");
            ViewBag.msgType      = getLabelString(MessageCatalog.Private, "msgType");
            ViewBag.contentTitle = getLabelString(MessageCatalog.Private, "contentTitle");
            ViewBag.backUrl      = getLabelString(MessageCatalog.Private, "backUrl");
            ViewBag.doAddUrl     = getLabelString(MessageCatalog.Private, "doAddUrl");
            return View();
        }

        public ActionResult DoPrivateAdd(MsgModel model , List<HttpPostedFileBase> iupexls)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            model.creater_id = Request.Cookies["UserInfo"]["user_id"];
            model.msg_no = (long)messageService.InsertMsgPrivate(model);

            #region 上傳訊息附件
            if (iupexls != null && model.msg_no != 0)
            {
                foreach( HttpPostedFileBase file in iupexls)
                {
                    if (file != null && file.ContentLength > 0 && !string.IsNullOrEmpty(model.creater_id))
                    {
                        Dictionary<string, string> uploadResult = null;
                        uploadResult = UploadHelper.doUploadFile(file, UploadConfig.subDirForMessageFile+model.msg_no, UploadConfig.AdminManagerDirName);
                        if ("success".Equals(uploadResult["result"]))
                        {
                            messageService.InsertMsgPrivateFile(model.msg_no,file.FileName);//uploadResult["relativFilepath"]
                        }
                        else
                        {
                            Console.WriteLine(LanguageResource.User.lb_upload_fail);
                        }
                    }
                }
            }
            #endregion

            return Redirect("MessagePrivateList");
        }

        
        public ActionResult PrivateDetailed(int msg_no)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            string current_user_id = Request.Cookies["UserInfo"]["user_id"];
            ViewBag.contentTitle = getLabelString(MessageCatalog.Private, "contentTitle");
            ViewBag.backUrl      = getLabelString(MessageCatalog.Private, "backUrl");

            if (msg_no != 0 )
            {
                if (isOwnViewPower(msg_no,MessageType.Person)) //檢查權限
                {
                    //messageViewModel.msgPrivate = messageService.SelectMsgPrivateOne(msg_no);
                    messageViewModel.msgPrivate = messageService.SelectMsgPrivateOneAndRead(msg_no , current_user_id);

                    ViewBag.msg_company = messageService.transferMsg_member2Msg_company(messageViewModel.msgPrivate.msg_member,MessageCatalog.Private);
                    messageViewModel.msgPrivateFileList = messageService.SelectMsgPrivateFileByMsg_no(msg_no);
                    messageViewModel.msgPrivateReplyList = messageService.SelectMsgPrivateReplyMsg_no(msg_no);
                    return View(messageViewModel);
                }
                else
                {
                    TempData["priDetailView_errmsg"] = LanguageResource.User.lb_msg_limit;
                    return Redirect(getLabelString(MessageCatalog.Private, "backUrl"));
                }
            }
            else
            {
                TempData["priDetailView_errmsg"] = LanguageResource.User.lb_click_correct;
                return Redirect(getLabelString(MessageCatalog.Private, "backUrl"));
            }
        }

        public ActionResult doPrivateDetailed(MsgReplyModel model)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");
            //int msg_no
            model.msg_reply = Request.Cookies["UserInfo"]["user_id"];
            messageService.InsertMsgPrivateReply(model);
            return Redirect(getLabelString(MessageCatalog.Private, "detailUrl")+"?msg_no=" +model.msg_no);
        }

        
        public ActionResult jsonMsgMemberForPrivate(string term)
        {
            var user_id = Request.Cookies["UserInfo"]["user_id"];
            IList<UserInfoModel> result = messageService.SelectUserKw(user_id , term);
            return Json(
                result.Select( userInfo => new { value = userInfo.user_id, label = userInfo.company }).ToList(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region --公司訊息--
        public ActionResult MessageCompanyList(string keyword)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            var user_id = Request.Cookies["UserInfo"]["user_id"];
            IList<MsgModel> result = messageService.SelectMsgCompany(keyword, user_id).Pages(Request, this, 10); ;
            ViewBag.keyword = keyword;
            ViewBag.contentTitle = getLabelString(MessageCatalog.Company, "contentTitle");
            ViewBag.searchUrl    = getLabelString(MessageCatalog.Company, "searchUrl");
            ViewBag.addUrl       = getLabelString(MessageCatalog.Company, "addUrl");
            ViewBag.detailUrl    = getLabelString(MessageCatalog.Company, "detailUrl");
            messageViewModel.msgPrivateList = result;
            return View("MessagePrivateList", messageViewModel);
        }

        public ActionResult MessageCompanyAdd()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            ViewBag.msgType      = getLabelString(MessageCatalog.Company, "msgType");
            ViewBag.contentTitle = getLabelString(MessageCatalog.Company, "contentTitle");
            ViewBag.backUrl      = getLabelString(MessageCatalog.Company, "backUrl");
            ViewBag.doAddUrl     = getLabelString(MessageCatalog.Company, "doAddUrl");

            return View(getLabelString(MessageCatalog.Private, "addUrl")); //共用
        }

        public ActionResult DoCompanyAdd(MsgModel model, List<HttpPostedFileBase> iupexls)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");
            model.user_id = Request.Cookies["UserInfo"]["user_id"]; //先寫死成登入者帳號,之後要記得用業務槷反查
            model.creater_id = Request.Cookies["UserInfo"]["user_id"];
            model.msg_no = (long)messageService.InsertMsgCompany(model); //這裡直接與私人訊息共用Service , 不是寫錯

            #region 上傳訊息附件
            if (iupexls != null && model.msg_no != 0)
            {
                foreach (HttpPostedFileBase file in iupexls)
                {
                    if (file != null && file.ContentLength > 0 && !string.IsNullOrEmpty(model.creater_id))
                    {
                        Dictionary<string, string> uploadResult = null;
                        uploadResult = UploadHelper.doUploadFile(file, UploadConfig.subDirForMessageFile + model.msg_no, UploadConfig.AdminManagerDirName);
                        if ("success".Equals(uploadResult["result"]))
                        {
                            messageService.InsertMsgPrivateFile(model.msg_no, file.FileName);//uploadResult["relativFilepath"]
                        }
                        else
                        {
                            Console.WriteLine(LanguageResource.User.lb_upload_fail);
                        }
                    }
                }
            }
            #endregion

            return Redirect(getLabelString(MessageCatalog.Company, "backUrl"));
        }

        public ActionResult MessageCompanyDetailed(int msg_no)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            ViewBag.contentTitle = getLabelString(MessageCatalog.Company, "contentTitle");
            ViewBag.backUrl      = getLabelString(MessageCatalog.Company, "backUrl");

            if (msg_no != 0)
            {
                if (isOwnViewPower(msg_no,MessageType.CompanyPublic)) //檢查權限
                {
                    messageViewModel.msgPrivate = messageService.SelectMsgPrivateOne(msg_no);

                    ViewBag.msg_company = messageService.transferMsg_member2Msg_company(messageViewModel.msgPrivate.msg_member,MessageCatalog.Company);
                    messageViewModel.msgPrivateFileList = messageService.SelectMsgPrivateFileByMsg_no(msg_no);
                    messageViewModel.msgPrivateReplyList = messageService.SelectMsgPrivateReplyMsg_no(msg_no);
                    return View(getLabelString(MessageCatalog.Private, "detailUrl"), messageViewModel); //共用
                }
                else
                {
                    TempData["priDetailView_errmsg"] = LanguageResource.User.lb_msg_limit;
                    return Redirect(getLabelString(MessageCatalog.Company, "backUrl"));
                }
            }
            else
            {
                TempData["priDetailView_errmsg"] = LanguageResource.User.lb_click_correct;
                return Redirect(getLabelString(MessageCatalog.Company, "backUrl"));
            }
        }

        public ActionResult jsonMsgMemberForCompany(string term)
        {
            if (Request.Cookies["SalesInfo"] == null && Request.Cookies["UserInfo"] == null) {
                return Json( new { } , JsonRequestBehavior.AllowGet);
            }
            var user_id = Request.Cookies["UserInfo"]["user_id"];
            var sales_id = Request.Cookies["SalesInfo"]["sales_id"];
            //var sales_id = Request.Cookies["UserInfo"]["user_id"];

            IList<SalesInfoModel> result = new List<SalesInfoModel>() ;
            result = messageService.SelectSalesKw(user_id, term);
            
            if(user_id == null && sales_id != null) //業務帳號登入
            {
                IList<UserInfoModel> users = messageService.SelectUserUserBySalesId(sales_id, term);
                ((List<SalesInfoModel>)result).AddRange(((List<UserInfoModel>)users).Select(user => new SalesInfoModel { sales_id = user.user_id , sales_name = user.company}).ToList());
            }

            return Json(
                result.Select(salesInfo => new { value = salesInfo.sales_id, label = salesInfo.sales_name }).ToList(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region --聚落訊息--

        public ActionResult MessageClusterMain(string cluster_public_where , string cluster_private_where , string is_public)
        {
            if (Request.Cookies["UserInfo"] == null || Request["cluster_no"] == null)
                return Redirect("~/Home/Index");
            string user_id = Request.Cookies["UserInfo"]["user_id"];
            IList<IList<MsgModel>> msgLists = new List<IList<MsgModel>>();
            ViewBag.cluster_public_where = cluster_public_where;
            ViewBag.cluster_private_where = cluster_private_where;
            var public_result = messageService.SelectMsgClusterPublic(Request["cluster_no"], user_id, cluster_public_where);
            var private_result = messageService.SelectMsgClusterPrivate(Request["cluster_no"], user_id, cluster_private_where);
            IList<MsgModel> result_public   = public_result.Where(msg => "0".Equals(msg.is_read)).ToList().Pages(Request,this,10);
            IList<MsgModel> result_public2  = public_result.Where(msg => "1".Equals(msg.is_read)).ToList().Pages(Request,this,10);
            IList<MsgModel> result_private  = private_result.Where(msg => "0".Equals(msg.is_read)).ToList().Pages(Request, this, 10);
            IList<MsgModel> result_private2 = private_result.Where(msg => "1".Equals(msg.is_read)).ToList().Pages(Request, this, 10);
            msgLists.Add(result_public);
            msgLists.Add(result_public2);
            msgLists.Add(result_private);
            msgLists.Add(result_private2);
            ViewBag.is_public = string.IsNullOrEmpty(is_public) ? "1" : is_public;
            ViewBag.ClusterInfo = clusterService.GetClusterInfo(int.Parse(Request["cluster_no"]),null,null);
            messageViewModel.msgLists = msgLists;
            docookie("_menu", "MessageClusterMain");
            return View(messageViewModel);
        }

        public ActionResult MessageClusterAdd(int is_public)
        {
            if (Request.Cookies["UserInfo"] == null || Request["cluster_no"] == null)
                return Redirect("~/Home/Index");
            string user_id = Request.Cookies["UserInfo"]["user_id"];

            IList<ClusterMemberModel> allMemberAtEnable1 = new List<ClusterMemberModel>();
            if (is_public == 0) {
                allMemberAtEnable1 = clusterService.GetClusterMemberListWithEnable1(int.Parse(Request["cluster_no"]));
            }
            ViewBag.AllMember = allMemberAtEnable1 ;
            ViewBag.ClusterInfo = clusterService.GetClusterInfo(int.Parse(Request["cluster_no"]), null, null);
            ViewBag.is_public = Convert.ToString(is_public);
            ViewBag.is_public_text = "0".Equals(ViewBag.is_public.ToString()) ? "私人" : "公告";
            return View();
        }

        public ActionResult DoClusterAdd(MsgModel model, List<HttpPostedFileBase> iupexls , string is_public)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            model.creater_id = Request.Cookies["UserInfo"]["user_id"];
            model.is_public = is_public ;
            model.cluster_no = int.Parse(Request["cluster_no"]);
            model.msg_member = model.msg_members == null ? "" :string.Join(", ", model.msg_members);
            model.msg_no = (long)messageService.InsertMsgCluster(model);

            #region 上傳訊息附件
            if (iupexls != null && model.msg_no != 0)
            {
                foreach (HttpPostedFileBase file in iupexls)
                {
                    if (file != null && file.ContentLength > 0 && !string.IsNullOrEmpty(model.creater_id))
                    {
                        Dictionary<string, string> uploadResult = null;
                        uploadResult = UploadHelper.doUploadFile(file, UploadConfig.subDirForMessageFile + model.msg_no, UploadConfig.AdminManagerDirName);
                        if ("success".Equals(uploadResult["result"]))
                        {
                            messageService.InsertMsgPrivateFile(model.msg_no, file.FileName);//uploadResult["relativFilepath"]
                        }
                        else
                        {
                            Console.WriteLine("上傳失敗");
                        }
                    }
                }
            }
            #endregion
            return Redirect("~/Message/MessageClusterMain" + "?is_public=" + is_public);
        }

        public ActionResult MessageClusterList(string keyword)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            var user_id = Request.Cookies["UserInfo"]["user_id"];
            IList<MsgModel> result = messageService.SelectMsgPrivate(keyword, user_id).Pages(Request, this, 10); ;
            ViewBag.keyword = keyword;
            messageViewModel.msgPrivateList = result;
            return View(messageViewModel);
        }
        
        [HttpGet]
        public ActionResult MessageClusterDetail(int msg_no , string is_public)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");
            ViewBag.is_public = is_public;
            var current_user_id = Request.Cookies["UserInfo"]["user_id"];
            var cluster_no = int.Parse(Request["cluster_no"]);
            ViewBag.cluster_info = clusterService.GetClusterInfo(cluster_no,null,null);
            if (msg_no != 0)
            {
                var type = "0".Equals(is_public) ? MessageType.ClusterPrivate : MessageType.ClusterPublic ;
                if (isOwnViewPower(msg_no, type, cluster_no)) //檢查權限
                {
                    messageViewModel.msgPrivate = messageService.SelectMsgPrivateOneAndRead(msg_no, current_user_id);
                    ViewBag.is_public = string.IsNullOrEmpty(is_public) ? "1" : is_public;
                    ViewBag.msg_company = messageService.transferMsg_member2Msg_company(messageViewModel.msgPrivate.msg_member, MessageCatalog.Private);
                    messageViewModel.msgPrivateFileList = messageService.SelectMsgPrivateFileByMsg_no(msg_no);
                    messageViewModel.msgPrivateReplyList = messageService.SelectMsgPrivateReplyMsg_no(msg_no);
                    ViewBag.clusterInfo = messageService.SelectClusterByMsg_no(msg_no);
                    return View(messageViewModel);
                }
                else
                {
                    TempData["clusterDetailView_errmsg"] = LanguageResource.User.lb_msg_limit;
                    return Redirect("/Message/MessageClusterMain?is_public="+ is_public);
                }
            }


                    return View(new Message_ViewModel());
        }

        [HttpPost]
        public ActionResult DoMessageClusterDetail(MsgReplyModel model , string is_public)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");
            ViewBag.is_public = is_public;

            model.msg_reply = Request.Cookies["UserInfo"]["user_id"];
            messageService.InsertMsgPrivateReply(model);
            return Redirect("~/Message/MessageClusterDetail" + "?is_public="+ is_public + "&msg_no=" + model.msg_no);
        }
        #endregion

        private bool isOwnViewPower(int msg_no, MessageType mtype , int cluster_no = 0)
        {
            var user_id = Request.Cookies["UserInfo"]["user_id"];
            switch (mtype)
            {
                case MessageType.Person:
                    return messageService.isOwnViewPower(msg_no, user_id);

                case MessageType.CompanyPublic:
                case MessageType.CompanyPrivate:
                    return messageService.isOwnViewPower(msg_no, user_id);

                case MessageType.ClusterPublic:
                    return messageService.isOwnViewPowerForClusterPublic(msg_no, cluster_no, user_id);
                case MessageType.ClusterPrivate:
                    return messageService.isOwnViewPowerForClusterPrivate(msg_no, cluster_no, user_id);

                default:
                    return false;
            }
        }

        /// <summary>
        /// ( catalog : Private, Company, Cluster )
        /// ( key :contentTitle／searchUrl／addUrl／detailUrl )
        /// </summary>
        private string getLabelString(MessageCatalog catalog,string key)
        {
            const string
                          msgType       = "msgType"
                        , contentTitle  = "contentTitle"
                        , searchUrl     = "searchUrl" 
                        , addUrl        = "addUrl" 
                        , doAddUrl      = "doAddUrl"
                        , detailUrl     = "detailUrl"
                        , backUrl       = "backUrl";

            Dictionary<string, string> resultDict = new Dictionary<string, string>();
            resultDict.Add(msgType, "");
            resultDict.Add(contentTitle, "");
            resultDict.Add(searchUrl, "");
            resultDict.Add(addUrl, "");
            resultDict.Add(doAddUrl, "");
            resultDict.Add(detailUrl, "");
            resultDict.Add(backUrl, "");

            switch (catalog)
            {
                case MessageCatalog.Private:
                    resultDict[msgType]         = "Private";
                    resultDict[contentTitle]    = "私人";
                    resultDict[searchUrl]       = "MessagePrivateList";
                    resultDict[addUrl]          = "PrivateAdd";
                    resultDict[doAddUrl]        = "DoPrivateAdd";
                    resultDict[detailUrl]       = "PrivateDetailed";
                    resultDict[backUrl]         = "MessagePrivateList";
                    break;

                case MessageCatalog.Company:
                    resultDict[msgType]         = "Company";
                    resultDict[contentTitle]    = "公司";
                    resultDict[searchUrl]       = "MessageCompanyList";
                    resultDict[addUrl]          = "MessageCompanyAdd";
                    resultDict[doAddUrl]        = "DoCompanyAdd";
                    resultDict[detailUrl]       = "MessageCompanyDetailed";
                    resultDict[backUrl]         = "MessageCompanyList";
                    break;
            }


            return resultDict.ContainsKey(key)? resultDict[key] : "";

        }
    }
}