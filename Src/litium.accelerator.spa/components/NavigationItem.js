import React, { Fragment, useRef } from 'react';
import Link from "next/link";
import { relativePath } from '../utilities/url';
import { useRouter } from 'next/router';

const NavigationItem = ({ links = [], contentLink = null }) => {
    const menuRef = useRef(null);
    const toggleMenu = () => {
        menuRef.current.classList.toggle('navbar__menu--show');
    };
    const additionClass =
        contentLink && contentLink.attributes
            ? contentLink.attributes.cssValue
            : null;
    const router = useRouter()
    const slug = router.query.slug || [];
    const selectedClass =
        contentLink && contentLink.href && `/${slug.join('/')}/`.startsWith(`${relativePath(contentLink.href)}/`) ? 'navbar__link--selected' : '';
    const hasChildrenClass = links.length > 0 ? 'has-children' : null;
    const hasNameOrChildren = (link) =>
        link.text || (link.sectionLinkList || []).length > 0;

    return (
        <Fragment>
            {!contentLink ? (
                <a
                    className="navbar__link--block navbar__icon--menu navbar__icon"
                    onClick={toggleMenu}
                ></a>
            ) : (
                <Fragment>
                    <Link href={`/categories${relativePath(contentLink.href)}`}>
                        <a className={`navbar__link ${selectedClass} ${hasChildrenClass || ''} ${additionClass || ''}`}>{contentLink.sectionText || contentLink.text}
                        </a>
                    </Link>
                    {links.length > 0 && (
                        <i
                            className="navbar__icon--caret-right navbar__icon navbar__icon--open"
                            onClick={toggleMenu}
                        ></i>
                    )}
                </Fragment>
            )}

            {links.length > 0 && (
                <ul className="navbar__menu" ref={menuRef}>
                    <div className="navbar__menu-header">
                        {!contentLink ? (
                            <span
                                className="navbar__icon navbar__icon--close"
                                onClick={toggleMenu}
                            ></span>
                        ) : (
                            <Fragment>
                                <i
                                    className="navbar__icon--caret-left navbar__icon"
                                    onClick={toggleMenu}
                                ></i>
                                <span
                                    className="navbar__title"
                                    onClick={toggleMenu}
                                    dangerouslySetInnerHTML={{
                                        __html: contentLink.sectionText || contentLink.text,
                                    }}
                                ></span>
                            </Fragment>
                        )}
                    </div>
                    {links.length > 0 &&
                        links.map(
                            (link, i) =>
                                hasNameOrChildren(link) && (
                                    <li className="navbar__item" key={link.href || link.text || link.sectionText}>
                                        <NavigationItem
                                            links={link.sectionLinkList}
                                            contentLink={link}
                                        />
                                    </li>
                                )
                        )}
                </ul>
            )}
        </Fragment>
    );
};

export default NavigationItem;
