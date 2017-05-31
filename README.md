# LIFX stockmonitor

LIFX Stockmonitor a simple IoT-app for the stocklover. Monitor the daily change (%) and use your LIFX-lamp to monitor one or several stocks on a specific lamp. The light shows red or green based on the selected stocks and their averaged daily change. 

- Instructions on how to acquire token is featured under the settings page
- Token for LIFX is saved encrypted on the device
- With LIFX-support activated user can change lamp for a stock
- Automatic data fetch every 3 seconds on Wifi, every 5 seconds on mobile network. List can also be refreshed when swiping
- Data is fetched from Yahoo Finance API, data is delayed, normally 15 minutes. Exchanges list with delaytimes http://finance.yahoo.com/exchanges
- Search stock via symbol or company name
- Searchable stocks from USA, Canada, Germany, Sweden, Norway, Finland and Denmark.
- Handles when lamp is offline
- Handles when device lost network

Avaliable in Google Play 
https://play.google.com/store/apps/details?id=com.lifx.stockmonitor

Licenses: 
* Xam.Plugins.Settings (https://github.com/jamesmontemagno/SettingsPlugin)
* CsvHelper (https://github.com/joshclose/csvhelper)
* LifxHttpNet (https://github.com/mensly/LifxHttpNet)
* MonoDroid Samples - Searchable Directory (https://github.com/xamarin/monodroid-samples)
* HTextView (https://github.com/hanks-zyh)
* Akavache (https://github.com/akavache/Akavache)