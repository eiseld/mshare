import {StatusCodes} from "../core/StatusCodes";
import {BaseController} from "../core/BaseController";
import {Email} from "../utils/Email";
import {ForgottenPasswordResponse} from "../core/Responses/ForgottenPasswordResponse";

const nodemailer = require('nodemailer');

export class ForgottenPasswordController extends BaseController{

    emailSender : Email;

    public registerRoutes() {
        super.registerRoutes();

        this.router.route('/forgotpass')
            .post(async (req, res) => {
                const email: string = req.get('email');
                if(email == null){
                    res.status(StatusCodes.InternalError).send();
                    return;
                }
                if (email){
                    async (error, result) => {
                        this.emailSender.sendMailText(email,"Forgotten Password", getEmailContent());
                        if(error) {
                            res.status(StatusCodes.InternalError).send(new ForgottenPasswordResponse('0'));
                            return;
                        }

                        if(result){
                            res.status(StatusCodes.OK).send(new ForgottenPasswordResponse('1'));
                        }
                    }
                }
            });

        // this.router.route('/resetpass')
        //     .post(async (req, res) => {
        //         const email: string = req.get('email');
        //         const oldPass: string = req.get('oldpass');
        //         const newPass: string = req.get('newpass');
        //         const token: string = req.get('token');
        //         if(email == null || oldPass == null || newPass == null || token == null){
        //             res.status(StatusCodes.InternalError).send();
        //             return;
        //         }
        //
        //         //check token valid
        //         //vagyis van egy modellunk email + token + valid-e + datum
        //         await this.getDb().collection('resetedmails').findOne(
        //             { $and: [
        //                     {email: email},
        //                     {token: token},
        //                     {valid: 1},
        //                     {created: '2019-01-01'}//datumot
        //                 ] },async (err, resetedmail) => {
        //                 // In case the user not found
        //                 if(err) {
        //                     res.status(StatusCodes.InternalError).send(err);
        //                     return;
        //                 }
        //                 if (resetedmail){
        //                     //itt kell a modositast a resetedben
        //
        //                     res.status(StatusCodes.OK).send()
        //                     );
        //                 } else {
        //                     //Itt kell az updatet letolni
        //                     res.status(StatusCodes.BadRequest).send()
        //                     );
        //                 }
        //             });
        //         //menteni a user passwordot
        //
        //         //ha minden ok akkor return ok
        //
        //
        //         if (email){
        //             async (error, result) => {
        //                 this.emailSender.sendMailText(email,"Forgotten Password", "Test");
        //                 if(error) {
        //                     res.status(StatusCodes.InternalError).send(error);
        //                     return;
        //                 }
        //
        //                 if(result){
        //                     res.status(StatusCodes.OK).send(result);
        //                 }
        //             }
        //         }
        //     });
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
        return 'Az alábbi linkre kattintva módosíthatja jelszavát: ' + 'htp://localhost:4200/resetpass?token='+this.makeString();
    }

    // transporter = nodemailer.createTransport({
    //     service: 'gmail',
    //     auth: {
    //         user: 'youremail@gmail.com',
    //         pass: 'yourpassword'
    //     }
    // });
    //
    // mailOptions = {
    //     from: 'youremail@gmail.com',
    //     to: 'myfriend@yahoo.com',
    //     subject: 'Sending Email using Node.js',
    //     text: 'That was easy!'
    // };
    //
    // public sendMail(param) {
    //     this.transporter.sendMail(this.mailOptions, function (error, info) {
    //         if (error) {
    //             console.log(error);
    //         } else {
    //             console.log('Email sent: ' + info.response);
    //         }
    //     });
    // }
}