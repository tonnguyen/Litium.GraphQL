import { gql, useQuery } from "@apollo/client";
import client from "../apollo-client";
import BlockArea from '../components/BlockArea';
import PageByTemplate from '../components/PageByTemplate';
import { useGlobalAsync } from '../hooks/useGlobal';
import Layout from '../components/layout';
import { useLayoutAsync } from '../hooks/useLayout';
import { relativePath } from "../utilities/url";
import LightboxImages from "../components/LightboxImages";
import ProductPrice from "../components/ProductPrice";
import BuyButton from "../components/BuyButton";
import { useRouter } from "next/router";
import ProductListPlaceholder from "../components/placeholders/ProductListPlaceHolder";
import ProductList from '../components/ProductList';

export default function Page({ content, global, layoutData }) {
  if (content.__typename === 'Product') {
    return <Product product={content} layoutData={layoutData} />
  }
  if (content.__typename === 'Category') {
    return <Category category={content} layoutData={layoutData} global={global} />
  }
  return (
      <main className="main-content">
          <PageByTemplate pageType={content.templateId} valueAsJSON={content.valueAsJSON} />
          {content.areas.map(area => (
              <BlockArea blocks={area.blocks} key={area.id}></BlockArea>
          ))}
      </main>
  );
}
Page.Layout = Layout;

function Category({ category, global }) {
  const router = useRouter()
  const slug = router.query.slug || [];
  const { loading, error, data, fetchMore } = useQuery(CATEGORY_QUERY, { variables: { 
    slug,
    global,
  } });
  return (
      <div className="row">
        <aside className="columns large-2 show-for-large"></aside>
        <div className="medium-12 large-10 columns">
          <h1>{category.name}</h1>
          {products(loading, error, data, fetchMore)}
        </div>
      </div>
  );
}
const products = (loading, error, data, fetchMore) => {
  if (error) {
    console.log(error);
  }
  if (error || loading) {
    return <ProductListPlaceholder />;
  }
  return (
    <ProductList products={data.category.products} 
      loading={loading} 
      fetchMore={() => 
        fetchMore({
          variables: {
            offset: data.category.products.list.length,
          }
        })
      }
    ></ProductList>
  );
}
Category.Layout = Layout;

function Product({ product }) {
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
  const pagePaths = response.data.pages.list.filter(page => page.slug && page.templateId !== 'Home').map(page => {
    return {
      params: {
        slug: relativePath(page.slug).split('/').slice(1),
      }
    }
  });
  const productPaths = response.data.products.list.map(product => {
    return {
      params: {
        slug: relativePath(product.slug).split('/').slice(1),
      }
    }
  });
  const categoryPaths = response.data.categories.list.map(category => {
    return {
      params: {
        slug: relativePath(category.slug).split('/').slice(1),
      }
    }
  });
  const paths = [
    ...pagePaths,
    ...categoryPaths,
    ...productPaths,
  ];
  return {
    paths,
    fallback: false //'blocking'
  }
}

export async function getStaticProps({ params }) {
  const global = await useGlobalAsync();
  const layoutData = await useLayoutAsync(global);
  const response = await client.query({
    query: QUERY,
    variables: { 
      global,
      slug: params.slug,
    },
  });
  return {
    props: {
      content: response.data.content,
      layoutData,
      global,
    }
  }
}

const STATIC_PATH_QUERY = gql`
query PageSearchQuery($global: GlobalInput!) {
  pages: searchPage(global: $global, offset: 0, take: 10000) {
      list {
        slug(global: $global)
        systemId
        templateId
      }
  }
  products(offset: 0, take: 10000, global: $global) {
    list {
      slug
      systemId
    }
  }
  categories: searchCategory(global: $global, offset: 0, take: 10000) {
      list {
        slug(global: $global)
        systemId
      }
  }
}      
`;

const QUERY = gql`
query IndexPageQuery($global: GlobalInput, $slug: [String]) {
  content(slug: $slug, global: $global) {
    __typename
    ... on Page {
      systemId
      templateId
      valueAsJSON(global: $global)
      areas {
        id
        blocks {
          systemId
          blockType
          valueAsJSON(global: $global)
        }
      }
    }
    ... on Product {
      id
      name
      systemId
      imageUrls: images
      url: slug
      formattedPrice
      brand
      description
    }

    ... on Category {
      name(global: $global)
      systemId
    }
  }
}
`;

const CATEGORY_QUERY = gql`
  query CategoryPageQuery($slug: [String]!, $global: GlobalInput,
      $offset: Int = 0, $take: Int = 8) {
    category(slug: $slug, global: $global) {
      name(global: $global)
      systemId
      products(offset: $offset, take: $take, global: $global) {
        hasNextPage
        list {
          id
          name
          systemId
          imageUrls: images
          url: slug
          formattedPrice
          brand
          showBuyButton
        }
      }
    }
  }
`;