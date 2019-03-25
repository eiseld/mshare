import {StatusCodes} from "../core/StatusCodes";
import {BaseController} from "../core/BaseController";
import {Email} from "../utils/Email";
import {ForgottenPasswordResponse} from "../core/Responses/ForgottenPasswordResponse";

const nodemailer = require('nodemailer');

export class ForgottenPasswordController extends BaseController{

    public registerRoutes() {
        super.registerRoutes();

        this.router.route('/forgotpass')
            .post(async (req, res) => {
                console.log('forgot pass called:' +  req.body.email);
                const email: string =  req.body.email;;
                if(email == null){
                    res.status(StatusCodes.InternalError).send();
                    return;
                }
                if (email) {
                    console.log('sendMail: ' + this.getEmailContent());
                    console.log('inside result');
                    this.getEmail().sendMailHtml(req.body.email, "Forgotten Password", this.getEmailContent());
                    res.status(StatusCodes.OK).send(new ForgottenPasswordResponse('1'));
                    return;
                }
            });

        this.router.route('/resetpass')
            .post(async (req, res) => {
                const email: string = req.body.email;
                const password: string =  req.body.password;
                const token: string =  req.body.token;
                if(email == null || password == null || token == null){
                    res.status(StatusCodes.InternalError).send();
                    return;
                }
                const hashedPassword: string = hasher(password);

                await this.getDb().collection('users').updateOne(
                    {$and: [
                            {email: email, token: token}
                        ]},
                    {
                        $unset: { token: '' },
                        $set: {password: hashedPassword}
                    },
                    async (err, user) =>{
                        if(err) {
                            res.status(StatusCodes.InternalError).send(err);
                            return;
                        }
                        if (user){
                            res.status(StatusCodes.OK).send();
                        } else {
                            res.status(StatusCodes.Forbidden).send();
                        }
                    }
                );
        });
    }

    private makeString() : string {
        let outString: string = '';
        let inOptions: string = 'abcdefghijklmnopqrstuvwxyz0123456789';

        for (let i = 0; i < 20; i++) {

            outString += inOptions.charAt(Math.floor(Math.random() * inOptions.length));
        }
        return outString;
    }

    private getEmailContent() : string {
        console.log('email');
        return "Az alábbi linkre kattintva módosíthatja jelszavát:<a href='"
            + this.config.site_url_for_user + 'resetpass?token=' + this.makeString() + "'>Confirm</a>";
    }
}
