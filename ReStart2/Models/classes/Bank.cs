//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ReStart2.Models.classes
//{
//    public class Bank
//    {
//    }
//}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReStart2.Models.classes
{
    public class Bank
    {
        public int Coins { get; set; }
        public int Comission { get; set; }

        public Bank()
        {
            Coins = 0;
            Comission = 0;
        }

        public void AddCoins(int count)
        {
            Coins += count;
            CalculationComission();
        }

        public int GetCoins()
        {
            return Coins;
        }

        public void CleardCoins()
        {
            Coins = 0;
            Comission = 0;
        }

        public void CalculationComission()
        {
            Comission = Convert.ToInt32(Convert.ToDouble(Coins) / 100 * 5);
        }
    }
}
