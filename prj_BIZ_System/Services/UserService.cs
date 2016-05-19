using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using System.Collections;

namespace prj_BIZ_System.Services
{
    public class UserService : BaseService
    {

        /* insert方法*/
        public void UserInfoInsertOne(UserInfoModel userInfoModel)
        {
            mapper.Insert("UserInfo.InsertOne", userInfoModel);
        }

        /* select方法*/
        public IList<EnterpriseSortModel> GetSortList()
        {
            return mapper.QueryForList<EnterpriseSortModel>("UserInfo.SelectAll_sort", null);
        }


        public IList<UserInfoModel> GetUserInfoList()
        {
            return mapper.QueryForList<UserInfoModel>("UserInfo.SelectAll", null);
        }

        public UserInfoModel GeUserInfoOne(string user_id)
        {
            return (UserInfoModel)mapper.QueryForObject("UserInfo.SelectOne", user_id);
        }

        /* delect方法*/
        public int UserInfoDelectOne(string user_id)
        {
            Object obj = mapper.Delete("UserInfo.DeleteOne", user_id);
            return (int)obj;
        }

        /*update方法*/
        public int UserInfoUpdateOne(UserInfoModel userInfoModel)
        {
            Object obj = mapper.Update("UserInfo.UpdateOne", userInfoModel);
            return (int)obj;
        }

        /* 使用者 - 產業對應表 */
        public IList<UserSortModel> SelectUserSortByUserId(string user_id)
        {
            UserSortModel param = new UserSortModel() { user_id = user_id };
            return mapper.QueryForList<UserSortModel>("UserInfo.SelectUserSortByUserId", param);
        }

        public bool RefreshUserSort(string user_id , int[] sort_ids)
        {
            UserSortModel param = new UserSortModel() { user_id = user_id };
            int deleteCount = mapper.Delete("UserInfo.DeleteUserSortByUserId", param);
            UserSortModel tempModel;
            if( sort_ids != null)
            {
                foreach ( int sort_id in sort_ids)
                {
                    tempModel = new UserSortModel(){ user_id = user_id, sort_id = sort_id };
                    mapper.Insert("UserInfo.InsertUserSortByUserId", tempModel);
                }
            }
            return true;
        }

        public IList<ProductListModel> getAllProduct(string user_id)
        {
            ProductListModel param = new ProductListModel() { user_id  = user_id };
            return mapper.QueryForList<ProductListModel>("UserInfo.SelectProductListByUserId", param);
        }

        public IList<CatalogListModel> getAllCatalog(string user_id)
        {
            CatalogListModel param = new CatalogListModel() { user_id = user_id };
            return mapper.QueryForList<CatalogListModel>("UserInfo.SelectCatalogListByUserId", param);
        }

        public bool CatalogListInsert(string user_id , string conver_fileName , string catalog_fileName)
        {
            CatalogListModel param = new CatalogListModel()
            {
                user_id = user_id,
                cover_file = conver_fileName,
                catalog_file = catalog_fileName,
                deleted = "0"
            };
            var obj = mapper.Insert("UserInfo.InsertCatalogList", param);
            return true;
        }

        public IList<CatalogListModel> SelectCatalogListByCatalogNo(string user_id , int[] catalog_no)
        {
            if(catalog_no != null)
            {
                ArrayList catalogNoList = new ArrayList(catalog_no);
                Hashtable map = new Hashtable();
                map.Add("user_id", user_id);
                map.Add("list", catalogNoList);
                return mapper.QueryForList<CatalogListModel>("UserInfo.SelectCatalogListByCatalogNo", map);
            }
            else
            {
                return null;
            }
        }

        public bool CatalogListDelete(string user_id , int[] catalog_no)
        {
            if (catalog_no != null)
            {
                ArrayList param = new ArrayList(catalog_no);
                int delCount = mapper.Delete("UserInfo.DeleteCatalogLists", param);
                return true;
                /*
                foreach (int no in catalog_no)
                {
                    var param = new CatalogListModel() { user_id = user_id, catalog_no = no };
                }
                */

            }

            return true;
        }
    }
}