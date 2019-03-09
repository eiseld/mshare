export enum LoginState{
    OK,
    BadAuth,
}

export class LoginResponse{
    public state : LoginState;
    public token : string;

    constructor(state : LoginState, token: string){
        this.state = state;
        this.token = token;
    }
}