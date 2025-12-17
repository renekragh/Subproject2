import { useState, useEffect } from "react";
import Person from "./Person";
import KnownFor from "./KnownFor";
import ImagesFor from "./ImagesFor";

function App({query}) {
    const [persons, setPersons] = useState([]);
    const [index, setIndex] = useState(0);
    useEffect(() => {
/* Task 1. Sign up.*/
/* Task 2. Register an API key. */
        fetch(`https://api.themoviedb.org/3/search/person?query=${query}&api_key=85eb017598662c36df7a4aaf487790ff`)
            .then(res => res.json())
            .then(data => setPersons(data.results));
    }, [query]);
    const length = persons.length;
    return (
        <div>
            <>
            <h2>Person:</h2>
            <Person person={persons[index]} />
            <h2>Known for movies:</h2>
            <KnownFor movies={persons[index].known_for} />
            <ImagesFor id={persons[index].id} />
          
            {
                index > 0 &&
                <button onClick={setIndex(0)}>
                    1
                </button>
            }

            {    
                index > 3 &&
                <span>&nbsp; . . . &nbsp;</span>
            }   

            {
                index > 1 &&
                <button onClick={setIndex(index-1)}>
                {index-1}
                </button>
            } 
              
            {
                index > 2 &&
                <button onClick={setIndex(index)}>
                    {index}
                </button>
            }   
        
                &nbsp; &nbsp; <b>{index+1}</b> &nbsp; &nbsp;

            {
                index < length-2 &&
                <button onClick={setIndex(index+1)}>
                    {index+2}
                </button>
            }

            {
                index < length-3 &&
                <button onClick={setIndex(index+2)}>
                    {index+3}
                </button>
            }

            {
                index < length-4 &&
                <span>&nbsp; . . . &nbsp;</span>
            }

            {
                index < length-1 &&
                <button onClick={setIndex(length-1)}>
                    {length}
                </button>
            }
           </>
        </div>
    );
}

export default App;