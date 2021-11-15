import { gql } from "@apollo/client";
import client from "../apollo-client";
import BlockArea from "../components/BlockArea";
import { useGlobalAsync } from "../hooks/useGlobal";
import Layout from "../components/layout";
import { useLayoutAsync } from "../hooks/useLayout";

export default function Home({ page }) {
  const mainBlockArea = page.areas.find(a => a.id === 'Main');
  return (
      <main className="main-content">
        <BlockArea blocks={mainBlockArea.blocks}></BlockArea>
      </main>
  );
}

Home.Layout = Layout;

export async function getStaticProps() {
  const global = await useGlobalAsync();
  const layoutData = await useLayoutAsync(global);
  const response = await client.query({
    query: PAGE_QUERY,
    variables: { 
      global,
    }
  });
  return {
    props: {
      page: response.data.page,
      layoutData,
    }
  }
}

const PAGE_QUERY = gql`
  query IndexPageQuery($global: GlobalInput) {
      page(global: $global) {
        systemId
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