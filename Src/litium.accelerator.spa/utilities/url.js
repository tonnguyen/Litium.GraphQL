export const relativePath = (url) => {
    try {
        return new URL(url).pathname;
    } catch {
        return url;
    }
}