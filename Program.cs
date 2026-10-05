using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static ASSLINQ.ListGenerators;

namespace session13C_
{

    class AnagramEqualityComparer : IEqualityComparer<string>
    {
        private static string Normalize(string s) => new string(s.Trim().OrderBy(c => c).ToArray());
        public bool Equals(string x, string y) => Normalize(x) == Normalize(y);
        public int GetHashCode(string obj) => Normalize(obj).GetHashCode();
    }
    class Program
        {
            static void Section(string title) => Console.WriteLine($"\n==================== {title} ====================");
            static void Q(string title) => Console.WriteLine($"\n--- {title}");

            static void Main(string[] args)
            {
                int[] Arr = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
                // dictionary_english.txt read into an array of strings first
                string[] words = File.ReadAllLines("dictionary_english.txt");

                // ====================================================================
                Section("LINQ - Element Operators");
                // ====================================================================
                Q("1. First Product out of Stock");
                var outOfStock = ProductList.FirstOrDefault(p => p.UnitsInStock == 0);
                Console.WriteLine(outOfStock);

                Q("2. First product with Price > 1000 (null if none)");
                var expensive = ProductList.FirstOrDefault(p => p.UnitPrice > 1000);
                Console.WriteLine(expensive == null ? "null" : expensive.ToString());

                Q("3. Second number greater than 5");
                var second = Arr.Where(n => n > 5).Skip(1).First();   // or ElementAt(1)
                Console.WriteLine(second);

                // ====================================================================
                Section("LINQ - Aggregate Operators");
                // ====================================================================
                Q("1. Count of odd numbers");
                Console.WriteLine(Arr.Count(n => n % 2 != 0));

                Q("2. Customers and how many orders each has");
                var custOrders = CustomerList.Select(c => new { c.CustomerName, OrdersCount = c.Orders.Length });
                foreach (var c in custOrders) Console.WriteLine($"{c.CustomerName} => {c.OrdersCount}");

                Q("3. Categories and how many products each has");
                var catCount = ProductList.GroupBy(p => p.Category)
                                          .Select(g => new { Category = g.Key, ProductsCount = g.Count() });
                foreach (var c in catCount) Console.WriteLine($"{c.Category} => {c.ProductsCount}");

                Q("4. Total of the numbers in the array");
                Console.WriteLine(Arr.Sum());

                Q("5. Total number of characters of all words in the dictionary");
                Console.WriteLine(words.Sum(w => w.Length));

                Q("6. Length of the shortest word");
                Console.WriteLine(words.Min(w => w.Length));

                Q("7. Length of the longest word");
                Console.WriteLine(words.Max(w => w.Length));

                Q("8. Average length of the words");
                Console.WriteLine(words.Average(w => w.Length));

                Q("9. Total units in stock for each product category");
                foreach (var g in ProductList.GroupBy(p => p.Category)
                                             .Select(g => new { Category = g.Key, Total = g.Sum(p => p.UnitsInStock) }))
                    Console.WriteLine($"{g.Category} => {g.Total}");

                Q("10. Cheapest price among each category's products");
                foreach (var g in ProductList.GroupBy(p => p.Category)
                                             .Select(g => new { Category = g.Key, Cheapest = g.Min(p => p.UnitPrice) }))
                    Console.WriteLine($"{g.Category} => {g.Cheapest}");

                Q("11. Products with the lowest price in each category (using let)");
                var lowest = from p in ProductList
                             group p by p.Category into g
                             let minPrice = g.Min(p => p.UnitPrice)
                             select new { Category = g.Key, Products = g.Where(p => p.UnitPrice == minPrice) };
                foreach (var g in lowest)
                {
                    Console.WriteLine(g.Category);
                    foreach (var p in g.Products) Console.WriteLine($"   {p}");
                }

                Q("12. Highest price among each category's products");
                foreach (var g in ProductList.GroupBy(p => p.Category)
                                             .Select(g => new { Category = g.Key, Highest = g.Max(p => p.UnitPrice) }))
                    Console.WriteLine($"{g.Category} => {g.Highest}");

                Q("13. Products with the highest price in each category");
                var highest = from p in ProductList
                              group p by p.Category into g
                              let maxPrice = g.Max(p => p.UnitPrice)
                              select new { Category = g.Key, Products = g.Where(p => p.UnitPrice == maxPrice) };
                foreach (var g in highest)
                {
                    Console.WriteLine(g.Category);
                    foreach (var p in g.Products) Console.WriteLine($"   {p}");
                }

                Q("14. Average price of each category's products");
                foreach (var g in ProductList.GroupBy(p => p.Category)
                                             .Select(g => new { Category = g.Key, Avg = g.Average(p => p.UnitPrice) }))
                    Console.WriteLine($"{g.Category} => {g.Avg:F2}");

                // ====================================================================
                Section("LINQ - Set Operators");
                // ====================================================================
                Q("1. Unique Category names from Product List");
                foreach (var c in ProductList.Select(p => p.Category).Distinct()) Console.WriteLine(c);

                var prodFirst = ProductList.Select(p => p.ProductName[0]);
                var custFirst = CustomerList.Select(c => c.CustomerName[0]);

                Q("2. Unique first letters from both product and customer names");
                Console.WriteLine(string.Join(", ", prodFirst.Union(custFirst)));

                Q("3. Common first letters of product and customer names");
                Console.WriteLine(string.Join(", ", prodFirst.Intersect(custFirst)));

                Q("4. First letters of product names that are not first letters of customer names");
                Console.WriteLine(string.Join(", ", prodFirst.Except(custFirst)));

                Q("5. Last three characters of every customer and product name (with duplicates)");
                Func<string, string> last3 = s => s.Length > 3 ? s.Substring(s.Length - 3) : s;
                var last3All = CustomerList.Select(c => last3(c.CustomerName))
                                           .Concat(ProductList.Select(p => last3(p.ProductName)));
                Console.WriteLine(string.Join(", ", last3All));

                // ====================================================================
                Section("LINQ - Partitioning Operators");
                // ====================================================================
                var waOrders = CustomerList.Where(c => c.Region == "WA")
                                           .SelectMany(c => c.Orders, (c, o) => new { c.CustomerName, Order = o });

                Q("1. First 3 orders from customers in Washington");
                foreach (var x in waOrders.Take(3)) Console.WriteLine($"{x.CustomerName}: {x.Order}");

                Q("2. All but the first 2 orders from customers in Washington");
                foreach (var x in waOrders.Skip(2)) Console.WriteLine($"{x.CustomerName}: {x.Order}");

                int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

                Q("3. From the beginning until a number is less than its position");
                Console.WriteLine(string.Join(", ", numbers.TakeWhile((n, i) => n >= i)));

                Q("4. Starting from the first element divisible by 3");
                Console.WriteLine(string.Join(", ", numbers.SkipWhile(n => n % 3 != 0)));

                Q("5. Starting from the first element less than its position");
                Console.WriteLine(string.Join(", ", numbers.SkipWhile((n, i) => n >= i)));

                // ====================================================================
                Section("LINQ - Quantifiers");
                // ====================================================================
                Q("1. Any word containing the substring 'ei'?");
                Console.WriteLine(words.Any(w => w.Contains("ei")));

                Q("2. Grouped products for categories having at least one out-of-stock product");
                var someOut = ProductList.GroupBy(p => p.Category).Where(g => g.Any(p => p.UnitsInStock == 0));
                foreach (var g in someOut)
                {
                    Console.WriteLine(g.Key);
                    foreach (var p in g) Console.WriteLine($"   {p}");
                }

                Q("3. Grouped products for categories where all products are in stock");
                var allIn = ProductList.GroupBy(p => p.Category).Where(g => g.All(p => p.UnitsInStock > 0));
                foreach (var g in allIn)
                {
                    Console.WriteLine(g.Key);
                    foreach (var p in g) Console.WriteLine($"   {p}");
                }

                // ====================================================================
                Section("LINQ - Grouping Operators");
                // ====================================================================
                Q("1. Partition numbers by remainder when divided by 5");
                List<int> nums = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
                foreach (var g in nums.GroupBy(n => n % 5).OrderBy(g => g.Key))
                {
                    Console.WriteLine($"Numbers with a remainder of {g.Key} when divided by 5:");
                    foreach (var n in g) Console.WriteLine(n);
                }

                Q("2. Partition dictionary words by first letter (word count shown; first 5 words as sample)");
                foreach (var g in words.Where(w => w.Length > 0).GroupBy(w => char.ToUpper(w[0])).OrderBy(g => g.Key))
                    Console.WriteLine($"{g.Key} ({g.Count()} words): {string.Join(", ", g.Take(5))} ...");



            string[] anagrams = { "from", "salt", "earn", " last", "near", "form" };
            foreach (var g in anagrams.GroupBy(w => w, new AnagramEqualityComparer()))
            {
                Console.WriteLine("...");
                foreach (var w in g) Console.WriteLine(w);
            }
        }
        }
    }


