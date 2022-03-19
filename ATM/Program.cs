using System;
using System.Data.SQLite;

namespace ATM
{
    class Program
    {
        static void Main(string[] args)
        {
            string cs = @"URI=file:C:\Users\Lucas\source\repos\ATM\ATM\Database.db";

            var con = new SQLiteConnection(cs);
            con.Open();
        
            var atm = new Program();
            Console.WriteLine("Welcome to the bank!");
            int number = atm.Login(con);

            atm.Userland(con, number);
            
        }
        public void Userland(object con, int cardNumber)
        {
            int Number = 0;
            string Name = null;
            int Balance = 0;
            bool menu = true;
            while (menu)
            {
                SQLiteDataReader rdr = (SQLiteDataReader)SQLExecute(con, $"SELECT * FROM users WHERE Number= {cardNumber}");
                while (rdr.Read())
                {
                    Number = rdr.GetInt32(1);
                    Name = rdr.GetString(3);
                    Balance = rdr.GetInt32(4);
                }
            
                Console.WriteLine($"Hello {Name}");
                Console.WriteLine($"Your balance is: {Convert.ToString(Balance)}");

                Console.WriteLine("1. Withdraw money");
                Console.WriteLine("2. Transfer money");
                Console.WriteLine("3. Exit");

                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "1":
                        withdrawMoney(con, Number, Balance);
                        break;
                    case "2":
                        transferMoney(con, Number, Balance);
                        break;
                    case "3":
                        menu = false;
                        break;
                }
                Console.Clear();
            }
            
        }
        public bool withdrawMoney(object con, int Number, int Balance)
        {
            Console.WriteLine("How much money dou you like to withdraw?");
            string amount = Console.ReadLine();
            int newBalance = Balance - Int32.Parse(amount);

            SQLExecute(con, $"UPDATE users SET Balance={newBalance} WHERE Number={Number}");

            Console.WriteLine("Successfully updatet");

            return true;
        }
        public bool transferMoney(object con, int Number, int Balance)
        {
            int transferBalance = 0;
            Console.WriteLine("On which account do you like to transfer?");
            string transferAccount = Console.ReadLine();
            Console.WriteLine("How much money dou you like to transfer?");
            string transferAmount = Console.ReadLine();
            int newBalance = Balance - Int32.Parse(transferAmount);
            SQLExecute(con, $"UPDATE users SET Balance={newBalance} WHERE Number={Number}");

            SQLiteDataReader rdr = (SQLiteDataReader)SQLExecute(con, $"SELECT Balance FROM users WHERE Number={transferAccount}");
            while (rdr.Read())
            {
                transferBalance = rdr.GetInt32(0);
            }
            transferBalance =+ Int32.Parse(transferAmount);
            SQLExecute(con, $"UPDATE users SET Balance={transferBalance} WHERE Number={transferAccount}");
            Console.WriteLine("Successfully transfer");
            return true;
        }
        public int Login(object con)
        {
            bool correct = true;
            int attemps = 0;
            string cardNumber = null;

            while (correct)
            {
                if (attemps == 3)
                {
                    System.Environment.Exit(1);
                }
                Console.WriteLine("Enter your card number: ");
                cardNumber = Console.ReadLine();
                Console.WriteLine("Enter your pin: ");
                string pin = Console.ReadLine();
                if (!checkLogin(con, Int32.Parse(cardNumber), Int32.Parse(pin)))
                {
                    attemps++;
                }else
                {
                    correct = false;
                }
            }
            Console.Clear();
            Console.WriteLine("Succesfully logged in");

            return Int32.Parse(cardNumber);
        }
        public bool checkLogin(object con, int cardNumber, int pin)
        {
            SQLiteDataReader rdr = (SQLiteDataReader)SQLExecute(con, $"SELECT Number, Pin FROM users  WHERE Number={cardNumber} AND Pin = {pin}");
            return rdr.Read();
        }
        public object SQLExecute(object con, string SQLString)
        {
            var cmd = new SQLiteCommand(SQLString, (SQLiteConnection)con);
            return cmd.ExecuteReader();
        }
    }
}
