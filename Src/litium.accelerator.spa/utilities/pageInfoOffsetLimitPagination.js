import { offsetLimitPagination } from "@apollo/client/utilities";

export default function pageInfoOffsetLimitPagination(keyArgs = false) {
  const fn = offsetLimitPagination(keyArgs);
  return {
    keyArgs,
    merge(existing, incoming, args) {
      existing = existing || { list: [] };
      args.offset--;
      const mergedList = fn.merge(
        existing.list,
        incoming?.list,
        args
      );
      return {
        ...existing,
        hasNextPage: incoming.hasNextPage,
        list: mergedList,
      };
    },
  };
}
