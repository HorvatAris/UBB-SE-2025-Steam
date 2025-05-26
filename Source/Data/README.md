# Setting up the Database
Please do the following in order to setup the database to ensure a smooth run of the app

1. In SSMS create a Database named FinalSteamDB
2. Run queries with the following
   1. (Optional) Database Drops
   2. Database Schema
   3. Database Stored Procedures
   4. Database Inserts
3. Open the files: appsettings.json (there are two !!) and modify the following
```
{
  "LocalDataSource": " database_server_name ", <-- e.g. LAPTOP-TONI/SQL-EXPRESS
  "InitialCatalog": "FinalSteamDB"
}
```

### That's all you are now ready to run the app!