import { Epic } from 'redux-observable';
import { isActionOf } from 'typesafe-actions';
import RootAction from '../actions';
import RootState from '../reducers';
import * as actions from '../actions';
import { of } from 'rxjs';
import { filter, catchError, switchMap, mapTo, map } from 'rxjs/operators';
import { ajax } from 'rxjs/ajax';
import * as endpoints from '../../routing/endpoints';
import { Token } from 'src/models/Token';

export const phoneEpic: Epic<RootAction, RootAction, RootState> = (action$) =>
    action$.pipe(
        filter(isActionOf(actions.checkPhone)),
        switchMap(action =>
            ajax({
                url: endpoints.apiEndpoints.checkPhone,
                method: 'POST',
                body: {
                    phoneNumber: action.payload
                }
            }).pipe(
                mapTo(actions.checkPhoneSuccess()),
                catchError(err => of(actions.checkPhoneFailure(err)))
            )
        ),
    );

export const codeEpic: Epic<RootAction, RootAction, RootState> = (action$) =>
    action$.pipe(
        filter(isActionOf(actions.checkCode)),
        switchMap(action =>
            ajax<Token>({
                url: endpoints.apiEndpoints.checkCode,
                method: 'POST',
                body: {
                    code: action.payload
                }
            }).pipe(
                map(data => {
                    var token = data.response;
                    localStorage.setItem('x-access-token', token.accessToken);
                    return actions.checkCodeSuccess(token);
                }),
                catchError(err => of(actions.checkPhoneFailure(err)))
            )
        ),
    );
