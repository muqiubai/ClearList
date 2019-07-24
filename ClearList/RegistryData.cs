using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearList
{
    /// <summary>
    /// 获取注册表信息
    /// </summary>
    class RegistryData
    {
        /// <summary>
        /// 获取Unity项目启动界面中的项目历史记录
        /// </summary>
        /// <param name="_registryPath">注册表路径</param>
        /// <param name="_field">文件名所包含的字段</param>
        /// <param name="errMessage">返回错误信息</param>
        /// <returns></returns>
        public static Dictionary<string,string> GetUnityProjectList(string _registryPath, string _field, ref string _errMessage)
        {
            //键值对：  键：表项名称      值：数值数据
            Dictionary<string, string> projectLists = new Dictionary<string, string>();

            try
            {
                //定义需要访问的注册表节点
                RegistryKey rootKey = Registry.CurrentUser;

                RegistryKey subKey = null;

                //判断一下系统禁止访问的情况
                if (rootKey != null)
                {
                    //打开获取对应路径下的子项
                    subKey = rootKey.OpenSubKey(_registryPath, true);

                    string[] list = subKey.GetValueNames();

                    for (int i = 0; i < list.Length; i++)
                    {
                        if (list[i].ToLower().Contains(_field.ToLower()))
                        {
                            if (!projectLists.ContainsKey(list[i]))
                            {
                                //获取相关路径
                                byte[] value = (byte[])subKey.GetValue(list[i]);

                                string path = Encoding.UTF8.GetString(value);

                                projectLists.Add(list[i], path);
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                subKey.Close();
                rootKey.Close();
            }
            catch(Exception ex)
            {
                _errMessage = "错误信息：" + ex.ToString();
            }

            return projectLists;
        }

        /// <summary>
        /// 删除注册表中指定的注册表项
        /// </summary>
        /// <param name="_registryPath"></param>
        /// <param name="_valueName">批量删除</param>
        public static void DeleteRegist(string _registryPath, string[] _valueName, ref string _errMessage)
        {
            //获取指定类型
            using (RegistryKey userKey = Registry.CurrentUser)
            {
                //获取相关注册表表项
                RegistryKey subKey = userKey.OpenSubKey(_registryPath, true);
                //获取键名称数组
                string[] valueNames = subKey.GetValueNames();

                foreach (string item in valueNames)
                {
                    for (int i = 0; i < _valueName.Length; i++)
                    {
                        if (item.Contains(_valueName[i]))
                        {
                            try
                            {
                                subKey.DeleteValue(item);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }

                //int i = 0;

                //while (i <= _valueName.Length)
                //{

                //    subKey.DeleteValue(_valueName[i]);

                //    i++;
                //}

                subKey.Close();
            }
        }
    }
}
