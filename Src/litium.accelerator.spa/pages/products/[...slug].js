import { gql } from "@apollo/client";
import client from "../../apollo-client";
import ProductPrice from "../../components/ProductPrice";
import BuyButton from "../../components/BuyButton";
import LightboxImages from "../../components/LightboxImages";
import { useGlobalAsync } from '../../hooks/useGlobal';
import Layout from '../../components/layout';
import { useLayoutAsync } from '../../hooks/useLayout';

export default function Product({ product }) {
  return (
    <div className="row">
        <div className="small-12 medium-7 columns">
          <LightboxImages images={product.imageUrls} thumbnails={product.imageUrls} />
        </div>
        <div className="small-12 medium-5 columns">
            <div className="product-detail">
                <hgroup>
                    {product.brand && <h2 className="product-detail__brand">{product.brand}</h2>}
                    <h1 className="product-detail__name">{product.name}</h1>
                </hgroup>
                {product.description && <div className="product-detail__description">{product.description}</div>}
                <div>
                    <div className="product-detail__price-info">
                        <ProductPrice price={product.formattedPrice} />
                    </div>
                    <div>
                        <div className="row">
                            <div className="small-12 columns">
                                {/* Color and size */}
                            </div>
                        </div>
                    </div>
                </div>
                <BuyButton articleNumber={product.id} cssClass="product-detail__buy-button" label={'Add to cart'} />
            </div>
        </div>
    </div>
  );
}

Product.Layout = Layout;

export async function getStaticPaths() {
  const global = await useGlobalAsync();
  const response = await client.query({
    query: STATIC_PATH_QUERY,
    variables: { 
      global,
    }
  });
  const paths = response.data.products.list.map(product => {
    return {
      params: {
        slug: product.slug?.split('/')?.slice(3) || [],
      }
    }
  });
  return {
    paths,
    fallback: false // 'blocking'
  }
}

export async function getStaticProps({ params }) {
  const global = await useGlobalAsync();
  const layoutData = await useLayoutAsync(global);
  const response = await client.query({
    query: PAGE_QUERY,
    variables: { 
      global,
      slug: params.slug.slice(-1)[0],
    },
  });
  return {
    props: {
      product: response.data.product,
      layoutData,
    }
  }
}

const STATIC_PATH_QUERY = gql`
query ProductSearchQuery($global: GlobalInput!) {
    products(offset: 0, take: 1000, global: $global) {
      list {
        slug
        systemId
      }
    }
}      
`;

const PAGE_QUERY = gql`
  query ProductPageQuery($slug: String!, $global: GlobalInput) {
    product(slug: $slug, global: $global) {
      id
      name
      systemId
      imageUrls: images
      url: slug
      formattedPrice
      brand
      description
    }
  }
`;