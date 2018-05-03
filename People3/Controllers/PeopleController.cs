using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using People3.Services;
using People3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace People3.Controllers
{
    public class PeopleController : Controller
    {
        private readonly IRepo _repo;
        private readonly UserManager<ApplicationUser> _userManager;

        public PeopleController(IRepo repo, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }

        // GET: People
        public async Task<IActionResult> Index()
        {
            return View(await _repo.GetAllAsync());
        }

        // GET: People/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }            

            var person = await _repo.DetailAsync(id);            

            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        public async Task<IActionResult> Search(string SearchString)
        {
            IEnumerable<PersonViewModel> FoundPersons = new List<PersonViewModel>();
            if (String.IsNullOrEmpty(SearchString))
            {
                return View(FoundPersons);
            }
            else
            {
                FoundPersons = await _repo.FindAsync(SearchString);
                return View(FoundPersons);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Person person)
        {
            //var person = new Person();
            if (ModelState.IsValid)
            {
                await _repo.AddAsync(person);
                return RedirectToAction("Details", new { id = person.ID });
            }            
            return RedirectToAction("Index");
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RateAsync(Person person, int rate)
        {

            string userID;

            if (User.Identity.IsAuthenticated && ModelState.IsValid)
            {
                userID = _userManager.GetUserId(User);
                var res = await _repo.RateAsync(person, userID, rate);
                if (res > 0)
                {
                    return RedirectToAction("Details", new { id = person.ID });
                }
            }
            
            return RedirectToAction("Index");
        }
    }
}