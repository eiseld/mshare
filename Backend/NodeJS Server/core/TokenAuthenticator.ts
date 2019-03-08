import {IAuthenticator} from "./IAuthenticator";
import {ObjectId} from "bson";

export class TokenAuthenticator implements IAuthenticator{
    check(token: string): ObjectId {
        // TODO: use JWT to get which user the token belongs to
        return new ObjectId('5c8116c94d2c620f900a82d9');
    }
}
