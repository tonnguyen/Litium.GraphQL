import dynamic from 'next/dynamic'

export default function BlockArea({ blocks }) {
    return (
        blocks && blocks.length > 0 && 
        <>
            {blocks.map(({ blockType, valueAsJSON, systemId }) => {
                const Block = dynamic(() => import(`./blocks/${blockType}`));
                return <section data-litium-block-type={blockType} key={systemId}
                                data-litium-block-id={systemId}>
                            <Block systemId={systemId} data={JSON.parse(valueAsJSON)} ></Block>
                        </section>
            })}
        </>
    );
}