using prj_BIZ_System.App_Start;
using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using prj_BIZ_System.Extensions;
//using prj_BIZ_System.WebService.Model;

namespace prj_BIZ_System.Controllers
{
    public class ClusterController : _BaseController
    {
        public ClusterService clusterService;
        public Cluster_ViewModel clusterViewModel;
        public SalesService salesService;

        public ClusterController()
        {
            clusterService = new ClusterService();
            clusterViewModel = new Cluster_ViewModel();
            salesService = new SalesService();
        }

        public string getuserid()
        {
            string user_id = "";
            if (Request.Cookies["UserInfo"]!=null && Request.Cookies["UserInfo"]["user_id"]!=null)
                user_id = Request.Cookies["UserInfo"]["user_id"];
            else
                user_id = Request.Cookies["SalesInfo"]["user_id"];
            return user_id;
        }

        public ActionResult ClusterList()
        {
            if (Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");
            string user_id = getuserid();

            if(Request.Cookies["SalesInfo"] != null && Request.Cookies["SalesInfo"]["sales_id"] != null)
                docookie("list", "6");
            else if (Request["list"] == null)
                docookie("list", "4");
            else
                docookie("list", Request["list"]);

            string _list = Request.Cookies["list"].Value;
            if (_list == "1")
            {
                //------可申請的聚落

                clusterViewModel.clusterWebServiceInfoList = clusterService.GetClusterListByApply(user_id).Pages(Request, this, 5);
                //------------------
            }
            else if (_list == "2")
            {
                //------申請中
                clusterViewModel.clusterWebServiceInfoList = clusterService.GetClusterListByIdAndClusterEnable(user_id, "4").Pages(Request, this, 5);
                //------------------
            }
            else if (_list == "3")
            {
                //------審核
                clusterViewModel.clusterWebServiceInfoList = clusterService.GetClusterListByChecked(user_id).Pages(Request, this, 5);
                //------------------
            }
            else if (_list == "4")
            {
                //------已加入
                clusterViewModel.clusterWebServiceInfoList = clusterService.GetClusterListByIdAndClusterEnable(user_id, "1").Pages(Request, this, 5);
                //------------------
            }
            else if (_list == "5")
            {
                //------受邀請中
                clusterViewModel.clusterWebServiceInfoList = clusterService.GetClusterListByIdAndClusterEnable(user_id, "2").Pages(Request, this, 5);
                //------------------
            }
            else if (_list == "6")
            {
                //------已加入已成立
                clusterViewModel.clusterWebServiceInfoList = clusterService.GetClusterListByIdAndClusterEnableForSales(user_id, "1").Pages(Request, this, 5);
                //------------------
            }
            docookie("_mainmenu", "ClusterList");
            return View(clusterViewModel);
        }

        public ActionResult ClusterListAll()
        {
            clusterViewModel.clusterWebServiceInfoList = clusterService.GetClusterListByApplyAll().Pages(Request, this, 5);
            docookie("_mainmenu", "ClusterListAll");
            return View(clusterViewModel);
        }

        public ActionResult ClusterInvited()
        {
            if (Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");
            string user_id = getuserid();

            clusterViewModel.clusterWebServiceInfoList = clusterService.GetClusterListByIdAndClusterEnable(user_id, "2").Pages(Request, this, 5);
            return View(clusterViewModel);
        }


        [HttpGet]
        public ActionResult CheckName(string user_id, string cluster_name)
        {
            clusterViewModel.clusterInfo = clusterService.GetClusterInfo(null,user_id, cluster_name);
            if (clusterViewModel.clusterInfo == null || clusterViewModel.clusterInfo.user_id == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckExist(string user_id, string cluster_no)
        {
            clusterViewModel.clusterMember = clusterService.GetClusterMember(int.Parse(Request["cluster_no"]), user_id);

            if (clusterViewModel.clusterMember == null || clusterViewModel.clusterMember.user_id == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Cluster_Delete(string user_id, string cluster_no)
        {
            if (Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");
            ClusterMemberModel clusterMemberModel = new ClusterMemberModel();
            clusterMemberModel.cluster_no = int.Parse(cluster_no);
            clusterMemberModel.user_id = user_id;
            clusterMemberModel.cluster_enable = "3";

            int kk= clusterService.ClusterMemberUpdateOne(clusterMemberModel);

            if (kk == 0 )
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Cluster_Status()
        {
            if (Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");
            ClusterMemberModel clusterMemberModel = new ClusterMemberModel();
            clusterMemberModel.cluster_no = int.Parse(Request["cluster_no"]);
            if (Request.QueryString["id"] == null || Request.QueryString["id"] == "")
                clusterMemberModel.user_id = getuserid();
            else
                clusterMemberModel.user_id = Request.QueryString["id"];

            clusterMemberModel.cluster_enable = Request["status"];

            int kk= clusterService.ClusterMemberUpdateOne(clusterMemberModel);

            if (kk == 0) clusterService.ClusterMemberInsertOne(clusterMemberModel);


            return Redirect("ClusterList");

        }

        public ActionResult Cluster_Members()
        {
            if (Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");
            string user_id = getuserid();
            clusterViewModel.clusterInfo = clusterService.GetClusterInfo(int.Parse(Request["cluster_no"]), null, null);
            clusterViewModel.clusterMemberList = clusterService.GetClusterMemberList(int.Parse(Request["cluster_no"]));
            ViewBag.PageType = "Edit";

            docookie("_menu", "Cluster_Members");

            return View(clusterViewModel);
        }

        public ActionResult DeleteFile(int cluster_file_no)
        {
            if (Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");
            clusterService.ClusterFileDeleteOne(cluster_file_no);
            return Redirect("Cluster_Files");
        }


        public ActionResult Cluster_Files()
        {
            if (Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");

            ClusterInfoModel clusterInfo = clusterService.GetClusterInfo(int.Parse(Request["cluster_no"]), null, null);
            double clusterMaxFileSize = clusterInfo.file_limit;
            double clusterFileSize = clusterService.GetClusterFileSize(int.Parse(Request["cluster_no"]));
            int capital = (int)(clusterFileSize / clusterMaxFileSize * 100);
            ViewBag.fs = capital;
            ViewBag.path = UploadConfig.UploadRootPath + UploadConfig.AdminManagerDirName + "/" + UploadConfig.subDirForCluster + Request["cluster_no"]+"/";
            clusterViewModel.clusterFileList = clusterService.GetClusterFileListkw(int.Parse(Request["cluster_no"])).Pages(Request, this, 10); 
            docookie("_menu", "Cluster_Files");
            string uploadsize = ((int)(clusterMaxFileSize - clusterFileSize) * 1024).ToString();
            docookie("_uploadsize", uploadsize);
            return View(clusterViewModel);
        }

        public ActionResult DownloadFile(string filepath,string filename)
        {
            if (Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");

            return File(filepath + filename, "application/", filename);
//            return File(filepath+filename, "application/pdf", filename);
    }

        public ActionResult Cluster_UploadFiles()
        {
            if (Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");
            docookie("_menu", "Cluster_Files");
            return View();
        }

        public ActionResult Cluster_Add()
        {
            if (Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");
            string user_id = getuserid();
            if (Request.QueryString["cluster_no"] != null)
            {
                clusterViewModel.clusterInfo = clusterService.GetClusterInfo(int.Parse(Request["cluster_no"]),null,null);
                clusterViewModel.clusterMemberList = clusterService.GetClusterMemberList(int.Parse(Request["cluster_no"]));
                ViewBag.PageType = "Edit";
            }
            else
            {
                clusterViewModel=new Cluster_ViewModel();
                clusterViewModel.clusterInfo = new ClusterInfoModel();
                ViewBag.PageType = "Create";
            }

            return View(clusterViewModel);
        }

        public ActionResult Cluster_Manager()
        {
            if (Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");
            string user_id = getuserid();
            if (Request.QueryString["cluster_no"] != null)
            {
                clusterViewModel.clusterInfo = clusterService.GetClusterInfo(int.Parse(Request["cluster_no"]), null, null);
                clusterViewModel.clusterMemberList = clusterService.GetClusterMemberList(int.Parse(Request["cluster_no"]));
            }

            return View(clusterViewModel);
        }


        public ActionResult EditCluster(string no, string members, ClusterInfoModel model)
        {
            if (Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");

            var user_id = "";
            if (Request.Cookies["UserInfo"] != null && Request.Cookies["UserInfo"]["user_id"] != null)
            {
                user_id = Request.Cookies["UserInfo"]["user_id"];
            }
            else {
                user_id = Request.Cookies["SalesInfo"]["user_id"];
            }


            int clusterNo = 0;
            int newcluster = 0;
            if (no != null && no != "")
            {
                clusterNo = int.Parse(no);
                model.cluster_no = clusterNo;
                if (user_id == model.user_id)
                    clusterService.ClusterInfoUpdateOne(model);
            }
            else
            {
                newcluster = 1;
                clusterNo = clusterService.ClusterInfoInsertOne(model);
            }

            if (members != null && members != "")
            {
                string members1 = members.Substring(1);
                string[] member = members1.Split(',');
                string creatorId = getuserid();
                for (int i = 0; i < member.Count(); i++)
                {
                    ClusterMemberModel membermodel = new ClusterMemberModel();
                    membermodel.user_id = member[i];
                    membermodel.cluster_no = clusterNo;

                    if (newcluster == 1)
                    {
                        membermodel.cluster_enable = creatorId == member[i] ? "1" : "2";
                        clusterService.ClusterMemberInsertOne(membermodel);
                    }
                    else
                    {
                        if (clusterService.GetClusterMember(membermodel.cluster_no, membermodel.user_id) == null)
                        {
                            membermodel.cluster_enable = creatorId == member[i] ? "1" : "2";
                            clusterService.ClusterMemberInsertOne(membermodel);
                        }
                        else
                        {
                            membermodel.cluster_enable = "2";
                            clusterService.ClusterMemberUpdateOne(membermodel);
                        }
                    }
                }
            }

            return Redirect("ClusterList");
        }



        public ActionResult EditClusterInfo(string no,ClusterInfoModel model)
        {
            if (Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");

            var user_id = "";
            if (Request.Cookies["UserInfo"] != null && Request.Cookies["UserInfo"]["user_id"] != null)
            {
                user_id = Request.Cookies["UserInfo"]["user_id"];
            }
            else {
                user_id = Request.Cookies["SalesInfo"]["user_id"];
            }


            int clusterNo = 0;
            int newcluster = 0;
            if (no != null && no != "")
            {
                clusterNo = int.Parse(no);
                model.cluster_no = clusterNo;
                if (user_id == model.user_id)
                    clusterService.ClusterInfoUpdateOne(model);
            }
            else
            {
                newcluster = 1;
                clusterNo = clusterService.ClusterInfoInsertOne(model);
            }

            return Redirect("Cluster_Detail");
        }

        public ActionResult EditClusterManager(string no, ClusterInfoModel model)
        {
            if (Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");

            int clusterNo = 0;
            if (no != null && no != "" && model.manager_id!=null)
            {
                clusterNo = int.Parse(no);
                model.cluster_no = clusterNo;
                clusterService.ClusterInfoUpdateManager(model);
            }
            return Redirect("ClusterList");
        }

        public ActionResult ClusterManagerOk()
        {
            if (Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");

            ClusterInfoModel model = new ClusterInfoModel();

            if (Request["id"] != null && Request["id"] != "" &&
                Request["cluster_no"] != null && Request["cluster_no"] != ""
                )
            {
                model.user_id = Request["id"];
                model.manager_id = "";
                model.cluster_no = int.Parse(Request["cluster_no"]);
                int updateok= clusterService.ClusterInfoUpdateManager(model);
                if (updateok > 0)
                    return Json(true, JsonRequestBehavior.AllowGet);
                else
                    return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(false, JsonRequestBehavior.AllowGet);
            //return Redirect("ClusterList");
        }


        [HttpPost]
        public ActionResult FilesUpload(HttpPostedFileBase upexl)
        {
            if (Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");
            TempData["errMsg"] = null;
            if (upexl != null)
            {
                if (upexl.ContentLength > 0)
                {
                    int filesize = int.Parse(Request.Cookies["_uploadsize"].Value);
                    if (upexl.ContentLength > filesize)
                    {
                        TempData["errMsg"] = "檔案容量超過限制!!";
                    }
                    else
                    {

                        string cluster_no = Request["cluster_no"];
                        Dictionary<string, string> uploadResult = null;
                        uploadResult = UploadHelper.doUploadFile(upexl, UploadConfig.subDirForCluster + cluster_no, UploadConfig.AdminManagerDirName);
                        if ("success".Equals(uploadResult["result"]))
                        {
                            ClusterFileModel filemodel = new ClusterFileModel();
                            filemodel.cluster_no = int.Parse(Request["cluster_no"]);
                            filemodel.user_id = getuserid();
                            filemodel.cluster_file_site = upexl.FileName;
                            filemodel.deleted = "1";
                            filemodel.file_size = (Double)upexl.ContentLength / 1024.0;
                            clusterService.ClusterFileInsertOne(filemodel);
                        }
                        else
                        {
                            Console.WriteLine(LanguageResource.User.lb_upload_fail);
                        }
                    }
                }
            }
            return Redirect("Cluster_Files");
        }


        public ActionResult EditClusterMember(string no, string members)
        {
            if (Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");

            int clusterNo = int.Parse(no); 

            if (members != null && members != "")
            {
                string members1 = members.Substring(1);
                string[] member = members1.Split(',');
                string creatorId = getuserid();
                for (int i = 0; i < member.Count(); i++)
                {
                    ClusterMemberModel membermodel = new ClusterMemberModel();
                    membermodel.user_id = member[i];
                    membermodel.cluster_no = clusterNo;
                    membermodel.cluster_enable = "2";
                    clusterService.ClusterMemberInsertOne(membermodel);
                }
            }
            return Redirect("Cluster_Members?cluster_no=" + clusterNo);
        }

        public ActionResult EditClusterLimit(string no, string set, string[] s2)
        {
            if (Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null)
                return Redirect("~/Home/Index");

            int clusterNo = int.Parse(no);

            string limit = "";
            if (set=="1")
            {
                limit = "all";
            }
            else if (set == "2")
            {
                if (s2 != null)
                {
                    for (int i = 0; i < s2.Count(); i++)
                    {
                        limit = s2[i] + "," + limit;
                    }
                    limit = "," + limit;
                }
            }
            ClusterMemberModel membermodel = new ClusterMemberModel();
            membermodel.user_id = getuserid();
            membermodel.cluster_no = clusterNo;
            membermodel.limit = limit;
            clusterService.ClusterLimitUpdateOne(membermodel);
            return Redirect("Cluster_Sales?cluster_no=" + clusterNo);
        }


        public ActionResult _ClusterMenuPartial()
        {
            return PartialView();
        }

        public ActionResult Cluster_Detail()
        {
            TempData["errMsg"] = null;
            if ((Request.Cookies["UserInfo"] == null && Request.Cookies["SalesInfo"] == null) || Request["cluster_no"] == null)
                return Redirect("~/Home/Index");
            string user_id = getuserid();

            if (Request.Cookies["UserInfo"] == null)
            {
                clusterViewModel.clusterMember = clusterService.GetClusterMember(int.Parse(Request["cluster_no"]), user_id);
                string limit = clusterViewModel.clusterMember.limit;
                if (limit == null || limit == "")
                {
                    TempData["errMsg"] = "你的業務權限無法進入該部落!!";
                    return Redirect("ClusterList");
                }
                string sales_id = "," + Request.Cookies["SalesInfo"]["sales_id"] + ",";
                if (limit != "all" && limit.IndexOf(sales_id) < 0)
                {
                    TempData["errMsg"] = "你的業務權限無法進入該部落!!";
                    return Redirect("ClusterList");
                }
            }

            docookie("_menu", "Cluster_Detail");

            clusterViewModel.clusterInfo = clusterService.GetClusterInfo(int.Parse(Request["cluster_no"]), null, null);
            //clusterViewModel.clusterMemberList = clusterService.GetClusterMemberList(int.Parse(Request["cluster_no"]));

            docookie("cluster_no", Request["cluster_no"]);
            docookie("cluster_name", HttpUtility.UrlEncode(clusterViewModel.clusterInfo.cluster_name));

            //            ViewBag.PageType = "Edit";
            return View(clusterViewModel);

        }

        public ActionResult Cluster_Sales()
        {
            if (Request.Cookies["UserInfo"] == null || Request["cluster_no"] == null)
                return Redirect("~/Home/Index");
            string user_id = getuserid();

            docookie("_menu", "Cluster_Sales");

            clusterViewModel.clusterInfo = clusterService.GetClusterInfo(int.Parse(Request["cluster_no"]), null, null);
            //clusterViewModel.clusterMemberList = clusterService.GetClusterMemberList(int.Parse(Request["cluster_no"]));
            clusterViewModel.clusterMember = clusterService.GetClusterMember(int.Parse(Request["cluster_no"]), user_id);
            clusterViewModel.salesList=salesService.SelectSalesInfos(user_id);
            string limit = clusterViewModel.clusterMember.limit;
            

            foreach (SalesInfoModel si in clusterViewModel.salesList)
            {
                if (limit == null)
                    si.limit = "1";
                else if (limit == "all")
                    si.limit = "2";
                else
                {
                    string s = "," + si.sales_id + ",";
                    if (limit.IndexOf(s) >= 0)
                        si.limit = "2";
                    else
                        si.limit = "1";
                }
            }

            docookie("cluster_no", Request["cluster_no"]);
            docookie("cluster_name", HttpUtility.UrlEncode(clusterViewModel.clusterInfo.cluster_name));

            return View(clusterViewModel);
        }

    }
}