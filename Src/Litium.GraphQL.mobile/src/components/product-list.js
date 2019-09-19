import React from 'react';
import { View, FlatList, Text, Button, Image, TouchableHighlight } from 'react-native';
import { Card, CardItem } from 'native-base';
import { Host } from '../server';

const ProductList = ({ data, navigation, loading, fetchMore, error, updateQuery }) => (
    <>
        {error && <View><Text>Error: {error.message}</Text></View>}
        
        {!loading && data && <FlatList data={data.list}
            renderItem={({ item: product }) =>
            <TouchableHighlight 
                onPress={() => navigation.navigate('Product', { systemId: product.systemId })}>
                <Card>
                    {product.images && product.images.length > 0 && <CardItem cardBody>
                        <Image source={{ uri: Host + product.images[0] }}
                            style={{height: 200, width: undefined, flex: 1, resizeMode: 'contain'}} />
                    </CardItem>}
                    <CardItem>
                        <Text>{product.name}</Text>
                    </CardItem>
                    {product.variants 
                        && product.variants.length > 0 
                        && <CardItem><Text>{product.variants[0].price.formattedPrice}</Text></CardItem>}
                </Card>
            </TouchableHighlight>
        }
        >
        </FlatList>}
        {!loading && data.hasNextPage && 
            <Button title="Load more" onPress={() => fetchMore({
                variables: {
                    skip: data.list.length,
                },
                updateQuery,
            })}>Load more</Button>}
    </>
)

export default ProductList;