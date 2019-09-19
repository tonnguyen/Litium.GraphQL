import React, { useState} from 'react';
import { View, FlatList, Text } from 'react-native';
import {
    ListItem,
    Right,
    Left,
    Icon,
    Container,
    Spinner,
} from 'native-base';
import { gql } from 'apollo-boost';
import { useQuery } from '@apollo/react-hooks';
import { AssortmentSystemId } from '../server';

const SideBar = props => {
    const query = gql`
    query LitiumQuery($assortmentSystemId: String = null, $categorySystemId: String = "00000000-0000-0000-0000-000000000000") {
        categories(assortmentSystemId: $assortmentSystemId, systemId: $categorySystemId, take: 100) {
          list {
            name
            systemId
            categories {
              total
            }
          }
        }
      }
    `;
    const feedRootOptions = {
        variables: {
            "assortmentSystemId": AssortmentSystemId
        }
    };
    const [queryOptionsHistory, setQueryOptionsHistory] = useState([feedRootOptions]);
    const feedByCategoryOptions = (categorySystemId) => ({
        variables: {
            categorySystemId,
            assortmentSystemId: undefined,
        }
    });
    const { loading, error, data, refetch } = useQuery(
        query,
        feedRootOptions);
    const onListPress = (systemId, hasChildren) => {
        if (hasChildren) {
            const queryOptions = feedByCategoryOptions(systemId);
            refetch(queryOptions.variables);
            setQueryOptionsHistory([
                ...queryOptionsHistory,
                queryOptions,
            ]);
        } else {
            props.onNavigate('Category', { systemId });
            props.closeDrawer();
        }
    }
    const back = () => {
        refetch(queryOptionsHistory[queryOptionsHistory.length - 2].variables);
        setQueryOptionsHistory(queryOptionsHistory.slice(0, queryOptionsHistory.length -1));
    }
    return (
        <Container style={{ backgroundColor: 'white' }}>
            <ListItem>
                <Left>
                    {queryOptionsHistory && queryOptionsHistory.length > 1 
                    && <Icon name="arrow-back" 
                                onPress={() => back()} />}
                </Left>
                <Right><Icon name="close" onPress={() => props.closeDrawer()} /></Right>
            </ListItem>
            {error && <View><Text>Error: {error.message}</Text></View>}
            {loading && <Spinner />}
            {!loading && 
                <FlatList data={data.categories.list}
                    renderItem={({ item }) => 
                        <ListItem button onPress={() => onListPress(item.systemId, item.categories.total > 0)}>
                            <Left>
                                <Text>{item.name}</Text>
                            </Left>
                            {item.categories.total > 0 && 
                            <Right>
                                <Icon name="arrow-forward" />
                            </Right>}
                        </ListItem>
                    }
                >
                </FlatList>
        }
        </Container>
    );
}

export default SideBar;