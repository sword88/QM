using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using log4net;

namespace QM.Demo.Quituser
{
    public class ZKSHandler
    {
        private readonly static ILog log = LogManager.GetLogger(typeof(ZKSHandler));

        private zkemkeeper.CZKEM axCZKEM1 = new zkemkeeper.CZKEM();
        private ResultSet _resultset = new ResultSet();

        private int iMachineNumber, ErrorCode, iPrivilege;
        private bool bEnabled;
        private string sdwEnrollNumber, sName, sPassword, sCardnumber, ErrorMessage;

        private static ZKSHandler _instance;
        private ZKSHandler()
        {
            iMachineNumber = 1;
            iPrivilege = 0;
            ErrorCode = 0;
            bEnabled = false;
            sdwEnrollNumber = "";
            sName = "";
            sPassword = "";
            sCardnumber = "";
        }

        /// <summary>
        /// 设备类型
        /// </summary>
        /// <param name="sDeviceIP"></param>
        /// <returns></returns>
        private string getDeviceType(string sDeviceIP)
        {
            try
            {
                object deviceType = Common.GetDeviceType(sDeviceIP);
                if (deviceType != null)
                    return deviceType.ToString();
            }
            catch { }

            return "";
        }

        public static ZKSHandler getInstance()
        {
            if (_instance == null)
                _instance = new ZKSHandler();

            return _instance;
        }

        /// <summary>
        /// 连接设备 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private bool Connect(string ip)
        {
            int i = 0;
            bool result = true;
            try
            {
                log.Info(string.Format("Try connection to ZK Device ({0})", ip));

                log.Info(string.Format("ZKSoftware API :  axCZKEM1.Connect_Net=>1"));
                if (!axCZKEM1.Connect_Net(ip, 4370))
                {
                    for(i=2;i< 4;i++)
                    {
                        log.Info(string.Format("ZKSoftware API :  axCZKEM1.Connect_Net=>{0}",i));
                        if (!axCZKEM1.Connect_Net(ip, 4370))
                        {
                            result = false;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if(!result)
                    {
                        axCZKEM1.GetLastError(ref ErrorCode);
                        ErrorMessage = "Unable to connect the device,ErrorCode= " + ErrorCode;
                        log.Warn(ErrorMessage);
                    }
                }

                if(result)
                {
                    log.Info(string.Format("ZKSoftware API :  axCZKEM1.EnableDevice: false"));
                    axCZKEM1.EnableDevice(iMachineNumber, false);
                    log.Info(string.Format("ZK Device ({0}) is Connected", ip));
                }
            }
            catch (Exception ex)
            {
                log.Fatal(ex);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns></returns>
        private bool Close()
        {
            try
            {
                log.Info(string.Format("Try to disconnect from the ZK Device"));

                log.Info(string.Format("ZKSoftware API :  axCZKEM1.EnableDevice : true"));
                axCZKEM1.EnableDevice(iMachineNumber, true);  //enable the device 

                log.Info(string.Format("ZKSoftware API :  axCZKEM1.Disconnect"));
                axCZKEM1.Disconnect();

                return true;
            }
            catch (Exception ex)
            {
                log.Fatal(ex);
                return false;
            }
        }

        #region 删除用户部分

        /// <summary>
        /// 删除用户从所有的设备中（必须中数据库在存在）
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ResultSet QuitUser(UserInfo[] user)
        {
            bool resultAll = true;
            string messageAll = "";
            for (int i = 0; i < user.Count(); i++)
            {
                UserInfo oUser = user[i];
                if (oUser != null)
                {
                    bool sresult = true;
                    IList<string> deviceList = Common.GetGateListByUser(oUser.Id);

                    for (int x = 0; x < deviceList.Count(); x++)
                    {
                        UserInfo[] deleteUser = new UserInfo[1] { oUser };
                        ResultSet result = DeleteUser(deviceList[x], 1, deleteUser);

                        if (result.SeccessFlag == false)
                        {
                            resultAll = false;
                            sresult = false;
                            messageAll += string.Format("IP:{0},ERROR:{1}\r\n", deviceList[x], result.Description);
                        }
                    }

                    if (sresult)
                    {
                        Common.UpdateUserStatus(oUser.Id);
                    } 
                }
            }


            _resultset = new ResultSet
            {
                SeccessFlag = resultAll,
                Result = "Delete Complete",
                Description = messageAll
            };

            return _resultset;

        }

        /// <summary>
        /// 删除人员by门禁
        /// </summary>
        /// <param name="device_ip"></param>
        /// <returns></returns>
        public ResultSet QuitUserByDvc(string device_ip)
        {
            bool resultAll = true;
            string messageAll = "";
            
            IList<string> userlist = Common.GetUserByGate(device_ip);
            UserInfo[] user = new UserInfo[userlist.Count()];
            for (int x = 0; x < userlist.Count(); x++)
            {
                UserInfo u = new UserInfo();
                u.Id = userlist[x].ToString();
                user[x] = u;
            }
            ResultSet result = DeleteUser(device_ip, 1, user);


            if (result.SeccessFlag == false)
            {
                resultAll = false;
                messageAll += string.Format("IP:{0},ERROR:{1}\r\n", device_ip, result.Description);
            }


            _resultset = new ResultSet
            {
                SeccessFlag = resultAll,
                Result = "Delete Complete",
                Description = messageAll
            };

            return _resultset;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="device_ip"></param>
        /// <param name="device_port"></param>
        /// <param name="userInfos"></param>
        /// <returns></returns>
        public ResultSet DeleteUser(string device_ip, int device_port, UserInfo[] userInfos)
        {
            try
            {
                log.Info(string.Format("Function : DeleteUser Start, Array Size: {0}", userInfos.Length));
                int successCount = 0;
                int failCount = 0;
                int deviceCount = 0;

                int backupnumber = 12;
                int idwErrorCode = 0;

                deviceCount++;
                if (Connect(device_ip))
                {
                    //log.Info(string.Format(" Looping ---- Device {0}, user count: {1}", deviceCount, u.user.Count));
                    int userCount = 0;
                    foreach (UserInfo user in userInfos)
                    {
                        if (user != null)
                        {
                            userCount++;
                            log.Info(string.Format(" Looping -------User {0}", userCount, user.ToLogString()));

                            if (getDeviceType(device_ip) == "accesscontrol")
                            {
                                object oEmpNo = Common.GetRefnoByUser(user.Id);

                                if (oEmpNo != null)
                                {
                                    log.Info(string.Format("ZKSoftware API : axCZKEM1.DeleteEnrollData, Id{0}", user.Id));
                                    if (axCZKEM1.DeleteEnrollData(iMachineNumber, int.Parse(oEmpNo.ToString()), iMachineNumber, backupnumber))
                                    {
                                        Common.DelDevice(device_ip, user.Id.ToString());                                                        
                                        successCount++;
                                    }
                                    else
                                    {
                                        failCount++;
                                        axCZKEM1.GetLastError(ref idwErrorCode);
                                        if (idwErrorCode == 0)
                                        {
                                            Common.DelDevice(device_ip, user.Id.ToString());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                object oEmpNo = Common.GetRefnoByUser(user.Id);

                                if (oEmpNo != null)
                                {
                                    log.Info(string.Format("ZKSoftware API : axCZKEM1.SSR_DeleteEnrollData, Id{0}", user.Id));
                                    if (axCZKEM1.SSR_DeleteEnrollData(iMachineNumber, oEmpNo.ToString(), backupnumber))
                                    {
                                        Common.DelDevice(device_ip, user.Id.ToString());
                                        successCount++;
                                    }
                                    else
                                    {
                                        failCount++;
                                        axCZKEM1.GetLastError(ref idwErrorCode);
                                        if (idwErrorCode == 0)
                                        {
                                            Common.DelDevice(device_ip, user.Id.ToString());
                                        }
                                    }
                                }
                            }                            
                        }                        
                    }

                    Close();

                    _resultset = new ResultSet
                    {
                        SeccessFlag = true,
                        Result = string.Format("Success : {0} Fail : {1}", successCount, failCount)
                    };
                }
                else
                {
                    _resultset = new ResultSet
                    {
                        SeccessFlag = false,
                        Result = device_ip,
                        Description = string.Format("can not connect to the device. please check it.")
                    };

                }

            }
            catch (Exception ex)
            {
                log.Fatal(ex);
                _resultset = new ResultSet
                {
                    SeccessFlag = false,
                    Result = ErrorCode.ToString(),
                    Description = string.Format("Try Catched Error {0}", ex.Message)
                };
            }
            finally
            {
                Close();
                log.Info(string.Format("Function : DeleteUser finish"));
            }

            return _resultset;
        }

        #endregion

        #region 其他功能

        /// <summary>
        /// 更新设备信息
        /// </summary>
        public void UpdateAcsDevice()
        {
            IList<string> list = Common.GetDeviceList();

            for (int i = 0; i < list.Count(); i++)
            {
                GetAllUser(list[i], 1);
                GetDeviceStatus(list[i], 1);
            }
        }

        /// <summary>
        /// 重启设备
        /// </summary>
        /// <param name="device_ip"></param>
        /// <returns></returns>
        public ResultSet RestartDevice(string device_ip)
        {
            ResultSet result = new ResultSet();
            int successCount = 0;
            int errorCount = 0;
            try
            {
                if (Connect(device_ip))
                {
                    if (axCZKEM1.RestartDevice(iMachineNumber))
                    {
                        log.Debug("restart device [" + device_ip + "] is ok.");
                        successCount++;
                    }
                    else
                    {
                        log.Debug("restart device [" + device_ip + "] is fail.");
                        errorCount++;
                    }
                    Close();
                }
            }
            catch (Exception ex)
            {

                log.Fatal(ex);
                _resultset = new ResultSet
                {
                    SeccessFlag = false,
                    Result = ErrorCode.ToString(),
                    Description = string.Format("Try Catched Error {0}", ex.Message)
                };
            }

            return new ResultSet
            {
                SeccessFlag = true,
                Result = ErrorCode.ToString(),
                Description = string.Format("Success Count{0}, Error Count {1}", successCount.ToString(), errorCount.ToString())
            };
        }

        /// <summary>
        /// 重启所有考勤设备
        /// </summary>
        public void RestartAttDevice()
        {
            IList<string> ilist = Common.GetDeviceList("attendanceIn");

            for (int i = 0; i < ilist.Count(); i++)
            {
                RestartDevice(ilist[i]);
            }

            IList<string> olist = Common.GetDeviceList("attendanceOut");

            for (int i = 0; i < olist.Count(); i++)
            {
                RestartDevice(olist[i]);
            }
        }

        /// <summary>
        /// 更新设备中的用户
        /// </summary>
        /// <param name="device_ip"></param>
        /// <param name="device_port"></param>
        /// <returns></returns>
        public ResultSet GetAllUser(string device_ip, int device_port)
        {
            try
            {
                if (Connect(device_ip))
                {
                    log.Info(string.Format("Function : GetAllUser Start,{0}:{1}", device_ip, device_port));
                    Common.db.BeginTransaction();

                    int deleteEffectRow = 0;
                    Hashtable param = new Hashtable(){
                        {"device_ip",device_ip},
                        {"device_port",device_port}
                    };
                    deleteEffectRow = Common.DelDevice(device_ip);

                    int insertEffectRow = 0;
                    //read all the user information to the memory
                    log.Info(string.Format("ZKSoftware API :  axCZKEM1.ReadAllUserID"));
                    axCZKEM1.ReadAllUserID(iMachineNumber);


                    if (getDeviceType(device_ip) == "accesscontrol")
                    {
                        int dwEnrollNumber = 0;

                        log.Info(string.Format("ZKSoftware API :  axCZKEM1.GetAllUserInfo[accesscontrol]"));
                        while (axCZKEM1.GetAllUserInfo(iMachineNumber
                            , ref dwEnrollNumber
                            , ref sName
                            , ref sPassword
                            , ref iPrivilege
                            , ref bEnabled))    //get user information from memory
                        {              
                            log.Info("ref_empno:" + dwEnrollNumber);

                            var empno = Common.GetUserByRefno(dwEnrollNumber.ToString());

                            if (empno == null)
                            {
                                log.Info(string.Format("ZKSoftware API :  axCZKEM1.GetStrCardNumber"));
                                //get the card number from the memory
                                if (axCZKEM1.GetStrCardNumber(out sCardnumber))
                                {
                                    object empno1 = Common.GetUserByTagid(sCardnumber);

                                    if (empno1 != null)
                                    {
                                        sdwEnrollNumber = empno1.ToString();
                                    }
                                    else
                                    {
                                        sdwEnrollNumber = "NA_" + sCardnumber;
                                    }
                                    log.Info(string.Format("{0},{1},{2}", sdwEnrollNumber, sCardnumber, iPrivilege));
                                    try
                                    {
                                        DeviceTagRegData tagdata = new DeviceTagRegData();
                                        tagdata.device_ip = device_ip;
                                        tagdata.device_port = 1;
                                        tagdata.empno = sdwEnrollNumber;
                                        tagdata.tagid = sCardnumber;
                                        tagdata.reg_date = DateTime.Now;
                                        Common.InsertTagReg(tagdata);
                                        log.Info(string.Format("{0},{1},{2},{3},{4}", device_ip, device_port, DateTime.Now, sdwEnrollNumber, sCardnumber));
                                        insertEffectRow++;
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error(ex);
                                    }
                                }
                            }
                            else
                            {
                                try
                                {
                                    DeviceTagRegData tagdata = new DeviceTagRegData();
                                    tagdata.device_ip = device_ip;
                                    tagdata.device_port = 1;
                                    tagdata.empno = empno.ToString();
                                    tagdata.tagid = "";
                                    tagdata.reg_date = DateTime.Now;
                                    Common.InsertTagReg(tagdata);
                                    log.Info(string.Format("{0},{1},{2},{3},{4}", device_ip, device_port, DateTime.Now, sdwEnrollNumber, ""));
                                    insertEffectRow++;
                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex);
                                }
                            }
                        }
                    }
                    else
                    {
                        string sEnrollNumber;

                        log.Info(string.Format("ZKSoftware API :  axCZKEM1.SSR_GetAllUserInfo"));
                        while (axCZKEM1.SSR_GetAllUserInfo(
                                            iMachineNumber,
                                            out sEnrollNumber,
                                            out sName,
                                            out sPassword,
                                            out iPrivilege,
                                            out bEnabled)) //get user information from memory
                        {
                            log.Info(string.Format("ZKSoftware API :  axCZKEM1.GetStrCardNumber"));
                            log.Info(string.Format("{0},{1},{2}", sEnrollNumber, sName, iPrivilege));

                            var empno = Common.GetUserByRefno(sEnrollNumber.ToString());

                            if (empno == null)
                            {
                                if (axCZKEM1.GetStrCardNumber(out sCardnumber))    //get the card number from the memory
                                {
                                    object empno1 = Common.GetUserByTagid(sCardnumber);

                                    if (empno1 != null)
                                    {
                                        sdwEnrollNumber = empno1.ToString();
                                    }
                                    else
                                    {
                                        sdwEnrollNumber = "NA_" + sCardnumber;
                                    }

                                    try
                                    {
                                        DeviceTagRegData tagdata = new DeviceTagRegData();
                                        tagdata.device_ip = device_ip;
                                        tagdata.device_port = device_port;
                                        tagdata.empno = sdwEnrollNumber;
                                        tagdata.tagid = sCardnumber;
                                        tagdata.reg_date = DateTime.Now;
                                        Common.InsertTagReg(tagdata);

                                        insertEffectRow++;
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error(ex);
                                    }

                                }
                            }
                            else
                            {
                                try
                                {
                                    DeviceTagRegData tagdata = new DeviceTagRegData();
                                    tagdata.device_ip = device_ip;
                                    tagdata.device_port = 1;
                                    tagdata.empno = empno.ToString();
                                    tagdata.tagid = "";
                                    tagdata.reg_date = DateTime.Now;
                                    Common.InsertTagReg(tagdata);
                                    log.Info(string.Format("{0},{1},{2},{3},{4}", device_ip, device_port, DateTime.Now, sdwEnrollNumber, ""));
                                    insertEffectRow++;
                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex);
                                }
                            }                               

                        }

                    }
                    Common.db.Commit();

                    _resultset = new ResultSet
                    {
                        SeccessFlag = true,
                        Result = string.Format("Insert : {0} delete : {1}", insertEffectRow, deleteEffectRow)
                    };

                }
                else
                {
                    _resultset = new ResultSet
                    {
                        SeccessFlag = false,
                        Result = device_ip,
                        Description = string.Format("can not connect to the device. please check it.")
                    };

                }
                
            }
            catch (Exception ex)
            {
                Common.db.Rollback();
                log.Fatal(ex);
                _resultset = new ResultSet
                {
                    SeccessFlag = false,
                    Result = ErrorCode.ToString(),
                    Description = string.Format("Try Catched Error {0}", ex.Message)
                };
            }
            finally
            {
                Close();
                log.Info(string.Format("Function : GetAllUser finish ,{0}", _resultset.Result));
            }

            return _resultset;
        }

        /// <summary>
        /// 清除所有记录(考勤/门禁/用餐)
        /// </summary>
        /// <param name="device_ip"></param>
        /// <returns></returns>
        public ResultSet ClearEventLog(string device_ip)
        {
            try
            {
                log.Info(string.Format("Function : ClearEventLog Start, device_ip {0}", device_ip));

                if (Connect(device_ip))
                {
                    log.Info(string.Format("ZKSoftware API : axCZKEM1.ClearGLog"));
                    if (axCZKEM1.ClearGLog(iMachineNumber))
                    {
                        //the data in the device should be refreshed
                        log.Info(string.Format("ZKSoftware API : axCZKEM1.RefreshData"));
                        axCZKEM1.RefreshData(iMachineNumber);

                        _resultset = new ResultSet
                        {
                            SeccessFlag = true,
                            Result = ErrorCode.ToString(),
                            Description = string.Format("All att Logs have been cleared from teiminal! Success")
                        };
                    }
                    else
                    {
                        axCZKEM1.GetLastError(ref ErrorCode);
                        _resultset = new ResultSet
                        {
                            SeccessFlag = false,
                            Result = ErrorCode.ToString(),
                            Description = string.Format("Operation failed,ErrorCode: {0} Error", ErrorCode.ToString())
                        };
                    }
                }
                else
                {
                    _resultset = new ResultSet
                    {
                        SeccessFlag = false,
                        Result = device_ip,
                        Description = string.Format("Can not connect to the device. please check it.")
                    };

                }
            }
            catch (Exception ex)
            {
                log.Fatal(ex);
                _resultset = new ResultSet
                {
                    SeccessFlag = false,
                    Result = ErrorCode.ToString(),
                    Description = string.Format("Try Catched Error {0}", ex.Message)
                };
            }
            finally
            {
                Close();
                log.Info(string.Format("Function : ClearEventLog finish"));
            }

            return _resultset;
        }

        /// <summary>
        /// 更新设备状态
        /// </summary>
        /// <param name="device_ip"></param>
        /// <param name="device_port"></param>
        /// <returns></returns>
        public ResultSet GetDeviceStatus(string device_ip, int device_port)
        {
            try
            {
                log.Info(string.Format("Function : GetDeviceStatus Start ,{0}:{1}", device_ip, device_port));
                if (Connect(device_ip))
                {
                    int idwValue = 0;
                    string NumberOfRegisteredUser = "NA";
                    string NumberOfAttendanceRecords = "NA";
                    string UserCapacity = "NA";
                    string ResidualAttendanceRecordCapacity = "NA";
                    //Number of registered users
                    log.Info(string.Format("ZKSoftware API : axCZKEM1.GetDeviceStatus, type=2"));
                    if (axCZKEM1.GetDeviceStatus(iMachineNumber, 2, ref idwValue))
                    {
                        NumberOfRegisteredUser = idwValue.ToString();
                    }
                    //Number of attendance records
                    log.Info(string.Format("ZKSoftware API : axCZKEM1.GetDeviceStatus, type=6"));
                    if (axCZKEM1.GetDeviceStatus(iMachineNumber, 6, ref idwValue))
                    {
                        NumberOfAttendanceRecords = idwValue.ToString();
                    }
                    //User capacity
                    log.Info(string.Format("ZKSoftware API : axCZKEM1.GetDeviceStatus, type=8"));
                    if (axCZKEM1.GetDeviceStatus(iMachineNumber, 8, ref idwValue))
                    {
                        UserCapacity = idwValue.ToString();
                    }
                    //User capacity
                    log.Info(string.Format("ZKSoftware API : axCZKEM1.GetDeviceStatus, type=12"));
                    if (axCZKEM1.GetDeviceStatus(iMachineNumber, 12, ref idwValue))
                    {
                        ResidualAttendanceRecordCapacity = idwValue.ToString();
                    }

                    string device_status = string.Format("USER:{0}/{1}, EVENT LOG:{2}/{3}"
                        , NumberOfRegisteredUser
                        , UserCapacity
                        , NumberOfAttendanceRecords
                        , ResidualAttendanceRecordCapacity);

                    Common.UpdateDeviceConfig(device_ip, device_status);
                }
                else
                {
                    _resultset = new ResultSet
                    {
                        SeccessFlag = false,
                        Result = device_ip,
                        Description = string.Format("Can not connect to the device. please check it.")
                    };

                }
            }
            catch (Exception ex)
            {
                log.Fatal(ex);
                _resultset = new ResultSet
                {
                    SeccessFlag = false,
                    Result = ErrorCode.ToString(),
                    Description = string.Format("Try Catched Error {0}", ex.Message)
                };
            }
            finally
            {
                Close();
                log.Info(string.Format("Function : GetDeviceStatus finish"));
            }

            return _resultset;
        }

        /// <summary>
        /// 同步卡机时间
        /// </summary>
        /// <returns></returns>
        public ResultSet SyncDeviceTime()
        {
            ResultSet result = new ResultSet();
            int successCount = 0;
            int errorCount = 0;
            try
            {
                IList<string> providingList = Common.GetDeviceList();

                for (int i = 0; i < providingList.Count(); i++)
                {
                    if (Connect(providingList[i]))
                    {
                        if (axCZKEM1.SetDeviceTime(iMachineNumber))
                        {
                            log.Debug("sync device [" + providingList[i] + "] is ok.");
                            successCount++;
                        }
                        else
                        {
                            log.Debug("sync device [" + providingList[i] + "] is fail.");
                            errorCount++;
                        }
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {

                log.Fatal(ex);
                _resultset = new ResultSet
                {
                    SeccessFlag = false,
                    Result = ErrorCode.ToString(),
                    Description = string.Format("Try Catched Error {0}", ex.Message)
                };
            }

            return new ResultSet
            {
                SeccessFlag = true,
                Result = ErrorCode.ToString(),
                Description = string.Format("Success Count{0}, Error Count {1}", successCount.ToString(), errorCount.ToString())
            };
        }

        #endregion

    }
}
