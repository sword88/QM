using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBatisNet.Common;
using IBatisNet.Common.Utilities;
using IBatisNet.Common.Utilities.Objects;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using IBatisNet.DataMapper.Configuration.Cache;
using IBatisNet.DataMapper.Configuration.ParameterMapping;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.DataExchange;
using IBatisNet.DataMapper.MappedStatements;
using IBatisNet.DataMapper.SessionStore;
using IBatisNet.DataMapper.TypeHandlers;

namespace QM.Server.Data
{
    public class MyBatis : ISqlMapper
    {
        protected ISqlMapper Mapper { get; set; }

        private DomSqlMapBuilder _Builder { get; set; }

        protected DomSqlMapBuilder Builder
        {
            get
            {
                if (_Builder == null)
                {
                    _Builder = new DomSqlMapBuilder();
                }
                return _Builder;
            }
            set
            {
                _Builder = value;
            }
        }

        public MyBatis(string resource)
        {
            Mapper = Builder.Configure(resource);
        }

        public string Id
        {
            get
            {
                return Mapper.Id;
            }
        }

        public ISessionStore SessionStore
        {
            set
            {
                Mapper.SessionStore = value;
            }
        }

        public bool IsSessionStarted
        {
            get
            {
                return Mapper.IsSessionStarted;
            }
        }

        public ISqlMapSession LocalSession
        {
            get
            {
                return Mapper.LocalSession;
            }
        }

        public DBHelperParameterCache DBHelperParameterCache
        {
            get
            {
                return Mapper.DBHelperParameterCache;
            }
        }

        public bool IsCacheModelsEnabled
        {
            get
            {
                return Mapper.IsCacheModelsEnabled;
            }

            set
            {
                Mapper.IsCacheModelsEnabled = value;
            }
        }

        public DataExchangeFactory DataExchangeFactory
        {
            get
            {
                return Mapper.DataExchangeFactory;
            }
        }

        public TypeHandlerFactory TypeHandlerFactory
        {
            get
            {
                return Mapper.TypeHandlerFactory;
            }
        }

        public IObjectFactory ObjectFactory
        {
            get
            {
                return Mapper.ObjectFactory;
            }
        }

        public AccessorFactory AccessorFactory
        {
            get
            {
                return Mapper.AccessorFactory;
            }
        }

        public HybridDictionary ParameterMaps
        {
            get
            {
                return Mapper.ParameterMaps;
            }
        }

        public HybridDictionary ResultMaps
        {
            get
            {
                return Mapper.ResultMaps;
            }
        }

        public HybridDictionary MappedStatements
        {
            get
            {
                return Mapper.MappedStatements;
            }
        }

        public IDataSource DataSource
        {
            get
            {
                return Mapper.DataSource;
            }

            set
            {
                Mapper.DataSource = value;
            }
        }

        public ISqlMapSession CreateSqlMapSession()
        {
            return Mapper.CreateSqlMapSession();
        }

        public ParameterMap GetParameterMap(string name)
        {
            return Mapper.GetParameterMap(name);
        }

        public void AddParameterMap(ParameterMap parameterMap)
        {
            Mapper.AddParameterMap(parameterMap);
        }

        public IResultMap GetResultMap(string name)
        {
            return Mapper.GetResultMap(name);
        }

        public void AddResultMap(IResultMap resultMap)
        {
            Mapper.AddResultMap(resultMap);
        }

        public CacheModel GetCache(string name)
        {
            return Mapper.GetCache(name);
        }

        public void AddCache(CacheModel cache)
        {
            Mapper.AddCache(cache);
        }

        public void AddMappedStatement(string key, IMappedStatement mappedStatement)
        {
            Mapper.AddMappedStatement(key, mappedStatement);
        }

        public ISqlMapSession BeginTransaction()
        {
            return Mapper.BeginTransaction();
        }

        public ISqlMapSession BeginTransaction(bool openConnection)
        {
            return Mapper.BeginTransaction(openConnection);
        }

        public ISqlMapSession BeginTransaction(string connectionString)
        {
            return Mapper.BeginTransaction(connectionString);
        }

        public ISqlMapSession BeginTransaction(bool openNewConnection, IsolationLevel isolationLevel)
        {
            return Mapper.BeginTransaction(openNewConnection, isolationLevel);
        }

        public ISqlMapSession BeginTransaction(string connectionString, bool openNewConnection, IsolationLevel isolationLevel)
        {
            return Mapper.BeginTransaction(connectionString, openNewConnection, isolationLevel);
        }

        public ISqlMapSession BeginTransaction(IsolationLevel isolationLevel)
        {
            return Mapper.BeginTransaction(isolationLevel);
        }

        public ISqlMapSession BeginTransaction(string connectionString, IsolationLevel isolationLevel)
        {
            return Mapper.BeginTransaction(connectionString, isolationLevel);
        }

        public void CloseConnection()
        {
            Mapper.CloseConnection();
        }

        public void CommitTransaction(bool closeConnection)
        {
            Mapper.CommitTransaction(closeConnection);
        }

        public void CommitTransaction()
        {
            Mapper.CommitTransaction();
        }

        public int Delete(string statementName, object parameterObject)
        {
            return Mapper.Delete(statementName, parameterObject);
        }

        public void FlushCaches()
        {
            Mapper.FlushCaches();
        }

        public string GetDataCacheStats()
        {
            return Mapper.GetDataCacheStats();
        }

        public IMappedStatement GetMappedStatement(string id)
        {
            return Mapper.GetMappedStatement(id);
        }

        public object Insert(string statementName, object parameterObject)
        {
            return Mapper.Insert(statementName, parameterObject);
        }

        public ISqlMapSession OpenConnection()
        {
            return Mapper.OpenConnection();
        }

        public ISqlMapSession OpenConnection(string connectionString)
        {
            return Mapper.OpenConnection(connectionString);
        }

        public IDictionary QueryForDictionary(string statementName, object parameterObject, string keyProperty, string valueProperty)
        {
            return Mapper.QueryForDictionary(statementName, parameterObject, keyProperty, valueProperty);
        }

        public IDictionary QueryForDictionary(string statementName, object parameterObject, string keyProperty)
        {
            return Mapper.QueryForDictionary(statementName, parameterObject, keyProperty);
        }

        public void QueryForList(string statementName, object parameterObject, IList resultObject)
        {
            Mapper.QueryForList(statementName, parameterObject, resultObject);
        }

        public IList QueryForList(string statementName, object parameterObject)
        {
            return Mapper.QueryForList(statementName, parameterObject);
        }

        public IList QueryForList(string statementName, object parameterObject, int skipResults, int maxResults)
        {
            return Mapper.QueryForList(statementName, parameterObject, skipResults, maxResults);
        }

        public IDictionary QueryForMap(string statementName, object parameterObject, string keyProperty)
        {
            return Mapper.QueryForMap(statementName, parameterObject, keyProperty);
        }

        public IDictionary QueryForMap(string statementName, object parameterObject, string keyProperty, string valueProperty)
        {
            return Mapper.QueryForMap(statementName, parameterObject, keyProperty, valueProperty);
        }

        public IDictionary QueryForMapWithRowDelegate(string statementName, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate)
        {
            return Mapper.QueryForMapWithRowDelegate(statementName, parameterObject, keyProperty, valueProperty, rowDelegate);
        }

        public object QueryForObject(string statementName, object parameterObject, object resultObject)
        {
            return Mapper.QueryForObject(statementName, parameterObject, resultObject);
        }

        public object QueryForObject(string statementName, object parameterObject)
        {
            return Mapper.QueryForObject(statementName, parameterObject);
        }

        public PaginatedList QueryForPaginatedList(string statementName, object parameterObject, int pageSize)
        {
            return Mapper.QueryForPaginatedList(statementName, parameterObject, pageSize);
        }

        public IList QueryWithRowDelegate(string statementName, object parameterObject, RowDelegate rowDelegate)
        {
            return Mapper.QueryWithRowDelegate(statementName, parameterObject, rowDelegate);
        }

        public void RollBackTransaction()
        {
            Mapper.RollBackTransaction();
        }

        public void RollBackTransaction(bool closeConnection)
        {
            Mapper.RollBackTransaction(closeConnection);
        }

        public int Update(string statementName, object parameterObject)
        {
            return Mapper.Update(statementName, parameterObject);
        }

        public IDictionary<K, V> QueryForDictionary<K, V>(string statementName, object parameterObject, string keyProperty, string valueProperty)
        {
            return Mapper.QueryForDictionary<K, V>(statementName, parameterObject, keyProperty, valueProperty);
        }

        public IDictionary<K, V> QueryForDictionary<K, V>(string statementName, object parameterObject, string keyProperty)
        {
            return Mapper.QueryForDictionary<K, V>(statementName, parameterObject, keyProperty);
        }

        public IDictionary<K, V> QueryForDictionary<K, V>(string statementName, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate<K, V> rowDelegate)
        {
            return Mapper.QueryForDictionary<K, V>(statementName, parameterObject, keyProperty, valueProperty, rowDelegate);
        }

        public T QueryForObject<T>(string statementName, object parameterObject, T instanceObject)
        {
            return Mapper.QueryForObject<T>(statementName, parameterObject, instanceObject);
        }

        public T QueryForObject<T>(string statementName, object parameterObject)
        {
            return Mapper.QueryForObject<T>(statementName, parameterObject);
        }

        public IList<T> QueryForList<T>(string statementName, object parameterObject)
        {
            return Mapper.QueryForList<T>(statementName, parameterObject);
        }

        public void QueryForList<T>(string statementName, object parameterObject, IList<T> resultObject)
        {
            Mapper.QueryForList<T>(statementName, parameterObject, resultObject);
        }

        public IList<T> QueryForList<T>(string statementName, object parameterObject, int skipResults, int maxResults)
        {
            return Mapper.QueryForList<T>(statementName, parameterObject, skipResults, maxResults);
        }

        public IList<T> QueryWithRowDelegate<T>(string statementName, object parameterObject, RowDelegate<T> rowDelegate)
        {
            return Mapper.QueryWithRowDelegate<T>(statementName, parameterObject, rowDelegate);
        }
    }
}
