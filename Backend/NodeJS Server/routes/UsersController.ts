import {BaseController} from "../core/BaseController";
import {LoginResponse, LoginState} from "../core/LoginResponse";

export class UsersController extends BaseController {
    public registerRoutes() {
        super.registerRoutes();

        this.router.route('/login/:email/:password')
            .get(function(req, res){
                const email: string = req.params.email;
                const password: string = req.params.email;
                // TODO: hash passwork with SHA-1

                // TODO: check email, password validity with database

                // TODO: respond with JWT token
                res.send(new LoginResponse(
                    LoginState.OK,
                    'jaudiem1l3928421najxkfpdnwnejrud82k3xa94')
                );
            });

        // TODO: remove from production, only use this for testing!
        this.router.get('/list', async (req, res) => {
            let docs = await this.getDb().collection("users").find().toArray();

            console.log(docs);
            res.send(docs);
        });
    }
}
