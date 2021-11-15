import dynamic from 'next/dynamic'

export default function PageByTemplate({ pageType, valueAsJSON }) {
    const Page = dynamic(() => import(`./pages/${pageType}`));
    return <section data-litium-page-type={pageType}>
                <Page data={JSON.parse(valueAsJSON)} ></Page>
            </section>
}