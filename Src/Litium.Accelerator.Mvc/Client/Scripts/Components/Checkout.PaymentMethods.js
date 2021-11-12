import React from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { translate } from '../Services/translation';
import {
    setCampaignCode,
    submitCampaignCode,
    setPayment,
} from '../Actions/Checkout.action';

const CheckoutPaymentMethods = () => {
    const dispatch = useDispatch();
    const { payload, errors = {} } = useSelector((state) => state.checkout);
    const { paymentMethods, selectedPaymentMethod } = payload;

    return (
        <section className="row checkout-info__container">
            <div className="columns small-12">
                {paymentMethods &&
                    paymentMethods.map((method) => (
                        <label className="row no-margin" key={method.id}>
                            <input
                                type="radio"
                                name="paymentMethods"
                                className="checkout-info__checkbox-radio"
                                value={method.id}
                                checked={method.id === selectedPaymentMethod}
                                onChange={() => dispatch(setPayment(method.id))}
                            />
                            <span className="columns">
                                <b> {method.name} </b> - {method.formattedPrice}
                            </span>
                        </label>
                    ))}
                <br />
                <label className="form__label" htmlFor="campaign-code">
                    {translate('checkout.campaigncode')}
                </label>
                <div className="row no-margin">
                    <div className="small-6 medium-4">
                        <input
                            className="form__input"
                            id="campaign-code"
                            placeholder={translate('checkout.campaigncode')}
                            onChange={(event) =>
                                dispatch(setCampaignCode(event.target.value))
                            }
                        />
                        {errors && errors['campaignCode'] && (
                            <span
                                className="form__validator--error form__validator--top-narrow"
                                data-error-for="campaign-code"
                            >
                                {errors['campaignCode'][0]}
                            </span>
                        )}
                    </div>
                    <div className="small-5 small-offset-1 medium-4 medium-offset-1">
                        <button
                            className="checkout-info__campaign-button"
                            onClick={() => dispatch(submitCampaignCode())}
                        >
                            {translate('checkout.usecampaigncode')}
                        </button>
                    </div>
                </div>
            </div>
        </section>
    );
};

export default CheckoutPaymentMethods;
