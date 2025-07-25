
[![license](https://img.shields.io/badge/license-MIT-blue)](https://opensource.org/license/mit)
[![release](https://img.shields.io/github/v/release/GameMill/WinPE_OS_Installer)](https://github.com/GameMill/Ebay_Variation_Price_Updater/releases)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-blue.svg)](https://github.com/GameMill/Ebay_Variation_Price_Updater/pulls)

## 📦 Setup Instructions

1. **Build the project**  
   Compile the project using your preferred build system.

2. **Create the database file**  
   Place a file named `data.db` in the root directory alongside `Ebay_Price_manager.exe`.

3. **Add your API credentials**  
   Create a JSON config file (`config.json`) with this structure and save it next to `data.db`:

   ```json
   {
     "Tokens": {
       "YourEbayUsername": "YourEbayAPIToken"
     },
     "DevID": "YourEbayDevID",
     "CertID": "YourEbayCertID",
     "AppID": "YourEbayAppID"
   }
   ```

---

## 🚀 Usage Instructions

1. **Download item variations**  
   Run `Ebay_Price_manager.exe` and select option **1**.  
   This generates `{EbayItemNumber}.txt` in the root folder.

2. **Edit the variation prices**  
   Open `{EbayItemNumber}.txt` and adjust the price values for each variation.

3. **Push updates to eBay**  
   Rerun `Ebay_Price_manager.exe`, choose option **2**, and confirm to submit your changes.

---

## 📝 Example Variation File

```json
{
  "ALL": {
    "addtoall": 2,
    "addtoall2": 2
  },
  "size": {
    "small": 5,
    "mid": 6,
    "large": 7
  },
  "color": {
    "red": 10,
    "blue": 12,
    "green": 15
  }
}
```

---

## 💡 Price Calculation Example

If a buyer selects **small** and **red**, the price is computed as:

| Source         | Adjustment |
| -------------- | ---------- |
| `ALL`          | 2 + 2 = 4  |
| `size: small`  | 5          |
| `color: red`   | 10         |
| **Total Price**| **£19**    |

---

## 🧩 Notes

- Variation dimensions can include any eBay-supported attribute (e.g., size, color, material).  
- Values under `ALL` are summed and applied to every variation.  
- Ensure the JSON files are well-formed to avoid runtime errors.  
- Test on a mock item before pushing updates to live listings.
