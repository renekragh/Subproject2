import { useState, useRef } from 'react';
import Button from 'react-bootstrap/Button';
import Offcanvas from 'react-bootstrap/Offcanvas';

// TO DO
// 1) DEFAULT_PLACEHOLDER_TXT show/hide in/out textarea
// 2) bookmarked: after clear note, pressing the enter button and clear note not disappear 
// 3) after update method on page refresh Update bookmark button is active (until manuel page refresh)

export default function Bookmark({ ...props }) {
  const bookmark = props.bookmarks.find(x => x.id === props.movie.id);
  const inputRef = useRef('');
  const [note, setNote] = useState('');
  const [show, setShow] = useState(false);
  const handleClose = () => { setShow(false)};
  const handleShow = () => setShow(true);
  const [hideClearBtn, setHideClearBtn] = useState(true);
  const [hideUpdateBtn, setHideUpdateBtn] = useState(true);
  const isBookmarked = props.bookmarks.map(x => x.id).includes(props.movie.id);
 
  const PLUS_ICON = <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-plus" viewBox="0 0 16 16">
                      <path d="M8 4a.5.5 0 0 1 .5.5v3h3a.5.5 0 0 1 0 1h-3v3a.5.5 0 0 1-1 0v-3h-3a.5.5 0 0 1 0-1h3v-3A.5.5 0 0 1 8 4"/>
                    </svg>
  const CHECK_ICON = <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-check-lg" viewBox="0 0 16 16">
                        <path d="M12.736 3.97a.733.733 0 0 1 1.047 0c.286.289.29.756.01 1.05L7.88 12.01a.733.733 0 0 1-1.065.02L3.217 8.384a.757.757 0 0 1 0-1.06.733.733 0 0 1 1.047 0l3.052 3.093 5.4-6.425z"/>
                      </svg>
  const CANCEL_ICON = <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-x" viewBox="0 0 16 16">
                        <path d="M4.646 4.646a.5.5 0 0 1 .708 0L8 7.293l2.646-2.647a.5.5 0 0 1 .708.708L8.707 8l2.647 2.646a.5.5 0 0 1-.708.708L8 8.707l-2.646 2.647a.5.5 0 0 1-.708-.708L7.293 8 4.646 5.354a.5.5 0 0 1 0-.708"/>
                      </svg>
  const DELETE_ICON = <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-trash3-fill" viewBox="0 0 16 16">
                        <path d="M11 1.5v1h3.5a.5.5 0 0 1 0 1h-.538l-.853 10.66A2 2 0 0 1 11.115 16h-6.23a2 2 0 0 1-1.994-1.84L2.038 3.5H1.5a.5.5 0 0 1 0-1H5v-1A1.5 1.5 0 0 1 6.5 0h3A1.5 1.5 0 0 1 11 1.5m-5 0v1h4v-1a.5.5 0 0 0-.5-.5h-3a.5.5 0 0 0-.5.5M4.5 5.029l.5 8.5a.5.5 0 1 0 .998-.06l-.5-8.5a.5.5 0 1 0-.998.06m6.53-.528a.5.5 0 0 0-.528.47l-.5 8.5a.5.5 0 0 0 .998.058l.5-8.5a.5.5 0 0 0-.47-.528M8 4.5a.5.5 0 0 0-.5.5v8.5a.5.5 0 0 0 1 0V5a.5.5 0 0 0-.5-.5"/>
                      </svg>
  const CLEAR_ICON = <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-eraser-fill" viewBox="0 0 16 16">
                      <path d="M8.086 2.207a2 2 0 0 1 2.828 0l3.879 3.879a2 2 0 0 1 0 2.828l-5.5 5.5A2 2 0 0 1 7.879 15H5.12a2 2 0 0 1-1.414-.586l-2.5-2.5a2 2 0 0 1 0-2.828zm.66 11.34L3.453 8.254 1.914 9.793a1 1 0 0 0 0 1.414l2.5 2.5a1 1 0 0 0 .707.293H7.88a1 1 0 0 0 .707-.293z"/>
                      </svg>
  const UPDATE_ICON = <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-arrow-clockwise" viewBox="0 0 16 16">
                        <path fillRule="evenodd" d="M8 3a5 5 0 1 0 4.546 2.914.5.5 0 0 1 .908-.417A6 6 0 1 1 8 2z"/>
                        <path d="M8 4.466V.534a.25.25 0 0 1 .41-.192l2.36 1.966c.12.1.12.284 0 .384L8.41 4.658A.25.25 0 0 1 8 4.466"/>
                      </svg>
  const DEFAULT_PLACEHOLDER_TXT = 'Your eventual note here...';

  const onChangeHandler = (e) => {
    setNote(e.target.value);
    const defaultNote =  document.getElementById('txtnote').defaultValue.trim();
    defaultNote.length !== e.target.value.trim().length ? setHideUpdateBtn(false) : setHideUpdateBtn(true);
    defaultNote.length !== e.target.value.trim().length ? setHideClearBtn(false) : setHideClearBtn(true);
    if (e.target.value === '') setHideClearBtn(true);
  }

  const handleBookmark = () => {
    props.handleBookmark(props.movie.id, note);
    handleClose();
  }

  const clearNote = () => {
    inputRef.current.value = '';
    setNote(undefined);
    const defaultNote =  document.getElementById('txtnote').defaultValue.trim();
    defaultNote.length !== inputRef.current.value.trim().length ? setHideUpdateBtn(false) : setHideUpdateBtn(true);
    setHideClearBtn(true);
  }

  const handleDeleteBookmark = () => {
    props.handleDeleteBookmark(props.movie.id);
    handleClose();
  }

  const handleCancel = () => {
    setNote(undefined);
    setHideClearBtn(true);
    setHideUpdateBtn(true);
    handleClose();
  }

  return (
    <>
       {!isBookmarked &&
        <Button variant="primary" onClick={handleShow} className="me-2">
          {PLUS_ICON} Bookmark
        </Button>
      }       
      {isBookmarked &&
        <Button variant="success" onClick={handleShow} className="me-2">
          {CHECK_ICON} Bookmarked
        </Button>
      }
      <Offcanvas /* {...props} SPREAD --> ERROR: React does not recognize the `%s` prop on a DOM element. */ 
        show={show} 
        onHide={handleClose} 
        key={props.movie.id} 
        placement='bottom'
        backdrop='false'
      >
        <Offcanvas.Header>
          {props.movie.primarytitle !== undefined &&
          <Offcanvas.Title>Bookmark "<em>{props.movie.primarytitle}</em>"</Offcanvas.Title>
          }
          {props.movie.primaryName !== undefined &&
          <Offcanvas.Title>Bookmark "<em>{props.movie.primaryname}</em>"</Offcanvas.Title>
          }
        </Offcanvas.Header>
        <Offcanvas.Body>
          {(!hideClearBtn || (bookmark !== undefined && bookmark.note !== null)) && <label>{DEFAULT_PLACEHOLDER_TXT}</label>}
          <br />
          <textarea 
            id="txtnote" 
            placeholder={DEFAULT_PLACEHOLDER_TXT}
            defaultValue={bookmark === undefined ? '' : bookmark.note}
            onChange={onChangeHandler}         
            ref={inputRef}                         
            rows={4} 
            cols={60} 
          />
          <br /> 
          {isBookmarked && 
            <>
              { (!hideClearBtn || (bookmark !== undefined && bookmark.note !== null && hideUpdateBtn)) &&
                  <Button onClick={() => clearNote()}  variant="warning">
                    {CLEAR_ICON} Clear note
                  </Button>
              } 
              { (!hideUpdateBtn) &&
                  <Button onClick={() => handleBookmark()}  variant="info">
                    {UPDATE_ICON} Update bookmark
                  </Button>
              }
                <Button onClick={() => handleDeleteBookmark()}  variant="danger">
                  {DELETE_ICON} Remove bookmark
                </Button>
                <Button onClick={() => handleCancel()}  variant="secondary">
                  {CANCEL_ICON} Cancel
                </Button>
            </>
          } 
          {!isBookmarked && 
            <>
              { !hideClearBtn &&  
                <Button onClick={() => clearNote()}  variant="warning">
                  {CLEAR_ICON} Clear note
                </Button>
              }
              <Button onClick={() => handleBookmark()}  variant="dark">
                {PLUS_ICON} Save bookmark
              </Button>
              <Button onClick={() => handleCancel()}  variant="secondary">
                {CANCEL_ICON} Cancel
              </Button>
            </>
          } 
        </Offcanvas.Body>
      </Offcanvas>
    </>
  );
}