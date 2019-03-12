import {IAuthenticator} from "./IAuthenticator";
import {ObjectId} from "bson";
import {Db} from "mongodb";
const jwt = require('jsonwebtoken');

export class TokenAuthenticator implements IAuthenticator{
    async check(token: string): Promise<ObjectId> {
        return new Promise<ObjectId>((result, reject) => {
            // TODO: move secret to config file
            jwt.verify(token, 'shhhhhhhhhhhhhh', (error, decoded) => {
                if(error){
                    result(null);
                }else{
                    result(new ObjectId(decoded.userId));
                }
            });
        });

    }
}
