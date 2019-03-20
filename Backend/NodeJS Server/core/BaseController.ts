import express = require('express');
import {App} from "../server";
import {Router} from "express-serve-static-core";
import {IAuthenticator} from "./IAuthenticator";
import {MConfig} from "../utils/MConfig";


export class BaseController{
    private readonly _authenticator : IAuthenticator;
    get authenticator(): IAuthenticator {
        return this._authenticator;
    }

    private _router = express.Router();
    get router(): Router {
        return this._router;
    }

    private readonly _config : MConfig;
    get config(): MConfig{
        return this._config;
    }

    private _application : App;

    constructor(app : App, auth : IAuthenticator, config : MConfig) {
        this._application = app;
        this._authenticator = auth;
        this._config = config;
    }

    public registerRoutes() {

    }

    public getName() : string {
        return this.constructor["name"].toLowerCase().slice(0, -10);
    }

    public getEmail(){
        return this._application.email;
    }

    public getDb(){
        return this._application.database;
    }

    public sendUnauthWarn(res){
        res.send('You do not have permission to access this functionality!');
    }
}
