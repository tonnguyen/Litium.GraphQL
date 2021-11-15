import { gql } from "@apollo/client";
import client from "../apollo-client";
import { useGlobalAsync } from "./useGlobal";

export async function useLayoutAsync (global = null) {
    global = global || await useGlobalAsync();
    const response = await client.query({
        query: QUERY,
        variables: {
            global,
            websiteSystemId: global.websiteSystemId,
        }
    });
  
    return response.data;
}

const QUERY = gql`
  query LayoutQuery($websiteSystemId: ID,
                    $global: GlobalInput
                      ) {
    website(systemId: $websiteSystemId) {
      logoUrl
      header(global: $global) {
        sectionList {
            sectionText
            sectionTitle
            href
            sectionLinkList {
              href
              text
            }
        }
      }
      footer(global: $global) {
        sectionList {
            sectionText
            sectionTitle
            sectionLinkList {
              href
              text
            }
        }
      }
    }
  }      
`;