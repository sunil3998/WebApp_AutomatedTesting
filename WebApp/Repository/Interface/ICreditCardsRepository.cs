using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Repository.Interface
{
    public interface ICreditCardsRepository
    {
        CreditCardsModel GetCreditCard(int id);
        IEnumerable<CreditCardsModel> GetAllCreditCards();
        void AddCreditCard(CreditCardsModel creditCard);
        void UpdateCreditCard(CreditCardsModel creditCard);
        void DeleteCreditCard(int id);
    }
}
