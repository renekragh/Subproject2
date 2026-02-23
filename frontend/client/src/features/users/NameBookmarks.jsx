import { useEffect, useState } from 'react'
import NameList from '../names/dashboard/NameList';
import Alert from 'react-bootstrap/Alert';
import { Link } from 'react-router-dom';
import Button from 'react-bootstrap/Button';
import NameDetail from '../names/NameDetail';

export default function NameBookmarks(props) {
    const [names, setNames] = useState([]);
    const [show, setShow] = useState(false);

    useEffect(() => {
        async function load() {
            try {
                const names = await Promise.all(
                    props.bookmarkNames.map(x =>
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
    },[props.bookmarkNames]);

    const handleDeleteAllBookmarks = async () => {
        props.handleDeleteAllNameBookmarks();
        setShow(false);
    }

    if (show) {
        return <Alert className='alert' variant='warning'>
                    Sure you wanna delete all title bookmarks?
                    &nbsp;
                    <Alert.Link onClick={handleDeleteAllBookmarks} >Yes</Alert.Link> 
                     &nbsp; &nbsp; &nbsp;
                    <Alert.Link onClick={() => setShow(false)} href="">No</Alert.Link>
                </Alert>
    }

    return (
        <>
             {
                !props.selectedName && 
                <>
                    <h1>Name bookmarks</h1>
                    <NameList
                        names={names}
                        selectName={props.selectName}
                        bookmarks={props.bookmarkNames}
                        handleBookmark={props.handleBookmark}
                        handleDeleteBookmark={props.handleDeleteBookmark}
                    />  
                </>
            }

            {props.selectedName && 
                <NameDetail 
                    name={props.selectedName}
                />
            }

            {
                props.bookmarkNames.length > 0 ? 
                <div className='deleteAllBtn'>
                    <Button onClick={() => setShow(true)} variant="warning">Delete all title bookmarks</Button>  
                </div>  
                :
                <>
                    <div className='emptyPage'>
                        <h2>You don't have any bookmarked names yet</h2>
                    </div>
                    <div className='emptyPage'>
                        <h3><Link to="/names">Browse Names</Link></h3>
                    </div>
                </>
            }
        </>
    )
}