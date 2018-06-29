using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReStart2.Models.classes;
using Microsoft.AspNetCore.Identity;
using ReStart2.Models;
using Microsoft.AspNetCore.Authorization;

namespace ReStart2.Controllers
{
    [Authorize]
    public class TableController : Controller
    {
        private AdminBot _adminBot;
        private readonly UserManager<ApplicationUser> _userManager;
        private TableModel _tableModel;

        public TableController(UserManager<ApplicationUser> userManager, AdminBot adminBot)
        {
            _adminBot = adminBot;
            _userManager = userManager;
            _tableModel = new TableModel();
        }


        [HttpPost]
        public IActionResult Call(int id)
        {
            TableModel tableM = null;
            foreach (var table in _adminBot.TableModels)
            {
                if (table.Table.Id == id)
                {
                    tableM = table;
                }
            }
            foreach(var u in tableM.Table.Users)
            {
                if(u.Email == User.Identity.Name)
                {
                    tableM.User = u;
                }
            }
            int maxRate = 0;
            int maxRateUser = 0;
            foreach (var rate in tableM.Table.Rates)
            {
                if(maxRate < rate.Coins)
                {
                    maxRate = rate.Coins;                    
                }
                if (maxRateUser < rate.Coins && rate.EmailUser == User.Identity.Name)
                {
                    maxRateUser = rate.Coins;
                }
            }            
            
            if (tableM.Table.StepUser.CoinsTable + maxRateUser - tableM.User.Stake >= 0)
            {
                tableM.Table.StepUser.CoinsTable -= maxRate - tableM.User.Stake;
                tableM.Table.StepUser.Stake = maxRate;                               
                tableM.Table.Rates.Add(new Rate() { Coins = maxRate, EmailUser = tableM.Table.StepUser.Email });
            }
            else
            {
                AllIn(tableM);
            }
            foreach (var j in tableM.Table.Users)
            { j.Hop = 5; }
                tableM.User.Hop = 2;
            tableM.Table.Step();
            ViewBag.tableModel = tableM;
            return Redirect("http://localhost:50309/Table/Table/"+id.ToString());            
        }

        private static void AllIn(TableModel tableM)
        {
            tableM.Table.Rates.Add(new Rate() { Coins = tableM.Table.StepUser.CoinsTable, EmailUser = tableM.Table.StepUser.Email });
            tableM.Table.StepUser.CoinsTable = 0;
        }

        [HttpPost]
        public IActionResult Fold(int id)
        {
            TableModel tableM = null;
            foreach (var table in _adminBot.TableModels)
            {
                if (table.Table.Id == id)
                {
                    tableM = table;
                }
            }
            tableM.Table.StepUser.CardsInHand.Clear();
            tableM.Table.StepUser.Stake = 0;
            User tempUser = tableM.Table.StepUser;
            tableM.Table.CalculatedNextUserStep();
            tableM.Table.PlayUsers.Remove(tempUser);
            /*if (tableM.Table.StepUser.Email == tableM.User.Email){}*/
            foreach (var j in tableM.Table.Users)
            { j.Hop = 5; }
            tableM.User.Hop = 2;
            tableM.Table.Shetchik = 0;
            tableM.Table.Step();
            return Redirect("http://localhost:50309/Table/Table/" + id.ToString());
        }

        [HttpPost]
        public IActionResult Raise(int id, int stavka)
        {
            TableModel tableM = null;
            foreach (var table in _adminBot.TableModels)
            {
                if (table.Table.Id == id)
                {
                    tableM = table;
                }
            }
            tableM.User = tableM.Table.Users.First(x => x.Email == User.Identity.Name);

            int maxRateUser = 0;
            foreach (var rate in tableM.Table.Rates)
            {
                    if (maxRateUser < rate.Coins && rate.EmailUser == User.Identity.Name)
                    {
                        maxRateUser = rate.Coins;
                    }
            }
            if (tableM.Table.StepUser.CoinsTable + maxRateUser - stavka>= 0)
            {
                tableM.Table.StepUser.CoinsTable -= stavka - tableM.User.Stake;
                tableM.Table.StepUser.Stake = stavka;
                tableM.Table.Rates.Add(new Rate() { Coins = stavka, EmailUser = tableM.Table.StepUser.Email });
            }
            else
            {
                AllIn(tableM);
            }
            foreach (var j in tableM.Table.Users)
            { j.Hop = 5; }
            tableM.User.Hop = 2;
            tableM.Table.Step();
            tableM.Table.Strings += "Игрок " +User.Identity.Name+ " поднял ставку до "+ stavka;
            return Redirect("http://localhost:50309/Table/Table/" + id.ToString());
        }

        [HttpGet]
        public IActionResult Table(int id)
        {
            var tableModel = _adminBot.TableModels.First(x => x.Table.Id == id);                               
            var user = new User(User.Identity.Name)
            {
                PlayIntable = tableModel.Table
            };
            bool check = false;
            foreach (var u in tableModel.Table.Users)
            {
                if (u.Email == user.Email)
                {
                    check = true;
                }
            }
            
            if (!check)
            {
                tableModel.Table.AddUser(user);
                _adminBot.CheckTables();
                tableModel.User = tableModel.Table.Users.First(x => x.Email == User.Identity.Name);               
                foreach (var j in tableModel.Table.Users)
                { j.Hop = 5; }
                tableModel.User.Hop = 2;
            }
            if(tableModel.User == null)
            {
                tableModel.User = tableModel.Table.Users.First(x => x.Email == User.Identity.Name);
            }
            else if(tableModel.User.Email != User.Identity.Name)
            {
                tableModel.User = tableModel.Table.Users.First(x => x.Email == User.Identity.Name);
            }
            //if (tableModel.Table.TableCanWinnner.Count != 0) { tableModel.Table.Step(); }
            if (tableModel.Table.StatusPause)
            {
                if (tableModel.Table.ReadyUsersPlay())
                {
                    tableModel.Table.StatusPause = false;
                    tableModel.Table.Dealer = tableModel.Table.DealerQueue();
                    tableModel.Table.GiveOutCards();
                    { tableModel.Table.Blinds(); }
                       
                    tableModel.Table.Step();
                    if (User.Identity.Name == tableModel.Table.StepUser.Email)
                    {
                        foreach (var btn in tableModel.View.Buttons)
                        {
                            btn.Visible = true;
                        }
                        tableModel.View.Input.Visible = true;
                    }
                    else
                    {
                        foreach (var btn in tableModel.View.Buttons)
                        {
                            btn.Visible = false;
                        }
                        tableModel.View.Input.Visible = false;
                    }
                }

            }

           

            if (!tableModel.Table.StatusPause)
            {
                tableModel.View.PozitionUser = tableModel.Table.ReadyPlayers.IndexOf(tableModel.User);
                if (User.Identity.Name == tableModel.Table.StepUser.Email)
                {
                    foreach (var btn in tableModel.View.Buttons)
                    {
                        btn.Visible = true;
                    }
                    tableModel.View.Input.Visible = true;
                }
                else
                {
                    foreach (var btn in tableModel.View.Buttons)
                    {
                        btn.Visible = false;
                    }
                    tableModel.View.Input.Visible = false;
                }
            }

            foreach (var rate in tableModel.Table.Rates)
            {
                if (tableModel.Table.MaxStake < rate.Coins)
                {
                    tableModel.Table.MaxStake = rate.Coins;
                }
            }          
            return View(tableModel);            
        }

        [HttpPost]
        public string Copy(int id)
        {
            var tableModel = _adminBot.TableModels.First(x => x.Table.Id == id);
       
            var user = new User(User.Identity.Name)
            {
                PlayIntable = tableModel.Table
            };
            bool check = false;
            foreach (var u in tableModel.Table.Users)
            {
                if (u.Email == user.Email)
                {
                    check = true;
                }
            }
            if (tableModel.Table.TableCanWinnner.Count != 0)
            {
                tableModel.Table.Shetchik++;
                if (tableModel.Table.Shetchik == tableModel.Table.PlayUsers.Count + 1)
                {
                    tableModel.Table.DistributionOfWinnings(tableModel.Table.TableCanWinnner);                                         
                    tableModel.Table.Shetchik = 0;

                    tableModel.Table.StatusPause = true;
                    tableModel.Table.CardsOnTable.Clear();
                    tableModel.Table.Deck = new Deck();
                    foreach (var u in tableModel.Table.PlayUsers)
                    {
                        u.CardsInHand.Clear();
                    }
                    tableModel.Table.Bank.CleardCoins();
                    tableModel.Table.Bank.Comission = 0;
                    tableModel.Table.Rates.Clear();
                    tableModel.Table.MaxStake = 0;
                    tableModel.Table.TableCanWinnner.Clear();
                }
            }
            if (!check)
            {
                tableModel.Table.AddUser(user);
                _adminBot.CheckTables();
            }
            if (tableModel.Table.TableCanWinnner.Count != 0) { tableModel.Table.Step(); }
            if (tableModel.Table.StatusPause)
            {
                if (tableModel.Table.ReadyUsersPlay())
                {
                    tableModel.Table.StatusPause = false;
                    tableModel.Table.Dealer = tableModel.Table.DealerQueue();
                    tableModel.Table.GiveOutCards();
                    { tableModel.Table.Blinds(); }

                    tableModel.Table.Step();
                    if (User.Identity.Name == tableModel.Table.StepUser.Email)
                    {
                        foreach (var btn in tableModel.View.Buttons)
                        {
                            btn.Visible = true;
                        }
                        tableModel.View.Input.Visible = true;
                    }
                    else
                    {
                        foreach (var btn in tableModel.View.Buttons)
                        {
                            btn.Visible = false;
                        }
                        tableModel.View.Input.Visible = false;
                    }
                }

            }
            tableModel.User = tableModel.Table.Users.First(x => x.Email == User.Identity.Name);
            //tableModel.User = tableModel.Table.ReadyPlayers.First(x => x.Email == User.Identity.Name);

            if (!tableModel.Table.StatusPause)
            {
                tableModel.View.PozitionUser = tableModel.Table.ReadyPlayers.IndexOf(tableModel.User);
                if (User.Identity.Name == tableModel.Table.StepUser.Email)
                {
                    foreach (var btn in tableModel.View.Buttons)
                    {
                        btn.Visible = true;
                    }
                    tableModel.View.Input.Visible = true;
                }
                else
                {
                    foreach (var btn in tableModel.View.Buttons)
                    {
                        btn.Visible = false;
                    }
                    tableModel.View.Input.Visible = false;
                }
            }

            foreach (var rate in tableModel.Table.Rates)
            {
                if (tableModel.Table.MaxStake < rate.Coins)
                {
                    tableModel.Table.MaxStake = rate.Coins;
                }
            }
            foreach (var j in tableModel.Table.Users) {                                  
            if  (j.Hop!=2 && j == tableModel.User) {
                    j.Hop = 2;
                    return tableModel.User.Email;                 
            }
            
        }
            return ".";
        }

        [HttpGet]
        public TableModel Ajax(int idTable)
        {
            TableModel tableM = new TableModel();
            tableM.Table = new Table(1, "Pufff");
            ViewBag.tableModel = tableM;
            return tableM;
        }

    }
}