export default function Article({ data }) {
    const { title, text, introduction } = data;
    return (
        <div className="row">
            <div className="small-12 medium-8 columns">
                <h1>{title}</h1>
                {introduction && <p className="intro">{introduction}</p>}
                <p dangerouslySetInnerHTML={{__html: text.value}}></p>
            </div>
        </div>
    );
}