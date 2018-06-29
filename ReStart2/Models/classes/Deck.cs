//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ReStart2.Models.classes
//{
//    public class Deck
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
    public class Deck
    {
        public List<Card> Cards { get; set; }

        public Deck()
        {
            Cards = new List<Card>
            {
                new Card(2, "крести"),
                new Card(3, "крести"),
                new Card(4, "крести"),
                new Card(5, "крести"),
                new Card(6, "крести"),
                new Card(7, "крести"),
                new Card(8, "крести"),
                new Card(9, "крести"),
                new Card(10, "крести"),
                new Card(11, "крести"),
                new Card(12, "крести"),
                new Card(13, "крести"),
                new Card(14, "крести"),

                new Card(2, "пики"),
                new Card(3, "пики"),
                new Card(4, "пики"),
                new Card(5, "пики"),
                new Card(6, "пики"),
                new Card(7, "пики"),
                new Card(8, "пики"),
                new Card(9, "пики"),
                new Card(10, "пики"),
                new Card(11, "пики"),
                new Card(12, "пики"),
                new Card(13, "пики"),
                new Card(14, "пики"),

                new Card(2, "черви"),
                new Card(3, "черви"),
                new Card(4, "черви"),
                new Card(5, "черви"),
                new Card(6, "черви"),
                new Card(7, "черви"),
                new Card(8, "черви"),
                new Card(9, "черви"),
                new Card(10, "черви"),
                new Card(11, "черви"),
                new Card(12, "черви"),
                new Card(13, "черви"),
                new Card(14, "черви"),

                new Card(2, "буби"),
                new Card(3, "буби"),
                new Card(4, "буби"),
                new Card(5, "буби"),
                new Card(6, "буби"),
                new Card(7, "буби"),
                new Card(8, "буби"),
                new Card(9, "буби"),
                new Card(10, "буби"),
                new Card(11, "буби"),
                new Card(12, "буби"),
                new Card(13, "буби"),
                new Card(14, "буби")
            };
        }

        public void DropCard(int item)
        {
            Cards.RemoveAt(item);
        }

        public void Restoring()
        {
            Cards = new Deck().Cards;
        }

    }
}
