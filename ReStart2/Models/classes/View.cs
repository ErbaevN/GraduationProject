using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReStart2.Models.classes
{
    /// <summary>
    /// Класс элементов которые нужно показывать или на оборот
    /// </summary>
    public class View
    {
        public List<Button> Buttons { get; set; }   // задуманно для кнопк "Принять, Скинуть, Поднять"
        public Input Input { get; set; }           // на данный момент только один input который принемает на сколько игрок хочет поднять ставку
        public int PozitionUser { get; set; }
    }
}
