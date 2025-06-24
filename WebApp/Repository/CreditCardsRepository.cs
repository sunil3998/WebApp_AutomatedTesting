using WebApp.Models;
using WebApp.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Repository
{
    
    public class CreditCardsRepository : ICreditCardsRepository
    {
        private readonly List<CreditCardsModel> _creditCards = new List<CreditCardsModel>
             {
               new CreditCardsModel { Id = 1, CardType = "MoneyBack", CreditLimit = 50000, AnnualCharge = 5000 },
               new CreditCardsModel { Id = 2, CardType = "Platinum", CreditLimit = 6000, AnnualCharge = 900 }
             };

        public void AddCreditCard(CreditCardsModel creditCard)
        {
            var model=_creditCards.OrderByDescending(x => x.Id).FirstOrDefault();
            creditCard.Id = model == null ? 1 : model.Id + 1;
            _creditCards.Add(creditCard);
        }

        public IEnumerable<CreditCardsModel> GetAllCreditCards()
        {
           return _creditCards.ToList();
        }

        public CreditCardsModel GetCreditCard(int id)
        {
            return _creditCards.FirstOrDefault(x=>x.Id==id);
        }

        public void UpdateCreditCard(CreditCardsModel creditCard)
        {
            var index = _creditCards.FindIndex(x => x.Id == creditCard.Id);
            if (index == -1)
                throw new ArgumentException("Credit card not found");
            _creditCards[index] = creditCard;
        }

        public void DeleteCreditCard(int id)
        {
            var cart = _creditCards.FirstOrDefault(x => x.Id == id);
            if (cart != null)
                _creditCards.Remove(cart);
        }
    }
}
