import { useRouter } from 'next/router'
import { gql, useQuery } from "@apollo/client";
import client from "../../apollo-client";
import ProductList from "../../components/ProductList";
import { useGlobalAsync } from '../../hooks/useGlobal';
import Layout from '../../components/layout';
import { useLayoutAsync } from '../../hooks/useLayout';
import ProductListPlaceholder from '../../components/placeholders/ProductListPlaceHolder';

export default function Category({ category, global }) {
  const router = useRouter()
  const slug = router.query.slug || [];
  const { loading, error, data, fetchMore } = useQuery(PAGE_QUERY, { variables: { 
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

export async function getStaticPaths() {
  const global = await useGlobalAsync();
  const response = await client.query({
    query: STATIC_PATH_QUERY,
    variables: { 
      global,
    }
  });
  const paths = response.data.searchCategory.list.map(category => {
    return {
      params: {
        slug: category.slug?.split('/')?.slice(3) || [],
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
      slug: params.slug,
    },
  });
  return {
    props: {
      category: response.data.category,
      layoutData,
      global,
    }
  }
}

const STATIC_PATH_QUERY = gql`
query CategorySearchQuery($global: GlobalInput!) {
    searchCategory(global: $global, offset: 0, take: 1000) {
        list {
          slug(global: $global)
          systemId
        }
    }
}      
`;

const PAGE_QUERY = gql`
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