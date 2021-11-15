import Footer from "./Footer";
import Header from "./Header";

function _Header({ data }) {
  return (
      <Header logoUrl={data?.website.logoUrl} contentLinks={data?.website.header.sectionList || []}></Header>
  )
}

function _Footer({ data }) {
  return (
      <Footer sectionList={data?.website.footer.sectionList || []}></Footer>
  )
}

export default function Layout({ data, children }) {
  return (
    <>
      <_Header data={data} />
      {children}
      <_Footer data={data} />
    </>
  );
}