import React, { useState, useEffect } from 'react';
import './App.css';
import * as signalR from "@microsoft/signalr"
import Login from './components/Login';
import Game from './components/Game';

import 'bootstrap/dist/css/bootstrap.min.css';

function App() {
  const [text, setText] = useState<string>("");
  const [connection, setConnection] = useState<signalR.HubConnection>();
  const [playerId, setPlayerId] = useState<string>();
  const [connectionStatus, setConnectionStatus] = useState<boolean>();
  const [isMoveFirst, setIsMoveFisrt] = useState<boolean>(false);
  const [token, setToken] = useState<string>("Invalid")

  const joinGame = async(username: string) => 
  {
    try
    {
      const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
      };

     const res = await fetch('http://localhost:5075/login/' + username, requestOptions);
      if (!res.ok)
      {
        console.log("Not ok")
        return;
      }

      const data = await res.json();
      if (data.token == "Invalid")
      {
        console.log("not valid username")
        return;
      }
      console.log(data.token);
      setToken(data.token)

      console.log(connectionStatus);
      if (connectionStatus)
      {
        connection!.stop();
        setConnectionStatus(false);
        //setConnection();
      }
      
      const connection_str: string = process.env.REACT_APP_CONNECTION_STR!;
      const conn = new signalR.HubConnectionBuilder()
      .withUrl(connection_str!,  {
        accessTokenFactory: () => data.token
      })
      .configureLogging(signalR.LogLevel.Information)
      .build();

      conn.onclose(e => 
        {
          //setConnection(null);
        })

      conn.on("usernameTaken", () => 
      {

      });

      conn.on("PlaceInWaitList", () => 
      {
        setIsMoveFisrt(true);
      });

      conn.on("playerJoined", (id: string) => 
      {
        setPlayerId(id);
      });

      //listen event if player start
      //to make move before game is begun
      conn.on("NeedToWait", () => 
      {

      });

      await conn.start();
      setConnection(conn);
      setConnectionStatus(true);
      await conn!.invoke("searchGame", username);
      
    } catch(e)
    {
      console.log(e);
    }
  }

  const takeTurn = async (row:number, col:number) => 
  {
    try 
    {
      await connection!.invoke("takeTurn", row, col);
    } catch(e) 
    {
      console.log(e);
    }
  }

  return (
    <div className="App">
      <h2>TicTacToe</h2>
      <hr className='line' style={{margin: "auto"}}/>
      {
        !connection 
        ? <Login joinGame={joinGame}/>
        : <Game connection={connection} takeTurn={takeTurn} isMoveFirst={isMoveFirst}/>
      }
      
    </div>
  )
}

export default App;
