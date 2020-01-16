using MGWDev.NETCore.Core.Common;
using MGWDev.NETCore.Core.Model;
using MGWDev.NETCore.Core.Model.SP;
using MGWDev.NETCore.Core.SP;
using MGWDev.NETCore.Core.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MGWDev.NETCore.Core.Tests
{
    [TestClass]
    public class ExpressionToRestMapperTests
    {
        [TestMethod]
        public void TestFilterClause_Eq_Id()
        {
            RESTUrlBuilder<ListItem> queryBuilder = new RESTUrlBuilder<ListItem>();
            string filterQuery = queryBuilder.BuildFilterClause(li => li.Id == 1);

            Assert.AreEqual("Id eq 1", filterQuery);
        }
        [TestMethod]
        public void TestFilterClause_Eq_Title_Constant()
        {
            RESTUrlBuilder<ListItem> queryBuilder = new RESTUrlBuilder<ListItem>();
            string filterQuery = queryBuilder.BuildFilterClause(li => li.Title == "Test");

            Assert.AreEqual("Title eq 'Test'", filterQuery);
        }
        [TestMethod]
        public void TestFilterClause_Eq_Title_Variable()
        {
            string title = "Test";
            RESTUrlBuilder<ListItem> queryBuilder = new RESTUrlBuilder<ListItem>();
            string filterQuery = queryBuilder.BuildFilterClause(li => li.Title == title);

            Assert.AreEqual("Title eq 'Test'", filterQuery);
        }
    }
}
