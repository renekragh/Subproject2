import Card from 'react-bootstrap/Card';
import Button from 'react-bootstrap/Button';
import { useState } from 'react';
import Popup from '../rating/Popup';
import Bookmark from '../../common/Bookmark';

export default function TitleCard(props) {
  const [modalShow, setModalShow] = useState(false);
  const defaultImage = e => { e.target.src = "/404_not_found.webp"};
  const currentRate = props.ratingHistory.find(x => x.id === props.movie.id);

  return (
    <>
     <Card>
        <Card.Link href="#" onClick={() => props.selectMovie(props.movie.url)}><Card.Img src={props.movie.url} onError={defaultImage}></Card.Img></Card.Link>
        <Card.Body>
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="yellow" className="bi bi-star-fill" viewBox="0 0 16 16">
            <path d="M3.612 15.443c-.386.198-.824-.149-.746-.592l.83-4.73L.173 6.765c-.329-.314-.158-.888.283-.95l4.898-.696L7.538.792c.197-.39.73-.39.927 0l2.184 4.327 4.898.696c.441.062.612.636.282.95l-3.522 3.356.83 4.73c.078.443-.36.79-.746.592L8 13.187l-4.389 2.256z"/>
          </svg>
          <span className='averagerating'>{props.movie.ratings[0].averagerating}</span>
          
           {props.ratingHistory.find(x => x.id === props.movie.id) === undefined &&
            <Button variant="link" className='ratingbtn' onClick={() => setModalShow(true)}>
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="blue" className="bi bi-star" viewBox="0 0 16 16">
                <path d="M2.866 14.85c-.078.444.36.791.746.593l4.39-2.256 4.389 2.256c.386.198.824-.149.746-.592l-.83-4.73 3.522-3.356c.33-.314.16-.888-.282-.95l-4.898-.696L8.465.792a.513.513 0 0 0-.927 0L5.354 5.12l-4.898.696c-.441.062-.612.636-.283.95l3.523 3.356-.83 4.73zm4.905-2.767-3.686 1.894.694-3.957a.56.56 0 0 0-.163-.505L1.71 6.745l4.052-.576a.53.53 0 0 0 .393-.288L8 2.223l1.847 3.658a.53.53 0 0 0 .393.288l4.052.575-2.906 2.77a.56.56 0 0 0-.163.506l.694 3.957-3.686-1.894a.5.5 0 0 0-.461 0z"/>
              </svg>
            </Button>
          }
          {currentRate != undefined  && 
            <Button variant="link" className='ratingbtn' onClick={() => setModalShow(true)}>
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-star-fill" viewBox="0 0 16 16">
                <path d="M3.612 15.443c-.386.198-.824-.149-.746-.592l.83-4.73L.173 6.765c-.329-.314-.158-.888.283-.95l4.898-.696L7.538.792c.197-.39.73-.39.927 0l2.184 4.327 4.898.696c.441.062.612.636.282.95l-3.522 3.356.83 4.73c.078.443-.36.79-.746.592L8 13.187l-4.389 2.256z"/>
              </svg>
            <span className='rating'>{currentRate.rating}</span>
            </Button>
          }
          <Card.Link href="#" onClick={() => props.selectMovie(props.movie.url)}><Card.Title>{props.movie.primarytitle}</Card.Title></Card.Link>
          <Bookmark 
            key={props.movie.id} 
            movie={props.movie} 
            handleBookmark={props.handleBookmark} 
            handleDeleteBookmark={props.handleDeleteBookmark}
            bookmarks={props.bookmarks}
          />
        </Card.Body>

       </Card>
        <Popup
          movie={props.movie}
          handleRate={props.handleRate}
          handleDeleteRate={props.handleDeleteRate}
          ratingHistory={props.ratingHistory}
          show={modalShow}
          onHide={() => setModalShow(false)}
        />
        </>
  )
}