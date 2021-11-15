import React, { Fragment, useEffect, useState, useCallback } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import AddressList from './AddressList';
import AddressForm from './AddressForm';
import { query, changeMode } from '../Actions/Address.action';
import { translate } from '../Services/translation';
import constants from '../constants';

const AddressListContainer = () => {
    const mode = useSelector((state) => state.myPage.addresses.mode);
    const dispatch = useDispatch();

    useEffect(() => {
        dispatch(query());
    }, [dispatch]);

    const [address, setAddress] = useState({});

    const showForm = useCallback(
        (address) => {
            setAddress(address);
            dispatch(changeMode('edit'));
        },
        [setAddress, dispatch]
    );

    const showList = useCallback(() => {
        setAddress({});
        dispatch(changeMode('list'));
    }, [setAddress, dispatch]);

    return (
        <Fragment>
            {mode !== 'list' && (
                <AddressForm address={address} onDismiss={showList} />
            )}
            {mode === 'list' && (
                <Fragment>
                    <h2>{translate('mypage.address.title')}</h2>
                    <p>
                        <b>{translate('mypage.address.subtitle')}</b>
                    </p>
                    <button
                        className="form__button"
                        onClick={() =>
                            showForm({ country: constants.countries[0].value })
                        }
                    >
                        {translate('mypage.address.add')}
                    </button>
                    <AddressList onEdit={showForm} />
                </Fragment>
            )}
        </Fragment>
    );
};

export default AddressListContainer;
