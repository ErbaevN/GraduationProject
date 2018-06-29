using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ReStart2.Models;
using Microsoft.AspNetCore.Authorization;

namespace ReStart2.Models.classes
{
    public class Table
    {
        public List<User> Users { get; set; }
        public Bank Bank { get; set; }
        public Deck Deck { get; set; }
        public int Size { get; set; }
        public List<Card> CardsOnTable { get; set; }
        public bool StatusPause { get; set; }
        public List<Rate> Rates { get; set; }
        public List<User> ReadyPlayers { get; set; }
        public User Dealer { get; set; }
        public string Name { get; set; }
        public User StepUser { get; set; }
        public int Id { get; set; }
        public List<User> PlayUsers { get; set; }
        private static int count;
        public bool[] BettingCircles { get; set; }            // возможно не понадбетса не используеться полноценно
        public List<User> TableCanWinnner { get; set; }
        int shetchik = 0;
        public int MaxStake { get; set; }
        public string Strings { get; set; }  // Данные которые нужно записать в консоль игры
        public int Shetchik { get; set; }
        
        public Table(int size, string name)
        {
            TableCanWinnner = new List<User>();
            Users = new List<User>();
            Bank = new Bank();          
            Deck = new Deck();
            Size = size;
            CardsOnTable = new List<Card>();
            StatusPause = true;
            ReadyPlayers = new List<User>();
            Dealer = null;
            StepUser = null;
            Name = name;
            Id = ++count;
            Rates = new List<Rate>();
            PlayUsers = new List<User>();
            BettingCircles = new bool[] { false, false, false, false, false };
            MaxStake = 0;
        }


        public void AddUser(User user)
        {
            if (user.Email != null)
            {
                if (Size > Users.Count && user.Coins >= 10000)
                {
                    user.Coins -= 10000;
                    user.CoinsTable += 10000;
                    Users.Add(user);
                    ReadyPlayers.Add(user);
                    user.PlayIntable = this;
                }
            }
            //Strings += "Игрок "+ User.
        }

        public void DropUser(User user)
        {
            user.Coins += user.CoinsTable;
            user.CoinsTable = 0;
            Users.Remove(user);
        }

        public User DealerQueue()
        {
            if (Dealer == null)
            {
                Dealer = PlayUsers.First();
                return PlayUsers.First();
            }
            else
            {
                if (PlayUsers.IndexOf(Dealer) == 0)
                {
                    Dealer = PlayUsers[PlayUsers.Count - 1];
                    return PlayUsers[PlayUsers.Count - 1];
                }
                else
                {
                    int index = PlayUsers.IndexOf(Dealer) - 1;
                    Dealer = PlayUsers[index];
                    return PlayUsers[index];
                }
            }
        }


        /// <summary>
        /// Раздача карт
        /// </summary>
        public void GiveOutCards()
        {
            Random r = new Random();
            foreach (User user in PlayUsers)
            {
                int randomIndex = r.Next(0, Deck.Cards.Count);
                user.CardsInHand.Add(Deck.Cards[randomIndex]);
                Deck.DropCard(randomIndex);
            }

            foreach (User user in PlayUsers)
            {
                int randomIndex = r.Next(0, Deck.Cards.Count);
                user.CardsInHand.Add(Deck.Cards[randomIndex]);
                Deck.DropCard(randomIndex);
            }
        }

        /// <summary>
        /// поставит на стол 3 карты
        /// </summary>
        public void GetFlop()
        {
            Random r = new Random();

            int index = r.Next(0, Deck.Cards.Count);
            CardsOnTable.Add(Deck.Cards[index]);
            Deck.DropCard(index);

            index = r.Next(0, Deck.Cards.Count);
            CardsOnTable.Add(Deck.Cards[index]);
            Deck.DropCard(index);

            index = r.Next(0, Deck.Cards.Count);
            CardsOnTable.Add(Deck.Cards[index]);
            Deck.DropCard(index);
        }

        /// <summary>
        /// выдать следуюшую карту на стол
        /// </summary>
        public void GetNextCardOnTable()
        {
            if (CardsOnTable.Count < 5)
            {
                Random r = new Random();

                int index = r.Next(0, Deck.Cards.Count);
                CardsOnTable.Add(Deck.Cards[index]);
                Deck.DropCard(index);
            }
            else
                throw new Exception();
        }

        /// <summary>
        /// Метод записываюший список пользователей готовых играть
        /// Но не записывает в список играюших если и игра уже идет
        /// </summary>
        /// <returns></returns>
        public bool ReadyUsersPlay()
        {
            if (Users.Count < 2)
            {
                StatusPause = true;
                return false;
            }

            List<User> result = new List<User>();
            foreach (var user in Users)
            {
                if (user.CoinsTable > 100)
                {
                    result.Add(user);
                }
            }

            if (result.Count < 2)
            {
                StatusPause = true;
                ReadyPlayers = null;
                return false;
            }

            ReadyPlayers = result;
            if (StatusPause==true && TableCanWinnner.Count==0)
            {
                List<User> temp = new List<User>();
                foreach (var user in result)
                {
                    temp.Add(user);
                }
                PlayUsers = temp;
                StatusPause = false;
            }
            return true;
        }

        /// <summary>
        /// Распределение выгрещей
        /// </summary>
        /// <param name="users"></param>
        public void DistributionOfWinnings(List<User> users)
        {
            Bank.CalculationComission();
            int sumWinnings = Convert.ToInt32((Convert.ToDouble(Bank.Coins - Bank.Comission)) / Convert.ToDouble(users.Count));
            foreach (User user in users)
            {
                user.AddCoinsOnTable(sumWinnings);
            }
            Bank.CleardCoins();
            foreach (var playUser in PlayUsers)
            {
                playUser.Stake = 0;
                playUser.CardsInHand.Clear();
            }
            CardsOnTable.Clear();
            Rates.Clear();
            MaxStake = 0;
            Deck = new Deck();
            StatusPause = true;
        }

        /// <summary>
        /// Записывает в свойство StepUser пользователя который ходит
        /// </summary>
        public void Step()
        {
            int countPlayers = PlayUsers.Count;
            if (countPlayers != 1)
            {
                if (shetchik >= countPlayers)
                {
                    bool allAligned = true;
                    foreach (var u in PlayUsers)
                    {
                        if (MaxStake != u.Stake)
                        {
                            allAligned = false;
                        }
                    }
                    if (allAligned)
                    {
                        shetchik = 1;
                        int t = PlayUsers[0].Stake;
                        if (CardsOnTable.Count == 0)
                        {
                            BettingCircles[0] = true;
                            GetFlop();
                            Bank.AddCoins((PlayUsers[0].Stake * PlayUsers.Count) - Bank.Coins);
                            CalculatedNextUserStep();
                        }
                        else if (CardsOnTable.Count > 2 && CardsOnTable.Count < 5)
                        {
                            if (t != PlayUsers[0].Stake)
                            {
                                Bank.AddCoins((PlayUsers[0].Stake - 0) * PlayUsers.Count);
                            }
                            GetNextCardOnTable();
                            CalculatedNextUserStep();
                        }
                        else if (CardsOnTable.Count == 5)
                        {                           
                            if (TableCanWinnner.Count == 0) {
                                TableCanWinnner = Winner_of_the_Game();
                                StatusPause = true;                                
                            }
                            //else {
                            //    DistributionOfWinnings(TableCanWinnner);
                                
                            //TableCanWinnner = null;
                            //}
                        }
                    }
                    else
                    {
                        CalculatedNextUserStep();
                    }
                }
                else
                {
                    CalculatedNextUserStep();
                    shetchik++;
                }
            }
            else
            {
                DistributionOfWinnings(PlayUsers);
            }
        }

        public List<User> Winner_of_the_Game()
        {
            foreach (var Us in Users)
            {
                //Собираем 5 + 2 карты в Card_in_Combination
                List<Card> Card_in_Combination = new List<Card>();
                foreach (var i in CardsOnTable)
                {
                    Card_in_Combination.Add(i);
                }

                foreach (var j in Us.CardsInHand)
                {
                    Card_in_Combination.Add(j);
                }

                int temp = 0;
                int k = 0;
                Us.Kikker[0].Lear = 0;
                Us.Kikker[1].Lear = 0;
                Us.Kikker[2].Lear = 0;
                Us.Kikker[3].Lear = 0;
                Us.Kikker[4].Lear = 0;
                Us.Kikker[5].Lear = 0;
                //Ищу комбинации 
                Card tempcard = new Card(0, "");
                Card tempcard2 = new Card(0, "");
                int sovpoden = 0;
                int sovpoden2 = 0;
                foreach (var i in Card_in_Combination)//1-й
                {
                    temp = 0;
                    foreach (var j in Card_in_Combination)
                    {
                        if ((i.Lear == j.Lear) && (i.Meaning != j.Meaning))
                        {
                            temp += 1;
                        }
                    }
                    if (temp > sovpoden)
                    {
                        sovpoden = temp;
                        tempcard = i;
                    }
                    if ((temp == sovpoden) && (tempcard.Lear < i.Lear))
                    {
                        tempcard = i;
                    }
                }
                foreach (var i in Card_in_Combination)//2-й
                {
                    temp = 0;
                    foreach (var j in Card_in_Combination)
                    {
                        if ((i.Lear == j.Lear) && (i.Meaning != j.Meaning) && (i.Lear != tempcard.Lear))
                        {
                            temp += 1;
                        }
                    }
                    if (temp > sovpoden2)
                    {
                        sovpoden2 = temp;
                        tempcard2 = i;
                    }
                    if ((temp == sovpoden2) && (tempcard2.Lear < i.Lear))
                    {
                        tempcard2 = i;
                    }
                }
                switch (sovpoden)
                {
                    case 1:
                        {
                            if ((Us.Force <= 1) && (Us.MaxValue.Lear < tempcard.Lear))
                            {
                                Us.Force = 1;
                                Us.MaxValue.Lear = tempcard.Lear; k = 2;
                            }
                            break;
                        }
                    case 2:
                        {
                            if ((Us.Force <= 3) && (Us.MaxValue.Lear < tempcard.Lear))
                            {
                                Us.Force = 3;
                                Us.MaxValue.Lear = tempcard.Lear; k = 3;
                            }
                            break;
                        }
                    case 3:
                        {
                            if (Us.Force <= 7)
                            {
                                Us.Force = 7;
                                Us.MaxValue.Lear = tempcard.Lear; k = 4;
                            }
                            break;
                        }

                }
                switch (sovpoden2)
                {
                    case 1:
                        {
                            if (Us.Force < 2)
                            {
                                Us.Force = 2;
                                Us.MaxValue2.Lear = tempcard2.Lear; k = 4;
                            }
                            if (Us.Force == 3)
                            {
                                Us.Force = 6;
                                Us.MaxValue2.Lear = tempcard2.Lear; k = 5;
                            }
                            break;
                        }
                    case 2:
                        {
                            if (Us.Force < 6)
                            {
                                Us.Force = 6;
                                Us.MaxValue2.Lear = tempcard2.Lear;
                            }
                            break;
                        }
                }

                foreach (var i in Card_in_Combination) //определяю киккер но для флеша будет отдельно ниже, стриту он не нужен
                {
                    temp = 0;
                    foreach (var j in Card_in_Combination)
                    {
                        if ((j.Lear > temp) && ((j.Lear < Us.Kikker[k].Lear) || (Us.Kikker[k].Lear == 0))) { temp = j.Lear; }
                    }
                    if (k < 5)
                    {
                        Us.Kikker[k + 1].Lear = temp; k++;
                    }
                }
                foreach (var i in Card_in_Combination)//стрит 
                {
                    temp = 1;
                    for (int ki = 1; ki < 8; ki++)
                    {
                        int t = temp;
                        foreach (var j in Card_in_Combination)
                        {
                            if (i.Lear + ki == j.Lear)
                            {
                                temp += 1;
                                if (temp >= 5)
                                {
                                    if (Us.Force <= 4)
                                    {
                                        Us.Force = 4;
                                        Us.MaxValue.Lear = j.Lear;
                                        Us.MaxValue2.Lear = 0;
                                        Us.Kikker[0].Lear = 0;
                                        Us.Kikker[1].Lear = 0;
                                        Us.Kikker[2].Lear = 0;
                                        Us.Kikker[3].Lear = 0;
                                        Us.Kikker[4].Lear = 0;
                                        Us.Kikker[5].Lear = 0;
                                    }
                                }
                                break;
                            }
                        }
                        if (t == temp) { break; }
                    }
                }
                int temp1 = 0;
                foreach (var i in Card_in_Combination)//флеш
                {
                    temp = 1;
                    foreach (var j in Card_in_Combination)
                    {
                        if ((i.Meaning == j.Meaning) && (i.Lear != j.Lear))
                        {
                            temp += 1;
                            if (temp >= 5)
                            {
                                if (Us.Force <= 5)
                                {
                                    k = 0;
                                    Us.Kikker[0].Lear = 0;
                                    Us.Kikker[1].Lear = 0;
                                    Us.Kikker[2].Lear = 0;
                                    Us.Kikker[3].Lear = 0;
                                    Us.Kikker[4].Lear = 0;
                                    Us.Kikker[5].Lear = 0;
                                    foreach (var i1 in Card_in_Combination) //определяю киккер для флеша
                                    {
                                        temp1 = 0;
                                        foreach (var j1 in Card_in_Combination)
                                        {
                                            if ((j1.Lear > temp1) && ((j1.Lear < Us.Kikker[k].Lear) || (Us.Kikker[k].Lear == 0)) && (j1.Meaning == i.Meaning)) { temp1 = j1.Lear; }
                                        }
                                        if (k < 5)
                                        {
                                            Us.Kikker[k + 1].Lear = temp1; k++;
                                        }
                                    }
                                    Us.Force = 5;
                                    Us.MaxValue.Lear = 0;
                                    Us.MaxValue2.Lear = 0;

                                }
                            }
                        }

                    }

                }
                foreach (var i in Card_in_Combination)//стритфлеш
                {
                    temp = 1;
                    for (int ki = 1; ki < 8; ki++)
                    {
                        int tyk = temp;
                        foreach (var j in Card_in_Combination)
                        {
                            if ((i.Lear + ki == j.Lear) && (i.Meaning == j.Meaning))
                            {
                                temp += 1;
                                if (temp >= 5)
                                {
                                    if (Us.Force <= 8)
                                    {
                                        Us.Force = 8;
                                        Us.MaxValue.Lear = j.Lear;
                                        Us.MaxValue2.Lear = 0;
                                        Us.Kikker[0].Lear = 0;
                                        Us.Kikker[1].Lear = 0;
                                        Us.Kikker[2].Lear = 0;
                                        Us.Kikker[3].Lear = 0;
                                        Us.Kikker[4].Lear = 0;
                                        Us.Kikker[5].Lear = 0;
                                    }
                                }
                                break;
                            }
                        }
                        if (tyk == temp) { break; }
                    }
                }

            }
            User tempUser = new User("");
            foreach (var Us in Users)
            {
                if ((Us.Force > tempUser.Force) || ((Us.Force == tempUser.Force) && (Us.MaxValue.Lear > tempUser.MaxValue.Lear)) ||
                ((Us.Force ==
                tempUser.Force) && (Us.MaxValue.Lear == tempUser.MaxValue.Lear) && (Us.MaxValue2.Lear > tempUser.MaxValue2.Lear))) { tempUser = Us; }
                else if
                ((Us.Force == tempUser.Force) && (Us.MaxValue.Lear == tempUser.MaxValue.Lear) && (Us.MaxValue2.Lear == tempUser.MaxValue2.Lear))
                {
                    for (int i = 1; i < 6; i++)
                    {
                        if (Us.Kikker[i].Lear > tempUser.Kikker[i].Lear) { tempUser = Us; break; }
                        else if (Us.Kikker[i].Lear < tempUser.Kikker[i].Lear) { break; }
                    }

                }
            }
            List<User> Winners = new List<User>();
            foreach (var Us in Users)
            {
                if
                ((Us.Force == tempUser.Force) && (Us.MaxValue.Lear == tempUser.MaxValue.Lear) && (Us.MaxValue2.Lear == tempUser.MaxValue2.Lear))
                {
                    for (int i = 1; i < 6; i++)
                    {
                        if (Us.Kikker[i].Lear > tempUser.Kikker[i].Lear) { break; }
                        else if (Us.Kikker[i].Lear < tempUser.Kikker[i].Lear) { break; }
                        if (i == 5) { Winners.Add(Us); }
                    }
                }
            }
            return (Winners);
        }

        public void CalculatedNextUserStep()
        {
            if (PlayUsers.Count > 3)
            {
                if (StepUser == null)
                {
                    StepUser = PlayUsers[3];
                }
                else
                {
                    if (PlayUsers.IndexOf(StepUser) == 0)
                    {
                        StepUser = PlayUsers[PlayUsers.Count - 1];
                    }
                    else
                    {
                        StepUser = PlayUsers[PlayUsers.IndexOf(StepUser) - 1];
                    }
                }
            }
            else if (PlayUsers.Count == 3)
            {
                if (StepUser == null)
                {
                    StepUser = PlayUsers[0];
                }
                else
                {
                    if (PlayUsers.IndexOf(StepUser) == 0)
                    {
                        StepUser = PlayUsers[PlayUsers.Count - 1];
                    }
                    else
                    {
                        StepUser = PlayUsers[PlayUsers.IndexOf(StepUser) - 1];
                    }
                }
            }
            else if (PlayUsers.Count == 2)
            {
                if (StepUser == null)
                {
                    StepUser = PlayUsers[1];
                }
                else
                {
                    if (PlayUsers.IndexOf(StepUser) == 0)
                    {
                        StepUser = PlayUsers[1];
                    }
                    else
                    {
                        StepUser = PlayUsers[0];
                    }
                }
            }
        }

        public void Blinds()
        {
            if (PlayUsers.Count == 2)
            {
                if (PlayUsers[0] == Dealer)
                {
                    Dealer.CoinsTable -= 50;
                    Rates.Add(new Rate { EmailUser = Dealer.Email, Coins = 50 });
                    Dealer.Stake = 50;
                    PlayUsers[1].CoinsTable -= 100;
                    Rates.Add(new Rate { EmailUser = PlayUsers[1].Email, Coins = 100 });
                    PlayUsers[1].Stake = 100;
                }
                else
                {
                    Dealer.CoinsTable -= 50;
                    Rates.Add(new Rate { EmailUser = Dealer.Email, Coins = 50 });
                    Dealer.Stake = 50;
                    PlayUsers[0].CoinsTable -= 100;
                    Rates.Add(new Rate { EmailUser = PlayUsers[0].Email, Coins = 100 });
                    PlayUsers[0].Stake = 100;
                }
            }
            else
            {
                int indexDealer = PlayUsers.IndexOf(Dealer);
                User user1;
                User user2;
                if (indexDealer + 1 == PlayUsers.Count)
                {
                    user1 = PlayUsers[0];
                    user2 = PlayUsers[1];
                }
                else if (indexDealer + 1 == PlayUsers.Count - 1)
                {
                    user1 = PlayUsers.Last();
                    user2 = PlayUsers.First();
                }
                else
                {
                    user1 = PlayUsers[indexDealer + 1];
                    user2 = PlayUsers[indexDealer + 2];
                }

                user1.CoinsTable -= 50;
                Rates.Add(new Rate { EmailUser = user1.Email, Coins = 50 });
                user2.CoinsTable -= 100;
                Rates.Add(new Rate { EmailUser = user2.Email, Coins = 100 });
            }
            foreach (var stake in Rates)
            {
                Bank.AddCoins(stake.Coins);
            }
        }

    }
}
