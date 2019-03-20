const nodemailer = require("nodemailer");
export class Email {

    private transporter = nodemailer.createTransport({
        service: 'gmail',
        auth: {
            user: 'noreply.mshare@gmail.com',
            pass: 'ilovescrum'
        },
        tls:{
            rejectUnauthorized: false
        }
    });

    public async sendMailText(to: string, subject:string, text:string) {


        let mailOptions = {
            from: 'noreply.mshare@gmail.com',
            to: to,
            subject: subject,
            text: text
        };

        /*transporter.sendMail(mailOptions, function (error, info) {
            if (error) {
                console.log(error);
            } else {
                console.log('Email sent: ' + info.response);
            }
        });*/

        let info = await this.transporter.sendMail(mailOptions);

        console.log("Message sent: %s", info.messageId);
        // Preview only available when sending through an Ethereal account
        console.log("Preview URL: %s", nodemailer.getTestMessageUrl(info));
    }

    public async sendMailHtml(to: string, subject:string, html:string) {


        let mailOptions = {
            from: 'noreply.mshare@gmail.com',
            to: to,
            subject: subject,
            html: html
        };

        /*transporter.sendMail(mailOptions, function (error, info) {
            if (error) {
                console.log(error);
            } else {
                console.log('Email sent: ' + info.response);
            }
        });*/

        let info = await this.transporter.sendMail(mailOptions);

        console.log("Message sent: %s", info.messageId);
        // Preview only available when sending through an Ethereal account
        console.log("Preview URL: %s", nodemailer.getTestMessageUrl(info));
    }
}
