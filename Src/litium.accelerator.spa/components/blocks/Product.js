import Link from "next/link";
import { relativePath } from "../../utilities/url";
import ProductItem from "../ProductItem";

export default function Product({ data }) {
    if (!data) {
        return <></>;
    }
    const { title, footerLinkUrl, footerLinkText, products } = data;
    return (
        <>
            <div className="row">
                <h2 className="columns small-12 product-list__header text--center">{title}</h2>
            </div>
            {footerLinkUrl && <div className="row">
                <div className="columns small-12 text--right">
                    <Link className="product-list__link" href={`${relativePath(footerLinkUrl)}`}><a>{footerLinkText}</a></Link>
                </div>
            </div>}
            {products && <ul className="row product-list">
                {products.map(product => (
                    <li className="product-list__item columns small-6 medium-4 large-3" key={product.id}>
                        <ProductItem {...product} />
                    </li>
                ))}
            </ul>}
        </>
    );
}