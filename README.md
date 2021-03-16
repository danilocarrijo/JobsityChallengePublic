# JobsityChallenge

## Conditions
So that the system works correctly and with 100% of its functionality. It is assumed that the machine where it will be run has ***Net Core 3*** installed and an instance of ***RabbitMQ*** running on localhost.

## How to run it

### Running the exe
Along with the challenge completion email, a folder containing two exes will be sent:

The first in the bot folder will run the chatbot, which is needed to search for stock quotes. You can change the port he will use in appsettings.json, tag Urls
```json

{
  ...
  "Urls": "http://localhost:5050"
}

The second in the chat folder, will run the system itself.
    
### Publishing
Visual Studio 2019 (any version) will be required for this step. Just open the project and publish according to preference (folder, IIS, remote server)

## Bot url

For the communication between chatroom and bot to happen it is necessary to configure the bot's url in the chat, changing the Url tag in appsettings.

```json

"BotSettings": {
    "Url": "https://localhost:44311"
  }
  
``` 

## Data Bases
The system can be executed using 2 types of database, for that, just change in the appsettings.json the dbType tag to:

```json

{
  ...
  "dbType": ""
}
    
```
### MEM:

```json

{
  ...
  "dbType": "MEM"
}
    
```

Using this type of db, the system will store all data in memory.

**PROS**: ideal for a quick test without the need to connect to a database.

**CONS**: When the system stops running, all information will be lost.

### DB

```json

{
  ...
  "dbType": "DB"
}
    
```
This tag will make the system connect to a MySQL database, for the correct functioning of the system, it will also be necessary to configure a suitable connection string in the dbConn tag. All tables needed for the system to work will be created automatically when it is run.

```json

"ConnectionStrings": {
    "dbConn": "Server=localhost;Database=test;Uid=root;Pwd=root;"
  }
    
```

**PROS**: data is persisted even if the system stops running.

**CONS**: you need a MySQL base to work.

## Main features
The system consists of a Login screen, registration, information of the logged user and the chat room

## Technologies Used
* RabbitMQ
* .Net Identity
* EF
* Razor Pages
* SignalR
