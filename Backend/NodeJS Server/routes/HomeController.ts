import {BaseController} from "../core/BaseController";

export class HomeController extends BaseController{
    public getName(): string {
        return '';
    }

    public registerRoutes() {
        super.registerRoutes();

        console.log("site_url_for_user: "+this.config.site_url_for_user);
        this.router.get('', async (req, res) => {
            // TODO: Make homepage?
            res.send('Hello World!!');
        });
    }
}
