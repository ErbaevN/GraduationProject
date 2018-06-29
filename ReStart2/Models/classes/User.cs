//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ReStart2.Models.classes
//{
//    public class User
//    {
//    }
//}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;


namespace ReStart2.Models.classes
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        //public string Avatar { get; set; }
        public int Coins { get; set; }
        public bool InTheGame { get; set; }
        public int Stake { get; set; }
        public int CoinsTable { get; set; }
        public List<Card> CardsInHand { get; set; }
        public Table PlayIntable { get; set; }
        public Card[] Kikker { get; set; }
        public int Force { get; set; }
        public Card MaxValue { get; set; }
        public Card MaxValue2 { get; set; }
        public int Hop { get; set; }

        public User(string email/*, string avatar*/)
        {
            Hop = 0;
            this.Email = email;
            //this.Avatar = avatar;
            this.InTheGame = true;
            this.Coins = 35000;
            this.CoinsTable = 0;
            this.CardsInHand = new List<Card>();
            this.PlayIntable = null;
            this.Force = 0;
            this.MaxValue = new Card(0, "0");
            this.MaxValue2 = new Card(0, "0");
            this.Kikker = new Card[6] {new Card(0,"0"), new Card(0, "0"), new Card(0, "0"), new Card(0, "0"), new Card(0, "0"), new Card(0, "0") };
        }

        public void Fold(Bank bank)
        {
            InTheGame = false;
            bank.Coins += Stake;
            Stake = 0;
            bank.CalculationComission();
        }

        public int GetId()
        {
            return Id;
        }

        public string GetEmail()
        {
            return Email;
        }

        public int GetCoins()
        {
            return Coins;
        }

        public int GetCoinsOnTable()
        {
            return CoinsTable;
        }


        public void AddCoinsOnTable(int sum)
        {
            CoinsTable += sum;
        }

        /// <summary>
        /// возвращяет ссылку url для изменения пароля
        /// </summary>
        public string GenerateLink()
        {
            return "localhost:8080/editPassword/" + AdminBot.SHA256(this.Email );
        }

        

        /// <summary>
        /// Метод для проверкий достаточно coin-ов чтоб сесть за стол или нет
        /// </summary>
        /// <returns>Если достаточно возвращяет true, если нет тогда false</returns>
        public bool CheckCoins()
        {
            if (Coins >= 10000)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Код написан так чтобы за стол размером 2 и когда стол полон, нельзя было сесть смотреть
        /// </summary>
        /// <param name="table"></param>
        public void Watch(Table table)
        {
            if (table.Size > 2 && table.Size != table.Users.Count)
            {
                Coins += CoinsTable;
                CoinsTable = 0;
                table.Users.Add(this); // некогда так не делал надеюсь будет работать
            }
        }

    }

}
