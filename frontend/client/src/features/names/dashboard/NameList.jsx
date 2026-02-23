import NameCard from "./NameCard";
import Container from 'react-bootstrap/Container';
import Col from 'react-bootstrap/Col';
import Row from 'react-bootstrap/Row';

export default function NameList(props) {

  return (
   <Container fluid>
    <h1>Names</h1>
    <Row xs={1} md={6} className="g-4">
      {Array.from(props.names).map((name, idx) => (
        <Col key={idx}>
              <NameCard 
                name={name} 
                selectName={props.selectName}
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