import {ObjectId} from "bson";

export interface IAuthenticator{
    check(token: string): ObjectId;
}
