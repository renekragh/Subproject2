import Card from 'react-bootstrap/Card';
import Button from 'react-bootstrap/Button';
import { useState } from 'react';
import Modal from 'react-bootstrap/Modal';
import CardText from 'react-bootstrap/esm/CardText';
import Container from 'react-bootstrap/Container';
import Col from 'react-bootstrap/Col';
import Row from 'react-bootstrap/Row';

export default function NameDetail(props) {
  const [modalShow, setModalShow] = useState(false);
  const defaultImage = e => { e.target.src = "/404_not_found.webp"};

  return (
    <>
      <Container fluid>
        <Row>
          <Col>
            <Card>
              <Card.Img src={props.name.url} onError={defaultImage}></Card.Img>
              <Card.Body>
                <CardText>
                  URL: {props.name.url}<br />
                  Primaryname: {props.name.primaryname}<br />
                  Birthyear: {props.name.birthyear}<br />
                  Deathyear: {props.name.deathyear}<br />
                  Startyear: {props.name.startyear}<br />
                  Endyear: {props.name.endyear}<br />
                  NameKnownForTitles: {props.name.nameKnownForTitles.map(x => <li>{x.knownfortitles}</li>)}
                  NamePrimaryProfessions: {props.name.namePrimaryProfessions.map(x => <><li>{x.primaryprofession}</li></>)}
                  Principals: {props.name.principals.map(x => <><li>{x.url}</li></>)}
                </CardText>
                <Card.Title>{props.name.primaryname}</Card.Title>
                <Button variant="primary">Bookmark</Button>
              </Card.Body>
            </Card>
          </Col>
        </Row>
      </Container>
      <MyVerticallyCenteredModal
        show={modalShow}
        onHide={() => setModalShow(false)}
      />
    </>
  )

    function MyVerticallyCenteredModal(props) {
    return (
    <Modal
      {...props}
      size="lg"
      aria-labelledby="contained-modal-title-vcenter"
      centered
    >
      <Modal.Header closeButton>
        <Modal.Title id="contained-modal-title-vcenter">
          Modal heading
        </Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <h4>Centered Modal</h4>
        <p>
          Cras mattis consectetur purus sit amet fermentum. Cras justo odio,
          dapibus ac facilisis in, egestas eget quam. Morbi leo risus, porta ac
          consectetur ac, vestibulum at eros.
        </p>
      </Modal.Body>
      <Modal.Footer>
        <Button onClick={props.onHide}>Close</Button>
      </Modal.Footer>
    </Modal>
  );
  }
}