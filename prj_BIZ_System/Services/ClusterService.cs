using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Services
{
    public class ClusterService : _BaseService
    {

        public IList<ClusterInfoModel> GetClusterInfoListkw(string user_id)
        {
            ClusterInfoModel tempModel = new ClusterInfoModel { user_id = user_id };
            return mapper.QueryForList<ClusterInfoModel>("Cluster.SelectClusterInfoList", tempModel);
        }

        public IList<ClusterMemberModel> GetClusterMemberList(int? cluster_no)
        {
            ClusterMemberModel tempModel = new ClusterMemberModel { cluster_no = cluster_no };
            return mapper.QueryForList<ClusterMemberModel>("Cluster.SelectClusterMemberList", tempModel);
        }

        public ClusterMemberModel GetClusterMember(int? cluster_no, string user_id)
        {
            ClusterMemberModel tempModel = new ClusterMemberModel { cluster_no = cluster_no,user_id= user_id };
            return mapper.QueryForObject<ClusterMemberModel>("Cluster.SelectClusterMember", tempModel);
        }

        public int ClusterInfoInsertOne(ClusterInfoModel clusterInfoModel)
        {
            var result = mapper.Insert("Cluster.ClusterInfoInsertOne", clusterInfoModel);
            return (int)result;
        }
        public object ClusterInfoUpdateOne(ClusterInfoModel clusterInfoModel)
        {
            var result = mapper.Update("Cluster.ClusterInfoUpdateOne", clusterInfoModel);
            return result;
        }

        public object ClusterMemberInsertOne(ClusterMemberModel clusterMemberModel)
        {
            var result = mapper.Insert("Cluster.ClusterMemberInsertOne", clusterMemberModel);
            return result;
        }
        public object ClusterMemberUpdateOne(ClusterMemberModel clusterMemberModel)
        {
            var result = mapper.Update("Cluster.ClusterMemberUpdateOne", clusterMemberModel);
            return result;
        }

    }
}