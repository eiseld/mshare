import {BaseController} from "../core/BaseController";
import {LoginResponse, LoginState} from "../core/LoginResponse";
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
                        res.status(500).send(err);
                        return;
                    }
                    if (user){
                        // TODO: respond with JWT token
                        res.status(200).send(new LoginResponse(
                            LoginState.OK,
                            'jaudiem1l3928421najxkfpdnwnejrud82k3xa94')
                        );
                    } else {
                        res.status(401).send(new LoginResponse(
                            LoginState.BadAuth,
                            '')
                        );
                    }
                });
            });

        // TODO: remove from production, only use this for testing!
        this.router.get('/list', async (req, res) => {
            let docs = await this.getDb().collection("users").find().toArray();

            console.log(docs);
            res.send(docs);
        });
    }
}
