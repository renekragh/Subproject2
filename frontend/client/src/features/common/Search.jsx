import { useEffect, useState } from 'react';
import { Form, Button } from 'react-bootstrap';
import TitleList from '../titles/dashboard/TitleList';
import { useAuth } from './AuthProvider';
import './search.css';

export default function Search(props) {

    const SEARCH_ICON = <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-search" viewBox="0 0 16 16">
                            <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001q.044.06.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1 1 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0"/>
                        </svg>
    const EMPTY_SEARCH_RESULT = 'No hits searching for ';
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(false);
    const [searchString, setSearchString] = useState('');
    const [searchResults, setSearchResults] = useState([]);
    const { token } = useAuth();
    const [ratingHistory, setRatingHistory] = useState([]);
    const [bookmarks, setBookmarks] = useState([]);
    const [movies, setMovies] = useState([]);

    async function ApiGetRatingHistoryAndBookmarks() {
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

    useEffect(() => {
            async function load() {
                try {
                    const movies = await Promise.all(
                        searchResults.map(x =>
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
    },[searchResults]);

    const search = async () => {
        try {
            const response = await fetch(`http://localhost:5193/api/titles/name/${searchString}`, {
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
            });
            if (!response.ok) throw new Error(`Login failed'${response.status, response.statusText}`);
            const result = await response.json();
            if (result.items === undefined) throw new Error('unexpected data');
            result.items.length > 0 ? setSearchResults(result.items) : setSearchResults({EMPTY_SEARCH_RESULT}+searchString);
            if (result.items.length > 0) ApiGetRatingHistoryAndBookmarks();
            return;
        } catch(err) {
            console.log("Error: " + err);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError(null);
        setLoading(true);
        try {
            await search();
        } catch(err) {
            setError(err.message || 'An error occurred during search');
        } finally {
            setLoading(false);
        }
    };  
    
    
    return (
        <>
        <div className="search-wrapper">
        <h1>{SEARCH_ICON} Search</h1>
        {error && <div className="error-message">{error}</div>}
        <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3" controlId="formBasicSeach">
                <Form.Control
                    type="text"
                    placeholder="Search titles by primary title or plot"
                    value={searchString}
                    onChange={(e) => setSearchString(e.target.value)}
                    disabled={loading}
                />
                <Form.Control.Feedback type="invalid">
                    {error}
                </Form.Control.Feedback>
            </Form.Group>

            <Button variant="primary" type="submit" className="search-button" disabled={loading}>
                {loading ? 'Searching...' : 'Search'}
            </Button>
        </Form>
        </div>
        <h1>{SEARCH_ICON} Search results</h1>
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