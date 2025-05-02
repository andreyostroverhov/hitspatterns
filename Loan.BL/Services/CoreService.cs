using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan.BL.Services
{
    public class CoreService
    {
        private readonly HttpClient client = new HttpClient();

        public async Task Transaction()
        {
            //перевод денег со счета на счет клиента при взятии кредита
        }
    }
}
