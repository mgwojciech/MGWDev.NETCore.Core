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
    public class UnitTest1
    {

        string login = "";
        string password = "";
        string site = "";

        [TestMethod]
        public void TestGetWeb()
        {
            SPOHttpClient spoClient = new SPOHttpClient(login, password, site);
            Web web = spoClient.GetData<Web>("/_api/web");
            Assert.AreEqual("Dev", web.Title);

            ListItem item = spoClient.GetData<ListItem>("/_api/web/lists/getByTitle('Umowy')/items(1)");
            item.Title = "Test updated!";
            item.Metadata = new Metadata()
            {
                Type = "SP.Data.UmowyListItem"
            };
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                {"IF-MATCH", "*" },
                {"X-HTTP-Method", "MERGE"}
            };
            dynamic response = spoClient.PostData<dynamic,ListItem>("/_api/web/lists/getByTitle('Umowy')/items(1)", item, headers);
        }
        [TestMethod]
        public void TestFilterClause()
        {
            RESTUrlBuilder<ListItem> queryBuilder = new RESTUrlBuilder<ListItem>();
            string filterQuery = queryBuilder.BuildFilterClause(li => li.Id == 1);

            SPOHttpClient spoClient = new SPOHttpClient(login, password, site);
            CollectionResponse<ListItem> items = spoClient.GetData<CollectionResponse<ListItem>>("/_api/web/lists/getByTitle('Umowy')/items?$filter=" + filterQuery);
        }
    }
}
