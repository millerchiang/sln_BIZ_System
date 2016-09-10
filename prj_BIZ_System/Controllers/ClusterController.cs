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

        public ClusterController()
        {
            clusterService = new ClusterService();
            clusterViewModel = new Cluster_ViewModel();
        }

        public ActionResult ClusterList()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");
            string user_id = Request.Cookies["UserInfo"]["user_id"];

            //            clusterViewModel.clusterList = clusterService.GetClusterList(user_id,"1").Pages(Request, this, 10);
            //            return View(clusterViewModel);
            clusterViewModel.clusterWebServiceInfoList = clusterService.GetClusterListByIdAndClusterEnable(user_id, "1").Pages(Request, this, 3);
            return View(clusterViewModel);

        }

        public ActionResult ClusterInvited()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");
            string user_id = Request.Cookies["UserInfo"]["user_id"];

            clusterViewModel.clusterWebServiceInfoList = clusterService.GetClusterListByIdAndClusterEnable(user_id, "2").Pages(Request, this, 3);
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
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");
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
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");
            ClusterMemberModel clusterMemberModel = new ClusterMemberModel();
            clusterMemberModel.cluster_no = int.Parse(Request["cluster_no"]);
            clusterMemberModel.user_id= Request.Cookies["UserInfo"]["user_id"];
            clusterMemberModel.cluster_enable = Request["status"];
            clusterService.ClusterMemberUpdateOne(clusterMemberModel);

            return Redirect("ClusterList");

        }

        public void docookie(string cks,string v)
        {
            HttpCookie cookie = Request.Cookies[cks];

            if (cookie != null)
            {
                // update cookie value 
                cookie.Value = v;
            }
            else
            {
                // create cookie value 
                cookie = new HttpCookie(cks);
                cookie.Value = v;
                //                cookie.Expires = DateTime.Now.AddYears(1);
            }

            Response.Cookies.Add(cookie);
        }

        public ActionResult Cluster_Members()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");
            string user_id = Request.Cookies["UserInfo"]["user_id"];
            clusterViewModel.clusterInfo = clusterService.GetClusterInfo(int.Parse(Request["cluster_no"]), null, null);
            clusterViewModel.clusterMemberList = clusterService.GetClusterMemberList(int.Parse(Request["cluster_no"]));
            ViewBag.PageType = "Edit";

            docookie("_menu", "Cluster_Members");

            return View(clusterViewModel);
        }


        public ActionResult Cluster_Add()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");
            string user_id = Request.Cookies["UserInfo"]["user_id"];
            if (Request["cluster_no"] != null)
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

        public ActionResult EditCluster(string no,string members, ClusterInfoModel model)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            int clusterNo = 0;
            int newcluster = 0;
            if (no != null && no != "")
            {
                clusterNo = int.Parse(no);
                model.cluster_no = clusterNo;
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
                string creatorId = Request.Cookies["UserInfo"]["user_id"];
                for (int i=0;i<member.Count();i++)
                {
                    ClusterMemberModel membermodel= new ClusterMemberModel();
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

        public ActionResult EditClusterMember(string no, string members)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            int clusterNo = int.Parse(no); 

            if (members != null && members != "")
            {
                string members1 = members.Substring(1);
                string[] member = members1.Split(',');
                string creatorId = Request.Cookies["UserInfo"]["user_id"];
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

        public ActionResult _ClusterMenuPartial()
        {
            return PartialView();
        }

        public ActionResult Cluster_Detail()
        {
            if (Request.Cookies["UserInfo"] == null || Request["cluster_no"]==null)
                return Redirect("~/Home/Login");
            string user_id = Request.Cookies["UserInfo"]["user_id"];

            docookie("_menu", "Cluster_Detail");

            clusterViewModel.clusterInfo = clusterService.GetClusterInfo(int.Parse(Request["cluster_no"]), null, null);
            clusterViewModel.clusterMemberList = clusterService.GetClusterMemberList(int.Parse(Request["cluster_no"]));
//            ViewBag.PageType = "Edit";
            return View(clusterViewModel);

        }

    }
}