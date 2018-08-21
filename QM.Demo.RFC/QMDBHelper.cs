using System;
using System.Data;
using System.Collections;
using System.Data.OleDb;
using System.Diagnostics;
using Oracle.ManagedDataAccess.Client;

namespace QM.Demo.RFC
{
    public class QMDBHelper
    {
        private string DbConStr { get; set; }
        public OracleTransaction p_trans = null;
        public OracleConnection p_con = null;
        private OracleCommand p_cmd = null;

        public QMDBHelper(string con)
        {
            DbConStr = con;
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        public void BeginTransaction()
        {
            p_con = new OracleConnection(DbConStr);
            p_con.Open();
            p_cmd = p_con.CreateCommand();
            p_trans = p_con.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            p_trans.Commit();
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void Rollback()
        {
            p_trans.Rollback();
        }

        /// <summary>
        /// 增删改操作使用此方法
        /// </summary>
        /// <param name="cmdText">要执行的sql语句或者存储过程名称</param>
        /// <param name="cmdType">命令类型（sql语句或者存储过程）</param>
        /// <param name="commandParameters">执行所需的一些参数</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteNonQuery(string cmdText, CommandType cmdType, params OracleParameter[] commandParameters)
        {
            // 创建一个OracleCommand
            OracleCommand cmd = new OracleCommand();
            //创建一个OracleConnection
            using (OracleConnection connection = new OracleConnection(DbConStr))
            {
                //调用静态方法PrepareCommand完成赋值操作
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                //执行命令返回
                int val = cmd.ExecuteNonQuery();
                //清空参数
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 增删改操作使用此方法（需要一个存在的事务参数）
        /// </summary>
        /// <param name="trans">一个存在的事务</param>
        /// <param name="commandType">命令类型（sql或者存储过程）</param>
        /// <param name="commandText">sql语句或者存储过程名称</param>
        /// <param name="commandParameters">命令所需参数数组</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteNonQuery(OracleTransaction trans, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            //调用静态方法PrepareCommand完成赋值操作
            PrepareCommand(p_cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            //执行命令返回
            int val = p_cmd.ExecuteNonQuery();
            //清空参数
            p_cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 增删改操作使用此方法（需要一个存在的连接）
        /// </summary> 
        /// <param name="conn">一个存在的OracleConnection参数</param>
        /// <param name="commandType">命令类型（sql或者存储过程）</param>
        /// <param name="commandText">sql语句或者存储过程名称</param>
        /// <param name="commandParameters">命令所需参数数组</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteNonQuery(OracleConnection connection, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            // 创建一个OracleCommand
            OracleCommand cmd = new OracleCommand();
            //调用静态方法PrepareCommand完成赋值操作
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            //执行命令返回
            int val = cmd.ExecuteNonQuery();
            //清空参数
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 查询返回一个结果集
        /// </summary>
        /// <param name="connString">连接字符串</param>
        //// <param name="commandType">命令类型（sql或者存储过程）</param>
        /// <param name="commandText">sql语句或者存储过程名称</param>
        /// <param name="commandParameters">命令所需参数数组</param>
        /// <returns></returns>
        public OracleDataReader ExecuteReader( CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {

            // 创建一个OracleCommand
            OracleCommand cmd = new OracleCommand();
            // 创建一个OracleConnection
            OracleConnection conn = new OracleConnection(DbConStr);
            try
            {
                //调用静态方法PrepareCommand完成赋值操作
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                //执行查询
                OracleDataReader odr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                //清空参数
                cmd.Parameters.Clear();
                return odr;

            }
            catch
            {
                //如果发生异常，关闭连接，并且向上抛出异常
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// 查询返回一个DataSet
        /// </summary>
        /// <param name="cmdText">sql语句</param>
        /// <param name="commandParameters">命令所需参数数组</param>
        /// <returns></returns>
        public DataSet ExecuteDataset(string cmdText, params OracleParameter[] commandParameters)
        {

            // 创建一个OracleCommand
            OracleCommand cmd = new OracleCommand();
            // 创建一个OracleConnection
            using (OracleConnection conn = new OracleConnection(DbConStr))
            { 
                try
                {
                    //调用静态方法PrepareCommand完成赋值操作
                    PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, commandParameters);
                    //执行查询
                    OracleDataAdapter oda = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    //填充dataset的值
                    oda.Fill(ds);
                    //连接关闭
                    conn.Close();
                    //清空参数
                    cmd.Parameters.Clear();

                    //输出datatable内容
                    //QMDebug debug = new QMDebug();
                    //debug.DebugTable(ds.Tables[0]);

                    return ds;
                }
                catch (Exception ex)
                {
                    //如果发生异常，关闭连接，并且向上抛出异常
                    conn.Close();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行语句返回的是单行单列的结果 
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="commandType">命令类型（sql或者存储过程）</param>
        /// <param name="commandText">sql语句或者存储过程名称</param>
        /// <param name="commandParameters">命令所需参数数组</param>
        /// <returns>返回是第一行第一列的结果（object类型）请使用Covert.to进行类型转换</returns>
        public object ExecuteScalar(CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            // 创建一个OracleCommand
            OracleCommand cmd = new OracleCommand();
            // 创建一个OracleConnection
            using (OracleConnection conn = new OracleConnection(DbConStr))
            {
                //调用静态方法PrepareCommand完成赋值操作
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                //执行查询
                object val = cmd.ExecuteScalar();
                //清空参数
                cmd.Parameters.Clear();
                return val;
            }
        }

        ///    <summary>
        ///    执行语句返回的是单行单列的结果（有指定的事务参数）
        ///    </summary>
        ///    <param name="transaction">一个存在的事务参数</param>
        ///    <param name="commandType">命令类型（sql或者存储过程）</param>
        ///    <param name="commandText">sql语句或者存储过程名称</param>
        ///    <param name="commandParameters">命令所需参数数组</param>
        ///    <returns>返回是第一行第一列的结果（object类型）请使用Covert.to进行类型转换</returns>
        public static object ExecuteScalar(OracleTransaction transaction, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            //如果传入的事务是空值，抛出异常
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            //如果传入的事务无连接，抛出异常（无连接，说明传入的事务参数是已经提交过或者回滚了的事务）
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked    or commited, please    provide    an open    transaction.", "transaction");
            // 创建一个OracleCommand
            OracleCommand cmd = new OracleCommand();
            //调用静态方法PrepareCommand完成赋值操作
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
            //执行查询
            object retval = cmd.ExecuteScalar();
            //清空参数
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        ///   执行语句返回的是单行单列的结果（有指定的连接参数）
        /// </summary>
        /// <param name="conn">一个存在的连接参数</param>
        /// <param name="commandType">命令类型（sql或者存储过程）</param>
        /// <param name="commandText">sql语句或者存储过程名称</param>
        /// <param name="commandParameters">命令所需参数数组</param>
        /// <returns>返回是第一行第一列的结果（object类型）请使用Covert.to进行类型转换</returns>
        public static object ExecuteScalar(OracleConnection connectionString, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            // 创建一个OracleCommand
            OracleCommand cmd = new OracleCommand();
            //调用静态方法PrepareCommand完成赋值操作
            PrepareCommand(cmd, connectionString, null, cmdType, cmdText, commandParameters);
            //执行查询
            object val = cmd.ExecuteScalar();
            //清空参数
            cmd.Parameters.Clear();
            return val;
        }


        /// <summary>
        /// 一个静态的预处理函数
        /// </summary>
        /// <param name="cmd">存在的OracleCommand对象</param>
        /// <param name="conn">存在的OracleConnection对象</param>
        /// <param name="trans">存在的OracleTransaction对象</param>
        /// <param name="cmdType">命令类型（sql或者存在过程）</param>
        /// <param name="cmdText">sql语句或者存储过程名称</param>
        /// <param name="commandParameters">Parameters for the command</param>
        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType cmdType, string cmdText, OracleParameter[] commandParameters)
        {

            //如果连接未打开，先打开连接
            if (conn.State != ConnectionState.Open)
                conn.Open();

            //未要执行的命令设置参数
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;

            //如果传入了事务，需要将命令绑定到指定的事务上去
            if (trans != null)
                cmd.Transaction = trans;

            //将传入的参数信息赋值给命令参数
            if (commandParameters != null)
            {
                cmd.Parameters.AddRange(commandParameters);
            }

            //输出sql到output窗口
            Debug.Write(cmd.CommandText);
        }

        public void Disponse()
        {
            OracleConnection conn = new OracleConnection(DbConStr);
            //如果连接未打开，先打开连接
            if (conn.State != ConnectionState.Closed)
                conn.Close();
        }
    }
}
