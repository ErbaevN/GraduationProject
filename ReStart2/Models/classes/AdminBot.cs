using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ReStart2.Models.classes
{
    public class AdminBot
    {
        public List<User> ActiveUsers { get; set; }     // похоже не нужное поле на даннный 21.05.2018 момент не где не используеться
        public List<TableModel> TableModels { get; set; }

        public AdminBot()
        {
            TableModels = new List<TableModel>
            {
                new TableModel(){Table = new Table(2,"Table №1"){ StepUser = new User(".")}, View = new View(){ Buttons = new List<Button>
                {
                    new Button() { Value = "", Visible = false },
                    new Button() { Value = "", Visible = false },
                    new Button() { Value = "", Visible = false }
                },
                Input = new Input(){ Active = true, Visible = false, Value = null}
            }
        },
                new TableModel(){Table = new Table(4,"Table №2"){ StepUser = new User(".")}, View = new View(){ Buttons = new List<Button>
                {
                    new Button() { Value = "", Visible = false },
                    new Button() { Value = "", Visible = false },
                    new Button() { Value = "", Visible = false }
                },
                Input = new Input(){ Active = true, Visible = false, Value = null}
            }
        },
                new TableModel(){Table = new Table(6,"Table №3"){ StepUser = new User(".")}, View = new View(){ Buttons = new List<Button>
                {
                    new Button() { Value = "", Visible = false },
                    new Button() { Value = "", Visible = false },
                    new Button() { Value = "", Visible = false }
                },
                Input = new Input(){ Active = true, Visible = false, Value = null},
            }
        }
        };
            ActiveUsers = new List<User>();
        }

        public static string SHA256(string str)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(str));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        #region MethodCheckTables
        public Task<string> CheckTables()
        {

            List<TableModel> tablesSize2 = new List<TableModel>();
            List<TableModel> tablesSize4 = new List<TableModel>();
            List<TableModel> tablesSize6 = new List<TableModel>();

            foreach (TableModel tableModel in TableModels)
            {
                if (tableModel.Table.Size == 2)
                {
                    tablesSize2.Add(tableModel);
                }
                else if (tableModel.Table.Size == 4)
                {
                    tablesSize4.Add(tableModel);
                }
                else
                {
                    tablesSize6.Add(tableModel);
                }
            }

            bool fullnessTableSize2 = true;
            bool fullnessTableSize4 = true;
            bool fullnessTableSize6 = true;

            fullnessTableSize2 = CheckingTheTableForCompleteness(tablesSize2, 2);
            fullnessTableSize4 = CheckingTheTableForCompleteness(tablesSize4, 4);
            fullnessTableSize6 = CheckingTheTableForCompleteness(tablesSize6, 6);


            RemovingUnnecessaryTables(tablesSize2, fullnessTableSize2);
            RemovingUnnecessaryTables(tablesSize4, fullnessTableSize4);
            RemovingUnnecessaryTables(tablesSize6, fullnessTableSize6);

            if (fullnessTableSize2)
            {
                TableModels.Add(new TableModel()
                {
                    Table = new Table(2, $"Table №{TableModels.Count + 1}") { StepUser = new User(".") },
                    View = new View()
                    {
                        Buttons = new List<Button>
                {
                    new Button() { Value = "", Visible = false },
                    new Button() { Value = "", Visible = false },
                    new Button() { Value = "", Visible = false }
                },
                        Input = new Input() { Active = true, Visible = false, Value = null }
                    }
                });

            }
            if (fullnessTableSize4)
            {
                TableModels.Add(new TableModel()
                {
                    Table = new Table(4, $"Table №{TableModels.Count + 1}") { StepUser = new User(".") },
                    View = new View()
                    {
                        Buttons = new List<Button>
                {
                    new Button() { Value = "", Visible = false },
                    new Button() { Value = "", Visible = false },
                    new Button() { Value = "", Visible = false }
                },
                        Input = new Input() { Active = true, Visible = false, Value = null }
                    },
                }
                );
            }
            if (fullnessTableSize6)
            {
                TableModels.Add(new TableModel()
                {
                    Table = new Table(6, $"Table №{TableModels.Count + 1}") { StepUser = new User(".") },
                    View = new View()
                    {
                        Buttons = new List<Button>
                {
                    new Button() { Value = "", Visible = false },
                    new Button() { Value = "", Visible = false },
                    new Button() { Value = "", Visible = false }
                },
                        Input = new Input() { Active = true, Visible = false, Value = null }
                    }
                });
            }
            return new Task<string>(() => "Fake");
        }


        private void RemovingUnnecessaryTables(List<TableModel> listTableModels, bool fullnessTableSize)
        {
            if (!fullnessTableSize)
            {
                int countEmptyTables = 0;
                foreach (var tableModel in listTableModels)
                {
                    if (tableModel.Table.Users.Count == 0)
                    {
                        countEmptyTables++;
                        if (countEmptyTables > 1)
                        {
                            TableModels.Remove(tableModel);
                        }
                    }
                }
            }
        }


        private bool CheckingTheTableForCompleteness(List<TableModel> tablesSize2, int size)
        {
            bool check = true;
            foreach (TableModel tableModel in tablesSize2)
            {
                if (tableModel.Table.Users.Count != size)
                {
                    check = false;
                }
            }
            return check;
        }
        #endregion
    }
}
