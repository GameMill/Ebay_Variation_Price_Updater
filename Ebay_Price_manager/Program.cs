using System;
using System.Collections.Generic;
using eBay;

namespace Ebay_Price_updater
{
    class Program
    {

        static string Select_A_Token()
        {
            List<string> keys = new List<string>(data.Tokens.Keys);
            int i = 0;
            foreach (var item in data.Tokens)
            {
                Console.WriteLine(i + ") " + item.Key);
                i++;
            }
            Console.WriteLine("Please enter a number: ");
            var key = int.Parse(Console.ReadLine());
            return data.Tokens[keys[key]];
        }
        static string Token = "";
        static Data data = Data.Load();

        static void Main(string[] args)
        {
            Token = Select_A_Token();
            return;
            Console.WriteLine("1. Create new database\n2. Update using database\n   Please enter a number: ");
            var key = Console.ReadKey();
            Console.WriteLine();
            if (key.Key == ConsoleKey.D1)
            {
                Create_Database();
            }
            else if(key.Key == ConsoleKey.D2)
            {
                Update_Listing_using_Database();
            }
            else
            {
                Console.WriteLine("Option Invalid");
                Program.Main(args);
            }
        }

        static void Update_Listing_using_Database()
        {


            Console.WriteLine("Please enter your item number to update: ");
            string item_number = Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine("Updating data on eBay\nPlease Wait...");
            var prices = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, decimal>> > (System.IO.File.ReadAllText(item_number + ".txt"));
            var item2 = Load_Item(item_number);
            var update_item = new eBay.Service.Core.Soap.ItemType()
            {
                ItemID = item_number,
                Variations = new eBay.Service.Core.Soap.VariationsType() { Variation=item2.Variations.Variation },
                
            };
            for (int i = 0; i < update_item.Variations.Variation.Count; i++)
            {
                update_item.Variations.Variation[i] = update_price(update_item.Variations.Variation[i], prices);
            }


            var item = new eBay.Service.Call.ReviseFixedPriceItemCall(new eBay.Service.Core.Sdk.ApiContext() { ApiCredential = new eBay.Service.Core.Sdk.ApiCredential(Token) { ApiAccount = new eBay.Service.Core.Sdk.ApiAccount(data.DevID, data.AppID, data.CertID) } }).ReviseFixedPriceItem(update_item,new eBay.Service.Core.Soap.StringCollection());

        }

        private static eBay.Service.Core.Soap.VariationType update_price(eBay.Service.Core.Soap.VariationType variationType, Dictionary<string, Dictionary<string, decimal>> Prices)
        {
            decimal price = 0;
            foreach (var item in Prices["ALL"])
            {
                price += item.Value;
            }
            foreach (eBay.Service.Core.Soap.NameValueListType item in variationType.VariationSpecifics)
            {
                price += Prices[item.Name][item.Value[0]];
            }
            variationType.StartPrice.Value = decimal.ToDouble(price);
            variationType.Quantity = 1;
            return variationType;
        }

        static eBay.Service.Core.Soap.ItemType Load_Item(string item_number)
        {
            Console.WriteLine("Loading data from eBay\nPlease Wait...");
            var item = new eBay.Service.Call.GetItemCall(new eBay.Service.Core.Sdk.ApiContext() { ApiCredential = new eBay.Service.Core.Sdk.ApiCredential(Token) { ApiAccount = new eBay.Service.Core.Sdk.ApiAccount(data.DevID, data.AppID, data.CertID) } }).GetItem(item_number);
            return item;
        }

        static void Create_Database()
        {
            Console.WriteLine("Please enter your item number to download: ");
            string item_number = Console.ReadLine();
            Console.WriteLine();
            var item = Load_Item(item_number);

            Dictionary<string, Dictionary<string, decimal>> Prices = new Dictionary<string, Dictionary<string, decimal>>();
            Prices["ALL"] = new Dictionary<string, decimal>();
            Prices["ALL"].Add("base", 30);
            foreach (eBay.Service.Core.Soap.NameValueListType item_row in item.Variations.VariationSpecificsSet)
            {
                if (Prices.ContainsKey(item_row.Name) == false)
                    Prices.Add(item_row.Name, new Dictionary<string, decimal>());
                foreach (string vara in item_row.Value)
                {
                    Prices[item_row.Name].Add(vara, 0);
                }
            }

            // System.IO.File.WriteAllText(item_number + "2.txt", Newtonsoft.Json.JsonConvert.SerializeObject(item.Variations.Variation, Newtonsoft.Json.Formatting.Indented));

            System.IO.File.WriteAllText(item_number + ".txt", Newtonsoft.Json.JsonConvert.SerializeObject(Prices, Newtonsoft.Json.Formatting.Indented));
            Console.WriteLine("Database Created.\nPress any key to exit");
            Console.ReadLine();
        }
    }
}
