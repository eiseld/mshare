import {BaseController} from "../core/BaseController";

export class HomeController extends BaseController{
    public getName(): string {
        return '';
    }

    public registerRoutes() {
        super.registerRoutes();

        this.router.get('', async (req, res) => {
            // TODO: Make homepage?
            res.send('Hello World!!');
        });
    }
}
