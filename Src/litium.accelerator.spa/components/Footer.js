import Link from "next/link";
import { relativePath } from "../utilities/url";

export default function Footer({ sectionList }) {
    return (
        <footer className="footer">
            <div className="row">
                {sectionList && sectionList.map(sectionItem => (
                    <section className="columns small-10 small-offset-1 medium-offset-0 medium-5 large-3 footer__section" key={sectionItem.sectionTitle}>
                        <h4 className="footer__header">{sectionItem.sectionTitle}</h4>
                        {sectionItem.sectionLinkList && sectionItem.sectionLinkList.map(linkItem => (
                            <p key={linkItem.href}><Link href={`${relativePath(linkItem.href)}`}><a className="footer__link">{linkItem.text}</a></Link></p>
                        ))}
                        {sectionItem.sectionText && <div dangerouslySetInnerHTML={{__html: sectionItem.sectionText}}></div>}
                    </section>
                ))}
            </div>
        </footer>
    );
}