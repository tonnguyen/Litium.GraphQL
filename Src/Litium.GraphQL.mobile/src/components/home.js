import React from 'react';
import { gql } from 'apollo-boost';
import { useQuery } from '@apollo/react-hooks';
import ProductList from './product-list';
import { Spinner } from 'native-base';

const Home = ({navigation}) => {
    const query = gql`
    query LitiumQuery($skip: Int = 0, $take: Int = 10) {
          products(skip: $skip, take: $take) {
            hasNextPage
            list {
              name
              systemId
              images
              variants {
                  price {
                    formattedPrice
                  }
              }
            }
          }
      }      
    `;
    const { loading, error, data, fetchMore } = useQuery(query);
    const updateQuery = (prev, { fetchMoreResult }) => {
        if (!fetchMoreResult) return prev;
        return Object.assign({}, prev, {
            products: {
                ...prev.products,
                hasNextPage: fetchMoreResult.products.hasNextPage,
                list: [...prev.products.list, ...fetchMoreResult.products.list],
                __typename: prev.products.__typename,
            },
        });
    }
    return (
        <>
            {loading && <Spinner />}
            {!loading && <ProductList data={data.products} 
                loading={loading}
                error={error}
                fetchMore={fetchMore}
                navigation={navigation}
                updateQuery={updateQuery} />}
        </>
    );
}

export default Home;