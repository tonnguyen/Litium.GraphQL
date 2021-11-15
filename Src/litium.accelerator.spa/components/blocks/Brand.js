import Link from "next/link";
import { relativePath } from '../../utilities/url';
import Image from "../Image";

export default function Brand({ data }) {
    const { title, linkText, urlLink, pageUrls } = data;
    return (
        <div className="columns">
            <div className="brand-block__header ">
                <h2 className="brand-block__title">{title}</h2>
                {urlLink && urlLink.href && 
                    <Link href={urlLink.href}><a className="brand-block__link">{linkText}</a></Link>}
            </div>
            <ul className="row small-up-2 medium-up-3 large-up-6 brand-block">
                {pageUrls && pageUrls.map(item => (
                    <li className="column column-block" key={item.href}>
                        <Link href={item.href}>
                            <a className="brand-block__image-link">
                                {item.imageUrl && <Image priority className="brand-block__image"
                                    src={`${process.env.CDN_URL}${relativePath(item.imageUrl)}`}
                                    height={60}
                                    width={200}
                                    alt={item.text}
                                />}
                            </a>
                        </Link>
                    </li>
                ))}
            </ul>
        </div>
    );
}