﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XM.Model;

namespace XM.Web.Controllers
{
    /// <summary>
    /// 创建人：朱茂琛
    /// 创建时间：2019/4/22
    /// 代理商控制器
    /// </summary>
    public class AgentController : BaseController
    {
        // GET: Agent
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetAllUserInfo()
        {
            string sort = Request["sort"] == null ? "AgentID" : Request["sort"];
            string order = Request["order"] == null ? "asc" : Request["order"];

            //首先获取前台传递过来的参数
            int pageindex = Request["page"] == null ? 1 : Convert.ToInt32(Request["page"]);
            int pagesize = Request["rows"] == null ? 10 : Convert.ToInt32(Request["rows"]);
            string userAn = Request["agent_AN"] == null ? "" : Request["agent_AN"];
            string userMp = Request["agent_mp"] == null ? "" : Request["agent_mp"];
            string userEmail = Request["agent_email"] == null ? "" : Request["agent_email"];
            string statusId = Request["status_id"] == null ? "" : Request["status_id"];



             int totalCount;   //输出参数
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["pi"] = pageindex;
            paras["pageSize"] = pagesize;
            paras["AgentName"] = userAn;
            paras["sort"] = sort;
            paras["order"] = order;
            var users = DALUtility.Agent.QryUsers<AgentEntity>(paras, out totalCount);
            return PagerData(totalCount, users);
        }

        public ActionResult AddAgent()
        {
            return View("_AddAgent");
        }
        /// <summary>
        /// 新增 用户
        /// </summary>
        /// <returns></returns>
        public ActionResult AddUser()
        {
            return SaveUser();

        }

        public ActionResult EditAgent()
        {
            return View("_EditAgent");
        }
        /// <summary>
        /// 编辑 用户
        /// </summary>
        /// <returns></returns>
        public ActionResult EditUser()
        {

            return SaveUser();
        }

        private ActionResult SaveUser()
        {
            int id = Convert.ToInt32(Request["id"]);
            string userid = Request["agent_AN"];
            string mobilephone = Request["agent_mp"];
            string email = Request["agent_email"];
            int statusID = Convert.ToInt32(Request["status_id"]);

            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["id"] = id;
            paras["agent_AN"] = userid;
            paras["agent_mp"] = mobilephone;
            paras["agent_email"] = email;


            int iCheck = DALUtility.Agent.CheckUseridAndEmail(paras);
            if (iCheck > 0)
            {
                return OperationReturn(false, iCheck == 1 ? "用户名重复" : "邮箱重复");
            }
            else
            {
                paras["status_id"] = statusID;
                if (id == 0)
                {
                    paras["agent_pwd"] = "xm123456";
                    paras["agent_CBY"] = "admin";
                    paras["agent_CDT"] = DateTime.Now;
                    return OperationReturn(DALUtility.Agent.Save(paras) > 0, "添加成功！初始密码：" + paras["agent_pwd"]);
                }
                return OperationReturn(DALUtility.Agent.Save(paras) > 0, "修改成功！");
            }




        }

        public ActionResult DelUserByIDs()
        {
            string Ids = Request["id"] == null ? "" : Request["id"];
            if (!string.IsNullOrEmpty(Ids))
            {
                return OperationReturn(DALUtility.Agent.DeleteUser(Ids));
            }
            else
            {
                return OperationReturn(false);
            }
        }

        
    }
}