import { ApolloClient, InMemoryCache } from "@apollo/client";
import pageInfoOffsetLimitPagination from "./utilities/pageInfoOffsetLimitPagination";

const client = new ApolloClient({
    uri: `${process.env.GRAPH_SERVER_URL}/graphql`,
    cache: new InMemoryCache({
        typePolicies: {
            Category: {
                keyFields: ["systemId"],
                fields: {
                    products: pageInfoOffsetLimitPagination(["slug"]),
                }
            },
            Query: {
                fields: {
                    products: pageInfoOffsetLimitPagination(["slug"]),
                    categories: pageInfoOffsetLimitPagination(["slug", "assortmentSystemId", "culture"]),
                },
            },
            // Page: {
            //     keyFields: ["systemId"],
            // },
            // BlockArea: {
            //     keyFields: ["id"],
            // },
            // Block: {
            //     keyFields: ["systemId"],
            // },
        },
      }),
});

export default client;