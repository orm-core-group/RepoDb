using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;
using System.Data.SQLite;

namespace RepoDb.SqLite.UnitTests
{
    [TestClass]
    public class StatementBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            SqLiteBootstrap.Initialize();
        }

        #region CreateBatchQuery

        [TestMethod]
        public void TestCreateBatchQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name"),
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT [Id], [Name] FROM [Table] ORDER BY [Id] ASC LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestCreateBatchQueryWithPage()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name"),
                3,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT [Id], [Name] FROM [Table] ORDER BY [Id] ASC LIMIT 30, 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnCreateBatchQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
                null,
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(EmptyException))]
        public void ThrowExceptionOnCreateBatchQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name"),
                0,
                10,
                null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnCreateBatchQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name"),
                -1,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnCreateBatchQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name"),
                0,
                -1,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        #endregion

        #region CreateExists

        [TestMethod]
        public void TestCreateExists()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateExists(new QueryBuilder(),
                "Table",
                QueryGroup.Parse(new { Id = 1 }));
            var expected = "SELECT 1 AS [ExistsValue] FROM [Table] WHERE ([Id] = @Id) LIMIT 1 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestCreateInsert()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateInsert(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                null);
            var expected = "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestCreateInsertWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateInsert(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT @Id AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestCreateInsertWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateInsert(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "INSERT INTO [Table] ( [Name], [Address] ) VALUES ( @Name, @Address ) ; SELECT CAST(last_insert_rowid() AS INT) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion

        #region CreateInsertAll

        [TestMethod]
        public void TestCreateInsertAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateInsertAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                null);
            var expected = "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; " +
                "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_1, @Name_1, @Address_1 ) ; " +
                "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_2, @Name_2, @Address_2 ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestCreateInserAlltWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateInsertAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; " +
                "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_1, @Name_1, @Address_1 ) ; " +
                "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_2, @Name_2, @Address_2 ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestCreateInsertAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateInsertAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "INSERT INTO [Table] ( [Name], [Address] ) VALUES ( @Name, @Address ) ; SELECT CAST(last_insert_rowid() AS INT) ; " +
                "INSERT INTO [Table] ( [Name], [Address] ) VALUES ( @Name_1, @Address_1 ) ; SELECT CAST(last_insert_rowid() AS INT) ; " +
                "INSERT INTO [Table] ( [Name], [Address] ) VALUES ( @Name_2, @Address_2 ) ; SELECT CAST(last_insert_rowid() AS INT) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion

        #region CreateMerge

        [TestMethod]
        public void TestCreateMerge()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateMerge(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(@Id AS BIGINT) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestCreateMergeWithPrimaryAsQualifier()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateMerge(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(@Id AS BIGINT) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestCreateMergeWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateMerge(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(COALESCE(last_insert_rowid(), @Id) AS BIGINT) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnCreateMergeIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateMerge(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnCreateMergeIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateMerge(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(InvalidQualifiersException))]
        public void ThrowExceptionOnCreateMergeIfThereAreOtherFieldsAsQualifers()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateMerge(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id", "Name"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
        }

        #endregion

        #region CreateMergeAll

        [TestMethod]
        public void TestCreateMergeAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateMergeAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(@Id AS BIGINT) AS [Result] ; " +
                "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_1, @Name_1, @Address_1 ) ; SELECT CAST(@Id_1 AS BIGINT) AS [Result] ; " +
                "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_2, @Name_2, @Address_2 ) ; SELECT CAST(@Id_2 AS BIGINT) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestCreateMergeAllWithPrimaryAsQualifier()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateMergeAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(@Id AS BIGINT) AS [Result] ; " +
                "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_1, @Name_1, @Address_1 ) ; SELECT CAST(@Id_1 AS BIGINT) AS [Result] ; " +
                "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_2, @Name_2, @Address_2 ) ; SELECT CAST(@Id_2 AS BIGINT) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestCreateMergeAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateMergeAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(COALESCE(last_insert_rowid(), @Id) AS INT) AS [Result] ; " +
                "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_1, @Name_1, @Address_1 ) ; SELECT CAST(COALESCE(last_insert_rowid(), @Id_1) AS INT) AS [Result] ; " +
                "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_2, @Name_2, @Address_2 ) ; SELECT CAST(COALESCE(last_insert_rowid(), @Id_2) AS INT) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnCreateMergeAllIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateMergeAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnCreateMergeAllIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateMergeAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(InvalidQualifiersException))]
        public void ThrowExceptionOnCreateMergeAllIfThereAreOtherFieldsAsQualifers()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateMergeAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id", "Name"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
        }

        #endregion

        #region CreateQuery

        [TestMethod]
        public void TestCreateQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null,
                null);
            var expected = "SELECT [Id], [Name], [Address] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestCreateQueryWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                QueryGroup.Parse(new { Id = 1, Name = "Michael" }),
                null,
                null,
                null);
            var expected = "SELECT [Id], [Name], [Address] FROM [Table] WHERE ([Id] = @Id AND [Name] = @Name) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestCreateQueryWithTop()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                10,
                null);
            var expected = "SELECT [Id], [Name], [Address] FROM [Table] LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestCreateQueryOrderBy()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending }),
                null,
                null);
            var expected = "SELECT [Id], [Name], [Address] FROM [Table] ORDER BY [Id] ASC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestCreateQueryOrderByFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending, Name = Order.Ascending }),
                null,
                null);
            var expected = "SELECT [Id], [Name], [Address] FROM [Table] ORDER BY [Id] ASC, [Name] ASC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestCreateQueryOrderByDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Descending }),
                null,
                null);
            var expected = "SELECT [Id], [Name], [Address] FROM [Table] ORDER BY [Id] DESC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestCreateQueryOrderByFieldsDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Descending, Name = Order.Descending }),
                null,
                null);
            var expected = "SELECT [Id], [Name], [Address] FROM [Table] ORDER BY [Id] DESC, [Name] DESC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestCreateQueryOrderByFieldsMultiDirection()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending, Name = Order.Descending }),
                null,
                null);
            var expected = "SELECT [Id], [Name], [Address] FROM [Table] ORDER BY [Id] ASC, [Name] DESC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(MissingFieldsException))]
        public void ThrowExceptionOnCreateQueryIfOrderFieldsAreNotPresentAtTheFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Descending, SSN = Order.Ascending }),
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnCreateQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null,
                "WhatEver");
        }

        #endregion

        #region CreateTruncate

        [TestMethod]
        public void TestCreateTruncate()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateTruncate(new QueryBuilder(),
                "Table");
            var expected = "DELETE FROM [Table] ; VACUUM ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion
    }
}
