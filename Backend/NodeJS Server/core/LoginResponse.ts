export enum LoginState{
    OK,
    BadAuth,
}
export class LoginResponse{
    private readonly _state : LoginState;
    get state():LoginState{
        return this._state;
    }

    readonly _token : string;
    get token():string{
        return this._token;
    }

    constructor(state : LoginState, token: string){
        this._state = state;
        this._token = token;
    }
}
