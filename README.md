# Ebay Variation Price Updater
Update Variation Price on a listing on ebay

1. Build the project
2. Create your database file at the root with the Ebay_Price_manager.exe. file name is data.db
```` 
{
  "Tokens": {
    "Add your ebay username": "your ebay api token"
  },
  "DevID": "your ebay api devid",
  "CertID": "your ebay api certid",
  "AppID": "your ebay api appid"
}
````
3. run Ebay_Price_manager.exe and select 1 to download the Variations for an ebay item number. this will create a file {EbayItemNumber}.txt at the root of your program
4. update the prices for each Variation in the {EbayItemNumber}.txt file
5. rerun the program and select option 2 to update the Variations on ebay
