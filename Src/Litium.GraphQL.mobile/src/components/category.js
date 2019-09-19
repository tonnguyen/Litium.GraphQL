import React from 'react';
import { gql } from 'apollo-boost';
import { useQuery } from '@apollo/react-hooks';
import ProductList from './product-list';
import { Spinner } from 'native-base';

const Category = ({navigation}) => {
    const systemId = navigation.getParam('systemId');
    const query = gql`
    query LitiumQuery($systemId: String!, $skip: Int = 0, $take: Int = 3) {
        category(systemId: $systemId) {
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
      }      
    `;
    const { loading, error, data, fetchMore } = useQuery(
        query,
        { variables: { systemId } });
    const updateQuery = (prev, { fetchMoreResult }) => {
        if (!fetchMoreResult) return prev;
        return Object.assign({}, prev, {
            category: {
                ...prev.category,
                products: {
                    hasNextPage: fetchMoreResult.category.products.hasNextPage,
                    list: [...prev.category.products.list, ...fetchMoreResult.category.products.list],
                    __typename: prev.category.products.__typename,
                }
            },
        });
    }

    return (
        <>
            {loading && <Spinner />}
            {!loading && <ProductList data={data.category.products} 
                loading={loading}
                error={error}
                fetchMore={fetchMore}
                navigation={navigation}
                updateQuery={updateQuery} />}
        </>
    );
}

export default Category;