using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Services
{
    public class BaseService
    {
        public static ISqlMapper mapper;
        public BaseService()
        {
            DomSqlMapBuilder builder = new DomSqlMapBuilder();
            mapper = builder.Configure("DBSource/Config/SqlMap.config");
        }
    }
}