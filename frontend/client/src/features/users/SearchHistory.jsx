import { useEffect, useState } from 'react'
import { useAuth } from '../common/AuthProvider';
import ListGroup from 'react-bootstrap/ListGroup';
import './searchHistory.css';
import Button from 'react-bootstrap/Button';
import './titlebookmarks.css' 
import Alert from 'react-bootstrap/Alert';
import { Link } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.css';

export default function SearchHistory() {
    const { token } = useAuth();
    const [searchHistory, setSearchHistory] = useState([]);
    const [show, setShow] = useState(false);

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
      } catch (err) {
          console.log("Error: " + err.message);
        } 
    }
    load();
  },[token]);

    const handleDeleteAllSearchHistory = async () => {
        try {
            const response = await fetch('http://localhost:5193/api/search-history', {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
            });
            if (response.status !== 200) throw new Error(`Deleting all search history failed: '${response.status + ' ' + response.statusText}`);
            const data = await response.json(); 
            if (data === undefined) throw new Error('unexpected data');
console.log('JSON.stringify(data) ',JSON.stringify(data.message))
            setSearchHistory([]) // trigger rerendering
            setShow(false);
        } catch (err) {
          console.log(err);
        } 
    }

    if (show) {
        return <Alert className='alert' variant='warning'>
                    Sure you wanna delete all search history?
                    &nbsp;
                    <Alert.Link onClick={() => handleDeleteAllSearchHistory()} >Yes</Alert.Link> 
                     &nbsp; &nbsp; &nbsp;
                    <Alert.Link onClick={() => setShow(false)} href="">No</Alert.Link>
                </Alert>
    }

    const listItems = searchHistory.map(x => <ListGroup.Item key={x.id} action onClick={x.url}>{x.searchQuery} : {x.timestamp}</ListGroup.Item>);
    return (
        <>
            <h1>Search history</h1>
            <div className="search-history-wrapper">
            <ListGroup className='list-group' defaultActiveKey="#link1">
                {listItems}
            </ListGroup>
            </div>
           {
                searchHistory.length > 0 ? 
                <div className='deleteAllBtn'>
                    <Button onClick={() => setShow(true)} variant="warning">Delete all search history</Button>  
                </div>  
                :
                <>
                    <div className='emptyPage'>
                        <h2>You don't have any search history yet</h2>
                    </div>
                    <div className='emptyPage'>
                        <h3><Link to="/search">Search</Link></h3>
                    </div>
                </>
            }            
        </>
    )
}