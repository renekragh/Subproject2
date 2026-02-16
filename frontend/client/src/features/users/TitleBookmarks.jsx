import { useEffect, useState } from 'react'
import { useAuth } from '../common/AuthProvider';
import TitleList from '../titles/dashboard/TitleList';
 

export default function TitleBookmarks(props) {
    const { token } = useAuth();
    const [bookmarks, setBookmarks] = useState([]);
    const [ratingHistory, setRatingHistory] = useState([]);
    const [movies, setMovies] = useState([]);
    
    useEffect(() => {
        async function load() {
            try {
                const response = await Promise.all([
                    fetch('http://localhost:5193/api/ratings-history', 
                        {
                            headers: 
                            {
                                'Content-Type': 'application/json',
                                'Authorization': `Bearer ${token}`
                            }
                        }
                    ),
                    fetch('http://localhost:5193/api/bookmark-titles', 
                        {
                            headers: {
                            'Content-Type': 'application/json',
                            'Authorization': `Bearer ${token}`
                            }
                        }
                    )                
                ]);
            const responseStatus = response.find(x => !x.ok);
            if (responseStatus !== undefined) throw new Error(responseStatus.url + ' ' + responseStatus.status + ' ' + responseStatus.statusText);
            const [data0, data1] = await Promise.all(response.map(async (x) => await x.json()));
            if ([data0, data1].length !== response.length) throw new Error('unexpected data');
            setRatingHistory(data0.items);
            setBookmarks(data1.items);
            return;
            } catch (error) {
                throw new Error(error);
            }        
        }
        load();
    },[token]);

    useEffect(() => {
        async function load() {
            try {
                const movies = await Promise.all(
                    bookmarks.map(x =>
                        fetch(`http://localhost:5193/api/titles/${x.id}`,
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
                setMovies(movies);
            } catch (error) {
                throw new Error(error);
            }        
        }
        load();
    },[bookmarks]);

    return (
        <>
            <h1>Title bookmarks</h1>
             <TitleList 
                movies={movies}
                selectMovie={props.selectMovie}
                handleRate={props.handleRate}
                handleDeleteRate={props.handleDeleteRate}
                ratingHistory={ratingHistory}
                bookmarks={bookmarks}
                handleBookmark={props.handleBookmark}
                handleDeleteBookmark={props.handleDeleteBookmark}
            />  
        </>
    )
}