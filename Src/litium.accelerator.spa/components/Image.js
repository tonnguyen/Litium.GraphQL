import NextImage from 'next/image'

export default function Image(props) {
    const altProps = {
        ...props,
        priority: undefined,
    }
    return <NextImage {
        ...altProps
        } 
        unoptimized={true}
    />
}