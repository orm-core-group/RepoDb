﻿using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A base object for all shared-based repositories.
    /// </summary>
    public partial class DbRepository<TDbConnection> : IDisposable where TDbConnection : DbConnection
    {
        #region CountAll<TEntity>

        /// <summary>
        /// Counts all the table data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public long CountAll<TEntity>(string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.CountAll<TEntity>(hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            catch
            {
                // Throw back the error
                throw;
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #region CountAllAsync<TEntity>

        /// <summary>
        /// Counts all the table data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public async Task<long> CountAllAsync<TEntity>(string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.CountAllAsync<TEntity>(hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            catch
            {
                // Throw back the error
                throw;
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #region CountAll(TableName)

        /// <summary>
        /// Counts all the table data from the database.
        /// </summary>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public long CountAll(string tableName,
            string hints = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.CountAll(tableName: tableName,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            catch
            {
                // Throw back the error
                throw;
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #region CountAllAsync(TableName)

        /// <summary>
        /// Counts all the data from the database in an asynchronous way.
        /// </summary>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<long> CountAllAsync(string tableName,
            string hints = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.CountAllAsync(tableName: tableName,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            catch
            {
                // Throw back the error
                throw;
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion
    }
}
