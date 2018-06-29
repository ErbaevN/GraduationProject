using ReStart2.Models.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReStart2.Models
{
    /// <summary>
    /// Класс(Данные) который будет передоваться на View
    /// </summary>
    public class TableModel
    {
        public Table Table { get; set; }
        public User User { get; set; }
        public View View { get; set; }        
    }
   
}
