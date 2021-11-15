import client from "../apollo-client";

const preloadedQuery = (query, options) => {
    let status = "pending";
    let result = null;
    const suspender = client
      .query({
          query,
          ...options,
      })
      .then((response) => {
        status = "success";
        result = response.data;
      })
      .catch((err) => {
        status = "error";
        result = err;
      });
  
    return {
      read() {
        if (status === "pending") {
          throw suspender;
        } else if (status === "error") {
          throw result;
        } else {
          return result;
        }
      }
    };
};

export const usePreloadedQuery = (query, options) => {
    const data = preloadedQuery(query, options);
    return { data };
};