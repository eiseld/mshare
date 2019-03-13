import express = require('express');
import DbClient = require("./utils/DbClient");
import {Application} from "express-serve-static-core";
import {Db} from "mongodb";
import {Autoloader} from "autoloader-ts";
import {BaseController} from "./core/BaseController";
const cookieParser = require('cookie-parser');
import {TokenAuthenticator} from "./core/TokenAuthenticator";

export class App {
  private _database : Db;

  get database(): Db {
    return this._database;
  }

  public exprApp : Application;
  public async start() {
    this._database = await DbClient.connect();

    console.log("Starting application...");

    this.exprApp = express();

    this.exprApp.use(express.json());
    this.exprApp.use(cookieParser());

    this.exprApp.listen(8081, function () {
      console.log('MShare is listening on port 8081!');
    });

    await this.loadControllers('routes');

    this.exprApp.get('/users', async (req, res) => {
      let docs = await this._database.collection("users").find().toArray();

      console.log(docs);
      res.send(docs);
    });

    this.exprApp.post('/createUser', async (req, res) => {

      let docs = await this._database.collection('users').insertOne(req.body, function(err, result){
        if(err) {
          res.send("Error!" + err);
        } else {
          res.send('Success!');
        }
        console.log(docs);
      })
    })

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
