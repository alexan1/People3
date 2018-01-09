using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using People3.Controllers;
using People3.Models;
using People3.Services;
using System.Threading.Tasks;

namespace People3Test
{
    [TestClass]
    public class PeopleControllerTest
    {
        private readonly PeopleController _peopleController;
        private readonly IRepo _repo;
        private readonly UserManager<ApplicationUser> _userManager;

        public PeopleControllerTest()
        {
            _peopleController = new PeopleController(_repo, _userManager);
        }


        [TestMethod]
        public void IndexTest()
        {
            //act
            var result = _peopleController.Index();
            //assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void DetailsTest()
        {
            //arrange

            //act
            var result = _peopleController.Details(7747);
            //accert
            //var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsInstanceOfType(result, typeof(Task<IActionResult>));
            //Assert.IsInstanceOfType(ViewResult);
        }
    }
}
