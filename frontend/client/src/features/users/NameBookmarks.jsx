import { useEffect, useState } from 'react'
import { useAuth } from '../common/AuthProvider';
import NameList from '../names/dashboard/NameList';
import NameDashboard from '../names/dashboard/NameDashboard';

export default function NameBookmarks(props) {
    const { token } = useAuth();
    const [bookmarks, setBookmarks] = useState([]);
    const [names, setNames] = useState([]);
    
    useEffect(() => {
        async function load() {
            try {
                const res = await fetch('http://localhost:5193/api/bookmark-names',
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
                setBookmarks(data.items);
            } catch (err) {
                console.log("Error: " + err.message);
            } 
        }
        load();
    },[token]);

    useEffect(() => {
        async function load() {
            try {
                const names = await Promise.all(
                    bookmarks.map(x =>
                        fetch(`http://localhost:5193/api/names/${x.id}`,
                            {
                                headers: 
                                {
                                    'Content-Type': 'application/json'
                                }
                            }
                        )
                        .then(responses => responses.json())
                    )
                );
                setNames(names);
            } catch (error) {
                throw new Error(error);
            }        
        }
        load();
    },[bookmarks]);

    return (
        <>
            <h1>Name bookmarks</h1>
             <NameDashboard 
                names={names}
                selectName={props.selectName}
                bookmarks={bookmarks}
                handleBookmark={props.handleBookmark}
                handleDeleteBookmark={props.handleDeleteBookmark}
            />  
        </>
    )
}