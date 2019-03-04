import express = require('express');
import DbClient = require("./utils/DbClient");
import {Application} from "express-serve-static-core";

class App {

  private exprApp : Application;
  public async start() {
    const db = await DbClient.connect();
    console.log("Starting application...");

    this.exprApp = express();

    this.exprApp.use(express.json());

    this.exprApp.listen(8081, function () {
      console.log('MShare is listening on port 8081!');
    });

    this.exprApp.get('/', (req, res) => {
      res.send('Hello World - v - async');
    });

    this.exprApp.get('/users', async (req, res) => {
      let docs = await db.collection("users").find().toArray();

      console.log(docs);
      res.send(docs);
    });
  }
}

const app = new App();
app.start();