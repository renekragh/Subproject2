import TitleCard from "./TitleCard";
import Container from 'react-bootstrap/Container';
import Col from 'react-bootstrap/Col';
import Row from 'react-bootstrap/Row';

export default function TitleList(props) {
 
  return (
   <Container fluid>
    <h1>Titles</h1>
    <Row xs={1} md={6} className="g-4">
      {Array.from(props.movies).map((movie, idx) => (
        <Col key={idx}>
              <TitleCard 
                movie={movie} 
                selectMovie={props.selectMovie}
                selectedMovie={props.selectedMovie}
                handleRate={props.handleRate}
                handleDeleteRate={props.handleDeleteRate}
                ratingHistory={props.ratingHistory}
                bookmarks={props.bookmarks}
                handleBookmark={props.handleBookmark}
                handleDeleteBookmark={props.handleDeleteBookmark}
              />
        </Col>
      ))}
    </Row>
    </Container>
  )
}