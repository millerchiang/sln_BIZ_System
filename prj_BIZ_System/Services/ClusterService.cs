using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Services
{
    public class ClusterService : _BaseService
    {

        public IList<ClusterModel> GetClusterList(string user_id,string cluster_enable,string deleted)
        {
            ClusterModel tempModel = new ClusterModel { user_id = user_id, cluster_enable= cluster_enable,deleted = deleted };
            return mapper.QueryForList<ClusterModel>("Cluster.SelectClusterList", tempModel);
        }

        public ClusterInfoModel GetClusterInfo(int? cluster_no)
        {
            ClusterInfoModel tempModel = new ClusterInfoModel { cluster_no = cluster_no };
            return mapper.QueryForObject<ClusterInfoModel>("Cluster.SelectClusterInfo", tempModel);
        }

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
        public object ClusterMemberQuitOne(ClusterMemberModel clusterMemberModel)
        {
            var result = mapper.Update("Cluster.ClusterMemberQuitOne", clusterMemberModel);
            return result;
        }

    }
}