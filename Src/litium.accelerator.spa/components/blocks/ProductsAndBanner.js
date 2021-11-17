import Link from "next/link";
import ProductItem from "../ProductItem";
import { relativePath } from "../../utilities/url";
import Image from "../Image";

export default function ProductsAndBanner({ data }) {
    if (!data) {
        return <></>;
    }
    const { showProductToTheRight } = data;
    return (
        <div className="row mixed">
            {!showProductToTheRight && (
                <>
                    <ProductMixed data={data} />
                    <BannersMixed data={data} />
                </>
            )}
            {showProductToTheRight && (
                <>
                    <BannersMixed data={data} />
                    <ProductMixed data={data} />
                </>
            )}
        </div>
    );
}

const ProductMixed = ({ data }) => {
    const { title, products } = data;
    const productColumn = products.products.length > 0 ? 12 / Math.min(products.products.length, 4) : 0;
    return (
    <div className="small-12 large-6 columns">
        <div className="row mixed-block">
            <div className="small-8 columns">
                <h2 className="mixed-block__header">{title}</h2>
            </div>
            {products.footerLinkUrl && products.footerLinkText &&
                <div className="small-4 columns text--right">
                    <Link className="mixed-block__header" href={relativePath(products.footerLinkUrl)}>{products.footerLinkText}</Link>
                </div>}
        </div>
        {products && products.products.length > 0 &&
            <div className="row">
                {products.products.map(product => (
                    <div className={`columns small-12 large-${productColumn} product--mixed`} key={product.id}>
                        <ProductItem {...product} />
                    </div>
                ))}
            </div>}
    </div>);
}

const BannersMixed = ({ data }) => {
    const { banners } = data;
    const bannerColumn = banners.banners.length > 0 ? 12 / Math.min(banners.banners.length, 4) : 0;
    return <div className="small-12 large-6 columns">
        {banners && banners.banners.length > 0 &&
            <div className="row">
                {banners.banners.map(banner => (
                    <div className={`columns small-12 large-${bannerColumn} product-and-banner-block__content`} key={banner.linkUrl}>
                        {banner.imageUrl && <Link href={relativePath(banner.linkUrl)} className="product-and-banner-block__image-link">
                            <a><Image priority className="product-and-banner-block__image"
                                src={`${process.env.CDN_URL}${banner.imageUrl}`}
                                width={768}
                                height={768}
                                alt={banner.linkText}
                            /></a>
                        </Link>}
                        <div className="banner-text">
                            <h3 className="banner-text__title">{banner.linkText}</h3>
                            {banner.actionText && <span className="banner-text__button">{banner.actionText}</span>}
                        </div>
                    </div>
                ))}
            </div>}
    </div>
}