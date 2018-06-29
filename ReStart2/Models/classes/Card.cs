//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ReStart2.Models.classes
//{
//    public class Card
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
    public class Card
    {
        public int Lear { get; set; }
        public string Meaning { get; set; }


        public Card(int lear, string meaning)
        {
            this.Lear = lear;
            this.Meaning = meaning;
        }



        //public int GetId()
        //{
        //    return this.Id;
        //}

        public string GetMeaning()
        {
            return this.Meaning;
        }


        public int GetLear()
        {
            return this.Lear;
        }
    }
}
