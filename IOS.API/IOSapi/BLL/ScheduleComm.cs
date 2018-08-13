using IOSapi.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace IOSapi.BLL
{
    public class ScheduleComm
    {

        APSEntities db = new APSEntities();

        /// <summary>
        /// 生成表记录id
        /// </summary>
        /// <param name="tbName">表名</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public int CreateTbID(string tbName, int count)
        {
            int id = 1;
            List<SqlParameter> paramArray = new List<SqlParameter>();
            paramArray.Add(new SqlParameter("@tablename", tbName));
            SqlParameter param = new SqlParameter("@id", System.Data.SqlDbType.Int);
            param.Direction = System.Data.ParameterDirection.Output;
            paramArray.Add(new SqlParameter("@num", count));
            paramArray.Add(param);

            try
            {
                this.db.Database.ExecuteSqlCommand("exec gen_create_db_newid @tablename,@id out,@num", paramArray.ToArray());
            }
            catch (Exception ex)
            {
                throw;
            }

            int result = (int)paramArray[2].Value;

            return result;

        }
    }
}