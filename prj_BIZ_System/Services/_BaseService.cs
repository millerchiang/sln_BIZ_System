using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Services
{
    public class _BaseService
    {
        public static ISqlMapper mapper;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public _BaseService()
        {
            DomSqlMapBuilder builder = new DomSqlMapBuilder();
            mapper = builder.Configure("DBSource/Config/SqlMap.config");
        }
    }
}