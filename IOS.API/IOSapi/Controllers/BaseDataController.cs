using IOSapi.Models;
using IOSapi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IOSapi.Controllers
{
    /// <summary>
    /// 基础数据
    /// </summary>
    public class BaseDataController : BaseApiController
    {
        APSEntities db = new APSEntities();

        /// <summary>
        /// 跨域处理
        /// </summary>
        /// <returns></returns>
        public string Options()
        {
            return null; // HTTP 200 response with empty body
        }

        /// <summary>
        /// 获取所有工厂数据信息
        /// </summary>
        /// <param name="isoutside">是否外协，0否；1是</param>
        /// <returns></returns>
        [HttpGet, Route("api/BaseData/GetFactory")]
        public List<Factory> GetFactory(int isoutside = -1)
        {
            var query = db.FACTORies.OrderBy(c => c.ID).Where(c => (isoutside == -1 ? true : c.ISOUTSIDE == isoutside)).Select(c => 
                new Factory() { id = c.ID, code = c.CODE, name = c.NAME, description = c.DESCRIPTION, isoutside = c.ISOUTSIDE.Value });
            return query.ToList();
        }


        /// <summary>
        /// 获取所有工厂数据(id/名称)
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/BaseData/GetFactorys")]
        public List<SelectModel> GetFactorys()
        {
            var query = db.FACTORies.OrderBy(c => c.ID).Select(c => new SelectModel() { itemId = c.ID, itemName = c.NAME });
            return query.ToList();
        }

        /// <summary>
        ///  获取所有工厂\车间\产线数据
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/BaseData/GetFactoryLines")]
        public List<FactoryLines> GetFactoryLines()
        {
            int[] fids = GetAllowedFactoryIDs();
            var query = db.FACTORies.Where(b => fids.Contains(b.ID)).OrderBy(c => c.ID).Select(c => new FactoryLines()
            {
                FId = c.ID,
                FName = c.NAME,
                Shops = c.WORKSHOPs.Select(p => new Workshop()
                {
                    WId = p.ID,
                    ShopName = p.GROUPNAME,
                    Lines = c.FACILITies.Where(f => f.GROUPNAME == p.GROUPNAME && f.STATE == 0).Select(q => new Line() { LId = q.ID, LineName = q.NAME }).ToList()
                }).ToList(),
            });
            return query.ToList();
        }

        /// <summary>
        /// 根据工厂id获取产线数据
        /// </summary>
        /// <param name="fid">工厂id</param>
        /// <returns></returns>
        [HttpGet, Route("api/BaseData/GetLines")]
        public List<SelectModel> GetLines(int fid)
        {
            var query = db.FACILITies.OrderBy(c => c.ID).Select(c => new SelectModel() { itemId = c.ID, itemName = c.NAME });
            return query.ToList();
        }

        ///// <summary>
        ///// 获取品牌客户
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet, Route("api/BaseData/GetCustomer")]
        //public List<SelectModel> GetCustomer()
        //{
        //    var query = db.CUSTOMERs.OrderBy(c => c.ID).Select(c => new SelectModel() { itemId = c.ID, itemName = c.NAME });
        //    return query.ToList();
        //}

        /// <summary>
        /// 获取所有工序(含排产工序)
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/BaseData/GetProcesses")]
        public List<SelectModel> GetProcesses()
        {
            var query = db.PROCESSes.OrderBy(c => c.ID).Select(c => new SelectModel() { itemId = c.ID, itemName = c.NAME });
            return query.ToList();
        }


        /// <summary>
        /// 获取所有非排产工序
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/BaseData/GetOthProcesses")]
        public List<SelectModel> GetOthProcesses()
        {
            var query = db.PROCESSes.Where(p => p.ISPRIMARY == 0).OrderBy(c => c.ID).Select(c => new SelectModel() { itemId = c.ID, itemName = c.NAME });
            return query.ToList();
        }

        /// <summary>
        /// 获取所有关键事件
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/BaseData/GetEvents")]
        public List<SelectModel> GetEvents()
        {
            var query = db.CRITICALEVENTs.Where(c => c.ISAFFECTPLAN == 1).OrderBy(c => c.ID).Select(c => new SelectModel() { itemId = c.ID, itemName = c.NAME });
            return query.ToList();
        }

        /// <summary>
        /// 获取所有产品顶级分类()
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/BaseData/GetProductTypes")]
        public List<SelectModel> GetProductTypes()
        {
            var query = db.PRODUCTTYPEs.Where(c => c.PARENTID == -1).OrderBy(c => c.CODE).Select(c => new SelectModel() { itemId = c.ID, itemName = c.NAME });
            return query.ToList();
        }


        /// <summary>
        /// 获取客户（品牌）信息
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="name">客户名称</param>
        /// <param name="code">客户编号</param>
        /// <returns></returns>
        [HttpGet, Route("api/BaseData/GetCustomers")]
        public List<Customer> GetCustomers(int pageIndex, int pageSize, string name = "", string code = "")
        {
            var query = db.CUSTOMERs.Where(c => (string.IsNullOrEmpty(name) ? true : c.NAME.Contains(name)) && 
                (string.IsNullOrEmpty(code) ? true : c.NAME.Contains(code))).Select(c => new Customer()
            {
                code = c.CODE,
                description = c.DESCRIPTION,
                id = c.ID,
                name = c.NAME,
                parentid = c.PARENTID.Value,
                updatedate = c.UPDATEDATE
            });

            return query.ToList();
        }
    }
}
