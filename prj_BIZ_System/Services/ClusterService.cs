using prj_BIZ_System.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Services
{
    public class ClusterService : _BaseService
    {

        public IList<ClusterModel> GetClusterList(string user_id,string cluster_enable)
        {
            ClusterModel tempModel = new ClusterModel { user_id = user_id, cluster_enable= cluster_enable };
            return mapper.QueryForList<ClusterModel>("Cluster.SelectClusterList", tempModel);
        }

        public IList<ClusterModel> GetClusterAll()
        {
            return mapper.QueryForList<ClusterModel>("Cluster.SelectClusterAll",null);
        }

        public IList<ClusterDetailModel> GetClusterListByApply(string user_id)
        {
            ClusterModel tempModel = new ClusterModel { user_id = user_id };
            return mapper.QueryForList<ClusterDetailModel>("Cluster.ClusterListByApply", tempModel);
        }

        public IList<ClusterDetailModel> GetClusterListByChecked(string user_id)
        {
            ClusterModel tempModel = new ClusterModel { user_id = user_id };
            return mapper.QueryForList<ClusterDetailModel>("Cluster.ClusterListByChecked", tempModel);
        }

        public IList<ClusterDetailModel> GetClusterListByApplyAll()
        {
            return mapper.QueryForList<ClusterDetailModel>("Cluster.ClusterListByApplyAll", null);
        }


        public IList<ClusterDetailModel> GetClusterListByIdAndClusterEnable(string user_id, string cluster_enable)
        {
            ClusterModel tempModel = new ClusterModel { user_id = user_id, cluster_enable = cluster_enable };
            return mapper.QueryForList<ClusterDetailModel>("Cluster.ClusterListByIdAndClusterEnable", tempModel);
        }

        public IList<ClusterDetailModel> GetClusterListByIdAndClusterEnableAll(string user_id, string cluster_enable, string is_public)
        {
            ClusterModel tempModel = new ClusterModel { user_id = user_id, cluster_enable = cluster_enable, is_public = is_public };
            return mapper.QueryForList<ClusterDetailModel>("Cluster.ClusterListByIdAndClusterEnableAll", tempModel);
        }

        public IList<ClusterDetailModel> GetClusterListByIdAndClusterEnableForSales(string user_id, string cluster_enable)
        {
            ClusterModel tempModel = new ClusterModel { user_id = user_id, cluster_enable = cluster_enable };
            return mapper.QueryForList<ClusterDetailModel>("Cluster.ClusterListByIdAndClusterEnableForSales", tempModel);
        }

        public ClusterDetailModel GetClusterDetailByNo(int cluster_no)
        {
            ClusterModel tempModel = new ClusterModel { cluster_no = cluster_no };
            return mapper.QueryForObject<ClusterDetailModel>("Cluster.ClusterDetail", tempModel);
        }

        public IList<Hashtable> GetNotInClusterMember(int cluster_no)
        {
            return mapper.QueryForList<Hashtable>("Cluster.SelectNotInClusterMember", cluster_no);
        }

        public ClusterInfoModel GetClusterInfo(int? cluster_no,string user_id,string cluster_name)
        {
            ClusterInfoModel tempModel = new ClusterInfoModel { cluster_no = cluster_no, user_id=user_id, cluster_name= cluster_name };
            return mapper.QueryForObject<ClusterInfoModel>("Cluster.SelectClusterInfo", tempModel);
        }

        public IList<ClusterInfoModel> GetClusterInfoListkw(string company, string cluster_name)
        {
            ClusterInfoModel tempModel = new ClusterInfoModel { company = company, cluster_name = cluster_name };
            return mapper.QueryForList<ClusterInfoModel>("Cluster.SelectClusterInfoList", tempModel);
        }

        public IList<ClusterFileModel> GetClusterFileListkw(int cluster_no)
        {
            ClusterFileModel tempModel = new ClusterFileModel { cluster_no = cluster_no };
            return mapper.QueryForList<ClusterFileModel>("Cluster.SelectClusterFileList", tempModel);
        }

        public double GetClusterFileSize(int cluster_no)
        {
            ClusterFileModel tempModel = new ClusterFileModel { cluster_no = cluster_no };
            double? size = 0;
            size = mapper.QueryForObject<double?>("Cluster.SelectClusterFileSize", tempModel);
            if (size == null)
                size = 0;

            return (double)size;
        }


        public IList<ClusterMemberModel> GetClusterMemberList(int? cluster_no)
        {
            ClusterMemberModel tempModel = new ClusterMemberModel { cluster_no = cluster_no };
            return mapper.QueryForList<ClusterMemberModel>("Cluster.SelectClusterMemberList", tempModel);
        }

        public IList<ClusterMemberModel> GetClusterMemberListWithEnable1(int? cluster_no)
        {
            ClusterMemberModel tempModel = new ClusterMemberModel { cluster_no = cluster_no };
            return mapper.QueryForList<ClusterMemberModel>("Cluster.SelectClusterMemberListWithEnable1", tempModel);
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
        public int ClusterInfoUpdateOne(ClusterInfoModel clusterInfoModel)
        {
            var result = mapper.Update("Cluster.ClusterInfoUpdateOne", clusterInfoModel);
            return (int)result;
        }

        public int ClusterInfoUpdateManager(ClusterInfoModel clusterInfoModel)
        {
            var result = mapper.Update("Cluster.ClusterInfoUpdateManager", clusterInfoModel);
            return (int)result;
        }

        public int ClusterInfoUpdateSize(ClusterInfoModel clusterInfoModel)
        {
            var result = mapper.Update("Cluster.ClusterInfoUpdateSize", clusterInfoModel);
            return (int)result;
        }


        public int ClusterMemberInsertOne(ClusterMemberModel clusterMemberModel)
        {
            var result = mapper.Insert("Cluster.ClusterMemberInsertOne", clusterMemberModel);
            return (int)result;
        }
        public int ClusterMemberUpdateOne(ClusterMemberModel clusterMemberModel)
        {
            int result = mapper.Update("Cluster.ClusterMemberUpdateOne", clusterMemberModel);
            return result;
        }

        public int ClusterLimitUpdateOne(ClusterMemberModel clusterMemberModel)
        {
            int result = mapper.Update("Cluster.ClusterLimitUpdateOne", clusterMemberModel);
            return result;
        }


        public int ClusterFileInsertOne(ClusterFileModel clusterFileModel)
        {
            var result = mapper.Insert("Cluster.ClusterFileInsertOne", clusterFileModel);
            return (int)result;
        }

        public int ClusterFileDeleteOne(int cluster_file_no)//假刪
        {
            int result = mapper.Update("Cluster.ClusterFileDeleteOne", cluster_file_no);
            return result;
        }

    }
}