import {BaseController} from "../core/BaseController";

export class GroupsController extends BaseController{
    public registerRoutes() {
        super.registerRoutes();

        this.router.get('/list', async (req, res) => {
            // TODO: get token from session
            const userId = this.authenticator.check('TODOTOKEN');
            if(userId == null){
                this.sendUnauthWarn(res);
                return;
            }

            let docs = await this.getDb().collection("groups").find().toArray();

            console.log(docs);
            res.send(docs);
        });
    }
}
