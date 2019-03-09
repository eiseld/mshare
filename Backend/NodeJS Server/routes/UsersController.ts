import {BaseController} from "../core/BaseController";
import {LoginResponse, LoginState} from "../core/Responses/LoginResponse";
import {ObjectId} from "bson";
import {StatusCodes} from "../core/StatusCodes";
const hasher = require('../core/Hasher');

export class UsersController extends BaseController {
    public registerRoutes() {
        super.registerRoutes();

        this.router.route('/login/:email/:password')
            .post(async (req, res) => {
                const email: string = req.params.email;
                const hashedPassword: string = hasher(req.params.password);

                this.getDb().collection('users').findOne(
                    { $and: [
                            {email: email},
                            {password: hashedPassword}
                        ] },function(err, user) {
                    // In case the user not found
                    if(err) {
                        res.status(StatusCodes.InternalError).send(err);
                        return;
                    }
                    if (user){
                        // TODO: respond with JWT token
                        res.status(StatusCodes.OK).send(new LoginResponse(
                            LoginState.OK,
                            'jaudiem1l3928421najxkfpdnwnejrud82k3xa94')
                        );
                    } else {
                        res.status(StatusCodes.Unauthorized).send(new LoginResponse(
                            LoginState.BadAuth,
                            '')
                        );
                    }
                });
            });

        this.router.get('/listgroups', async (req, res) => {
                // TODO: get token from session
                const userId = this.authenticator.check('TODOTOKEN');

                if(userId == null) {
                    res.status(StatusCodes.Forbidden).send("");
                    return;
                }

                this.getDb().collection("users").findOne(
                    {_id:userId},
                    {fields:{_id:false,groups:true}},
                    function (error, result) {
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

        this.router.put('/addgroup', async (req, res) => {
            const userId: ObjectId = new ObjectId(req.query.userid);
            const groupId: ObjectId = new ObjectId(req.query.groupid);

            console.log(userId);
            console.log(groupId);

            let docs = await this.getDb().collection("users").updateOne(
                {_id:userId},
                {$addToSet:{groups:groupId}}

            );

            console.log(docs);
        });

    }
}
