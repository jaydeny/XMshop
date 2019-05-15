﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using XM.Comm;
using XM.Model;
using XM.Web.Controllers;

namespace XM.WebVip.Controllers
{
    public class VipInfoController : BaseController
    {
        // GET: VipInfo
        #region _vipInfo
        /// <summary>
        /// 作者:曾贤鑫
        /// 日期:2019/4/28
        /// 功能:返回vip个人中心页面
        /// </summary>
        /// <returns>页面</returns>
        public ActionResult VipInfoPage()
        {
            if (Session["AN"] == null)
            {
                Url.Action("Index", "Home");
            }

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("vip_AN", Session["AN"].ToString());

            var result = DALUtility.Vip.QryVipInfo<VipInfoDTO>(param);

            ViewData["VipAccountName"] = Session["AN"];
            ViewData["Remainder"] = Remainder;
            ViewData["Email"] = result.VipEmail;
            ViewData["MP"] = result.VipMobliePhone;

            return View();
        }


        /// <summary>
        /// 作者:曾贤鑫
        /// 日期:2019/4/28
        /// 功能:返回vip个人信息
        /// </summary>
        /// <returns>json值</returns>
        //[HttpPost]
        public ActionResult VipInfo()
        {

            if (Session["AN"] == null)
            {
                Url.Action("Index", "Home");
            }

            //判断回收区,是否存在sessionID
            if (recycle.ContainsKey(Session.SessionID))
            {
                if (recycle[Session.SessionID] == false)
                {
                    RemoveSession();
                    //存在就踢下线
                    return OperationReturn(false, "被踢下线");
                }
            }
            return OperationReturn(true, "登录状态");
        }
        #endregion

        #region _address
        /// <summary>
        /// 作者:曾贤鑫
        /// 日期:2019/4/28
        /// 功能:返回vip收货地址
        /// </summary>
        /// <returns>页面</returns>
        public ActionResult AddressPage()
        {
            return View("_AddressPage");
        }

        /// <summary>
        /// 作者:曾贤鑫
        /// 日期:2019/4/28
        /// 功能:返回vip收货地址
        /// </summary>
        /// <returns>json值</returns>
        [HttpPost]
        public ActionResult Address()
        {

            string sort = Request["sort"] == null ? "id" : Request["sort"];
            string order = Request["order"] == null ? "desc" : Request["order"];
            int pageindex = Request["page"] == null ? 1 : Convert.ToInt32(Request["page"]);
            int pagesize = Request["rows"] == null ? 10 : Convert.ToInt32(Request["rows"]);

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("pi", pageindex);
            param.Add("pageSize", pagesize);
            param.Add("sort", sort);
            param.Add("vip_id", int.Parse(Session["ID"].ToString()));

            string result = DALUtility.Vip.QryVipAddress(param, out int iCount);

            return Content(result);
        }

        /// <summary>
        /// 作者:曾贤鑫
        /// 日期:2019/4/28
        /// 功能:vip地址添加
        /// </summary>
        /// <returns></returns>
        public ActionResult InsertAddress()
        {
            return SaveAddress(0);
        }

        public ActionResult UpdateAddress()
        {
            return SaveAddress(int.Parse(Request["address_id"]));
        }

        /// <summary>
        /// 作者:曾贤鑫
        /// 日期:2019/4/28
        /// 功能:删除收货地址
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteAddress()
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("id", int.Parse(Request["address_id"]));
            param.Add("vip_id", int.Parse(Session["ID"].ToString()));
            //param.Add("vip_id", Request["vip_id"]);

            int iCheck = DALUtility.Vip.DeleteAddress(param);

            if (iCheck == 0)
            {
                return OperationReturn(false, "删除收货地址失败");
            }
            return OperationReturn(true, "删除收货地址成功");
        }
        #endregion
        
        #region _recharge
        /// <summary>
        /// 作者:曾贤鑫
        /// 日期:2019/4/26
        /// 功能:会员端进行充值
        /// </summary>
        /// <param name="vip_AN"></param>
        /// <returns>页面</returns>
        public ActionResult RechargePage()
        {
            return View("_RechargePage");
        }
        
        /// <summary>
        /// 作者:曾贤鑫
        /// 日期:2019/4/26
        /// 功能:会员端进行充值
        /// </summary>
        /// <param name="vip_AN"></param>
        /// <returns>json值</returns>
        public ActionResult Recharge()
        {
            DateTime date = DateTime.Now;

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("recharge_name", "测试充值");
            param.Add("recharge_price", Request["recharge_price"]);
            param.Add("recharge_time", date);
            param.Add("agent_id", Session["Agent_ID"].ToString());
            param.Add("vip_id", Session["ID"].ToString());

            int iCheck = DALUtility.Vip.Recharge(param);

            if (iCheck > 0)
            {
                Dictionary<string, object> p = new Dictionary<string, object>();
                p.Add("remainder", Request["recharge_price"]);
                p.Add("vip_AN", Session["AN"].ToString());
                //p.Add("vip_AN", HttpContext.Session["vip_AN"]);
                int i = DALUtility.Vip.InsertRemainder(p);

                if (i != 2)
                {
                    Url.Action("CheckRecharge", "Agent", new RouteValueDictionary {
                    { "vip_id",Request["vip_id"]},
                    { "recharge_price",Request["recharge_price"]},
                    { "recharge_time", date}
                    });

                    return OperationReturn(true, "充值成功");
                }
                else
                {
                    return OperationReturn(false, "充值失败");
                }
            }
            return OperationReturn(false, "充值失败");
        }
        #endregion

        #region _invitation
        /// <summary>
        /// 作者:曾贤鑫
        /// 日期:2019/4/26
        /// 功能:会员端进行邀请注册
        /// </summary>
        /// <param name="vip_AN"></param>
        /// <returns>json值</returns>
        public ActionResult Invitation(string vip_AN)
        {
            return OperationReturn(true, vip_AN);
        }
        #endregion

        #region _自定义
        /// <summary>
        /// 作者:曾贤鑫
        /// 日期:2019/4/28
        /// 功能:vip地址的添加或者修改公用方法
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveAddress(int ID)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("id", ID);
            param.Add("address_name", Request["address_name"]);
            param.Add("vip_id", int.Parse(Session["ID"].ToString()));
            //param.Add("vip_id", Request["vip_id"]);

            int iCheck = DALUtility.Vip.SaveAddress(param);
            ContentResult strResult = null;
            switch (iCheck)
            {
                case 0:
                    strResult = OperationReturn(true, "添加收货地址成功");
                    break;
                case 1:
                    strResult = OperationReturn(true, "修改收货地址成功");
                    break;
                case 2:
                    if (ID == 0)
                    {
                        strResult = OperationReturn(false, "添加收货地址失败");
                    }
                    strResult = OperationReturn(false, "修改收货地址失败");
                    break;
            }
            return strResult;
        }

        
        /// <summary>
        /// 作者：曾贤鑫
        /// 创建时间:2019-4-28
        /// 修改时间：2019-
        /// 功能：安全退出
        /// </summary>
        public ActionResult RemoveSession()
        {
            pairs.Remove(Session["AN"].ToString());
            Session.Remove("AN");
            Session.Remove("ID");
            Session.Remove("Agent_ID");
            Session.Remove("Agent_AN");
            return OperationReturn(true, "退出成功");
        }
        #endregion
    }
}