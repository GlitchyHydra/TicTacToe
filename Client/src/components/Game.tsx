import {useState} from "react";
//import {signalR} from signalR
import Board from "./Board";
import "./Game.css"
import * as signalR from "@microsoft/signalr";
import { nextTick } from "process";
//import { clearInterval } from "timers";
import {useEffect} from "react";


type Props = {
    connection: signalR.HubConnection;
    takeTurn : (row: number, col: number) => void;
    isMoveFirst : boolean;
  };


const Game: React.FC<Props> = props =>
{
    //const squares: Array<string> = new Array(9);
    const [counter, setCounter] = useState(15);

    const [squares, setSquares] = useState<string[]>(new Array(9));
    const [IsMyTurn, setTurn] = useState<boolean>(props.isMoveFirst);
    const [finished, setFinished] = useState<boolean>(false);
    const [gameStatus, setGameStatus] = useState<string>("Idle");
    const [roundTime, setRoundTime] = useState<number>(15);
    const [checked, setChecked] = useState(false);
    const [isGameStarted, setGameStarted] = useState(false);

    const handleClick = (i: number) => {
        var row = Math.floor(i / 3);
        var col = i % 3;
        props.takeTurn(row, col);
      };


      props.connection.on("NotPlayerTurn", () => 
      {
        //TODO nothing
      });

      props.connection.on("NotValidMove", () => 
      {
        //TODO nothing
      });

      props.connection.on("StartGame", () => 
      {
        setGameStarted(true);
      });

      props.connection.on("PlaceShape", (row: number, col: number, shape: string) => 
      {
        var i = row * 3 + col;
        let tmpSquares = squares;
        tmpSquares[i] = shape;
        setSquares(tmpSquares);
        setRoundTime(15);
      });

      props.connection.on("UpdateTurn", () => 
      {
        setTurn(!IsMyTurn);
        if (IsMyTurn)
        {
          setGameStatus("My turn");
        } else 
        {
          setGameStatus("Enemy turn");
        }
      });

      props.connection.on("TieGame", () => 
      {
        setGameStatus("Tie");
        //TODO block all input or transent to another screen
      });

      props.connection.on("GameOver", (playerId: string) => 
      {
        var isWin = playerId == props.connection.connectionId; 
        setGameStatus(isWin ? "Win" : "Lose");
      });

      props.connection.on("NeedToStartAgain", () => 
      {
        setGameStatus("You need to start again");
      })

      const onReadyClick = () => 
      {
        setChecked(true);
        props.connection.invoke("Play", props.connection.connectionId);
      }

      useEffect(() => 
      {
        if (isGameStarted && counter > 0)
        {
          /*const timer = setInterval(() => setCounter(counter - 1), 1000);
          if (counter <= 0)
            return () => clearInterval(timer);*/
        }
        /*return () => {
          if (props.connection)
          {
            console.log("stop connection");
            props.connection.stop();
          }
        }*/
      }, [isGameStarted, counter])

    return <div>
        <div className="game-info">
                <div>{gameStatus}</div>
                <div>Round time: {counter}</div>
                <div>Ready <input type="checkbox" checked={checked} onChange={onReadyClick} disabled={isGameStarted}/></div>
                
            </div>
        <div className='game'>
            <Board squares={squares}
            finished={finished}
            onClick={i => handleClick(i)} 
            />
        </div>
    </div>;
}

export default Game;