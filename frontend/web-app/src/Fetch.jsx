import { useState, useEffect } from "react";
function Fetch() {
    const QUERY = "spielberg";
    const [persons, setPersons] = useState([]);
    useEffect(() => {
        fetch(`https://api.themoviedb.org/3/search/person?query=${QUERY}&api_key=85eb017598662c36df7a4aaf487790ff`)
            .then(res => res.json())
            .then(data => setPersons(data.results));
    }, []);
    persons.map((person) => console.log(person));
    console.log(JSON.stringify(persons, null, 2));
    let lenght = persons.length;
    return (
        <div>
            <h1>Array of {lenght} person objects</h1>
           {persons.map((person) => <p>{person.name}</p>)}
        </div>
    );
}

export default Fetch;
