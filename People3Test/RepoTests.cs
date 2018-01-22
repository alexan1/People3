using Microsoft.VisualStudio.TestTools.UnitTesting;
using People3.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace People3Test
{
    [TestClass]
    public class RepoTests
    {
        private readonly IRepo _repo;

        public RepoTests()
        {
        }

        public RepoTests(IRepo repo)
        {
            _repo = repo;
        }

        //[TestInitialize]
        //public void Initalize()
        //{
        //    _repo = repo;
        //}

        [TestMethod]
        public void DetailAsyncTest()
        {
            var person = "Vladimir Putin";
            var result = _repo.DetailAsync(7747);

            Assert.AreEqual(result, person);

            //Assert.IsTrue(true);


        }
    }
}
