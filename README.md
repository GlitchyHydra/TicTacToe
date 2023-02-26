# TicTacToe

TicTacToe game with authentication and SignalR server.

There is the list of users for testing
(Only username is used to login for simplicity)
Username|role|Authorized to play
-----|------|------
tony | player | yes
pawn | player | yes
hydra | player | yes
ghost | player | yes
rat | spectator | no

# Database
Postgresql

1. run sql scripts to init db

# Server
Postgresql, Asp.Net Core, SignalR, EF core<br>

1. Set ConnectionStrings in appsettings.json

What you can't do:
1. If user is already in game you can not start another
2. You can't pick any game you like, only that present in database

The token is generated on login endpoint for client. <br>
Then it will be used to connect with SignalR Hub and to be authorized

# Client

Typescripts, React

To launch client the next steps are required (run in cmd):
1. ```yarn install```
2. ```yarn start```

Round timer (15 sec) rendering is not working on client, for now, but
calculated at server

How to play:
1. Pass valid username and press search game
2. Press ready checkbox, when you are ready
3. Wait for opponent (if there no one is searching)
4. Play the game!
