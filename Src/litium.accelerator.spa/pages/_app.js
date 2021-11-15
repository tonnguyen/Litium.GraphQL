import { ApolloProvider } from "@apollo/client";
import client from "../apollo-client";
import '../styles/global.css'

const Noop = ({ children }) => <>{children}</>

const MyApp = ({ Component, pageProps }) => {
    const Layout = Component.Layout || Noop
    return (
      <ApolloProvider client={client}>
          <Layout data={pageProps.layoutData}>
            <Component {...pageProps} />
          </Layout>
      </ApolloProvider>
    );
}

export default MyApp;