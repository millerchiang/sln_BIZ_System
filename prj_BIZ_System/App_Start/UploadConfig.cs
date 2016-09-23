using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.App_Start
{
    public class UploadConfig
    {
        /// <summary>
        /// 管理者建立的檔案目錄
        /// </summary>
        public static string AdminManagerDirName = "_biz_admin";

        /// <summary>
        /// 上傳檔案根目錄實體檔位置
        /// </summary>
        public static string UploadRootDir { get; set; }

        /// <summary>
        /// 上傳檔案根目錄路徑
        /// </summary>
        public static string UploadRootPath { get; set; }

        /// <summary>
        /// 型錄封面圖目錄路徑
        /// </summary>
        public static string subDirForCover { get; set; }

        /// <summary>
        /// 型錄檔案目錄路徑
        /// </summary>
        public static string subDirForCatalog { get; set; }

        /// <summary>
        /// 聚落檔案目錄路徑
        /// </summary>
        public static string subDirForCluster { get; set; }

        /// <summary>
        /// Logo圖片儲存路徑
        /// </summary>
        public static string subDirForLogo { get; set; }

        /// <summary>
        /// Message附件路徑
        /// </summary>
        public static string subDirForMessageFile { get; set; }

        /// <summary>
        /// 新聞訊息圖片儲存路徑
        /// </summary>
        public static string subDirForNews { get; set; }

        /// <summary>
        /// 產品列表圖片儲存路徑
        /// </summary>
        public static string subDirForProduct { get; set; }

        public static void RegisterCustomSetting(string rootPath, string realRootDir)
        {
            UploadRootPath = "/" + rootPath;
            UploadRootDir = realRootDir;

            #region 型錄顯示 和 存檔位置
            subDirForCover = "Catalog/cover_file/";
            subDirForCatalog = "Catalog/catalog_file/";
            subDirForLogo = "Logo/";
            subDirForNews = "News/";
            subDirForMessageFile = "Pri_Message/";
            subDirForCluster = "Cluster/";
            subDirForProduct = "Product/";
            #endregion
        }
    }


    public class UploadHelper
    {
        public static string defaultImgSmall = "/images/logopic.jpg";
        public static string defaultImgBig = "/images/productpic.jpg";
        public static string[] sniff = { "jpg", "gif", "png", "pdf", "txt" };
        /// <summary>
        /// 取得私訊附件資料夾路徑(使用者id)
        /// </summary>
        public static string getMessageFileDirPath(long msg_no)
        {
            string folder_name = "";
            folder_name = getFolderName("primessage");
            return UploadConfig.UploadRootPath + UploadConfig.AdminManagerDirName + "/" + folder_name + msg_no + "/";
        }

        /// <summary>
        /// 取得圖片資料夾路徑(使用者id , 資料夾型態)
        /// </summary>
        public static string getPictureDirPath(string user_id , string dir_type)
        {
            string folder_name = "";
            folder_name = getFolderName(dir_type);
            return UploadConfig.UploadRootPath + user_id + "/" + folder_name;
        }

        /// <summary>
        /// 取得圖片資料夾實體位置(使用者id , 資料夾型態)
        /// </summary>
        public static string getPictureDirLocation(string user_id, string dir_type)
        {
            string folder_name = "";
            folder_name = getFolderName(dir_type);
            return UploadConfig.UploadRootDir + user_id + "/" + folder_name;
        }

        private static string getFolderName(string dir_type)
        {
            string folder_name = "";
            switch (dir_type)
            {
                case "catalog_cover":
                    folder_name = UploadConfig.subDirForCover;
                    break;

                case "catalog_file":
                    folder_name = UploadConfig.subDirForCatalog;
                    break;

                case "cluster_file":
                    folder_name = UploadConfig.subDirForCluster;
                    break;

                case "logo":
                    folder_name = UploadConfig.subDirForLogo;
                    break;

                case "news":
                    folder_name = UploadConfig.subDirForNews;
                    break;

                case "primessage":
                    folder_name = UploadConfig.subDirForMessageFile;
                    break;

                case "product":
                    folder_name = UploadConfig.subDirForProduct;
                    break;

                default : 
                    folder_name = dir_type;
                    break;
            }

            return folder_name;
        }


        /// <summary>
        /// 上傳檔案 (檔案 , 資料夾位置 , 使用者id)
        /// </summary>
        public static Dictionary<string, string> doUploadFile(HttpPostedFileBase uploadFile, string subFileDir, string user_id )
        {
            Dictionary<string, string> resultDict = new Dictionary<string, string>();
            
            string targetRootDir = Path.Combine(UploadConfig.UploadRootDir, user_id);
            string targetFilePath = "";
            targetFilePath = Path.Combine(targetRootDir, subFileDir);
            if (!Directory.Exists(UploadConfig.UploadRootDir))
            {
                Directory.CreateDirectory(UploadConfig.UploadRootDir);
            }
            if (!Directory.Exists(targetRootDir))
            {
                Directory.CreateDirectory(targetRootDir);
            }
            if (!Directory.Exists(targetFilePath))
            {
                Directory.CreateDirectory(targetFilePath);
            }
            string targetCoverFilePath = Path.Combine(targetFilePath, uploadFile.FileName);
            try
            {
                uploadFile.SaveAs(targetCoverFilePath);
                resultDict.Add("result","success");
                resultDict.Add("msg","");
            }
            catch (Exception ex)
            {
                resultDict.Add("result", "fail");
                resultDict.Add("msg", ex.ToString());
            }
            finally{
                resultDict.Add("filepath", targetCoverFilePath);
                resultDict.Add("relativFilepath", user_id +"/"+ subFileDir + "/"+ uploadFile.FileName);
            }
            return resultDict;
        }

        /// <summary>
        /// 上傳檔案 (檔案 , 資料夾位置 , 使用者id)
        /// </summary>
        public static Dictionary<string, string> doUploadFilePlus(HttpPostedFileBase uploadFile, string subFileDir, string user_id, string newFileName)
        {
            Dictionary<string, string> resultDict = new Dictionary<string, string>();

            string targetRootDir = Path.Combine(UploadConfig.UploadRootDir, user_id);
            string targetFilePath = "";
            targetFilePath = Path.Combine(targetRootDir, subFileDir);
            if (!Directory.Exists(UploadConfig.UploadRootDir))
            {
                Directory.CreateDirectory(UploadConfig.UploadRootDir);
            }
            if (!Directory.Exists(targetRootDir))
            {
                Directory.CreateDirectory(targetRootDir);
            }
            if (!Directory.Exists(targetFilePath))
            {
                Directory.CreateDirectory(targetFilePath);
            }
            string targetCoverFilePath = Path.Combine(targetFilePath, newFileName==null? uploadFile.FileName: newFileName);
            try
            {
                uploadFile.SaveAs(targetCoverFilePath);
                resultDict.Add("result", "success");
                resultDict.Add("msg", "");
            }
            catch (Exception ex)
            {
                resultDict.Add("result", "fail");
                resultDict.Add("msg", ex.ToString());
            }
            finally
            {
                resultDict.Add("filepath", targetCoverFilePath);
                resultDict.Add("relativFilepath", user_id + "/" + subFileDir + "/" + uploadFile.FileName);
            }
            return resultDict;
        }

        /// <summary>
        /// 刪除檔案 (檔案名稱 , 資料夾位置 , 使用者id) ps.資料夾位置字串請參考 getFolderName方內的switch字串
        /// </summary>
        public static void deleteUploadFile(string file_name, string dir_type, string user_id)
        {
            if (!string.IsNullOrEmpty(file_name))
            {
                string targetDirPath = getPictureDirLocation(user_id, dir_type);
                string file_path = Path.Combine(targetDirPath, file_name);
                if (File.Exists(file_name))
                {
                    File.Delete(file_name);
                }
            }
        }
    }
}