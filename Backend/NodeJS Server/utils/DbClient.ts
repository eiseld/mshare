import { MongoClient, Db } from "mongodb";

class DbClient {
    private _db: Db;
    get db(): Db {
        return this._db;
    }


    public async connect() {
        console.log("Connecting to database...");
        const client: MongoClient = await MongoClient.connect("mongodb://database:27017/mshare",  { useNewUrlParser: true });
        this._db = client.db();
        console.log("Connected to database");
        return this.db;
    }
}

export = new DbClient();
