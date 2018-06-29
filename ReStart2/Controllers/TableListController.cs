using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReStart2.Models;
using ReStart2.Models.classes;
using ReStart2.Services;
using System.Collections.Generic;
using System.Linq;

namespace ReStart2.Controllers
{
    [Authorize]
    public class TableListController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly AdminBot _bot;

        public TableListController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            AdminBot bot)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _bot = bot;
        }

        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet]      
        //public IActionResult Tables()
        //{
        //    return View(_bot.TableModels);
        //}
        [HttpGet]
        public IActionResult Tables(int id)
        {
            TableModel tableModel = null;
            User user = null;
            foreach(var tableM in _bot.TableModels)
            {
                foreach(var u in tableM.Table.Users)
                {
                    if(u.Email == User.Identity.Name)
                    {
                        user = u;
                        tableModel = tableM;
                    }
                }
            }
            if (user != null)
            {
                if(tableModel.Table.StepUser == user)
                {                    
                    tableModel.Table.StepUser.CardsInHand.Clear();
                    tableModel.Table.StepUser.Stake = 0;
                    User tempUser = tableModel.Table.StepUser;
                    tableModel.Table.CalculatedNextUserStep();
                    tableModel.Table.Step();
                }
                foreach (var j in tableModel.Table.Users)
                { j.Hop = 5; }
                tableModel.User.Hop = 2;
                tableModel.Table.DropUser(user);
                tableModel.Table.ReadyPlayers.Remove(user);
                tableModel.Table.PlayUsers.Remove(user);
                if(tableModel.Table.PlayUsers.Count == 1)
                {
                    tableModel.Table.DistributionOfWinnings(tableModel.Table.PlayUsers);
                }
            }

            if (id == 1)
            {                
                       var sorted  = from a1 in _bot.TableModels
                                     orderby a1.Table.Id descending
                                     select a1;
                List<TableModel> temp = new List<TableModel>();
                foreach(var a2 in sorted)
                {
                    temp.Add(a2);
                }
                
                return View(temp);
            }
            else if (id == 2)
            {
                var sorted = from a1 in _bot.TableModels
                             orderby a1.Table.Users.Count descending
                             select a1;
                List<TableModel> temp = new List<TableModel>();
                foreach (var a2 in sorted)
                {
                    temp.Add(a2);
                }

                return View(temp);
            }
            else if (id == 3)
            {
                var sorted = from a1 in _bot.TableModels
                             orderby  a1.Table.Size descending
                             select a1;
                List<TableModel> temp = new List<TableModel>();
                foreach (var a2 in sorted)
                {
                    temp.Add(a2);
                }

                return View(temp);
            }
            else if (id == 4)
            {
                var sorted = from a1 in _bot.TableModels
                             orderby a1.Table.Id ascending
                             select a1;
                List<TableModel> temp = new List<TableModel>();
                foreach (var a2 in sorted)
                {
                    temp.Add(a2);
                }

                return View(temp);
            }
            else if (id == 5)
            {
                var sorted = from a1 in _bot.TableModels
                             orderby a1.Table.Users.Count ascending
                             select a1;
                List<TableModel> temp = new List<TableModel>();
                foreach (var a2 in sorted)
                {
                    temp.Add(a2);
                }

                return View(temp);
           }
            else if (id == 6)
            {
              var sorted = from a1 in _bot.TableModels
                             orderby a1.Table.Size ascending
                           select a1;
                List<TableModel> temp = new List<TableModel>();
                foreach (var a2 in sorted)
                {
                    temp.Add(a2);
                
                }  

                return View(temp);
            }
            else
            {
                return View(_bot.TableModels);
            }

        }

        [HttpPost]
        public IActionResult Table(Table table)
        {                              
            return RedirectPermanent("http://localhost:50309/Table/Table/"+table.Id);
        }
    }
}