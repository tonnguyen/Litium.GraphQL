export const LIGHTBOX_IMAGES_SET_CURRENT_IMAGE =
    'LIGHTBOX_IMAGES_SET_CURRENT_IMAGE';
export const LIGHTBOX_IMAGES_SHOW = 'LIGHTBOX_IMAGES_SHOW';

export const setCurrentIndex = (index) => ({
    type: LIGHTBOX_IMAGES_SET_CURRENT_IMAGE,
    payload: {
        index,
    },
});

export const show = (visible) => ({
    type: LIGHTBOX_IMAGES_SHOW,
    payload: {
        visible,
    },
});
