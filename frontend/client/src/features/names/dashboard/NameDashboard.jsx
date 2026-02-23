import NameList from './NameList';
import NameDetail from '../NameDetail'
//import { useEffect, useState } from 'react'
//import { useAuth } from '../../common/AuthProvider';

export default function NameDashboard(props) {
  //const [names, setNames] = useState([]);
  //const [bookmarks, setBookmarks] = useState([]);
  //const { token } = useAuth();

  /*
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
*/

/*
  useEffect(() => {
      console.log("____________________ NameDashboard useEffect( ");
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
*/

  return (
    <>
      {!props.selectedName && 
        <NameList 
          names={props.names} 
          selectName={props.selectName}
          bookmarks={props.bookmarks}
          handleBookmark={props.handleBookmark}
          handleDeleteBookmark={props.handleDeleteBookmark}
        />  
      }
        
      {props.selectedName && 
        <NameDetail 
          name={props.selectedName}
          cancelSelectName={props.cancelSelectName}
        />
      }
    </>
  )
}