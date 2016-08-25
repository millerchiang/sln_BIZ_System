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

            clusterViewModel.clusterInfoList = clusterService.GetClusterInfoListkw(user_id).Pages(Request, this, 10);
            return View(clusterViewModel);
        }

        public ActionResult Cluster_Add()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");
            string user_id = Request.Cookies["UserInfo"]["user_id"];
            if (Request["cluster_no"] != null)
            {
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

        public ActionResult EditCluster(string members, ClusterInfoModel model)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            if (members != null && members != "")
            {
                string members1 = members.Substring(1);
                string[] member = members1.Split(',');
                int clusterNo = clusterService.ClusterInfoInsertOne(model);
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