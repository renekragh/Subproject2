import Card from 'react-bootstrap/Card';
import Bookmark from '../../common/Bookmark';
import { useAuth } from '../../common/AuthProvider';
import { useEffect, useState } from 'react'

export default function NameCard(props) {
  const defaultImage = e => { e.target.src = "/404_not_found.webp"};
  const [bookmarks, setBookmarks] = useState([]);
  const { token } = useAuth();

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

  return (
     <Card>
        <Card.Link href="#" onClick={() => props.selectName(props.name.url)}><Card.Img src={props.name.url} onError={defaultImage}></Card.Img></Card.Link>
        <Card.Body>
          <Card.Link href="#" onClick={() => props.selectName(props.name.url)}><Card.Title>{props.name.primaryname}</Card.Title></Card.Link>
          <Bookmark 
            key={props.name.id} 
            movie={props.name} //name is the value and movie is the key in order to reuse component. TO DO make generic!
            bookmarks={bookmarks}
            handleBookmark={props.handleBookmark} 
            handleDeleteBookmark={props.handleDeleteBookmark}
          />
        </Card.Body>
      </Card>
  )
}