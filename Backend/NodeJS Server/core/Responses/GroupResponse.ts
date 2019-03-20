import {ObjectId} from "bson";

export class GroupResponse{

    public name: string;
    public creator: string;
    public members: Array<string>;
    public memberCount: number;
    public balance: number;

    constructor(name: string, creator: string, members: Array<string>){
        this.name = name;
        this.creator = creator;
        this.members = members;
        this.memberCount = members.length;
        this.balance = 0;//TODO: Next Sprint
    }
}