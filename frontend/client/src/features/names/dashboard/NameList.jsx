import { useEffect, useState } from 'react'
import NameCard from "./NameCard";
import Container from 'react-bootstrap/Container';
import Col from 'react-bootstrap/Col';
import Row from 'react-bootstrap/Row';
import { ImagesFor } from './ImagesFor';

export default function NameList(props) {
  const [names, setNames] = useState([]);

  useEffect(() => {
    async function load() {
      try {
        const res = await fetch('http://localhost:5193/api/names');
        if (!res.ok) throw new Error("status = " + res.status);
        const data = await res.json(); 
        if (data.items === undefined) throw new Error('unexpected data');
        setNames(data.items);
      } catch (err) {
          console.log("Error: " + err.message);
        } 
    }
    load();
  },[]);

  return (
   <Container fluid>
    <h1>Names</h1>
    <Row xs={1} md={6} className="g-4">
      {Array.from(names).map((name, idx) => (
        <Col key={idx}>
              <NameCard 
                name={name} 
                selectName={props.selectName}
                handleBookmark={props.handleBookmark}
                handleDeleteBookmark={props.handleDeleteBookmark}
              />
        </Col>
      ))}
    </Row>
    </Container>
  )
}