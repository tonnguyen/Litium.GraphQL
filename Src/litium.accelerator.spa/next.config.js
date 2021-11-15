module.exports = {
    trailingSlash: true,
    env: {
        GRAPH_SERVER_URL: process.env.GRAPH_SERVER_URL,
        CDN_URL: process.env.CDN_URL,
    },
    images: {
        loader: 'custom'
    },
    // images: {
    //   domains: ['core.localtest.me'],
    // },
    // experimental: {
    //   concurrentFeatures: true,
    // },
    // reactStrictMode: true,
  }