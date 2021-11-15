import { gql } from "@apollo/client";
import client from "../apollo-client";
import BlockArea from '../components/BlockArea';
import PageByTemplate from '../components/PageByTemplate';
import { useGlobalAsync } from '../hooks/useGlobal';
import Layout from '../components/layout';
import { useLayoutAsync } from '../hooks/useLayout';
import { relativePath } from "../utilities/url";

export default function Page({ page }) {
  return (
      <main className="main-content">
          <PageByTemplate pageType={page.templateId} valueAsJSON={page.valueAsJSON} />
          {page.areas.map(area => (
              <BlockArea blocks={area.blocks} key={area.id}></BlockArea>
          ))}
      </main>
  );
}

Page.Layout = Layout;

export async function getStaticPaths() {
  const global = await useGlobalAsync();
  const response = await client.query({
    query: STATIC_PATH_QUERY,
    variables: { 
      global,
    }
  });
  const paths = response.data.searchPage.list.filter(page => page.slug && page.templateId !== 'Home').map(page => {
    return {
      params: {
        slug: relativePath(page.slug).split('/').slice(1),
      }
    }
  });
  return {
    paths,
    fallback: false //'blocking'
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
      page: response.data.page,
      layoutData,
    }
  }
}

const STATIC_PATH_QUERY = gql`
query PageSearchQuery($global: GlobalInput!) {
    searchPage(global: $global, offset: 0, take: 1000) {
        list {
          slug(global: $global)
          systemId
          templateId
        }
    }
}      
`;

const PAGE_QUERY = gql`
  query PageQuery($slug: [String]!, $global: GlobalInput) {
    page(slug: $slug, global: $global) {
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
  }
`;