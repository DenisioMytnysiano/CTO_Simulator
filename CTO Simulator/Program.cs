using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace CTO_Simulator
{
    class Test<T1, T2>
    {
        public List<Tuple<T1, T2>> tests;
        public Test()
        {
            this.tests = new List<Tuple<T1, T2>> { };
        }
        public void Add(T1 val1, T2 val2)
        {
            tests.Add(Tuple.Create(val1, val2));
        }
        public int Length => tests.Count;
        public Tuple<T1, T2> this[int index]
        {
            get
            {
                return tests[index];
            }
        }
    }
    class Client
    {
        public string name;
        public string surname;
        public int age;
        public string date;
        public Client(string name, string surname, int age, string date)
        {
            this.name = name;
            this.surname = surname;
            this.age = age;
            this.date = date;
        }
    }
    class Car
    {
        public string model;
        public List<string> bugs;
        public string color;
        public Car(string model, string color)
        {
            this.model = model;
            this.color = color;
        }
        public void Bugs(List<string> bugs) => this.bugs = bugs;
        
    }
    class CTO
    {
        public static Dictionary<string, int> time_for_fix = new Dictionary<string, int> { };
        public static Dictionary<string, int> price_for_fix = new Dictionary<string, int> { };
        public static Dictionary<string, List<string>> database = new Dictionary<string, List<string>> { };
        public static List<string> records = new List<string> { };
        public static Dictionary<string, int> materials = new Dictionary<string, int> { };
        public static Dictionary<string, int> orders = new Dictionary<string, int> { };
        public static int diagnostics=0;
        public static int fixes = 0;
        public int days_to_deliver_new_materials;
        public CTO()
        {
            time_for_fix["engine"] = 5;
            materials["engine"] = 3;
            price_for_fix["engine"] = 200;
            time_for_fix["wheels"] = 2;
            materials["wheels"] = 3;
            price_for_fix["wheels"] = 150;
            time_for_fix["cardan"] = 3;
            materials["cardan"] = 3;
            price_for_fix["cardan"] = 200;
            time_for_fix["glass"] = 3;
            materials["glass"] = 3;
            price_for_fix["glass"] = 250;
            time_for_fix["oil"] = 4;
            materials["oil"] = 3;
            price_for_fix["oil"] = 200;
            days_to_deliver_new_materials = 5;
            orders["engine"] = 0;
            orders["wheels"] = 0;
            orders["cardan"] = 0;
            orders["glass"] = 0;
            orders["oil"] = 0;
        }
        public bool checkVaranty(string date1, string date2, int varanty)
        {
            Console.WriteLine(date1 + " " + date2);
            var dd1 = date1.Split('-').Select(x => int.Parse(x)).ToList();
            var dd2 = date2.Split('-').Select(x => int.Parse(x)).ToList();
            DateTime d1 = new DateTime(dd1[2], dd1[1], dd1[0]);
            DateTime d2 = new DateTime(dd2[2], dd2[1], dd2[0]);
            if (Math.Abs(d1.Subtract(d2).TotalDays) < varanty * 30) {
                return true;
            }
            return false;
        }
        public void indentifyProblems(ref Car car, Random rnd)
        {
            List<string> problems = new List<string> { "engine", "cardan", "glass", "wheels", "oil" };
            car.Bugs(problems.Select(i => new { I = i, sort = Guid.NewGuid() }).OrderBy(i => i.sort).Select(i => i.I).Take(rnd.Next(1, 5)).ToList());
        }
        public void Diagnostics(Client client, Car car)
        {
            diagnostics += 1;
            Console.WriteLine(client.name + " " + client.surname + " came to diagnostics");
            Random rnd = new Random();
            int time_consume = 0;
            int money_consume = 0;
            indentifyProblems(ref car, rnd);
            foreach(var problem in car.bugs)
            {
                if (materials[problem] > 0)
                {
                    time_consume += time_for_fix[problem];
                    money_consume += price_for_fix[problem];
                }
                else
                {
                    Console.WriteLine("We have to buy some materials");
                    time_consume += days_to_deliver_new_materials;
                    time_consume += time_for_fix[problem];
                    money_consume += price_for_fix[problem];
                }
            }
            Console.WriteLine("Days to fix all the problems: " + time_consume + " days");
            Console.WriteLine("Price for fixing all the problems: " + money_consume + " $");
            database[client.name + " " + client.surname] = new List<string> { client.age.ToString(), car.model.ToString(), string.Join(",", car.bugs), time_consume.ToString(), money_consume.ToString(), client.date, "diagnostics"};
            records.Add(client.date+" "+client.name + " " + client.surname + " "+client.age.ToString()+" "+ car.model.ToString()+" " + string.Join(",", car.bugs)+" "+ time_consume.ToString()+" "+ money_consume.ToString()+" "+ "diagnostics");
        }
        public void Fix(Client client, Car car)
        {
            fixes += 1;
            int varanty = 6; //month
            Console.WriteLine(client.name + " " + client.surname + " came to fix his car");
            Random rnd = new Random();
            indentifyProblems(ref car, rnd);
            int time_consume = 0;
                int money_consume = 0;
                foreach (var problem in car.bugs)
                {
                    if (materials[problem] > 0)
                    {
                        time_consume += time_for_fix[problem];
                        money_consume += price_for_fix[problem];
                        materials[problem] -= 1;
                }
                    else
                    {
                        Console.WriteLine("Ordering new materials...");
                        materials[problem] += 2;
                        orders[problem] += 2;
                        time_consume += days_to_deliver_new_materials;
                        time_consume += time_for_fix[problem];
                        money_consume += price_for_fix[problem];
                        materials[problem] -= 1;
                }
            }
                if (database.Keys.Contains(client.name + " " + client.surname) && checkVaranty(database[client.name + " " + client.surname][5], client.date, varanty) && car.model == database[client.name + " " + client.surname][1] && database[client.name + " " + client.surname][6] == "fixing") { 
                    Console.WriteLine(client.name + " " + client.surname + " fixes a car using a varanty");
                Console.WriteLine("Days to fix all the problems: " + time_consume + " days");
                database[client.name + " " + client.surname] = new List<string> { client.age.ToString(), car.model.ToString(), string.Join(",", car.bugs), time_consume.ToString(), "0", client.date, "fixing for varanty" };
                records.Add(client.date +" "+client.name + " " + client.surname +" "+client.age.ToString() + " " + car.model.ToString() + " " + string.Join(",", car.bugs) + " " + time_consume.ToString() + " " + "0" + " " + "fixing for varanty");
            }
                else { 
                Console.WriteLine("Days to fix all the problems: " + time_consume + " days");
                Console.WriteLine("Price for fixing all the problems: " + money_consume + " $");
                database[client.name + " " + client.surname] = new List<string> { client.age.ToString(), car.model.ToString(), string.Join(",", car.bugs), time_consume.ToString(), money_consume.ToString(), client.date, "fixing" };
                records.Add(client.date+" "+client.name + " " + client.surname + " "+client.age.ToString() + " " + car.model.ToString() + " " + string.Join(",", car.bugs) + " " + time_consume.ToString() + " " + money_consume.ToString() + " " + "fixing");
            }

                if(rnd.Next(10000)%5 ==1)
            {
                Console.WriteLine("Car was fixed earlier");
                records[records.Count - 1] += "(earlier)";
            }
        }
            }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Denys Mytnyk IP-91 Variant 19 В");
            List<Client> clients = new List<Client> {
                new Client("Carlos", "Huan", 34, "14-05-2015"),
                new Client("Pedro", "Gonzales", 21, "11-06-2015"),
                new Client("Den", "Brown", 24, "23-07-2015"),
                new Client("Michael", "Lolo", 42, "14-08-2015"),
                new Client("Mark", "Abram", 37, "25-09-2015"),
                new Client("Den", "Brown", 24, "15-10-2015"),
                new Client("Carlos", "Huan", 34, "17-05-2017"),
                new Client("Pedro", "Gonzales", 21, "14-08-2017"),
                new Client("Toby", "Macho", 56, "14-05-2019"),
                new Client("Can", "Cone", 32, "14-08-2019")
            };
            List<Car> cars = new List<Car> {
                new Car("Ford Mustang", "red"),
                new Car("Lada Sedan", "blue"),
                new Car("Ford Fiesta", "green"),
                new Car("Ferrari 612", "white"),
                new Car("Citroen Cactus", "yellow"),
                new Car("Mercedes AMG", "red"),
                new Car("Ford Fiesta", "green"),
                new Car("Shigul", "red"),
                new Car("Geely Coolray", "red"),
                new Car("Audi A3", "red") 
            };
            Test<Client, Car> tests = new Test<Client, Car> { };
            for (int i = 0; i < clients.Count(); i++)
            {
                tests.Add(clients[i], cars[i]);
            }
            Random rnd = new Random();
            CTO cto = new CTO();
            for(int i = 0; i < tests.Length; i++)
            {

                if(rnd.Next(1000)%2== 1)
                {
                    cto.Fix(tests[i].Item1, tests[i].Item2);
                    Console.WriteLine();
                }
                else
                {
                    cto.Diagnostics(tests[i].Item1, tests[i].Item2);
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
            Console.WriteLine("The records of a CTO:\n");
            Console.WriteLine("Date Name Surname Age Car Problems Days Price Type");
            foreach(var record in CTO.records)
            {
                Console.WriteLine(record);
            }
            Console.WriteLine();
            Console.WriteLine("Summary:");
            Console.WriteLine("Diagnostics:" + CTO.diagnostics);
            Console.WriteLine("Fixes:" + CTO.fixes);
            if (CTO.orders.Count() != 0)
            {
                Console.WriteLine("Orders:");
                foreach(var key in CTO.orders.Keys)
                {
                    Console.WriteLine(key + ":" + CTO.orders[key]);
                }
            }
        }
    }
}
