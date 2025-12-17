function KnownFor({movies}) {
    return (
             movies.map(m => { 
                return (
                    <>
                        <li key={m.title}>
                            <br /><span><b>Title:</b></span><br /> {m.title}
                        </li>
                        <li key={m.release_date}>
                             <span><b>Release date:</b></span><br />{m.release_date}
                        </li>
                            <li key={m.overview}>
                             <span><b>Overview:</b></span><br />{m.release_date}{m.overview}
                        </li>
                    </>
                        )
            })
    );
}

export default KnownFor;