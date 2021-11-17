import Link from "next/link";
import ProductPrice from "./ProductPrice";
import { relativePath } from "../utilities/url";
import BuyButton from "./BuyButton";
import Image from "./Image";

export default function ProductItem({ imageUrls, url, formattedPrice, name, brand, id,
                                    showBuyButton/*, showQuantityField, useVariantUrl*/ }) {
    const maxHeight = 300, maxWidth = 220;
    return (
        <div itemType="http://schema.org/Product" className="product__wrapper">
            <figure className="product__figure">
                {imageUrls && imageUrls.length > 0 && 
                <Link href={`${relativePath(url)}`} className="product__image-link">
                    <a><Image priority className="product__image"
                        src={`${process.env.CDN_URL}${imageUrls[0]}`}
                        height={maxHeight}
                        width={maxWidth}
                        alt={name}
                    /></a>
                </Link>}
                {(!imageUrls || imageUrls.length < 1) && <span className="product__image--missing">{'product.noproductimage'}</span>}
            </figure>
            <article className="product__info">
                <Link href={`${relativePath(url)}`}>
                    <a>
                        <h3 itemProp="name" className="product__name">{name}</h3>
                        {brand && <h4 itemProp="brand" className="product__brand">{brand}</h4>}
                    </a>
                </Link>
                <div className="product__price" itemProp="offers" itemType="http://schema.org/Offer">
                    {formattedPrice && <ProductPrice price={formattedPrice} />}
                </div>
                {showBuyButton && <BuyButton articleNumber={id} cssClass="button buy-button product__buy-button" label={'Add to cart'} />}
            </article>
        </div>
    );
}