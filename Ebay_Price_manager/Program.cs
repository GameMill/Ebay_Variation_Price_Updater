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
            //System.IO.File.WriteAllText("Daata.json", Newtonsoft.Json.JsonConvert.SerializeObject(item2.Variations, Newtonsoft.Json.Formatting.Indented));
            
            
            
           // 

          

            var update_item = new eBay.Service.Core.Soap.ItemType()
            {
                ItemID = item_number,
                Variations = getVariationsTypes(prices, item2),
                Quantity = 3
                
            };


            

            for (int i = 0; i < update_item.Variations.Variation.Count; i++)
            {
                update_item.Variations.Variation[i] = update_price(update_item.Variations.Variation[i], prices);
            }

            System.IO.File.WriteAllText("Daata2.json", Newtonsoft.Json.JsonConvert.SerializeObject(update_item.Variations, Newtonsoft.Json.Formatting.Indented));

            var item = new eBay.Service.Call.ReviseFixedPriceItemCall(new eBay.Service.Core.Sdk.ApiContext() { ApiCredential = new eBay.Service.Core.Sdk.ApiCredential(Token) { ApiAccount = new eBay.Service.Core.Sdk.ApiAccount(data.DevID, data.AppID, data.CertID) } }).ReviseFixedPriceItem(update_item,new eBay.Service.Core.Soap.StringCollection());

        }

        public static List<string> GetAllVar(Dictionary<string, Dictionary<string, decimal>> prices)
        {
            List<string> AllItems = new List<string>();
            foreach (var item22 in prices)
            {
                if (item22.Key != "ALL")
                {
                    bool isFirst = AllItems.Count == 0;
                    List<string> Temp = new List<string>();

                    foreach (var item22v in item22.Value)
                    {
                        if (isFirst)
                        {
                            Temp.Add(item22.Key + ":" + item22v.Key);
                        }
                        else
                        {
                            foreach (var item222v in AllItems)
                            {
                                Temp.Add(item222v + "|" + item22.Key + ":" + item22v.Key);
                            }
                        }
                    }
                    AllItems = new List<string>(Temp);
                }

            }

            return AllItems;
        }

        private static eBay.Service.Core.Soap.VariationsType getVariationsTypes(Dictionary<string, Dictionary<string, decimal>> items, eBay.Service.Core.Soap.ItemType EbayItem)
        {
            List<eBay.Service.Core.Soap.VariationType> variations = new List<eBay.Service.Core.Soap.VariationType>();
            var Org = new List<eBay.Service.Core.Soap.VariationType>(EbayItem.Variations.Variation.ToArray());

            var Pic = EbayItem.Variations.Pictures;
            if (Pic.Count != 0)
            {
                foreach (var item in items)
                {
                    if (item.Key != "ALL")
                    {
                        Pic[0].VariationSpecificName = item.Key;
                        foreach (var item2 in item.Value)
                        {
                            Pic[0].VariationSpecificPictureSet[0].VariationSpecificValue = item2.Key;
                            break;
                        }
                        break;
                    }
                }

            }



            foreach (var item in GetAllVar(items))
            {
                eBay.Service.Core.Soap.VariationType variation = getNew(item,ref Org,out bool NeedUpdate);

                if (NeedUpdate)
                {
                    var t = item.Split('|'); // Return List of Key:Value

                    foreach (var item2 in t)
                    {
                        var Data = item2.Split(':');
                        variation.VariationSpecifics.Add(
                            new eBay.Service.Core.Soap.NameValueListType()
                            {
                                Name = Data[0],
                                Value = new eBay.Service.Core.Soap.StringCollection(new string[1] { Data[1] }),
                                Any = new eBay.Service.Core.Soap.XmlElementCollection()
                            }
                            );

                    }
                }
                variations.Add(variation);
            }
            Org.ForEach(p => p.Delete = true);
            Org.ForEach(p => variations.Add(p));

            return new eBay.Service.Core.Soap.VariationsType()
            {
                Pictures = Pic,
                Variation = new eBay.Service.Core.Soap.VariationTypeCollection(variations.ToArray()),
                VariationSpecificsSet = convetToEbay(items)
            };


        }

        public static eBay.Service.Core.Soap.VariationType getNew(string Item, ref List<eBay.Service.Core.Soap.VariationType> OrgItems,out bool NeedUpdate)
        {
            eBay.Service.Core.Soap.VariationType returnItem = null;
            foreach (var item2 in OrgItems)
            {

                if (IsVariationTypeSame(Item, item2))
                {
                    returnItem = item2;
                    
                    break;
                }
            }
            if (returnItem != null)
            {
                OrgItems.Remove(returnItem);
                returnItem.Quantity = 5;
                NeedUpdate = false;
            }
            else
            {
                returnItem = new eBay.Service.Core.Soap.VariationType()
                {
                    Quantity = 5,
                    StartPrice = new eBay.Service.Core.Soap.AmountType() { currencyID = eBay.Service.Core.Soap.CurrencyCodeType.GBP, Value = 999 },
                    VariationSpecifics = new eBay.Service.Core.Soap.NameValueListTypeCollection(),
                    SellingStatus = new eBay.Service.Core.Soap.SellingStatusType()
                    {
                    }
                };
                NeedUpdate = true;
            }
            return returnItem;
        }

        private static bool IsVariationTypeSame(string item, eBay.Service.Core.Soap.VariationType item2)
        {
            Dictionary<string, string> List = new Dictionary<string, string>();

            foreach (var item3 in item.Split('|'))
            {
                var data = item3.Split(':');
                List.Add(data[0], data[1]);
            }
            
             foreach (eBay.Service.Core.Soap.NameValueListType item4 in item2.VariationSpecifics)
             {
                if (List.ContainsKey(item4.Name))
                {
                    if (List[item4.Name] != item4.Value[0])
                    {
                        return false;
                    }
                }
                else
                    return false;
             }
            return true;
        }

        private static eBay.Service.Core.Soap.NameValueListTypeCollection convetToEbay(Dictionary<string, Dictionary<string, decimal>> items)
        {
            eBay.Service.Core.Soap.NameValueListTypeCollection ebay = new eBay.Service.Core.Soap.NameValueListTypeCollection();
            foreach (var item in items)
            {
                if (item.Key != "ALL")
                {
                    var list = new eBay.Service.Core.Soap.NameValueListType() { Name = item.Key,Value = new eBay.Service.Core.Soap.StringCollection() };
                    foreach (var item2 in item.Value)
                    {
                        list.Value.Add(item2.Key);
                    }
                    ebay.Add(list);
                }
            }
            return ebay;
        }

        private static eBay.Service.Core.Soap.VariationType update_price(eBay.Service.Core.Soap.VariationType variationType, Dictionary<string, Dictionary<string, decimal>> Prices)
        {
            decimal price = 0;
            foreach (var item in Prices["ALL"])
            {
                price += item.Value;
            }
            if (variationType.Delete == false)
            {
                foreach (eBay.Service.Core.Soap.NameValueListType item in variationType.VariationSpecifics)
                {
                    price += Prices[item.Name][item.Value[0]];
                }
            }
            
            variationType.StartPrice.Value = decimal.ToDouble(price);
            variationType.Quantity = 3;
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
