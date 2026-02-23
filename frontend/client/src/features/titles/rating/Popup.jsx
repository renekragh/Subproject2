import Modal from 'react-bootstrap/Modal';
import Rate from '../rating/Rate';

export default function Popup(props) {
  //console.log('Popup --> props.selectedRatings.length: '+props.selectedRatings.length)  
  return (
    <Modal
    //  {...props} SPREAD --> ERROR: React does not recognize the `%s` prop on a DOM element.
      show={props.show}
      onHide={props.onHide}
      size="lg"
      aria-labelledby="contained-modal-title-vcenter"
      centered
    >
      <Modal.Header closeButton>
        <Modal.Title id="contained-modal-title-vcenter">
        </Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <h5>RATE THIS</h5>
        <h2>{props.movie.primarytitle}</h2>
        <Rate 
          movie={props.movie}
          handleRate={props.handleRate} 
          handleDeleteRate={props.handleDeleteRate}
          ratingHistory={props.ratingHistory}
          onHide={props.onHide}
        />
      </Modal.Body>
    </Modal>
  );
}