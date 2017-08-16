using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Core.Common
{
    public class QMExtend
    {
        /// <summary>
        /// 任务类型
        /// </summary>
        /// <param name="type">任务类型</param>
        /// <param name="def">是否显示"请选择任务类型"</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetTaskType(string type = null, bool def = true)
        {
            var result = new List<SelectListItem>();
            if (def == true)
            {
                result.Add(new SelectListItem()
                {
                    Text = "请选择任务类型",
                    Value = "",
                    Selected = true
                });
            }

            result.Add(new SelectListItem()
            {
                Text = "SQL文件",
                Value = "SQL-FILE",
                Selected = ("SQL-FILE" == type ? true : false)
            });
            result.Add(new SelectListItem()
            {
                Text = "SQL导出",
                Value = "SQL-EXP",
                Selected = ("SQL-EXP" == type ? true : false)
            });
            result.Add(new SelectListItem()
            {
                Text = "标准接口程序",
                Value = "DLL-STD",
                Selected = ("DLL-STD" == type ? true : false)
            });
            result.Add(new SelectListItem()
            {
                Text = "非标准程序",
                Value = "DLL-UNSTD",
                Selected = ("DLL-UNSTD" == type ? true : false)
            });

            return result;
        }

        /// <summary>
        /// 任务状态
        /// </summary>
        /// <param name="status">任务状态</param>
        /// <param name="def">是否显示"请选择任务状态"</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetTaskStatus(string status = null, bool def = true)
        {
            var result = new List<SelectListItem>();
            if (def == true)
            {
                result.Add(new SelectListItem()
                {
                    Text = "请选择任务状态",
                    Value = "",
                    Selected = true
                });
            }

            result.Add(new SelectListItem()
            {
                Text = "启用",
                Value = "Y",
                Selected = ("Y" == status ? true : false)
            });
            result.Add(new SelectListItem()
            {
                Text = "禁用",
                Value = "N",
                Selected = ("N" == status ? true : false)
            });

            return result;
        }

        /// <summary>
        /// 发送类型
        /// </summary>
        /// <param name="send">发送类型</param>
        /// <param name="def">是否显示"请选择发送类型"</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetSendBy(string send = null, bool def = true)
        {
            var result = new List<SelectListItem>();
            if (def == true)
            {
                result.Add(new SelectListItem()
                {
                    Text = "请选择发送类型",
                    Value = "",
                    Selected = true
                });
            }

            result.Add(new SelectListItem()
            {
                Text = "MAIL",
                Value = "MAIL",
                Selected = ("MAIL" == send ? true : false)
            });
            result.Add(new SelectListItem()
            {
                Text = "FTP",
                Value = "FTP",
                Selected = ("FTP" == send ? true : false)
            });
            result.Add(new SelectListItem()
            {
                Text = "MAIL+FTP",
                Value = "MAIL+FTP",
                Selected = ("FTP" == send ? true : false)
            });


            return result;
        }

        /// <summary>
        /// 文件类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetFileType(string type = "XLS")
        {
            var result = new List<SelectListItem>();
            result.Add(new SelectListItem()
            {
                Text = "XLS",
                Value = "XLS",
                Selected = ("XLS" == type ? true : false)
            });
            result.Add(new SelectListItem()
            {
                Text = "CSV",
                Value = "CSV",
                Selected = ("CSV" == type ? true : false)
            });
            result.Add(new SelectListItem()
            {
                Text = "TXT",
                Value = "TXT",
                Selected = ("TXT" == type ? true : false)
            });

            return result;
        }

        /// <summary>
        /// 获得日期格式
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetDateFormat(string type = "yyyy/MM/dd")
        {
            var result = new List<SelectListItem>();
            result.Add(new SelectListItem()
            {
                Text = "yyyy/MM/dd",
                Value = "yyyy/MM/dd",
                Selected = ("yyyy/MM/dd" == type ? true : false)
            });

            return result;
        }

        /// <summary>
        /// 返回当前年
        /// </summary>
        /// <returns></returns>
        public static string GetCurYear()
        {
            return DateTime.Now.Year.ToString("0000");
        }

        /// <summary>
        /// 返回当前月份
        /// </summary>
        /// <returns></returns>
        public static string GetCurMonth()
        {
            return DateTime.Now.Month.ToString("00");
        }

        /// <summary>
        /// 返回当前日期
        /// </summary>
        /// <returns></returns>
        public static string GetCurDay()
        {
            return DateTime.Now.Day.ToString("00");
        }
    }
}
