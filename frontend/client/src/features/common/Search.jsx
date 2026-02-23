import { useEffect, useState } from 'react';
import { Form, Button } from 'react-bootstrap';
import TitleList from '../titles/dashboard/TitleList';
import { useAuth } from './AuthProvider';
import './search.css';

export default function Search(props) {

    const SEARCH_ICON = <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-search" viewBox="0 0 16 16">
                            <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001q.044.06.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1 1 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0"/>
                        </svg>
    const [error, setError] = useState(null);
    const [searchString, setSearchString] = useState('');
    const [tmpSearchString, setTmpSearchString] = useState('');
    const [searchResults, setSearchResults] = useState([]);
    const { token } = useAuth();
    const [movies, setMovies] = useState([]);
    const [isSearchBtnDisabled, setIsSearchBtnDisabled] = useState(true);
    const NO_RESULT_MSG = 'No results searching for ';

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
            if (!response.ok) throw new Error(response.status, response.statusText);
            const result = await response.json();
            if (result.items === undefined) throw new Error('unexpected data');
            setSearchResults(result.items);
            return;
        } catch(err) {
            throw new Error(err);
        }
    };

    const handleSearch = (e) => {
        setSearchString(e.target.value);
        setIsSearchBtnDisabled(false);
    } 

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError(null);
        setSearchResults([]);
        setTmpSearchString('');
        try {
            await search();
            setIsSearchBtnDisabled(true);
            setSearchString('');
        } catch(err) {
            console.log('err.message',JSON.stringify(err.message))
            if (err.message.includes('404')) {
                setIsSearchBtnDisabled(true);
                setTmpSearchString(searchString);
                setSearchString('');
            } 
            else setError(err.message || 'An error occurred during search');
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
                    onChange={(e) => handleSearch(e) }
                    style={{width: "400px"}}
                />
                <Form.Control.Feedback type="invalid">
                    {error}
                </Form.Control.Feedback>
            </Form.Group>
           
            <Button className='submitBtn' disabled={isSearchBtnDisabled} variant="primary" type="submit" >
                Search
            </Button>
        </Form>
        </div>
            {
                searchResults.length > 0 &&
                <>
                    <h1>{SEARCH_ICON} Search results</h1>
                    <TitleList 
                        movies={movies}
                        selectMovie={props.selectMovie}
                        handleRate={props.handleRate}
                        handleDeleteRate={props.handleDeleteRate}
                        ratingHistory={props.ratingHistory}
                        bookmarks={props.bookmarks}
                        handleBookmark={props.handleBookmark}
                        handleDeleteBookmark={props.handleDeleteBookmark}
                    />  
                </>
            }
            {
                (tmpSearchString.length > 0) &&
                <><div className='search-wrapper'><h2>{NO_RESULT_MSG}</h2><span className='noSearchResult'>{tmpSearchString}</span></div></>
            }
      </>
    )
}