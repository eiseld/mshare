import {ObjectId} from "bson";

export class GroupResponse{

    public name: string;
    public creator: ObjectId;
    public members: Array<ObjectId>;
    public memberCount: number;
    public balance: number;

    constructor(name: string, creator: ObjectId, members: Array<ObjectId>){
        this.name = name;
        this.creator = creator;
        this.members = members;
        this.memberCount = members.length;
        this.balance = 0;//TODO: Next Sprint
    }
}