import { useState, useEffect } from "react";

export function ImagesFor(props) {
    const API_KEY = '85eb017598662c36df7a4aaf487790ff';
    const [personId, setPersonId] = useState(null);
    const [profiles, setProfiles] = useState([]);
    const baseUrl = 'https://image.tmdb.org/t/p/';
    const fileSize = 'w45';
    const defaultImage = '/404_not_found.webp';
    let firstValidFilePath = undefined;

    useEffect(() => {
        async function load() {
            try {
                const res = await fetch(`https://api.themoviedb.org/3/find/${props.id}?external_source=imdb_id&api_key=${API_KEY}`);
                if (!res.ok) throw new Error("status = " + res.status);
                const data = await res.json(); 
                if (data.person_results[0] === undefined) throw new Error('unexpected data');
                setPersonId(data.person_results[0].id);
            } catch (err) {
          //  console.log("Error: " + err.message);
            setPersonId(null);
            } 
        }   
        load();
        },[props.id]
    );

    useEffect(() => {
        async function load() {
            try {
                const res = await fetch(`https://api.themoviedb.org/3/person/${personId}/images?api_key=${API_KEY}`);
                if (!res.ok) throw new Error("status = " + res.status);
                const data = await res.json(); 
                if (data.profiles === undefined) throw new Error('unexpected data');
                setProfiles(data.profiles);
            } catch (err) {
          // console.log("Error: " + err.message);
            setProfiles([])
            } 
        }
        load();
        },[personId]
    );

        if (profiles.length > 0) {
            let isImageFound = true;
            profiles.some((p) => 
                {
                    <img src={baseUrl+fileSize+p.file_path} onError={isImageFound === false} />
                    if (isImageFound) {
                        firstValidFilePath = p.file_path;
                        return true;
                    } 
                    return false;
                }
            )
        }
    return firstValidFilePath !== undefined ? <img src={baseUrl+fileSize+firstValidFilePath} /> : <img src={defaultImage} />
}