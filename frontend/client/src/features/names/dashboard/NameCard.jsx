import Card from 'react-bootstrap/Card';
import Bookmark from '../../common/Bookmark';
import { ImagesFor } from './ImagesFor';

export default function NameCard(props) {

  return (
     <Card>
        <Card.Link href="#" onClick={() => props.selectName(props.name.url)}><ImagesFor id={props.name.id} /></Card.Link>
        <Card.Body>
          <Card.Link href="#" onClick={() => props.selectName(props.name.url)}><Card.Title>{props.name.primaryname}</Card.Title></Card.Link>
          <Bookmark 
            key={props.name.id} 
            movie={props.name} //name is the value and movie is the key in order to reuse component. TO DO make generic!
            bookmarks={props.bookmarks}
            handleBookmark={props.handleBookmark} 
            handleDeleteBookmark={props.handleDeleteBookmark}
          />
        </Card.Body>
      </Card>
  )
}