import ReactPlaceholder from 'react-placeholder';
import { TextRow } from 'react-placeholder/lib/placeholders';
import "react-placeholder/lib/reactPlaceholder.css";

export default function ProductListPlaceholder() {
    return (
        <div data-productlist>
            <div className="product-list">
                <ul className="row product-list">
                    <Product />
                    <Product />
                    <Product />
                    <Product />
                    <Product />
                    <Product />
                    <Product />
                    <Product />
                </ul>
            </div>
        </div>
    )
}

const Product = () => (
    <li className="product-list__item columns small-6 medium-4 large-3">
        <ReactPlaceholder showLoadingAnimation={true}
            type='rect' ready={false} color='#E0E0E0' 
            style={{ height: 400 }} />
        <TextRow color='#E0E0E0' showLoadingAnimation={true} />
        <TextRow color='#E0E0E0' showLoadingAnimation={true} />
        <TextRow color='#E0E0E0' showLoadingAnimation={true} />
        <TextRow color='#E0E0E0' showLoadingAnimation={true} />
    </li>
)