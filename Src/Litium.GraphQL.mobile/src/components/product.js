import React from 'react';
import { gql } from 'apollo-boost';
import { useQuery } from '@apollo/react-hooks';
import { Spinner } from 'native-base';
import { ScrollView } from 'react-native';
import Variant from './variant';

const Product = ({navigation}) => {
    const systemId = navigation.getParam('systemId');
    const query = gql`
    query LitiumQuery($systemId: String!) {
        product(systemId: $systemId) {
            variants {
              name,
              id
              images,
              price {
                  formattedPrice,
              }
          }
        }
      }      
    `;
    const { loading, error, data } = useQuery(
        query,
        { variables: { systemId } });

    return (
        <ScrollView>
            {loading && <Spinner />}
            {error && <Text>{error}</Text>}
            {!loading && data && data.product.variants.map(variant => <Variant key={variant.id} variant={variant} />)}
        </ScrollView>
    );
}

export default Product;