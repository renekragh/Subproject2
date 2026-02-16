import { useState } from 'react'  
import Button from 'react-bootstrap/Button';
  
  const COUNT = 10;
  const ICON = <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="currentColor" className="bi bi-star-fill" viewBox="0 0 16 16">
                    <path d="M3.612 15.443c-.386.198-.824-.149-.746-.592l.83-4.73L.173 6.765c-.329-.314-.158-.888.283-.95l4.898-.696L7.538.792c.197-.39.73-.39.927 0l2.184 4.327 4.898.696c.441.062.612.636.282.95l-3.522 3.356.83 4.73c.078.443-.36.79-.746.592L8 13.187l-4.389 2.256z"/>
                </svg>;
  const UNSELECTED_COLOR = "grey";
  const SELECTED_COLOR = "yellow";

export default function Rate(props) {
    const currentRate = props.ratingHistory.find(x => x.id === props.movie.id);
    const [rating, setRating] = useState(currentRate !== undefined ? currentRate.rating : undefined);
    const [tempRating, setTempRating] = useState(0);
    const [isFinalRateBtnDisabled, setIsFinalRateBtnDisabled] = useState(true);
    const [isRemoveRatingBtnVisible, setIsRemoveRatingBtnVisible] = useState(rating === undefined ? false : true);
    const handleSelectedRate= (selectedRate) => {
        setRating(selectedRate);
        setIsFinalRateBtnDisabled(false);
    };
    const handleFinalRate = () => {
        props.handleRate(props.movie.id, rating);
        props.onHide(true);
        setIsFinalRateBtnDisabled(true);
    }
    const handleDeleteRating = () => {
        props.handleDeleteRate(props.movie.id);
        props.onHide(true);
        setIsRemoveRatingBtnVisible(false);
    }
    let stars = Array(COUNT).fill(ICON);

    //console.log('Rate --> props.selectedRatings.length: '+props.selectedRatings.length)  

    return (
        <>
        <br />
        <svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" fill="yellow" className="bi bi-star-fill" viewBox="0 0 16 16">
                <path d="M3.612 15.443c-.386.198-.824-.149-.746-.592l.83-4.73L.173 6.765c-.329-.314-.158-.888.283-.95l4.898-.696L7.538.792c.197-.39.73-.39.927 0l2.184 4.327 4.898.696c.441.062.612.636.282.95l-3.522 3.356.83 4.73c.078.443-.36.79-.746.592L8 13.187l-4.389 2.256z"/>
          <text  x="5" y="10" className="starIcon">{tempRating > 0 ? tempRating : rating}</text>
        </svg>
        <br /><br />
        <div className="starsContainer">     
            {stars.map((item, index) => {
                const isActiveColor =
                (rating || tempRating) &&
                (index < rating || index < tempRating);

                let elementColor = "";
                if (isActiveColor) elementColor = SELECTED_COLOR;
                else elementColor = UNSELECTED_COLOR;

                return (
                    <div 
                        className="star" 
                        key={index} 
                        style={{color: elementColor}}
                        onMouseEnter={() => setTempRating(index +1)}
                        onMouseLeave={() => setTempRating(0)}
                        onClick={() => handleSelectedRate(index +1) }
                    >
                        {ICON}
                    </div>
                );
            })}
        </div>
        <br />
         <Button onClick={() => handleFinalRate()} variant="primary" disabled={isFinalRateBtnDisabled}>Rate</Button>
         <span className='separator'></span>
         {isRemoveRatingBtnVisible &&
            <Button onClick={() => handleDeleteRating()} variant="primary">Remove rating</Button>
         }
        </>
    )
}