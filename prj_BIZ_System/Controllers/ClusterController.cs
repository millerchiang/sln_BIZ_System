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

            clusterViewModel.clusterList = clusterService.GetClusterList(user_id,"0","1").Pages(Request, this, 10);
            return View(clusterViewModel);
        }

        public ActionResult Cluster_Quit()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");
            ClusterMemberModel clusterMemberModel = new ClusterMemberModel();
            clusterMemberModel.cluster_no = int.Parse(Request["cluster_no"]);
            clusterMemberModel.user_id= Request.Cookies["UserInfo"]["user_id"];
            clusterMemberModel.deleted = "0";
            clusterMemberModel.cluster_enable = "2";
            clusterService.ClusterMemberQuitOne(clusterMemberModel);

            return Redirect("ClusterList");

        }

        public ActionResult Cluster_Reject()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");
            ClusterMemberModel clusterMemberModel = new ClusterMemberModel();
            clusterMemberModel.cluster_no = int.Parse(Request["cluster_no"]);
            clusterMemberModel.user_id = Request.Cookies["UserInfo"]["user_id"];
            clusterMemberModel.deleted = "1";
            clusterMemberModel.cluster_enable = "0";
            clusterService.ClusterMemberQuitOne(clusterMemberModel);
            return Redirect("ClusterList");
        }

        public ActionResult Cluster_Accept()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");
            ClusterMemberModel clusterMemberModel = new ClusterMemberModel();
            clusterMemberModel.cluster_no = int.Parse(Request["cluster_no"]);
            clusterMemberModel.user_id = Request.Cookies["UserInfo"]["user_id"];
            clusterMemberModel.deleted = "1";
            clusterMemberModel.cluster_enable = "1";
            clusterService.ClusterMemberQuitOne(clusterMemberModel);
            return Redirect("ClusterList");
        }


        public ActionResult Cluster_Add()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");
            string user_id = Request.Cookies["UserInfo"]["user_id"];
            if (Request["cluster_no"] != null)
            {
                clusterViewModel.clusterInfo = clusterService.GetClusterInfo(int.Parse(Request["cluster_no"]));
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
            if (no != null && no != "")
            {
                clusterNo = int.Parse(no);
                model.cluster_no = clusterNo;
                clusterService.ClusterInfoUpdateOne(model);
            }
            else
            {
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
                    membermodel.cluster_enable = creatorId == member[i] ? "1" : "0";
                    clusterService.ClusterMemberInsertOne(membermodel);
                }
            }

            return Redirect("ClusterList");
        }


    }
}