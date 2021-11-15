# Litium.GraphQL
Demonstrate how to query data in Litium using GraphQL and to build a Single page application using NextJS

- [GrahpQL.NET](https://github.com/graphql-dotnet/examples)
- [Learn Next.js](https://nextjs.org/learn/basics/create-nextjs-app)

## Disclaimer
This is only to show the idea of how to integrate GraphQL to Litium. It should not be used in production.

## Running locally

### GraphQL server

```bash
# cd into Src/Litium.GraphQL
dotnet restore
dotnet build
# Run on http://localhost:5003
# Need to update GRAPH_SERVER_URL in litium.accelerator.spa/.env
# if running on different url
dotnet run
```

Then open a browser, navigate to http://localhost:5003/ui/playground to access the [GraphQL playground](https://github.com/graphql/graphql-playground) interface.

### Litium site to deliver images

```bash
# cd into Src/Litium.Accelerator.Mvc
dotnet restore
dotnet build
# Run on http://localhost:5000
# Need to update CDN_URL in litium.accelerator.spa/.env
# if running on different url
dotnet run
```

Make sure Accelerator content was deployed

### Single page application
Set the CDN_URL in litium.accelerator.spa/.env to point to the Litium site where it can deliver files. For example Litium.Accelerator.Mvc site.
```bash
# cd in to Src/litium.accelerator.spa
yarn install
# Run on http://localhost:3000. 
# If running on different url, Bootstrap.cs should be updated 
#to add the address to "DefaultGraphQLCorsPolicy"
yarn run dev
```