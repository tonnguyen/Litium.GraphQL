import React from 'react';
import { Text, Image, View } from 'react-native';
import { Host } from '../server';

const Variant = ({variant}) => {
    return (
    <View key={variant.id}>
        {variant.images && variant.images.length > 0 &&
            <Image source={{ uri: Host + variant.images[0] }}
                style={{height: 100, width: undefined, flex: 1, resizeMode: 'contain'}} />}
        <Text>{ variant.name }</Text>
    </View>
    );
}

export default Variant;