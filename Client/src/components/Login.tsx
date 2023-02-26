import {Button, Form} from "react-bootstrap"
import {useState} from "react"

import './Login.css';

type Props = {
    joinGame : (username: string) => void;
  };

const Login : React.FC<Props> = props => 
{
    const [username, setUsername] = useState<string>("");
    //const [pass, setPass] = useState<string>("");
    //<Form.Control placeholder="pass" onChange={e => setPass(e.target.value)} />
    return <Form className='lobby'
    onSubmit={e => {
        e.preventDefault();
        props.joinGame(username);
    }}>
        <Form.Group>
            <Form.Control placeholder="username" onChange={e => setUsername(e.target.value)} />
            
        </Form.Group>
        <Button variant='success' type='submit' disabled={!username }>Search Game</Button>
    </Form>
}

export default Login;