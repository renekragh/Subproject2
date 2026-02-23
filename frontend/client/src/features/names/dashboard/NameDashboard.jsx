
import NameList from './NameList';
import NameDetail from '../NameDetail'

export default function NameDashboard(props) {
  
  return (
    <>
      {!props.selectedName && 
        <NameList 
          selectName={props.selectName}
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