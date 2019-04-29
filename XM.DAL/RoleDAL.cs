﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XM.DAL.comm;
using XM.IDAL;
using XM.Model;

namespace XM.DAL
{
    public class RoleDAL : BaseDal, IRoleDAL
    {
        public int AddRole(RoleEntity role)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into tbrole (name,code,state)");
            strSql.Append("values");
            strSql.Append("(@RoleNamem,@Code,@State)");
            strSql.Append(";SELECT @@IDENTITY");
            SqlParameter[] paras =
            {
                new SqlParameter("@RoleName",role.Name),
                new SqlParameter("@Code",role.Code),
                new SqlParameter("@State",role.State)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.connStr, CommandType.Text, strSql.ToString(), paras));
        }

        public bool DeleteRole(string id)
        {
            List<string> list = new List<string>();
            list.Add("delete from tbrole where id in (" + id + ")");
            try
            {
                int count = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, list);
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool EditRole(RoleEntity role)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update  tbrole set");
            strSql.Append("name=@RoleName,code=@Code,state=@State");
            strSql.Append("where id = @RoleID");
            SqlParameter[] paras =
            {
                new SqlParameter("@RoleName",role.Name),
                new SqlParameter("@Code",role.Code),
                new SqlParameter("@State",role.State),
                new SqlParameter("@RoleID",role.Id)
            };
            object obj = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), paras);
            if (Convert.ToInt32(obj) > 0)
                return true;
            else
                return false;
        }

        public IEnumerable<T> QryRole<T>(Dictionary<string, object> paras, out int iCount)
        {
            iCount = 0;
            WhereBuilder builder = new WhereBuilder();
            builder.FromSql = "v_role_list";
            GridData grid = new GridData()
            {
                PageIndex = Convert.ToInt32(paras["pi"]),
                PageSize = Convert.ToInt32(paras["pageSize"]),
                SortField = paras["sort"].ToString(),
                SortDirection = paras["order"].ToString()
            };
            builder.AddWhereAndParameter(paras, "RoleName", "name", "LIKE", "'%'+@RoleName+'%'");
            return SortAndPage<T>(builder, grid, out iCount);
        }
        
        public int Save(Dictionary<string, object> paras)
        {
            DataTable dtRolememu = paras["rolememu"] as DataTable;
            paras["rolememu"] = dtRolememu.AsTableValuedParameter();
            return QuerySingle<int>("P_Role_Save", paras, CommandType.StoredProcedure); 
                //StandarInsertOrUpdate("tbrole", paras);
        }
    }
}
