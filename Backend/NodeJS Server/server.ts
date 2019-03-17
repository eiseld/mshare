import express = require('express');
import DbClient = require("./utils/DbClient");
import {Application} from "express-serve-static-core";
import {Db} from "mongodb";
import {Autoloader} from "autoloader-ts";
import {BaseController} from "./core/BaseController";
const cookieParser = require('cookie-parser');
import {TokenAuthenticator} from "./core/TokenAuthenticator";
import {Email} from "./utils/Email";

export class App {
  private _database : Db;
  private _email: Email;

  get database(): Db {
    return this._database;
  }

  get email(): Email {
    return this._email;
  }

  public exprApp : Application;
  public async start() {
    this._database = await DbClient.connect();
    this._email = new Email();
    console.log("Starting application...");

    this.exprApp = express();

    this.exprApp.use(express.json());
    this.exprApp.use(cookieParser());

    this.exprApp.listen(8081, function () {
      console.log('MShare is listening on port 8081!');
    });

    await this.loadControllers('routes');

  }

  private async loadControllers(folder: string){
    const tokenAuthenticator : TokenAuthenticator = new TokenAuthenticator();

    const loader = await Autoloader.dynamicImport();
    await loader.fromDirectories(`${__dirname}/${folder}`);
    for(const exported of loader.getResult().exports){
      try {
        const a: BaseController = new exported(this, tokenAuthenticator);
        a.registerRoutes();
        console.log("Binding " + a.getName() + " ...");
        this.exprApp.use('/' + a.getName(), a.router);
      }catch (e) {
        console.log(e);
      }
    }
  }

}

const app = new App();
app.start();
