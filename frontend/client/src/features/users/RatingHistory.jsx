import { useEffect, useState } from 'react'
import TitleList from '../titles/dashboard/TitleList';
import Button from 'react-bootstrap/Button';
import './titlebookmarks.css' 
import Alert from 'react-bootstrap/Alert';
import { Link } from 'react-router-dom';
import TitleDetail from '../titles/details/TitleDetail';
 

export default function RatingHistory(props) {
    const [movies, setMovies] = useState([]);
    const [show, setShow] = useState(false);
    
    useEffect(() => {
        async function load() {
            try {
                const movies = await Promise.all(
                    props.ratingHistory.map(x =>
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
    },[props.ratingHistory]);

    const handleDeleteAllRatings = async () => {
        props.handleDeleteAllRatings();
        setShow(false);
    }

    if (show) {
        return <Alert className='alert' variant='warning'>
                    Sure you wanna delete all ratings?
                    &nbsp;
                    <Alert.Link onClick={() => handleDeleteAllRatings()} >Yes</Alert.Link> 
                     &nbsp; &nbsp; &nbsp;
                    <Alert.Link onClick={() => setShow(false)} href="">No</Alert.Link>
                </Alert>
    }

    return (
        <>
            {!props.selectedMovie && 
                <>
                    <h1>Rating history</h1>
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
                props.selectedMovie && 
                    <TitleDetail 
                      movie={props.selectedMovie}
                      cancelSelectMovie={props.cancelSelectMovie}
                    />
            } 
            {
                props.ratingHistory.length > 0 ? 
                <div className='deleteAllBtn'>
                    <Button onClick={() => setShow(true)} variant="warning" disabled>Delete all ratings (not impl)</Button>  
                </div>  
                :
                <>
                    <div className='emptyPage'>
                        <h2>You don't have any ratings yet</h2>
                    </div>
                    <div className='emptyPage'>
                        <h3><Link to="/titles">Browse Titles</Link></h3>
                    </div>
                </>
            }
        </>
    )
}