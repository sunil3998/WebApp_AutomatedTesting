using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repository.Interface;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICreditCardsRepository _repository;
        public HomeController(ILogger<HomeController> logger, ICreditCardsRepository repository)
        {
            _logger = logger;
            this._repository = repository;
        }

        public IActionResult Index()
        {
            var vv = 5 / 0;
            var cards = _repository.GetAllCreditCards();
            return View(cards);
        }

        public IActionResult Privacy()
        {
            string message = "I am doing testing";
            return View(model: message);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // GET: Home/Create
        public IActionResult Create()
        {
            return View(new CreditCardsModel());
        }

        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreditCardsModel creditCard)
        {
            if (ModelState.IsValid)
            {
                _repository.AddCreditCard(creditCard);
                return RedirectToAction(nameof(Index));
            }
            return View(creditCard);
        }

        // GET: Home/Edit/5
        public IActionResult Edit(int id)
        {
            if (id <= 0)
                return BadRequest();
            var card = _repository.GetCreditCard(id);
            if (card == null)
                return NotFound();
            return View(card);
        }

        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CreditCardsModel creditCard)
        {
            if (ModelState.IsValid)
            {
                _repository.UpdateCreditCard(creditCard);
                return RedirectToAction(nameof(Index));
            }
            return View(creditCard);
        }

        // GET: Home/Details/5
        public IActionResult Details(int id)
        {
            if (id <= 0)
                return BadRequest();
            var card = _repository.GetCreditCard(id);
            if (card == null)
                return NotFound();
            return View(card);
        }

        // GET: Home/Delete/5
        public IActionResult Delete(int id)
        {
            if (id <= 0)
                return BadRequest();
            var card = _repository.GetCreditCard(id);
            if (card == null)
            {
                return NotFound();
            }
            return View(card);
        }

        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (id <= 0)
                return BadRequest();
            var cart = _repository.GetCreditCard(id);
            if (cart != null)
                _repository.DeleteCreditCard(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
