﻿/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) LH.Studio 2015. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

using System;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;

namespace LH.Data
{
    /// <summary>
    /// OleDb extension.
    /// </summary>
    internal static class OleDbHelper
    {
        #region ConnectionBehavior

        /// <summary>
        /// Connection open/close behavior when using DataAdapter. Default Auto.
        /// </summary>
        internal static OleDbConnectionBehavior DataAdapterConnectionBehavior { get; set; } = OleDbConnectionBehavior.Auto;

        /// <summary>
        /// Connection open/close behavior when using DataReader. Default Manual.
        /// </summary>
        internal static OleDbConnectionBehavior DataReaderConnectionBehavior { get; set; } = OleDbConnectionBehavior.Manual;

        /// <summary>
        /// Connection open/close behavior when using Execute. Default Manual.
        /// </summary>
        internal static OleDbConnectionBehavior ExecuteConnectionBehavior { get; set; } = OleDbConnectionBehavior.Manual;

        /// <summary>
        /// Connection open/close behavior when using Transaction. Default Manual.
        /// </summary>
        internal static OleDbConnectionBehavior TransactionConnectionBehavior { get; set; } = OleDbConnectionBehavior.Manual;

        #endregion ConnectionBehavior

        #region Connection

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="connectionStringBuilder">ConnectionStringBuilder.</param>
        /// <returns></returns>
        internal static OleDbConnection BuildConnection(OleDbConnectionStringBuilder connectionStringBuilder)
        {
            if (connectionStringBuilder == null)
            {
                throw new ArgumentNullException(nameof(connectionStringBuilder));
            }

            return new OleDbConnection(connectionStringBuilder.ConnectionString);
        }

        /// <summary>
        /// Creating a data connection does not test the validity of the connection.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <returns></returns>
        internal static OleDbConnection BuildConnection(string connectionString)
        {
            return new OleDbConnection(connectionString);
        }

        /// <summary>
        /// Creating a data connection string.
        /// </summary>
        /// <param name="connectionStringBuilder">ConnectionStringBuilder.</param>
        /// <returns></returns>
        internal static string BuildConnectionString(OleDbConnectionStringBuilder connectionStringBuilder)
        {
            if (connectionStringBuilder == null)
            {
                throw new ArgumentNullException(nameof(connectionStringBuilder));
            }

            return connectionStringBuilder.ConnectionString;
        }

        #endregion Connection

        #region DataAdapter

        /// <summary>
        /// Append the fill DataSet with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataSet">DataSet.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <returns></returns>
        internal static int FillDataSet(DataSet dataSet, OleDbConnection connection, string selectCommandText)
        {
            return FillDataSet(dataSet, connection, selectCommandText, null);
        }

        /// <summary>
        /// Append the fill DataSet with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataSet">DataSet.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        internal static int FillDataSet(DataSet dataSet, OleDbConnection connection, string selectCommandText, params OleDbParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (DataAdapterConnectionBehavior == OleDbConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result;
            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
            {
                if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand.Parameters.AddRange(parameters); }
                result = dataAdapter.Fill(dataSet);
            }
            return result;
        }

        /// <summary>
        /// Append the fill DataTable with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataTable">DataTable.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <returns></returns>
        internal static int FillDataTable(DataTable dataTable, OleDbConnection connection, string selectCommandText)
        {
            return FillDataTable(dataTable, connection, selectCommandText, null);
        }

        /// <summary>
        /// Append the fill DataTable with records and schemas. Returns the number of populated data rows.
        /// </summary>
        /// <param name="dataTable">DataTable.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        internal static int FillDataTable(DataTable dataTable, OleDbConnection connection, string selectCommandText, params OleDbParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (DataAdapterConnectionBehavior == OleDbConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result;
            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
            {
                if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand.Parameters.AddRange(parameters); }
                result = dataAdapter.Fill(dataTable);
            }
            return result;
        }

        /// <summary>
        /// Get DataAdapter.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <returns></returns>
        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        internal static OleDbDataAdapter GetDataAdapter(OleDbConnection connection, string selectCommandText)
        {
            return new OleDbDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };
        }

        /// <summary>
        /// Get DataAdapter.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        internal static OleDbDataAdapter GetDataAdapter(OleDbConnection connection, string selectCommandText, params OleDbParameter[] parameters)
        {
            OleDbDataAdapter result = new OleDbDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey };
            if (parameters != null && parameters.Length > 0) { result.SelectCommand.Parameters.AddRange(parameters); }
            return result;
        }

        /// <summary>
        /// Create a new DataSet with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <returns></returns>
        internal static DataSet GetDataSet(OleDbConnection connection, string selectCommandText)
        {
            return GetDataSet(connection, selectCommandText, null);
        }

        /// <summary>
        /// Create a new DataSet with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        internal static DataSet GetDataSet(OleDbConnection connection, string selectCommandText, params OleDbParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (DataAdapterConnectionBehavior == OleDbConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            DataSet result = new DataSet();
            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
            {
                if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand.Parameters.AddRange(parameters); }
                dataAdapter.Fill(result);
            }
            return result;
        }

        /// <summary>
        /// Create a new DataTable with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <returns></returns>
        internal static DataTable GetDataTable(OleDbConnection connection, string selectCommandText)
        {
            return GetDataTable(connection, selectCommandText, null);
        }

        /// <summary>
        /// Create a new DataTable with records and schemas.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="selectCommandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        internal static DataTable GetDataTable(OleDbConnection connection, string selectCommandText, params OleDbParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (DataAdapterConnectionBehavior == OleDbConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            DataTable result = new DataTable();
            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(selectCommandText, connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey })
            {
                if (parameters != null && parameters.Length > 0) { dataAdapter.SelectCommand.Parameters.AddRange(parameters); }
                dataAdapter.Fill(result);
            }
            return result;
        }

        #endregion DataAdapter

        #region DataReader

        /// <summary>
        /// Get DataReader.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        internal static OleDbDataReader GetDataReader(OleDbConnection connection, string commandText)
        {
            return GetDataReader(connection, commandText, null);
        }

        /// <summary>
        /// Get DataReader.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        internal static OleDbDataReader GetDataReader(OleDbConnection connection, string commandText, params OleDbParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            CommandBehavior commandBehavior;
            if (DataReaderConnectionBehavior == OleDbConnectionBehavior.Manual)
            {
                if (connection.State != ConnectionState.Open)
                {
                    throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
                }
                commandBehavior = CommandBehavior.Default;
            }
            else
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    commandBehavior = CommandBehavior.CloseConnection;
                }
                else
                {
                    commandBehavior = CommandBehavior.Default;
                }
            }
            OleDbCommand command = new OleDbCommand(commandText, connection);
            if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
            return command.ExecuteReader(commandBehavior);
        }

        #endregion DataReader

        #region Execute

        /// <summary>
        /// Execute the query command. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        internal static int ExecuteNonQuery(OleDbConnection connection, string commandText)
        {
            return ExecuteNonQuery(connection, commandText, null);
        }

        /// <summary>
        /// Execute the query command. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        internal static int ExecuteNonQuery(OleDbConnection connection, string commandText, params OleDbParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (ExecuteConnectionBehavior == OleDbConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result;
            using (OleDbCommand command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                ConnectionState state = connection.State;
                if (state != ConnectionState.Open) { connection.Open(); }
                result = command.ExecuteNonQuery();
                if (state != ConnectionState.Open) { connection.Close(); }
            }
            return result;
        }

        /// <summary>
        /// Executing stored procedure.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        internal static void ExecuteProcedure(OleDbConnection connection, string procedure)
        {
            ExecuteProcedure(connection, procedure, null);
        }

        /// <summary>
        /// Executing stored procedure.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        internal static void ExecuteProcedure(OleDbConnection connection, string procedure, params OleDbParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (ExecuteConnectionBehavior == OleDbConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            using (OleDbCommand command = new OleDbCommand(procedure, connection) { CommandType = CommandType.StoredProcedure })
            {
                if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                ConnectionState state = connection.State;
                if (state != ConnectionState.Open) { connection.Open(); }
                command.ExecuteNonQuery();
                if (state != ConnectionState.Open) { connection.Close(); }
            }
        }

        /// <summary>
        /// Execute the query command. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        internal static object ExecuteScalar(OleDbConnection connection, string commandText)
        {
            return ExecuteScalar(connection, commandText, null);
        }

        /// <summary>
        /// Execute the query command. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        internal static object ExecuteScalar(OleDbConnection connection, string commandText, params OleDbParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (ExecuteConnectionBehavior == OleDbConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            object result;
            using (OleDbCommand command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                ConnectionState state = connection.State;
                if (state != ConnectionState.Open) { connection.Open(); }
                result = command.ExecuteScalar();
                if (state != ConnectionState.Open) { connection.Close(); }
            }
            return result;
        }

        #endregion Execute

        #region Transaction

        /// <summary>
        /// Execute the query command by transaction. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        internal static int TransactionExecuteNonQuery(OleDbConnection connection, string commandText)
        {
            return TransactionExecuteNonQuery(connection, IsolationLevel.ReadCommitted, commandText, null);
        }

        /// <summary>
        /// Execute the query command by transaction. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        internal static int TransactionExecuteNonQuery(OleDbConnection connection, string commandText, params OleDbParameter[] parameters)
        {
            return TransactionExecuteNonQuery(connection, IsolationLevel.ReadCommitted, commandText, parameters);
        }

        /// <summary>
        /// Execute the query command by transaction. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        internal static int TransactionExecuteNonQuery(OleDbConnection connection, IsolationLevel iso, string commandText)
        {
            return TransactionExecuteNonQuery(connection, iso, commandText, null);
        }

        /// <summary>
        /// Execute the query command by transaction. Returns the number of rows affected.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        internal static int TransactionExecuteNonQuery(OleDbConnection connection, IsolationLevel iso, string commandText, params OleDbParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (TransactionConnectionBehavior == OleDbConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            int result = 0;
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (OleDbTransaction transaction = connection.BeginTransaction(iso))
            {
                using (OleDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                    try
                    {
                        result += command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch { }
                        exception = ex;
                    }
                }
            }
            if (state != ConnectionState.Open) { connection.Close(); }
            if (exception == null)
            {
                return result;
            }
            else
            {
                throw exception;
            }
        }

        /// <summary>
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        internal static void TransactionExecuteProcedure(OleDbConnection connection, string procedure)
        {
            TransactionExecuteProcedure(connection, IsolationLevel.ReadCommitted, procedure, null);
        }

        /// <summary>
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        internal static void TransactionExecuteProcedure(OleDbConnection connection, string procedure, params OleDbParameter[] parameters)
        {
            TransactionExecuteProcedure(connection, IsolationLevel.ReadCommitted, procedure, parameters);
        }

        /// <summary>
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        internal static void TransactionExecuteProcedure(OleDbConnection connection, IsolationLevel iso, string procedure)
        {
            TransactionExecuteProcedure(connection, iso, procedure, null);
        }

        /// <summary>
        /// Executing stored procedure by transaction.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="procedure">Sql procedure.</param>
        /// <param name="parameters">Parameters.</param>
        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        internal static void TransactionExecuteProcedure(OleDbConnection connection, IsolationLevel iso, string procedure, params OleDbParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (TransactionConnectionBehavior == OleDbConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (OleDbTransaction transaction = connection.BeginTransaction(iso))
            {
                using (OleDbCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = procedure;
                    if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                    try
                    {
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch { }
                        exception = ex;
                    }
                }
            }
            if (state != ConnectionState.Open) { connection.Close(); }
            if (exception != null)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Execute the query command by transaction. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        internal static object TransactionExecuteScalar(OleDbConnection connection, string commandText)
        {
            return TransactionExecuteScalar(connection, IsolationLevel.ReadCommitted, commandText, null);
        }

        /// <summary>
        /// Execute the query command by transaction. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        internal static object TransactionExecuteScalar(OleDbConnection connection, string commandText, params OleDbParameter[] parameters)
        {
            return TransactionExecuteScalar(connection, IsolationLevel.ReadCommitted, commandText, parameters);
        }

        /// <summary>
        /// Execute the query command by transaction. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <returns></returns>
        internal static object TransactionExecuteScalar(OleDbConnection connection, IsolationLevel iso, string commandText)
        {
            return TransactionExecuteScalar(connection, iso, commandText, null);
        }

        /// <summary>
        /// Execute the query command by transaction. Returns the first column of the first row in the query result set.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="iso">The transaction isolation level of the connection.</param>
        /// <param name="commandText">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns></returns>
        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        internal static object TransactionExecuteScalar(OleDbConnection connection, IsolationLevel iso, string commandText, params OleDbParameter[] parameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (TransactionConnectionBehavior == OleDbConnectionBehavior.Manual && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be Open. Current state is " + connection.State.ToString());
            }
            object result = null;
            Exception exception = null;
            ConnectionState state = connection.State;
            if (state != ConnectionState.Open) { connection.Open(); }
            using (OleDbTransaction transaction = connection.BeginTransaction(iso))
            {
                using (OleDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    if (parameters != null && parameters.Length > 0) { command.Parameters.AddRange(parameters); }
                    try
                    {
                        result = command.ExecuteScalar();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch { }
                        exception = ex;
                    }
                }
            }
            if (state != ConnectionState.Open) { connection.Close(); }
            if (exception == null)
            {
                return result;
            }
            else
            {
                throw exception;
            }
        }

        #endregion Transaction
    }

    #region ConnectionBehavior

    /// <summary>
    /// The mode of the connection when querying.
    /// </summary>
    internal enum OleDbConnectionBehavior
    {
        /// <summary>Does not open automatically. If the connection is not open  when the querying, an exception is thrown.</summary>
        Manual,

        /// <summary>If the connection is not open, automatically open and close when the query is complete. If the connection is already open, keep it turned on when the query is complete.</summary>
        Auto
    }

    #endregion ConnectionBehavior
}