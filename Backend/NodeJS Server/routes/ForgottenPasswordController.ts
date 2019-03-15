import {StatusCodes} from "../core/StatusCodes";
import {BaseController} from "../core/BaseController";

const nodemailer = require('nodemailer');

export class ForgottenPasswordController extends BaseController{
    public registerRoutes() {
        super.registerRoutes();

        this.router.route('/sendmail')
            .post(async (req, res) => {
                const registration: string = req.get('registration');
                const forgottenpw: string = req.get('forgottenpw');
                if(registration == null && forgottenpw == null){
                    res.status(StatusCodes.InternalError).send();
                    return;
                }
                if (registration){
                    async (error, result) => {
                        if(error) {
                            res.status(StatusCodes.InternalError).send(error);
                            return;
                        }

                        if(result){
                            this.sendMail('registration');
                            res.status(StatusCodes.OK).send(result);
                        }
                    }
                } else if(forgottenpw){
                    async (error, result) => {
                        if(error) {
                            res.status(StatusCodes.InternalError).send(error);
                            return;
                        }

                        if(result){
                            this.sendMail('forgottenpw');
                            res.status(StatusCodes.OK).send(result);
                        }
                    }
                }
            });
    }
    transporter = nodemailer.createTransport({
        service: 'gmail',
        auth: {
            user: 'youremail@gmail.com',
            pass: 'yourpassword'
        }
    });

    mailOptions = {
        from: 'youremail@gmail.com',
        to: 'myfriend@yahoo.com',
        subject: 'Sending Email using Node.js',
        text: 'That was easy!'
    };

    public sendMail(param) {
        this.transporter.sendMail(this.mailOptions, function (error, info) {
            if (error) {
                console.log(error);
            } else {
                console.log('Email sent: ' + info.response);
            }
        });
    }
}