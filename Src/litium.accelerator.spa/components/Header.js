import Link from "next/link";
import { relativePath } from "../utilities/url";
import Image from "./Image";
import Navigation from "./Navigation";

export default function Header({ logoUrl, topLinkList, contentLinks }) {
    const isBigHeader = false, oneHeaderRow = true;
    const logoHeight = isBigHeader ? 60 : 50,
        headerClass = isBigHeader ? "extended-header" : "compact-header",
        headerRow = oneHeaderRow ? "header__row--one-row" : "";
    return (
        <header id="header" role="banner" className="header">
            <div className={`header__row ${headerClass} ${headerRow}`}>
                <Link href={'/'}>
                    <a className="header__logo">
                        <Image priority
                            src={`${process.env.CDN_URL}${logoUrl}`}
                            height={logoHeight}
                            width={203}
                            alt={'Accelerator'}
                        />
                    </a>
                </Link>
                <div className="header__components">
                    {topLinkList && topLinkList.length && topLinkList.map(link => {
                        {link && <div className="top-link">
                            <Link className="top-link__link--block" href={relativePath(link.href)}>
                                <span className="top-link__title">{link.text}</span>
                            </Link>
                        </div>}
                    })}
                    <div className="profile">
                        <a className="profile__link--block">
                            <i className="profile__icon"></i>
                            <span className="profile__title"></span>
                        </a>
                    </div>
                </div>
                <div className="header__break"></div>
                <nav role="navigation" className="navbar">
                    <Navigation contentLinks={contentLinks} />
                </nav>
            </div>
        </header>
    );
}