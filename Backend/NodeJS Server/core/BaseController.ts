import express = require('express');
import {App} from "../server";
import {Router} from "express-serve-static-core";
import {IAuthenticator} from "./IAuthenticator";


export class BaseController{
    private readonly _authenticator : IAuthenticator;
    get authenticator(): IAuthenticator {
        return this._authenticator;
    }

    private _router = express.Router();
    get router(): Router {
        return this._router;
    }

    private _application : App;

    constructor(app : App, auth : IAuthenticator) {
        this._application = app;
        this._authenticator = auth;
    }

    public registerRoutes() {

    }

    public getName() : string {
        return this.constructor["name"].toLowerCase().slice(0, -10);
    }

    public getDb(){
        return this._application.database;
    }
    public sendUnauthWarn(res){
        res.send('You do not have permission to access this functionality!');
    }
}
