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
using prj_BIZ_System.Extensions;

namespace prj_BIZ_System.WebService
{
    [MvcStyleBinding]
    public class ClusterController : ApiController
    {
        private ClusterService clusterService = new ClusterService();

        [HttpGet]
        public object GetClusterInfoList(string user_id)
        {
            if (user_id.IsNullOrEmpty()) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "user id is null.");

            IList<ClusterInfo> clusterInfoList = clusterService.GetClusterListByIdAndClusterEnable(user_id, "1");

            IDictionary<string, Cluster[]> clusterDic = new Dictionary<string, Cluster[]>();
            clusterDic["enableCluster"] = getClusterByEnable(clusterInfoList, "1");
            clusterDic["nonEnableCluster"] = getClusterByEnable(clusterInfoList, "0");
            return Request.CreateResponse(HttpStatusCode.OK, clusterDic);
        }

        [HttpGet]
        public object GetClusterInfo(int cluster_no)
        {
            ClusterInfo clusterInfo = clusterService.GetClusterDetailByNo(cluster_no);
            if (clusterInfo == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "cluster no is error.");

            return Request.CreateResponse(HttpStatusCode.OK, clusterInfo);
        }

        [HttpPost]
        public object ModifyClusterInfo(ClusterInfo model)
        {
            if(model.cluster_name.IsNullOrEmpty()) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "cluster name is null.");

            ClusterInfoModel clusterInfoModel = new ClusterInfoModel { cluster_no = model.cluster_no,
                                                                       cluster_name = model.cluster_name,
                                                                       cluster_info = model.cluster_info };
            int updateRowCount = clusterService.ClusterInfoUpdateOne(clusterInfoModel);
            return Request.CreateResponse(HttpStatusCode.OK, updateRowCount);
        }

        private Cluster[] getClusterByEnable(IList<ClusterInfo> clusterInfoList, string enable)
        {
            return clusterInfoList
                            .Where(clusterInfoModel =>
                                clusterInfoModel.enable == enable
                            )
                            .Select(clusterInfo =>
                                new Cluster
                                {
                                    cluster_no = clusterInfo.cluster_no,
                                    cluster_name = clusterInfo.cluster_name,
                                    cluster_members = clusterInfo.cluster_members
                                }
                            ).ToArray();
        }

        [HttpGet]
        public object GetClusterMember(int cluster_no)
        {
            if (cluster_no == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "cluster no is null.");
            IList<ClusterMemberModel> memberList = clusterService.GetClusterMemberList(cluster_no);
            IDictionary<string, Dictionary<string, string>[]> members = new Dictionary<string, Dictionary<string, string>[]>();
            members["enableMember"] = memberList
                                .Where(clusterMember =>
                                    clusterMember.cluster_enable == "1"
                                ).Select(clusterMember =>
                                new Dictionary<string, string>() {
                                        {"user_id", clusterMember.user_id},
                                        {"company", clusterMember.company}
                                    }
                                ).ToArray();
            members["nonEnableMember"] = memberList
                                    .Where(clusterMember =>
                                        clusterMember.cluster_enable == "2"
                                    ).Select(clusterMember =>
                                        new Dictionary<string, string>() {
                                            {"user_id", clusterMember.user_id},
                                            {"company", clusterMember.company}
                                        }
                                    ).ToArray();
            return Request.CreateResponse(HttpStatusCode.OK, members);
        }

        [HttpGet]
        public object GetNotInClusterMember(int cluster_no)
        {
            if (cluster_no == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "cluster no is null.");
            var notInClusterMembers = clusterService.GetNotInClusterMember(cluster_no);

            return Request.CreateResponse(HttpStatusCode.OK, notInClusterMembers);
        }

        [HttpPost]
        public object ClusterMemberInvite(int cluster_no, string cluster_members)
        {
            if (cluster_no == null || cluster_members.IsNullOrEmpty())
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "cluster no or member is null.");
            }
            int insertSuccessCount = insertMember(cluster_no, "", cluster_members);
            return Request.CreateResponse(HttpStatusCode.OK, insertSuccessCount);
        }

        [HttpPost]
        public object ClusterInfoInsert(ClusterInfoModel model, string cluster_members)
        {
            model.enable = "0";

            if (model.user_id.IsNullOrEmpty() || model.cluster_name.IsNullOrEmpty())
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "user id or cluster name is null.");
            }

            int clusterNo = clusterService.ClusterInfoInsertOne(model);
            string creatorId = model.user_id;
            insertMember(clusterNo, creatorId, cluster_members);

            return Request.CreateResponse(HttpStatusCode.OK, clusterNo);
        }

        private int insertMember(int clusterNo, string creatorId, string member)
        {
            int insertSuccessCount = 0;
            string[] members = member.Split(',');
            for (int i = 0; i < members.Count(); i++)
            {
                ClusterMemberModel membermodel = new ClusterMemberModel();
                membermodel.user_id = members[i];
                membermodel.cluster_no = clusterNo;
                membermodel.cluster_enable = creatorId == members[i] ? "1" : "2";
                int clusterMemberNo = clusterService.ClusterMemberInsertOne(membermodel);
                insertSuccessCount = insertSuccessCount + (clusterMemberNo > 0 ? 1 : 0);
            }
            return insertSuccessCount;
        }

        [HttpPost]
        public object ClusterStatusChange(int cluster_no, string user_id, string status)
        {
            ClusterMemberModel clusterMemberModel = new ClusterMemberModel { cluster_no = cluster_no,
                                                                            user_id = user_id,
                                                                            cluster_enable = status };
            if (cluster_no == null || user_id.IsNullOrEmpty())
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "user id or cluster no is null.");
            }
            int updateRowCount = clusterService.ClusterMemberUpdateOne(clusterMemberModel);
            return Request.CreateResponse(HttpStatusCode.OK, updateRowCount);
        }

        [HttpGet]
        public object GetClusterInvite(string user_id)
        {
            if (user_id.IsNullOrEmpty()) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "user id is null.");
            IList<Dictionary<string, object>> clusterInviteList;
            clusterInviteList = clusterService.GetClusterListByIdAndClusterEnable(user_id, "2")
                                    .Select(clusterInfo =>
                                        new Dictionary<string, object>() {
                                            {"cluster_no", (int)clusterInfo.cluster_no},
                                            {"cluster_name", (string)clusterInfo.cluster_name},
                                            {"creator_name", (string)clusterInfo.creator_name},
                                            {"cluster_info", (string)clusterInfo.cluster_info},
                                            {"cluster_members", (string)clusterInfo.cluster_members},
                                            {"cluster_create_time", (string)clusterInfo.cluster_create_time.ToString("yyyy-MM-dd HH:mm")},
                                            {"member_invite_time", (string)clusterInfo.member_invite_time.ToString("yyyy-MM-dd HH:mm")},
                                            {"enable", (string)clusterInfo.enable}
                                        }
                                    ).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, clusterInviteList);
        }
    }
}
