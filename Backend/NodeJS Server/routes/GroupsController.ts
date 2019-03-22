import {BaseController} from "../core/BaseController";
import {StatusCodes} from "../core/StatusCodes";
import {ObjectId} from "bson";
import {GroupResponse} from "../core/Responses/GroupResponse";

export class GroupsController extends BaseController{
    public registerRoutes() {
        super.registerRoutes();

        this.router.get('/:group', async (req, res) => {

            if(!ObjectId.isValid(req.params.group)){
                res.status(StatusCodes.BadRequest).send();
                return;
            }

            const userId : ObjectId = await this.authenticator.check(req.get('Authorization'));
            const groupId : ObjectId = new ObjectId(req.params.group);

            if(userId == null) {
                res.status(StatusCodes.Forbidden).send();
                return;
            }

            await this.getDb().collection("groups").findOne(
                { _id: groupId, members: userId },
                {fields:{_id:false}},
                async (error, result) => {
                    if(error) {
                        res.status(StatusCodes.InternalError).send(error);
                        return;
                    }

                    if(result){
                        let arr = await this.getDb()
                            .collection("users")
                            .find({ _id: { $in: result.members } })
                            .toArray();

                        let list = arr.map(function (document) {
                                return [document._id.toHexString(), document.displayname];
                            });

                        let creator: string = "";
                        list.forEach(([x,y]) => {
                            if(creator === "" && x === result.creator.toHexString())
                                creator = y;
                        });

                        let members = list.map(([,y]) => y);

                        res.status(StatusCodes.OK).send(new GroupResponse(result.name, creator, members));
                    }
                    else{
                        res.status(StatusCodes.InternalError).send(result);
                    }
                }
            );
        });

        this.router.post('/newgroup/:name', async (req, res) => {
            const userId : ObjectId = await this.authenticator.check(req.get('Authorization'));
            const name : string = req.params.name;

            if(userId == null) {
                res.status(StatusCodes.Forbidden).send();
                return;
            }

            await this.getDb().collection("groups").updateOne(
                {name:name,creator:userId},
                { $setOnInsert: { members:[userId]} },
                { upsert: true },
                async (error, result) => {
                    if(error) {
                        res.status(StatusCodes.InternalError).send(error);
                        return;
                    }

                    if(result && result.matchedCount  === 0){
                        res.status(StatusCodes.OKCreated).send(result);
                    }
                    else{
                        res.status(StatusCodes.InternalError).send(result);
                        return;
                    }
                }
            );
        });

    }
}
