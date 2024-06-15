﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Extensions.Vitorm_Extensions;
using System.Data;

namespace Vitorm.MsTest.CommonTest
{

    [TestClass]
    public class Query_LinqMethods_Test
    {



        [TestMethod]
        public void Test_PlainQuery()
        {
            using var dbContext = DataSource.CreateDbContext();
            var userQuery = dbContext.Query<User>();

            {
                var userList = userQuery.ToList();
                Assert.AreEqual(6, userList.Count);
                Assert.AreEqual(1, userList.First().id);
                Assert.AreEqual(6, userList.Last().id);
            }


            {
                var userList = userQuery.Select(u => u.id).ToList();
                Assert.AreEqual(6, userList.Count);
                Assert.AreEqual(1, userList.First());
                Assert.AreEqual(6, userList.Last());
            }
        }


        [TestMethod]
        public void Test_AllFeatures()
        {
            using var dbContext = DataSource.CreateDbContext();
            var userQuery = dbContext.Query<User>();

            #region users.Where().OrderBy().Skip().Take().ToList
            /*
            users.Where(row => row.user.id > 2)
            .OrderBy(user=>user.id)
            .Select(row => new {row.user })
            .Skip(1).Take(2);
             */
            {
                var query = (from user in userQuery
                             where user.id > 2
                             orderby user.id descending
                             select new
                             {
                                 user
                             })
                            .Skip(1).Take(2);

                //var sql = query.ToExecuteString();
                var list = query.ToList();

                Assert.AreEqual(2, list.Count);
                Assert.AreEqual(5, list[0].user.id);
                Assert.AreEqual(4, list[1].user.id);
            }
            #endregion
        }



        [TestMethod]
        public void Test_Get()
        {
            {
                using var dbContext = DataSource.CreateDbContext();
                var user = dbContext.Get<User>(3);
                Assert.AreEqual(3, user?.id);
            }
            {
                using var dbContext = DataSource.CreateDbContext();
                var user = dbContext.DbSet<User>().Get(5);
                Assert.AreEqual(5, user?.id);
            }
        }



        [TestMethod]
        public void Test_Select()
        {
            using var dbContext = DataSource.CreateDbContext();
            var userQuery = dbContext.Query<User>();

            {
                var userList = userQuery.Select(u => u).Where(user => user.id == 3).Select(u => u).ToList();
                Assert.AreEqual(1, userList.Count);
                Assert.AreEqual(3, userList.First().id);
            }

            {
                var userList = userQuery.Where(user => user.id == 3).Select(u => (float)u.id).ToList();
                Assert.AreEqual(1, userList.Count);
                Assert.AreEqual(3.0, userList.First());
            }


            {
                var query =
                    from user in userQuery
                    select new
                    {
                        uniqueId1 = user.id + "_" + user.fatherId + "_" + user.motherId,
                        uniqueId2 = $"{user.id}_{user.fatherId}_{user.motherId}"
                    };

                var userList = query.ToList();
                Assert.AreEqual(6, userList.Count);
                Assert.AreEqual("1_4_6", userList.First().uniqueId1);
            }

        }

        [TestMethod]
        public void Test_Count()
        {
            using var dbContext = DataSource.CreateDbContext();
            var userQuery = dbContext.Query<User>();

            {
                var count = (from user in userQuery
                             where user.id > 2
                             select new
                             {
                                 user
                             }).Count();

                Assert.AreEqual(4, count);
            }
        }





        [TestMethod]
        public void Test_FirstOrDefault()
        {
            using var dbContext = DataSource.CreateDbContext();
            var userQuery = dbContext.Query<User>();

            {
                var id = userQuery.Select(u => u.id).FirstOrDefault();
                Assert.AreEqual(1, id);
            }

            {
                var user = userQuery.FirstOrDefault();
                Assert.AreEqual(1, user?.id);
            }

            {
                var user = userQuery.FirstOrDefault(user => user.id == 3);
                Assert.AreEqual(3, user?.id);
            }

            {
                var user = userQuery.FirstOrDefault(user => user.id == 13);
                Assert.AreEqual(null, user?.id);
            }

            {
                var user = userQuery.OrderByDescending(m => m.id).FirstOrDefault();
                Assert.AreEqual(6, user?.id);
            }
        }


        [TestMethod]
        public void Test_First()
        {
            using var dbContext = DataSource.CreateDbContext();
            var userQuery = dbContext.Query<User>();

            {
                var user = userQuery.First();
                Assert.AreEqual(1, user?.id);
            }

            {
                var user = userQuery.First(user => user.id == 3);
                Assert.AreEqual(3, user?.id);
            }

            {
                try
                {
                    var user = userQuery.First(user => user.id == 13);
                    Assert.Fail("IQueryalbe.First should throw Exception");
                }
                catch (Exception ex)
                {
                }

            }

        }



        [TestMethod]
        public void Test_LastOrDefault()
        {
            using var dbContext = DataSource.CreateDbContext();
            var userQuery = dbContext.Query<User>();

            {
                var id = userQuery.Select(u => u.id).FirstOrDefault();
                Assert.AreEqual(1, id);
            }
            {
                var user = userQuery.LastOrDefault();
                Assert.AreEqual(6, user?.id);
            }

            {
                var user = userQuery.LastOrDefault(user => user.id == 3);
                Assert.AreEqual(3, user?.id);
            }

            {
                var user = userQuery.LastOrDefault(user => user.id == 13);
                Assert.AreEqual(null, user?.id);
            }

            {
                var user = userQuery.OrderByDescending(m => m.id).LastOrDefault();
                Assert.AreEqual(1, user?.id);
            }
        }


        [TestMethod]
        public void Test_Last()
        {
            using var dbContext = DataSource.CreateDbContext();
            var userQuery = dbContext.Query<User>();

            {
                var user = userQuery.Last();
                Assert.AreEqual(6, user?.id);
            }

            {
                var user = userQuery.Last(user => user.id == 3);
                Assert.AreEqual(3, user?.id);
            }

            {
                try
                {
                    var user = userQuery.Last(user => user.id == 13);
                    Assert.Fail("IQueryalbe.First should throw Exception");
                }
                catch (Exception ex)
                {
                }

            }

        }

        // Enumerable.ToArray
        [TestMethod]
        public void Test_Enumerable_ToArray()
        {
            using var dbContext = DataSource.CreateDbContext();
            var userQuery = dbContext.Query<User>();

            {
                var userList = userQuery.ToArray();
                Assert.AreEqual(6, userList.Length);
                Assert.AreEqual(1, userList.First().id);
                Assert.AreEqual(6, userList.Last().id);
            }


            {
                var userList = userQuery.Select(u => u.id).ToArray();
                Assert.AreEqual(6, userList.Length);
                Assert.AreEqual(1, userList.First());
                Assert.AreEqual(6, userList.Last());
            }
        }




    }
}
