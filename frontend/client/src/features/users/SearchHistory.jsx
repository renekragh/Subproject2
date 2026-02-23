import { useEffect, useState } from 'react'
import { useAuth } from '../common/AuthProvider';
import ListGroup from 'react-bootstrap/ListGroup';
import './searchHistory.css';

export default function SearchHistory() {
    const { token } = useAuth();
    const [searchHistory, setSearchHistory] = useState([]);

   useEffect(() => {
    async function load() {
      try {
        const res = await fetch('http://localhost:5193/api/search-history',
                        {
                            headers: 
                            {
                                'Content-Type': 'application/json',
                                'Authorization': `Bearer ${token}`
                            }
                        }
                    );
        if (!res.ok) throw new Error("status = " + res.status);
        const data = await res.json(); 
        if (data.items === undefined) throw new Error('unexpected data');
        setSearchHistory(data.items);
        console.log('data.items[0].searchquery ' +JSON.stringify(data.items[0]))
        console.log('JSON.stringify(listItems) ' +JSON.stringify(listItems))
      } catch (err) {
          console.log("Error: " + err.message);
        } 
    }
    load();
  },[token]);

    const listItems = searchHistory.map(x => <ListGroup.Item key={x.id} action onClick={x.url}>{x.searchQuery} : {x.timestamp}</ListGroup.Item>);
    return (
        <>
            <h1>Search history</h1>
            <div className="search-history-wrapper">
            <ListGroup className='list-group' defaultActiveKey="#link1">
                {listItems}
            </ListGroup>
            </div>
        </>
    )
}