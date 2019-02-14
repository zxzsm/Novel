using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Novel.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Dictionary<string, object> payLoad = new Dictionary<string, object>();
            payLoad.Add("sub", "rober");
            payLoad.Add("jti", "09e572c7-62d0-4198-9cce-0915d7493806");
            payLoad.Add("nbf", null);
            payLoad.Add("exp", null);
            payLoad.Add("iss", "roberIssuer");
            payLoad.Add("aud", "roberAudience");
            payLoad.Add("age", 30); 
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TokenCustomerValidateTest()
        {
            Dictionary<string, object> payLoad = new Dictionary<string, object>();
            payLoad.Add("sub", "rober");
            payLoad.Add("jti", Guid.NewGuid().ToString());
            payLoad.Add("nbf", null);
            payLoad.Add("exp", null);
            payLoad.Add("iss", "roberIssuer");
            payLoad.Add("aud", "roberAudience");
            payLoad.Add("age", 40);

          
            Assert.IsTrue(true);
        }
    }
}
