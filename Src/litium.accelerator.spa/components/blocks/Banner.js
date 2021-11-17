import Link from "next/link";
import { relativePath } from '../../utilities/url';
import Image from "../Image";

export default function Banner({ data, systemId }) {
    const { banners } = data;
    const hasBanner = banners && banners.length > 0;
    const largeColumn = hasBanner ? 12 / Math.min(banners.length, 4) : 12;
    const width = largeColumn ? 1314 : 642;
    const height = largeColumn ? 503 : 642;
    const cssClasses = `columns small-12 large-${largeColumn} ${largeColumn === 12 ? 'single-banner' : 'multiple-banner'}`;
    return (
        <>
        {hasBanner && <div className="row">
            {banners.map(({ imageUrl, linkText, linkUrl, actionText }) => (
                <div className={cssClasses} key={`${systemId}${imageUrl}`}>
                    <div className="teaser">
                        <Link href={linkUrl} className="banner-block__image-link">
                            <a>
                                <Image priority className="banner-block__image"
                                    src={`${process.env.CDN_URL}${relativePath(imageUrl)}`}
                                    height={height}
                                    width={width}
                                    alt={linkText}
                                />
                            </a>
                        </Link>
                        <div className="banner-text">
                            <h3 className="banner-text__title">{linkText}</h3>
                            {actionText && <span className="banner-text__button">{actionText}</span>}
                        </div>
                    </div>
                </div>
            ))}
        </div>}
        </>
    );
}