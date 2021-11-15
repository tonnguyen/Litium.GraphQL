import React, { Fragment, useCallback, useState } from 'react';
import Modal from 'react-modal';
import 'react-responsive-carousel/lib/styles/carousel.min.css';
import { Carousel as Lightbox } from 'react-responsive-carousel';
import Image from './Image';

const customStyles = {
    content: {
        background: 'none',
        padding: 0,
        border: 'none',
    },
    overlay: {
        background: 'rgba(0, 0, 0, 0.8)',
        zIndex: 2003,
    },
};
const LightboxSettings = {
    showStatus: false,
    showIndicators: false,
    useKeyboardArrows: true,
    thumbWidth: 50,
    autoPlay: false,
    autoFocus: true,
};

const renderThumbnails = (children) => {
    const images = children.map((item, index) => {
        if (!item || !item.props || !item.props.children) {
            return undefined;
        }

        // find img to get source
        const imageElement = item.props.children.find(
            (ele) => ele.type === 'img'
        );
        if (!imageElement || !imageElement.props || !imageElement.props.src) {
            return undefined;
        }

        return (
            <div
                key={index}
                style={{
                    backgroundImage: 'url(' + imageElement.props.src + ')',
                }}
                className="thumbnail__image"
            />
        );
    });

    return images;
};

const LightboxImages = (props) => {
    const [visible, show] = useState(false);
    const [index, setCurrentIndex] = useState(0);
    const onClose = useCallback(() => show(false), [show]);
    const onClickThumbnail = (index) => {
        show(true);
        setCurrentIndex(index);
    };

    return !props.images || props.images.length < 1 ? (
        <Fragment />
    ) : (
        <Fragment>
            <div className="row product-images">
                <div className="small-12 large-9 columns large-order-1">
                    <figure className="product-detail__image-container">
                        <a
                            data-src={props.images[0]}
                            itemProp="url"
                            onClick={() => onClickThumbnail(0)}
                            className="product-image"
                        >
                            <img className="product-detail__image--main" src={`${process.env.CDN_URL}${props.thumbnails[0]}`} />
                        </a>
                    </figure>
                </div>
                <div className="small-12 large-3 columns medium-flex-dir-column">
                    <div className="row">
                        {props.images.map(
                            (image, index) =>
                                index > 0 && (
                                    <div
                                        className="product-detail__image-container columns large-12"
                                        key={index}
                                    >
                                        <a
                                            data-src={image}
                                            itemProp="url"
                                            onClick={() =>
                                                onClickThumbnail(index)
                                            }
                                            className="product-image"
                                        >
                                            <Image priority className="product-detail__image--alter"
                                                src={`${process.env.CDN_URL}${props.thumbnails[index]}`}
                                                height={120}
                                                width={100}
                                            />
                                        </a>
                                    </div>
                                )
                        )}
                    </div>
                </div>
            </div>
            <Modal
                ariaHideApp={false}
                preventScroll={true}
                isOpen={visible}
                onRequestClose={onClose}
                style={customStyles}
                shouldCloseOnOverlayClick={false}
            >
                <Lightbox
                    {...LightboxSettings}
                    selectedItem={index}
                    className="light-box"
                    renderThumbs={renderThumbnails}
                >
                    {props.images.map((value, index) => (
                        <div
                            key={`figure${index}`}
                            className="light-box__container"
                        >
                            <button
                                onClick={onClose}
                                className="light-box__close-btn"
                            >
                                &times;
                            </button>
                            <img src={`${process.env.CDN_URL}${value}`} className="light-box__image" />
                        </div>
                    ))}
                </Lightbox>
            </Modal>
        </Fragment>
    );
};

export default LightboxImages;
