import TitleList from './TitleList';
import TitleDetail from '../details/TitleDetail';

export default function TitleDashboard(props) {

  return (
    <>
      {!props.selectedMovie && 
        <TitleList 
          movies={props.movies} 
          selectMovie={props.selectMovie}
          handleRate={props.handleRate}
          handleDeleteRate={props.handleDeleteRate}
          ratingHistory={props.ratingHistory}
          bookmarks={props.bookmarks}
          handleBookmark={props.handleBookmark}
          handleDeleteBookmark={props.handleDeleteBookmark}
        />  
      }
        
      {props.selectedMovie && 
        <TitleDetail 
          movie={props.selectedMovie}
          cancelSelectMovie={props.cancelSelectMovie}
        />
      }
    </>
  )
}