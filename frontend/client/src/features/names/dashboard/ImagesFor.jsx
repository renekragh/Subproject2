import { useState, useEffect } from "react";

export function ImagesFor({id}) {
    const [profiles, setProfiles] = useState([]);

    useEffect(() => {
        fetch(`https://api.themoviedb.org/3/person/${id}/images?api_key=85eb017598662c36df7a4aaf487790ff`)
            .then(res => res.json())
            .then(data => setProfiles(data.profiles));
    }, [id]);
    const baseUrl = "https://image.tmdb.org/t/p/";
    const fileSize = "w45";

     return (
      profiles.map((p) => 
                    <img src={baseUrl+fileSize+p.file_path} />
                )
     );
}