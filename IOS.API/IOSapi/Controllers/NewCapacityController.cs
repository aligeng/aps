using AMO.MacroPlan;
using AMOData;
using IOS.Tools;
using IOSapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Drawing;
using AMOData.Settings;

namespace IOSapi.Controllers
{
    /// <summary>
    /// 新宏观计划
    /// </summary>
    public class NewCapacityController : BaseApiController
    {
        /// <summary>
        /// 跨域处理
        /// </summary>
        /// <returns></returns>
        public string Options()
        {
            return null; // HTTP 200 response with empty body
        }


        private DateTime GetDateEnd(DateTime starDate, int timeSpan, int timeType)
        {
            DateTime dEnd = DateTime.MinValue;
            int selNO = timeSpan;
            if (timeType == 0)
            {
                dEnd = starDate.Date.AddDays(-starDate.Day + 1);
                dEnd = dEnd.AddMonths(selNO).AddDays(-1);
            }
            else if (timeType == 1)
            {
                int ys = 7 - (int)starDate.Date.DayOfWeek;
                dEnd = starDate.Date.AddDays((selNO - 1) * 7 + ys);
            }
            else if (timeType == 2)
                dEnd = starDate.Date.AddDays(selNO - 1);
            return dEnd;
        }

        /// <summary>
        /// 宏观计划（APP）
        /// </summary>
        /// <param name="starDate">开始时间</param>
        /// <param name="capIncOut">是否产能包含外协  0否，1是</param>
        /// <param name="loadingIncOut">是否负载包含外协  0否，1是</param>
        /// <param name="timeSpan">周期数</param>
        /// <param name="timeType">周期类型  0月,1周,2日</param>
        /// <param name="unitType">单位 0人*工时，1件，2标准件</param>
        /// <param name="fids">工厂id集（如1,3,5），默认所有工厂</param>
        [HttpGet, Route("api/Capacity/GetCapacity")]
        public List<IOSapi.ViewModels.CapacityChartRow> GetCapacity(DateTime starDate, int capIncOut, int loadingIncOut, int timeSpan, int timeType, int unitType, string fids = "")
        {
            List<IOSapi.ViewModels.CapacityChartRow> CapacityChartRowList = new List<IOSapi.ViewModels.CapacityChartRow>();
            try
            {
                List<int> fidLs = new List<int>();
                if (!string.IsNullOrEmpty(fids))
                {
                    string[] fidsArry = fids.Split(',');
                    foreach (var item in fidsArry)
                    {
                        fidLs.Add(Convert.ToInt32(item));
                    }
                }
                else
                {
                    int[] fidsArry = GetAllowedFactoryIDs();
                    foreach (var item in fidsArry)
                    {
                        fidLs.Add(item);
                    }
                }

                APSEntities db = new APSEntities();
                var query = db.FACTORies.OrderBy(c => c.ID).Where(c => (fids == "" ? true : fidLs.Contains(c.ID))).Select(c =>
                   new { ID = c.ID, Code = c.CODE, Name = c.NAME, Description = c.DESCRIPTION, IsOutside = c.ISOUTSIDE, Capacity = c.CAPACITY }).ToList();

                if (query == null)
                {
                    return CapacityChartRowList;
                }


                List<Factory> LstFactory = new List<Factory>();
                foreach (var item in query)
                {
                    Factory Factory = new Factory();
                    Factory.ID = item.ID;
                    Factory.Name = item.Name;
                    Factory.Description = item.Description;
                    Factory.IsOutside = item.IsOutside == 1;
                    Factory.Code = item.Code;
                    Factory.Capacity = item.Capacity == null ? 0 : item.Capacity.Value;
                    LstFactory.Add(Factory);
                }

                DateTime endTime = GetDateEnd(starDate, timeSpan, timeType);
                AMOData.Settings.UserSetting.Load(0);//加载用户参数
                AMOData.Settings.SysSetting.Load();//加载系统参数
                AMOData.Settings.ImpSetting.Load();//实施参数
                MacroDataCreator mdc = new MacroDataCreator(LstFactory, starDate, endTime);

                ChartParameter.LstLoadingToolTip = new List<string>() { "PoPlanLoading", "PoLoading", "VirPlanLoading", "VirPoLoading" };
                ChartParameter.LstLoadingColor = new List<Color>() { 
                    AMOData.Settings.UserSetting.PlanLoadingColor,
                    AMOData.Settings.UserSetting.PoLoadingColor ,
                      AMOData.Settings.UserSetting.VirPlanLoadingColor, 
                    AMOData.Settings.UserSetting.VirPoLoadingColor
                };


                ChartParameter.OutCapTooltip = "OutCapacity";
                ChartParameter.OtCapTooltip = "OtCapacity";
                ChartParameter.TotalCapTooltip = "TotalCapacity";
                ChartParameter.NormalCapTooltip = "NormalCapacity";

                //for (int i = 0; i < 4; i++)
                //{
                //    ChartParameter.LstLoadingToolTip.Add((i + 1).ToString());
                //    ChartParameter.LstLoadingColor.Add(Color.Red);
                //}

                mdc.InitData();

                List<string> chartHeader = new List<string>() { "工厂" };
                //List<ProductType> pt = ProductType.GetList(null);
                IOS.DBUtility.Filter filter = new IOS.DBUtility.Filter();
                List<ProductType> pt = ProductType.GetList(filter);
                List<WorkShop> ws = new List<WorkShop>();
                ws.Add(new WorkShop() { ID = -1 });//

                List<ChartDataRow> datas = mdc.GetUIData(LstFactory, ws, pt, "total", 0, 0, Convert.ToBoolean(capIncOut), Convert.ToBoolean(loadingIncOut), chartHeader, (EmDateCycleType)timeType, (MacroUnitTypes)unitType, 0);


                int rowIndex = 0;
                foreach (var row in datas)
                {
                    rowIndex++;
                    if (rowIndex < datas.Count)
                    {
                        IOSapi.ViewModels.CapacityChartRow charRow = new ViewModels.CapacityChartRow();
                        charRow.Name = "";
                        charRow.CapacityChartDataList = new List<ViewModels.CapacityChartData>();
                        foreach (var blok in row.LstDataBlock)
                        {
                            IOSapi.ViewModels.CapacityChartData capacityChartData = new ViewModels.CapacityChartData();
                            capacityChartData.dataTime = blok.OfColumn.ToString();
                            capacityChartData.capData = new List<ViewModels.ChartDataItem>();
                            capacityChartData.loadingData = new List<ViewModels.ChartDataItem>();
                            if (string.IsNullOrEmpty(charRow.Name))
                            {
                                if (blok.CapacityData.Source != null && blok.CapacityData.Source.Count > 0)
                                {
                                    charRow.Name = blok.CapacityData.Source[0].FactoryName;
                                }
                            }

                            //产能数据
                            foreach (var item in blok.CapacityData.DataItem)
                            {

                                ViewModels.ChartDataItem chartDataItem = new ViewModels.ChartDataItem();
                                chartDataItem.name = Enum.GetName(typeof(ChartItemType), item.ItemType);
                                chartDataItem.value = Math.Round(item.Value, 0);
                                chartDataItem.color = string.Format("#{0:X2}{1:X2}{2:X2}", item.ItemColor.R, item.ItemColor.G, item.ItemColor.B);
                                capacityChartData.capData.Add(chartDataItem);
                            }
                            //负载
                            foreach (var item in blok.LoadingData.DataItem)
                            {
                                ViewModels.ChartDataItem chartDataItem = new ViewModels.ChartDataItem();
                                chartDataItem.name = Enum.GetName(typeof(ChartItemType), item.ItemType);
                                chartDataItem.value = Math.Round(item.Value, 0);
                                chartDataItem.color = string.Format("#{0:X2}{1:X2}{2:X2}", item.ItemColor.R, item.ItemColor.G, item.ItemColor.B);
                                capacityChartData.loadingData.Add(chartDataItem);
                            }

                            charRow.CapacityChartDataList.Add(capacityChartData);

                        }

                        CapacityChartRowList.Add(charRow);
                    }
                    else
                    {
                        IOSapi.ViewModels.CapacityChartRow charRow = new ViewModels.CapacityChartRow();
                        charRow.Name = "汇总";
                        charRow.CapacityChartDataList = new List<ViewModels.CapacityChartData>();
                        foreach (var blok in row.LstDataBlock)
                        {
                            IOSapi.ViewModels.CapacityChartData capacityChartData = new ViewModels.CapacityChartData();
                            capacityChartData.dataTime = blok.OfColumn.ToString();
                            capacityChartData.capData = new List<ViewModels.ChartDataItem>();
                            capacityChartData.loadingData = new List<ViewModels.ChartDataItem>();

                            //产能数据
                            foreach (var item in blok.CapacityData.DataItem)
                            {

                                ViewModels.ChartDataItem chartDataItem = new ViewModels.ChartDataItem();
                                chartDataItem.name = item.ItemText;
                                chartDataItem.value = Math.Round(item.Value, 0);
                                chartDataItem.color = string.Format("#{0:X2}{1:X2}{2:X2}", item.ItemColor.R, item.ItemColor.G, item.ItemColor.B);
                                capacityChartData.capData.Add(chartDataItem);
                            }
                            //负载
                            foreach (var item in blok.LoadingData.DataItem)
                            {
                                ViewModels.ChartDataItem chartDataItem = new ViewModels.ChartDataItem();
                                chartDataItem.name = item.ItemText;
                                chartDataItem.value = Math.Round(item.Value, 0);
                                chartDataItem.color = string.Format("#{0:X2}{1:X2}{2:X2}", item.ItemColor.R, item.ItemColor.G, item.ItemColor.B);
                                capacityChartData.loadingData.Add(chartDataItem);
                            }

                            charRow.CapacityChartDataList.Add(capacityChartData);

                        }

                        CapacityChartRowList.Add(charRow);
                    }



                }
            }
            catch (Exception)
            {
                return null;
            }


            return CapacityChartRowList;

        }



        /// <summary>
        /// 宏观计划（监控中心，只返回汇总数据）
        /// </summary>
        /// <param name="starDate">开始时间</param>
        /// <param name="capIncOut">是否产能包含外协  0否，1是</param>
        /// <param name="loadingIncOut">是否负载包含外协  0否，1是</param>
        /// <param name="timeSpan">周期数</param>
        /// <param name="timeType">周期类型  0月,1周,2日</param>
        /// <param name="unitType">单位 0人*工时，1件，2标准件</param>
        /// <param name="fids">工厂id集（如1,3,5），默认所有工厂</param>
        [HttpGet, Route("api/Capacity/GetCapacitySum")]
        public List<IOSapi.ViewModels.CapacityChartRow> GetCapacitySum(DateTime starDate, int capIncOut, int loadingIncOut, int timeSpan, int timeType, int unitType, string fids = "")
        {
            List<IOSapi.ViewModels.CapacityChartRow> CapacityChartRowList = new List<IOSapi.ViewModels.CapacityChartRow>();
            try
            {
                List<int> fidLs = new List<int>();
                if (!string.IsNullOrEmpty(fids))
                {
                    string[] fidsArry = fids.Split(',');
                    foreach (var item in fidsArry)
                    {
                        fidLs.Add(Convert.ToInt32(item));
                    }
                }
                else
                {
                    int[] fidsArry = GetAllowedFactoryIDs();
                    foreach (var item in fidsArry)
                    {
                        fidLs.Add(item);
                    }
                }

                APSEntities db = new APSEntities();
                var query = db.FACTORies.OrderBy(c => c.ID).Where(c => (fids == "" ? true : fidLs.Contains(c.ID))).Select(c =>
                   new { ID = c.ID, Code = c.CODE, Name = c.NAME, Description = c.DESCRIPTION, IsOutside = c.ISOUTSIDE, Capacity = c.CAPACITY }).ToList();

                if (query == null)
                {
                    return CapacityChartRowList;
                }


                List<Factory> LstFactory = new List<Factory>();
                foreach (var item in query)
                {
                    Factory Factory = new Factory();
                    Factory.ID = item.ID;
                    Factory.Name = item.Name;
                    Factory.Description = item.Description;
                    Factory.IsOutside = item.IsOutside == 1;
                    Factory.Code = item.Code;
                    Factory.Capacity = item.Capacity == null ? 0 : item.Capacity.Value;
                    LstFactory.Add(Factory);
                }

                DateTime endTime = GetDateEnd(starDate, timeSpan, timeType);
                AMOData.Settings.UserSetting.Load(0);//加载用户参数
                AMOData.Settings.SysSetting.Load();//加载系统参数
                AMOData.Settings.ImpSetting.Load();//实施参数
                MacroDataCreator mdc = new MacroDataCreator(LstFactory, starDate, endTime);

                ChartParameter.LstLoadingToolTip = new List<string>() { "PoPlanLoading", "PoLoading", "VirPlanLoading", "VirPoLoading" };
                ChartParameter.LstLoadingColor = new List<Color>() { 
                    AMOData.Settings.UserSetting.PlanLoadingColor,
                    AMOData.Settings.UserSetting.PoLoadingColor ,
                      AMOData.Settings.UserSetting.VirPlanLoadingColor, 
                    AMOData.Settings.UserSetting.VirPoLoadingColor
                };


                ChartParameter.OutCapTooltip = "OutCapacity";
                ChartParameter.OtCapTooltip = "OtCapacity";
                ChartParameter.TotalCapTooltip = "TotalCapacity";
                ChartParameter.NormalCapTooltip = "NormalCapacity";

                mdc.InitData();

                List<string> chartHeader = new List<string>() { "工厂" };
                //List<ProductType> pt = ProductType.GetList(null);
                IOS.DBUtility.Filter filter = new IOS.DBUtility.Filter();
                List<ProductType> pt = ProductType.GetList(filter);
                List<WorkShop> ws = new List<WorkShop>();
                ws.Add(new WorkShop() { ID = -1 });//

                List<ChartDataRow> datas = mdc.GetUIData(LstFactory, ws, pt, "total", 0, 0, Convert.ToBoolean(capIncOut), Convert.ToBoolean(loadingIncOut), chartHeader, (EmDateCycleType)timeType, (MacroUnitTypes)unitType, 0);


                int rowIndex = datas.Count - 1;

                IOSapi.ViewModels.CapacityChartRow charRow = new ViewModels.CapacityChartRow();
                charRow.Name = "汇总";
                charRow.CapacityChartDataList = new List<ViewModels.CapacityChartData>();
                foreach (var blok in datas[rowIndex].LstDataBlock)
                {
                    IOSapi.ViewModels.CapacityChartData capacityChartData = new ViewModels.CapacityChartData();
                    capacityChartData.dataTime = blok.OfColumn.ToString();
                    capacityChartData.capData = new List<ViewModels.ChartDataItem>();
                    capacityChartData.loadingData = new List<ViewModels.ChartDataItem>();

                    //产能数据
                    foreach (var item in blok.CapacityData.DataItem)
                    {

                        ViewModels.ChartDataItem chartDataItem = new ViewModels.ChartDataItem();
                        chartDataItem.name = item.ItemText;
                        chartDataItem.value = Math.Round(item.Value, 0);
                        chartDataItem.color = string.Format("#{0:X2}{1:X2}{2:X2}", item.ItemColor.R, item.ItemColor.G, item.ItemColor.B);
                        capacityChartData.capData.Add(chartDataItem);
                    }
                    //负载
                    foreach (var item in blok.LoadingData.DataItem)
                    {
                        ViewModels.ChartDataItem chartDataItem = new ViewModels.ChartDataItem();
                        chartDataItem.name = item.ItemText;
                        chartDataItem.value = Math.Round(item.Value, 0);
                        chartDataItem.color = string.Format("#{0:X2}{1:X2}{2:X2}", item.ItemColor.R, item.ItemColor.G, item.ItemColor.B);
                        capacityChartData.loadingData.Add(chartDataItem);
                    }

                    charRow.CapacityChartDataList.Add(capacityChartData);

                }

                CapacityChartRowList.Add(charRow);

            }
            catch (Exception)
            {
                return null;
            }


            return CapacityChartRowList;

        }


        /// <summary>
        /// 宏观计划颜色图例
        /// </summary>
        /// <returns></returns>

        [HttpGet, Route("api/Capacity/GetCapacityColorTip")]
        public List<ViewModels.CapacityColorTip> GetCapacityColorTip()
        {
            AMOData.Settings.UserSetting.Load(0);

            List<ViewModels.CapacityColorTip> ls = new List<ViewModels.CapacityColorTip>();

            ls.Add(new ViewModels.CapacityColorTip()
            {
                Color = string.Format("#{0:X2}{1:X2}{2:X2}", UserSetting.TotalCapColor.R, UserSetting.TotalCapColor.G, UserSetting.TotalCapColor.B),
                Name = "TotalCapacity",
                Remark = "总产能",
                Type = 0
            });
            ls.Add(new ViewModels.CapacityColorTip()
            {
                Color = string.Format("#{0:X2}{1:X2}{2:X2}", UserSetting.NormalCapColor.R, UserSetting.NormalCapColor.G, UserSetting.NormalCapColor.B),
                Name = "NormalCapacity",
                Remark = "正常产能",
                Type = 0
            });
            ls.Add(new ViewModels.CapacityColorTip()
            {
                Color = string.Format("#{0:X2}{1:X2}{2:X2}", UserSetting.OtCapColor.R, UserSetting.OtCapColor.G, UserSetting.OtCapColor.B),
                Name = "OtCapacity",
                Remark = "加班产能",
                Type = 0
            });
            ls.Add(new ViewModels.CapacityColorTip()
            {
                Color = string.Format("#{0:X2}{1:X2}{2:X2}", UserSetting.OutCapColor.R, UserSetting.OutCapColor.G, UserSetting.OutCapColor.B),
                Name = "OutCapacity",
                Remark = "外发产能",
                Type = 0
            });

            //ChartParameter.LstLoadingToolTip = new List<string>() { "VirPlanLoading", "VirPoLoading", "PlanLoading", "PoLoading" };
            //ChartParameter.LstLoadingColor = new List<Color>() { AMOData.Settings.UserSetting.VirPlanLoadingColor, AMOData.Settings.UserSetting.VirPoLoadingColor,
            //        AMOData.Settings.UserSetting.PlanLoadingColor, AMOData.Settings.UserSetting.PoLoadingColor };

            ls.Add(new ViewModels.CapacityColorTip()
            {
                Color = string.Format("#{0:X2}{1:X2}{2:X2}", UserSetting.PlanLoadingColor.R, UserSetting.PlanLoadingColor.G, UserSetting.PlanLoadingColor.B),
                Name = "PoPlanLoading",
                Remark = "实单已排负载",
                Type = 1
            });
            ls.Add(new ViewModels.CapacityColorTip()
            {
                Color = string.Format("#{0:X2}{1:X2}{2:X2}", UserSetting.PoLoadingColor.R, UserSetting.PoLoadingColor.G, UserSetting.PoLoadingColor.B),
                Name = "PoLoading",
                Remark = "实单未排负载",
                Type = 1
            });
            ls.Add(new ViewModels.CapacityColorTip()
            {
                Color = string.Format("#{0:X2}{1:X2}{2:X2}", UserSetting.VirPlanLoadingColor.R, UserSetting.VirPlanLoadingColor.G, UserSetting.VirPlanLoadingColor.B),
                Name = "VirPlanLoading",
                Remark = "非实单已排负载",
                Type = 1
            });
            ls.Add(new ViewModels.CapacityColorTip()
            {
                Color = string.Format("#{0:X2}{1:X2}{2:X2}", UserSetting.VirPoLoadingColor.R, UserSetting.VirPoLoadingColor.G, UserSetting.VirPoLoadingColor.B),
                Name = "VirPoLoading",
                Remark = "非实单未排负载",
                Type = 1
            });



            return ls;
        }

    }
}
