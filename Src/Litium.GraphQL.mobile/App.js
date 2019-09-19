import React, { useRef } from 'react';
import ApolloClient from 'apollo-boost';
import { ApolloProvider } from '@apollo/react-hooks';
import { createStackNavigator } from 'react-navigation-stack';
import { createAppContainer, NavigationActions, SafeAreaView } from 'react-navigation';
import {
    Body,
    Container,
    Drawer,
    Header,
    Icon,
    Left,
    Right,
} from 'native-base';
import { Image, TouchableHighlight } from 'react-native';
import Home from './src/components/home';
import Category from './src/components/category';
import Product from './src/components/product';
import SideBar from './src/components/sidebar';
import { Server } from './src/server';

const AppNavigator = createStackNavigator({
    Home: { screen: Home },
    Category: { screen: Category },
    Product: { screen: Product },
}, { initialRouteName: 'Home', headerMode: 'none' });

const AppContainer = createAppContainer(AppNavigator);

const App = () => {
    const client = new ApolloClient({
        uri: Server,
    });
    const drawer = useRef(null);
    const navigator = useRef(null);
    const closeDrawer = () => drawer.current._root.close();
    const openDrawer = () => drawer.current._root.open();
    return (
        <ApolloProvider client={client}>
            <SafeAreaView forceInset={{top: 'always'}} style={{flex: 1}}>
                <Drawer 
                    ref={drawer}
                    onClose={() => closeDrawer()}
                    content={<SideBar 
                                onNavigate={(routeName, params) => 
                                    navigator.current.dispatch(NavigationActions.navigate({
                                        routeName,
                                        params
                                    }))} 
                                closeDrawer={() => closeDrawer()}
                        />}
                    >
                    <Container>
                        <Header style={{ backgroundColor: 'white' }}>
                            <Left>
                                <Icon name="menu" color="black" onPress={() => openDrawer()} />
                            </Left>
                            <Body>
                                <TouchableHighlight underlayColor="white"
                                    onPress={() => navigator.current.dispatch(NavigationActions.navigate({ routeName: 'Home', params: {} }))}>
                                    <Image 
                                        style={{width: 203, height: 50}}
                                        source={require('./assets/logo.png')}
                                        />
                                </TouchableHighlight>
                            </Body>
                            <Right/>
                        </Header>
                        <AppContainer ref={navigator} />
                    </Container> 
                </Drawer>
            </SafeAreaView>
        </ApolloProvider>
    );
}

export default App;