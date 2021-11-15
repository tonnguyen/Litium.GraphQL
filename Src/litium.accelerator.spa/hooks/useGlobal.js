import { gql } from "@apollo/client";
import client from "../apollo-client";

export async function useGlobalAsync () {
    const response = await client.query({
        query: QUERY,
    });
  
    const obj = {
        ...response.data.global,
        __typename: undefined,
    };
    delete obj.__typename;
    return obj;
}

const QUERY = gql`
    query GlobalQuery {
            global {
            channelSystemId
            currencySystemId
            countrySystemId
            assortmentSystemId
            currentCulture
            currentUICulture
            websiteSystemId
        }
    }      
`;