using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections;
using System.Reflection;

namespace QM.Core.Common
{
    public static class QMExtend
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

        /// <summary>    
        /// 将集合类转换成DataTable    
        /// </summary>    
        /// <param name="list">集合</param>    
        /// <returns></returns>    
        public static DataTable ToDataTableTow(IList list)
        {
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();

                foreach (PropertyInfo pi in propertys)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }
                foreach (object t in list)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(t, null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        /// <summary>
        /// Convert a List{T} to a DataTable.    
        /// </summary>
        public static DataTable ToDataTable<T>(IList<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }

            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                tb.Rows.Add(values);            
            }

            return tb;
        }

        /// <summary>
        /// Determine of specified type is nullable
        /// </summary>
        public static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }


        /// <summary>
        /// Return underlying type if type is Nullable otherwise return the type
        /// </summary>
        public static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {            
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }            
            }
            else        
            {
                return t;
            }
        }

        /// <summary>    
        /// DataTable 转换为List 集合    
        /// </summary>    
        /// <typeparam name="TResult">类型</typeparam>    
        /// <param name="dt">DataTable</param>    
        /// <returns></returns>    
        public static List<T> ToList<T>(this DataTable dt) where T : class, new()
        {
            //创建一个属性的列表    
            List<PropertyInfo> prlist = new List<PropertyInfo>();
            //获取TResult的类型实例  反射的入口    

            Type t = typeof(T);

            //获得TResult 的所有的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表     
            Array.ForEach<PropertyInfo>(t.GetProperties(), p => { if (dt.Columns.IndexOf(p.Name) != -1) prlist.Add(p); });

            //创建返回的集合    

            List<T> oblist = new List<T>();

            foreach (DataRow row in dt.Rows)
            {
                //创建TResult的实例    
                T ob = new T();
                //找到对应的数据  并赋值    
                prlist.ForEach(p => { if (row[p.Name] != DBNull.Value) p.SetValue(ob, row[p.Name], null); });
                //放入到返回的集合中.    
                oblist.Add(ob);
            }
            return oblist;
        }
    }
}
