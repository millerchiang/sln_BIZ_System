using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using prj_BIZ_System.WebService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiContrib.ModelBinders;

namespace prj_BIZ_System.WebService
{
    [MvcStyleBinding]
    public class ClusterController : ApiController
    {
        private ClusterService clusterService = new ClusterService();

        [HttpGet]
        public object GetClusterInfoList(string user_id)
        {
            if (user_id == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "user id is null.");

            var clusterInfoList = clusterService.GetClusterInfoListkw(user_id)
                .Select(clusterInfoModel =>
                    new Cluster
                    {
                        cluster_no = clusterInfoModel.cluster_no,
                        cluster_name = clusterInfoModel.cluster_name,
                        user_id = clusterInfoModel.user_id,
                        cluster_info = clusterInfoModel.cluster_info
                    }                              
                ).ToList();
            clusterInfoList.ForEach(cluster =>
                {
                    string[] clusterAry = clusterService.GetClusterMemberList(cluster.cluster_no)
                    .Select(clusterMember =>
                        clusterMember.company
                    ).ToArray();
                    string clusterMembers = string.Join(",", clusterAry);
                    cluster.cluster_members = clusterMembers;
                }
            );

            return Request.CreateResponse(HttpStatusCode.OK, clusterInfoList);
        }

        [HttpPost]
        public object ClusterInfoInsert(ClusterInfoModel model, string member)
        {
            if (model.user_id == null || model.cluster_name == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "user id or cluster name is null.");
            }

            string[] members = member.Split(',');
            int clusterNo = clusterService.ClusterInfoInsertOne(model);
            string creatorId = model.user_id;
            for (int i = 0; i < members.Count(); i++)
            {
                ClusterMemberModel membermodel = new ClusterMemberModel();
                membermodel.user_id = members[i];
                membermodel.cluster_no = clusterNo;
                membermodel.cluster_enable = creatorId == members[i] ? "1" : "0";
                clusterService.ClusterMemberInsertOne(membermodel);
            }
            return Request.CreateResponse(HttpStatusCode.OK, clusterNo);
        }
    }
}
