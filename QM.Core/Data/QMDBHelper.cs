using System;
using System.Data;
using System.Collections;
using System.Data.OleDb;
using QM.Core.Exception;
using System.Diagnostics;

namespace QM.Core.Data
{
    public class QMDBHelper
    {
        private string DbConStr { get; set; }

        public QMDBHelper(string con)
        {
            DbConStr = con;
        }

        #region private utility methods & constructors
        /// <summary>
        /// This method is used to attach array of OleDbParameters to a OleDbCommand.
        /// 
        /// This method will assign a value of DbNull to any parameter with a direction of
        /// InputOutput and a value of null.  
        /// 
        /// This behavior will prevent default values from being used, but
        /// this will be the less common case than an intended pure output parameter (derived as InputOutput)
        /// where the user provided no input value.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">an array of OleDbParameters tho be added to command</param>
        private static void AttachParameters(OleDbCommand command, OleDbParameter[] commandParameters)
        {
            foreach (OleDbParameter p in commandParameters)
            {
                //check for derived output value with no value assigned
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }

                command.Parameters.Add(p);
            }
        }

        /// <summary>
        /// This method assigns an array of values to an array of OleDbParameters.
        /// </summary>
        /// <param name="commandParameters">array of OleDbParameters to be assigned values</param>
        /// <param name="parameterValues">array of objects holding the values to be assigned</param>
        private static void AssignParameterValues(OleDbParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                //do nothing if we get no data
                return;
            }

            // we must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            //iterate through the OleDbParameters, assigning the values from the corresponding position in the 
            //value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                commandParameters[i].Value = parameterValues[i];
            }
        }

        /// <summary>
        /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
        /// to the provided command.
        /// </summary>
        /// <param name="command">the OleDbCommand to be prepared</param>
        /// <param name="connection">a valid OleDbConnection, on which to execute this command</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the SQL command</param>
        /// <param name="commandParameters">an array of OleDbParameters to be associated with the command or 'null' if no parameters are required</param>
        private static void PrepareCommand(OleDbCommand command,
                                           OleDbConnection connection,
                                           CommandType commandType,
                                           string commandText,
                                           OleDbParameter[] commandParameters)
        {
            //if the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            //associate the connection with the command
            command.Connection = connection;

            //set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            //set the command type
            command.CommandType = commandType;

            //attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }

            Debug.Write(commandText);

            return;
        }

#endregion private utility methods & constructors

        #region ExecuteReader

        /// <summary>
        /// Execute a OleDbCommand (that returns a resultset) against the database specified.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OleDbDataReader dr = ExecuteReader(CommandType.Text, "SELECT * FROM FwLot WHERE appId = ?", new OleDbParameter("@appId", 40));
        /// </remarks>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a OleDbDataReader containing the resultset generated by the command</returns>
        public OleDbDataReader ExecuteReader(string commandText,
                                                    params OleDbParameter[] commandParameters)
        {
            //create OleDbConnection
            OleDbConnection cn = new OleDbConnection(DbConStr);
            //cn.Open();

            try
            {
                // create a command and prepare it for execution
                OleDbCommand cmd = new OleDbCommand();
                PrepareCommand(cmd, cn, CommandType.Text, commandText, commandParameters);
                
                // create a reader
                OleDbDataReader dr;
                // call ExecuteReader
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                // detach the OleDbParameters from the command object, so they can be used again.
                cmd.Parameters.Clear();

                return dr;
            }
            catch
            {
                //if we fail to return the OleDbDataReader, we need to close the connection ourselves
                if (cn.State != ConnectionState.Closed)
                    cn.Close();

                throw;
            }
        }

        /// <summary>
        /// Execute a OleDbCommand (that returns a resultset) against the database specified.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OleDbDataReader dr = ExecuteReader(CommandType.Text, "SELECT * FROM FwLot WHERE appId = ?", new OleDbParameter("@appId", 40));
        /// </remarks>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a OleDbDataReader containing the resultset generated by the command</returns>
        public OleDbDataReader ExecuteReader(string _connnectionstring, string commandText,
                                                    params OleDbParameter[] commandParameters)
        {
            //create OleDbConnection
            OleDbConnection cn = new OleDbConnection(_connnectionstring);
            //cn.Open();

            try
            {
                // create a command and prepare it for execution
                OleDbCommand cmd = new OleDbCommand();
                PrepareCommand(cmd, cn, CommandType.Text, commandText, commandParameters);

                // create a reader
                OleDbDataReader dr;
                // call ExecuteReader
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                // detach the OleDbParameters from the command object, so they can be used again.
                cmd.Parameters.Clear();

                return dr;
            }
            catch
            {
                //if we fail to return the OleDbDataReader, we need to close the connection ourselves
                if (cn.State != ConnectionState.Closed)
                    cn.Close();

                throw;
            }
        }

        /**//// <summary>
            /// 使用默认连接,CommandType默认为StoredProcedure
            /// </summary>
            /// <param name="cmdText">存储过程名</param>
            /// <param name="commandParameters">参数集</param>
            /// <returns>OleDbDataReader</returns>
        public OleDbDataReader ExecuteReader(string commandText, CommandType type, params OleDbParameter[] commandParameters)
        {
            //create OleDbConnection
            OleDbConnection cn = new OleDbConnection(DbConStr);
            //cn.Open();

            try
            {
                // create a command and prepare it for execution
                OleDbCommand cmd = new OleDbCommand();
                PrepareCommand(cmd, cn, type, commandText, commandParameters);

                // create a reader
                OleDbDataReader dr;
                // call ExecuteReader
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                // detach the OleDbParameters from the command object, so they can be used again.
                cmd.Parameters.Clear();

                return dr;
            }
            catch
            {
                //if we fail to return the OleDbDataReader, we need to close the connection ourselves
                if (cn.State != ConnectionState.Closed)
                    cn.Close();

                throw;
            }
        }

        /// <summary>
        /// Execute a OleDbCommand (that returns a resultset and takes no parameters).
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OleDbDataReader dr = ExecuteReader(CommandType.Text, "SELECT * FROM FwLot WHERE appId = 'LOT01'");
        /// </remarks>
        /// <param name="commandText">the stored procedure name or SQL command</param>
        /// <returns>a OleDbDataReader containing the resultset generated by the command</returns>
        public OleDbDataReader ExecuteReader(string commandText)
        {
            //pass through the call providing null for the set of OleDbParameters
            return ExecuteReader(commandText, (OleDbParameter[])null);
        }

        /// <summary>
        /// Execute a OleDbCommand (that returns a resultset) against the database specified.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OleDbDataReader dr = ExecuteReader(CommandType.Text, "SELECT * FROM FwLot WHERE appId = ?", new OleDbParameter("@appId", 40));
        /// </remarks>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a OleDbDataReader containing the resultset generated by the command</returns>
        public OleDbDataReader ExecuteReaderofBinary(string commandText,
                                                            params OleDbParameter[] commandParameters)
        {
            //create OleDbConnection
            OleDbConnection cn = new OleDbConnection(DbConStr);
            //cn.Open();

            try
            {
                // create a command and prepare it for execution
                OleDbCommand cmd = new OleDbCommand();
                PrepareCommand(cmd, cn, CommandType.Text, commandText, commandParameters);

                // create a reader
                OleDbDataReader dr;
                // call ExecuteReader
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                // detach the OleDbParameters from the command object, so they can be used again.
                cmd.Parameters.Clear();

                return dr;
            }
            catch
            {
                //if we fail to return the OleDbDataReader, we need to close the connection ourselves
                if (cn.State != ConnectionState.Closed)
                    cn.Close();
                throw;
            }
        }

        /// <summary>
        /// Execute a OleDbCommand (that returns a resultset and takes no parameters).
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OleDbDataReader dr = ExecuteReader(CommandType.Text, "SELECT * FROM FwLot WHERE appId = 'LOT01'");
        /// </remarks>
        /// <param name="commandText">the stored procedure name or SQL command</param>
        /// <returns>a OleDbDataReader containing the resultset generated by the command</returns>
        public OleDbDataReader ExecuteReaderofBinary(string commandText)
        {
            //pass through the call providing null for the set of OleDbParameters
            return ExecuteReaderofBinary(commandText, (OleDbParameter[])null);
        }

        #endregion ExecuteReader

        #region ExecuteScaler

        /// <summary>
        /// Execute a OleDbCommand (that returns a 1x1 resultset) against the specified OleDbConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar("SELECT COUNT(*) FROM FwLot WHERE processingStatus = ?, new OleDbParameter("@processingStatus", 40));
        /// </remarks>
        /// <param name="commandText">the SQL command</param>
        /// <param name="commandParameters">an array of OleDbParamters used to execute the command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public object ExecuteScalar(string commandText, params OleDbParameter[] commandParameters)
        {
            //create OleDbConnection
            OleDbConnection cn = new OleDbConnection(DbConStr);

            //create a command and prepare it for execution
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, cn, CommandType.Text, commandText, commandParameters);

            object retval = null;
            try
            {
                //execute the command & return the results
                retval = cmd.ExecuteScalar();
            }
            catch
            {
                ;
            }
            return retval;
        }

        /// <summary>
        /// Execute a OleDbCommand (that returns a 1x1 resultset and takes no parameters). 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar("SELECT COUNT(*) FROM FwLot WHERE processingStatus = 'Active'");
        /// </remarks>
        /// <param name="commandText">the SQL command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public object ExecuteScalar(string commandText)
        {
            //pass through the call providing null for the set of OleDbParameters
            return ExecuteScalar(commandText, (OleDbParameter[])null);
        }

        #endregion ExecuteScaler

        #region ExecuteDataSet

        /// <summary>
        /// Execute a OleDbCommand (that returns a resultset and takes no parameters).
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset("SELECT * FROM FwLot WHERE processingStatus = 'Active'");
        /// </remarks>
        /// <param name="commandText">SQL command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDataset(string commandText)
        {
            //pass through the call providing null for the set of OleDbParameters
            return ExecuteDataset(commandText, (OleDbParameter[])null);
        }

        /// <summary>
        /// Execute a OleDbCommand (that returns a resultset) using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset("SELECT * FROM FwLot WHERE appID = ?", new OleDbParameter("@lotId", 40));
        /// </remarks>
        /// <param name="commandText">the SQL command</param>
        /// <param name="commandParameters">an array of OleDbParamters used to execute the command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDataset(string commandText, params OleDbParameter[] commandParameters)
        {
            //create OleDbConnection
            OleDbConnection cn = new OleDbConnection(DbConStr);

            //create a command and prepare it for execution
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, cn, CommandType.Text, commandText, commandParameters);

            //create the DataAdapter & DataSet
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataSet ds = new DataSet();

            //fill the DataSet using default values for DataTable names, etc.
            da.Fill(ds);

            // close the connection.
            cn.Close();

            // detach the OleDbParameters from the command object, so they can be used again.			
            cmd.Parameters.Clear();

            //return the dataset
            return ds;
        }


        public DataSet ExecuteDataset(string _ConnectionString, string commandText)
        {
            //create OleDbConnection
            OleDbConnection cn = new OleDbConnection(_ConnectionString);

            //create a command and prepare it for execution
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, cn, CommandType.Text, commandText, (OleDbParameter[])null);

            //create the DataAdapter & DataSet
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataSet ds = new DataSet();

            //fill the DataSet using default values for DataTable names, etc.
            da.Fill(ds);

            // close the connection.
            cn.Close();

            // detach the OleDbParameters from the command object, so they can be used again.			
            cmd.Parameters.Clear();

            //return the dataset
            return ds;
        }

        /// <summary>
        /// Execute a OleDbCommand (that returns a resultset and takes no parameters).
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset("SELECT * FROM FwLot WHERE processingStatus = 'Active'");
        /// </remarks>
        /// <param name="commandText">SQL command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDatasetofBinary(string commandText)
        {
            //pass through the call providing null for the set of OleDbParameters
            return ExecuteDatasetofBinary(commandText, (OleDbParameter[])null);
        }

        /// <summary>
        /// Execute a OleDbCommand (that returns a resultset) using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset("SELECT * FROM FwLot WHERE appID = ?", new OleDbParameter("@lotId", 40));
        /// </remarks>
        /// <param name="commandText">the SQL command</param>
        /// <param name="commandParameters">an array of OleDbParamters used to execute the command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDatasetofBinary(string commandText, params OleDbParameter[] commandParameters)
        {
            //create OleDbConnection
            OleDbConnection cn = new OleDbConnection(DbConStr);

            //create a command and prepare it for execution
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, cn, CommandType.Text, commandText, commandParameters);

            //create the DataAdapter & DataSet
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataSet ds = new DataSet();

            //fill the DataSet using default values for DataTable names, etc.
            da.Fill(ds);

            // close the connection.
            cn.Close();

            // detach the OleDbParameters from the command object, so they can be used again.			
            cmd.Parameters.Clear();

            //return the dataset
            return ds;
        }

        #endregion ExecuteDataSet

        #region ExecuteNonQuery


        // 저장 프로시져 호출 하기 위한 ExecuteNonQuery 추가 by ythong
        public int ExecuteNonQuery(string commandText, CommandType commandType, OleDbCommand cmd, params OleDbParameter[] commandParameters)
        {
            //create OleDbConnection
            OleDbConnection cn = new OleDbConnection(DbConStr);

            //create a command and prepare it for execution
            PrepareCommand(cmd, cn, commandType, commandText, commandParameters);

            int retval = -1;
            try
            {
                //execute the command & return the results
                retval = cmd.ExecuteNonQuery();
            }
            catch
            {
                // error 처리 해줘야 함

                throw;
            }

            return retval;
        }


        public int ExecuteNonQuery(string _connectionString, string commandText, params OleDbParameter[] commandParameters)
        {
            //create OleDbConnection
            OleDbConnection cn = new OleDbConnection(DbConStr);
            OleDbCommand cmd = new OleDbCommand();
            //create a command and prepare it for execution
            PrepareCommand(cmd, cn, CommandType.Text, commandText, commandParameters);

            int retval = -1;
            try
            {
                //execute the command & return the results
                retval = cmd.ExecuteNonQuery();
            }
            catch
            {
                // error 

                throw;
            }

            return retval;
        }

        public int ExecuteNonQuery(string commandText, CommandType commandType, params OleDbParameter[] commandParameters)
        {
            //create OleDbConnection
            OleDbConnection cn = new OleDbConnection(DbConStr);

            //create a command and prepare it for execution
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, cn, commandType, commandText, commandParameters);

            int retval = -1;
            try
            {
                //execute the command & return the results
                retval = cmd.ExecuteNonQuery();
            }
            catch
            {
                ;
            }
            return retval;
        }

        public int ExecuteNonQuery(OleDbConnection cn, string commandText, CommandType commandType, params OleDbParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, cn, commandType, commandText, commandParameters);

            int retval = -1;
            try
            {
                //execute the command & return the results
                retval = cmd.ExecuteNonQuery();
            }
            catch
            {
                ;
            }
            return retval;
        }

        public int ExecuteNonQuery(string commandText, params OleDbParameter[] commandParameters)
        {
            //create OleDbConnection
            OleDbConnection cn = new OleDbConnection(DbConStr);

            //create a command and prepare it for execution
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, cn, CommandType.Text, commandText, commandParameters);

            int retval = -1;
            try
            {
                //execute the command & return the results
                retval = cmd.ExecuteNonQuery();
            }
            catch(QMException ex)
            {
                throw ex;
            }
            return retval;
        }

        public int ExecuteNonQuery(string commandText)
        {
            //pass through the call providing null for the set of OleDbParameters
            return ExecuteNonQuery(commandText, (OleDbParameter[])null);
        }

        #endregion ExecuteNonQuery
    }
}
