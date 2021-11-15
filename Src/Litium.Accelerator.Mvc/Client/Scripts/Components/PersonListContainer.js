import React, { Fragment, useEffect, useState, useCallback } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import PersonList from './PersonList';
import PersonForm from './PersonForm';
import { query, changeMode } from '../Actions/Person.action';
import { translate } from '../Services/translation';
import constants from '../constants';

const PersonListContainer = () => {
    const mode = useSelector((state) => state.myPage.persons.mode);
    const dispatch = useDispatch();

    useEffect(() => {
        dispatch(query());
    }, [dispatch]);

    const [person, setPerson] = useState({});

    const showForm = useCallback(
        (person) => {
            setPerson(person);
            dispatch(changeMode('edit'));
        },
        [setPerson, dispatch]
    );

    const showList = useCallback(() => {
        setPerson({});
        dispatch(changeMode('list'));
    }, [setPerson, dispatch]);

    return (
        <Fragment>
            {mode !== 'list' && (
                <PersonForm person={person} onDismiss={showList} />
            )}
            {mode === 'list' && (
                <Fragment>
                    <h2>{translate('mypage.person.title')}</h2>
                    <p>
                        <b>{translate('mypage.person.subtitle')}</b>
                    </p>
                    <button
                        className="form__button"
                        onClick={() =>
                            showForm({
                                role: constants.role.approver,
                                editable: true,
                            })
                        }
                    >
                        {translate('mypage.person.add')}
                    </button>
                    <PersonList onEdit={showForm} />
                </Fragment>
            )}
        </Fragment>
    );
};

export default PersonListContainer;
