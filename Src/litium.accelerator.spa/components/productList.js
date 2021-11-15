import ProductItem from "./ProductItem";

export default function ProductList({ products, loading, fetchMore }) {
    return (
        products.list && products.list.length > 0 && 
        <>
            <div data-productlist>
                <div className="product-list">
                    <ul className="row product-list">
                        {products.list.map((product) => (
                        <li className="product-list__item columns small-6 medium-4 large-3" key={product.systemId}>
                            <ProductItem {...product} />
                        </li>
                        ))}
                    </ul>
                </div>
            </div>
            
            {!loading && products.hasNextPage && 
            <span><a className="button buy-button product__buy-button" onClick={() => fetchMore && fetchMore()}>Show more</a></span>}
        </>
    );
}