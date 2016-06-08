using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.App_Start
{
    public class CacheConfig
    {
        private static string propDir = "Properties/";

        /// <summary>
        /// 是否啟動側邊欄產業別查詢顯示快取
        /// </summary>
        public static bool _NavSearchPartial_load_cache_isOn;
        public static void RegisterCustomSetting(string baseDir)
        {
            Properties cache_props = new Properties(baseDir + propDir + "cache.prop");
            _NavSearchPartial_load_cache_isOn = "on".Equals(cache_props.getProperty("_NavSearchPartial_load_cache_switch") , StringComparison.OrdinalIgnoreCase);
        }
    }

    public class CacheDataStore
    {

        /// <summary>
        /// 側邊欄產業別查詢顯示
        /// </summary>
        public static IList<EnterpriseSortListModel> EnterpriseSortListModelCache;
    }


    public class Properties
    {
        private Dictionary<string , string> props = new Dictionary<string, string>();

        public Properties() { }
        public Properties(string filepath)
        {
            load(filepath);
        }

        public void load(string filepath)
        {
            if (!string.IsNullOrEmpty(filepath))
            {
                if (File.Exists(filepath))
                {
                    props = new Dictionary<string, string>();
                    foreach (string row_str in File.ReadAllLines(filepath))
                    {
                        if (!string.IsNullOrEmpty(row_str) && !string.IsNullOrEmpty(row_str.Trim()) && !row_str.Trim().StartsWith("#"))
                        {
                            string[] row_ary = row_str.Split('=');
                            if (row_ary != null)
                            {
                                var value = "";
                                if (row_ary.Length > 1)
                                {
                                    value = row_ary[1].Trim();
                                }
                                if (!string.IsNullOrEmpty(row_ary[0]))
                                {
                                    props.Add(row_ary[0].Trim(), value);
                                }
                            }
                        }
                    }
                }
            }
        }

        public string getProperty(string key)
        {
            return props[key];
        }

        public string setProperty(string key , string value)
        {
            return props[key]=value;
        }

        public void clear()
        {
            props.Clear();
        }
    }
}