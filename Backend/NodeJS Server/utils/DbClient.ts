import { MongoClient, Db } from "mongodb";

class DbClient {
    public db: Db;

    public async connect() {
        console.log("Connecting to database...");
        const client: MongoClient = await MongoClient.connect("mongodb://database:27017/mshare",  { useNewUrlParser: true });
        this.db = client.db();
        console.log("Connected to database");
        return this.db;
    }
}

export = new DbClient();