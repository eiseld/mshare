import {BaseController} from "../core/BaseController";
import {LoginResponse, LoginState} from "../core/Responses/LoginResponse";
import {ObjectId} from "bson";
import {StatusCodes} from "../core/StatusCodes";
const hasher = require('../core/Hasher');
const jwt = require('jsonwebtoken');

export class UsersController extends BaseController {
    public registerRoutes() {
        super.registerRoutes();

        this.router.route('/login/:email/:password')
            .post(async (req, res) => {
                const email: string = req.params.email;
                const hashedPassword: string = hasher(req.params.password);

                await this.getDb().collection('users').findOne(
                    { $and: [
                            {email: email},
                            {password: hashedPassword}
                        ] },async (err, user) => {
                    // In case the user not found
                    if(err) {
                        res.status(StatusCodes.InternalError).send(err);
                        return;
                    }
                    if (user){
                        // TODO: move secret to config file
                        const token: string = jwt.sign({userId: user._id}, 'shhhhhhhhhhhhhh');
                        res.status(StatusCodes.OK).send(new LoginResponse(
                            LoginState.OK,
                            token)
                        );
                    } else {
                        res.status(StatusCodes.Unauthorized).send(new LoginResponse(
                            LoginState.BadAuth,
                            '')
                        );
                    }
                });
            });

        this.router.route('/validateemail/:token')
            .post(async (req, res) =>{
                const token: string = req.params.token;

                await this.getDb().collection('users').updateOne(
                    {$and: [
                            {token: token},
                            {state: 'Unapproved'}
                        ]},
                    {
                            $unset: { token: '' },
                            $set: {state: 'Approved'}
                        },
                    async (err, user) =>{
                        // In case the user not found
                        if(err) {
                            res.status(StatusCodes.InternalError).send(err);
                            return;
                        }
                        if (user && user.modifiedCount === 1){
                            res.status(StatusCodes.OK).send();
                        } else {
                            res.status(StatusCodes.Forbidden).send();
                        }
                    }
                );
            });

        this.router.get('/listgroups', async (req, res) => {
            const userId: ObjectId = await this.authenticator.check(req.get('Authorization'));

            if(userId == null) {
                res.status(StatusCodes.Forbidden).send("");
                return;
            }

            await this.getDb().collection("users").findOne(
                {_id:userId},
                {fields:{_id:false,groups:true}},
                async (error, result) => {
                    if(error) {
                        res.status(StatusCodes.InternalError).send(error);
                        return;
                    }

                    if(result){
                        if(result.groups != null)
                            res.status(StatusCodes.OK).send(result.groups);
                        else
                            res.status(StatusCodes.OK).send(new Array<ObjectId>());
                    }
                    else{
                        res.status(StatusCodes.InternalError).send(result);
                    }
                }
            );
        });

        this.router.post('/updategroups', async (req, res) => {
            const userId = await this.authenticator.check(req.get('Authorization'));

            if(userId == null) {
                res.status(StatusCodes.Forbidden).send("");
                return;
            }

            let documents = await this.getDb().collection("groups").find(
                {creator:userId},
                {fields:{_id:true}}
                ).toArray();

            let groupIds: Array<ObjectId> = documents.map(function (document):ObjectId {
                return document._id;
            });

            await this.getDb().collection("users").updateOne(
                {_id:userId},
                {$set: {groups:groupIds}},
                async (error, result) => {
                    if(error) {
                        res.status(StatusCodes.InternalError).send(error);
                        return;
                    }

                    if(result){
                        res.status(StatusCodes.OK).send(result);
                    }
                    else{
                        res.status(StatusCodes.InternalError).send(result);
                    }
                }
            );
        });

    }
}
